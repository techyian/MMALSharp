// <copyright file="MMALCamera.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports.Outputs;
using MMALSharp.Utility;

namespace MMALSharp
{
    /// <summary>
    /// This class provides an interface to the Raspberry Pi camera module. 
    /// </summary>
    public sealed class MMALCamera
    {
        /// <summary>
        /// Gets the singleton instance of the MMAL Camera. Call to initialise the camera for first use.
        /// </summary>
        public static MMALCamera Instance => Lazy.Value;

        private static readonly Lazy<MMALCamera> Lazy = new Lazy<MMALCamera>(() => new MMALCamera());

        /// <summary>
        /// Reference to the camera component.
        /// </summary>
        public MMALCameraComponent Camera { get; set; }

        /// <summary>
        /// List of all encoders currently in the pipeline.
        /// </summary>
        public List<MMALDownstreamComponent> DownstreamComponents { get; set; }
        
        private MMALCamera()
        {
            BcmHost.bcm_host_init();

            MMALLog.ConfigureLogger();

            this.Camera = new MMALCameraComponent();
            this.DownstreamComponents = new List<MMALDownstreamComponent>();
        }

        /// <summary>
        /// Begin capture on one of the camera's output ports.
        /// </summary>
        /// <param name="port">An output port of the camera component.</param>
        public void StartCapture(OutputPortBase port)
        {
            if (port == this.Camera.StillPort || port == this.Camera.VideoPort)
            {
                port.SetImageCapture(true);
            }
        }

        /// <summary>
        /// Stop capture on one of the camera's output ports.
        /// </summary>
        /// <param name="port">An output port of the camera component.</param>
        public void StopCapture(OutputPortBase port)
        {
            if (port == this.Camera.StillPort || port == this.Camera.VideoPort)
            {
                port.SetImageCapture(false);
            }
        }

        /// <summary>
        /// Force capture to stop on a port (Still or Video).
        /// </summary>
        /// <param name="port">The capture port.</param>
        public void ForceStop(OutputPortBase port)
        {
            port.Trigger = true;
        }

        /// <summary>
        /// Self-contained method for recording H.264 video for a specified amount of time. Records at 30fps, 25Mb/s at the highest quality.
        /// </summary>
        /// <param name="handler">The video capture handler to apply to the encoder.</param>
        /// <param name="cancellationToken">A cancellationToken to signal when to stop video capture.</param>
        /// <param name="split">Used for Segmented video mode.</param>
        /// <returns>The awaitable Task.</returns>
        public async Task TakeVideo(ICaptureHandler handler, CancellationToken cancellationToken, Split split = null)
        {
            if (split != null && !MMALCameraConfig.InlineHeaders)
            {
                MMALLog.Logger.Warn("Inline headers not enabled. Split mode not supported when this is disabled.");
                split = null;
            }

            using (var vidEncoder = new MMALVideoEncoder(handler, null, split))
            using (var renderer = new MMALVideoRenderer())
            {
                this.ConfigureCameraSettings();

                vidEncoder.ConfigureOutputPort(MMALEncoding.H264, MMALEncoding.I420, 0, MMALVideoEncoder.MaxBitrateLevel4);

                // Create our component pipeline.
                this.Camera.VideoPort.ConnectTo(vidEncoder);
                this.Camera.PreviewPort.ConnectTo(renderer);

                MMALLog.Logger.Info($"Preparing to take video. Resolution: {vidEncoder.Width} x {vidEncoder.Height}. " +
                                    $"Encoder: {vidEncoder.Outputs[0].EncodingType.EncodingName}. Pixel Format: {vidEncoder.Outputs[0].PixelFormat.EncodingName}.");

                // Camera warm up time
                await Task.Delay(2000).ConfigureAwait(false);
                await this.ProcessAsync(this.Camera.VideoPort, cancellationToken).ConfigureAwait(false);
            }
        }
        
        /// <summary>
        /// Self-contained method to capture raw image data directly from the Camera component - this method does not use an Image encoder.
        /// Note: We cannot use the OPAQUE encoding format with this helper method, the capture will not fail, but will not produce valid data. For reference, RaspiStillYUV uses YUV420.
        /// </summary>
        /// <param name="handler">The image capture handler to use to save image.</param>
        /// <returns>The awaitable Task.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PiCameraError"/>
        public async Task TakeRawPicture(ICaptureHandler handler)
        {
            if (this.Camera.StillPort.ConnectedReference != null)
            {
                throw new PiCameraError("A connection was found to the Camera still port. No encoder should be connected to the Camera's still port for raw capture.");
            }

            this.Camera.StillPort.Handler = handler ?? throw new ArgumentNullException(nameof(handler));

            if (this.Camera.StillPort.Handler.GetType().IsSubclassOf(typeof(FileStreamCaptureHandler)))
            {
                ((FileStreamCaptureHandler)this.Camera.StillPort.Handler).NewFile();
            }
                
            using (var renderer = new MMALNullSinkComponent())
            {
                this.ConfigureCameraSettings();                
                this.Camera.PreviewPort.ConnectTo(renderer);
                
                // Enable the image encoder output port.
                MMALLog.Logger.Info($"Preparing to take raw picture - Resolution: {MMALCameraConfig.StillResolution.Width} x {MMALCameraConfig.StillResolution.Height}. " +
                                  $"Encoder: {MMALCameraConfig.StillEncoding.EncodingName}. Pixel Format: {MMALCameraConfig.StillSubFormat.EncodingName}.");

                // Camera warm up time
                await Task.Delay(2000).ConfigureAwait(false);
                await this.ProcessAsync(this.Camera.StillPort).ConfigureAwait(false);
            }            
        }

        /// <summary>
        /// Self-contained method for capturing a single image from the camera still port.
        /// An MMALImageEncoder component will be created and attached to the still port.
        /// </summary>
        /// <param name="handler">The image capture handler to apply to the encoder component.</param>
        /// <param name="encodingType">The image encoding type e.g. JPEG, BMP.</param>
        /// <param name="pixelFormat">The pixel format to use with the encoder e.g. I420 (YUV420).</param>
        /// <returns>The awaitable Task.</returns>
        public async Task TakePicture(ICaptureHandler handler, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            using (var imgEncoder = new MMALImageEncoder(handler))
            using (var renderer = new MMALNullSinkComponent())
            {
                this.ConfigureCameraSettings();

                imgEncoder.ConfigureOutputPort(encodingType, pixelFormat, 90);

                // Create our component pipeline.
                this.Camera.StillPort.ConnectTo(imgEncoder);
                this.Camera.PreviewPort.ConnectTo(renderer);
                
                // Enable the image encoder output port.
                MMALLog.Logger.Info($"Preparing to take picture. Resolution: {imgEncoder.Width} x {imgEncoder.Height}. " +
                                    $"Encoder: {encodingType.EncodingName}. Pixel Format: {pixelFormat.EncodingName}.");

                // Camera warm up time
                await Task.Delay(2000).ConfigureAwait(false);
                await this.ProcessAsync(this.Camera.StillPort).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Self-contained method for capturing a continual images from the camera still port for a specified period of time.
        /// An MMALImageEncoder component will be created and attached to the still port.
        /// </summary>
        /// <param name="handler">The image capture handler to apply to the encoder component.</param>
        /// <param name="encodingType">The image encoding type e.g. JPEG, BMP.</param>
        /// <param name="pixelFormat">The pixel format to use with the encoder e.g. I420 (YUV420).</param>
        /// <param name="cancellationToken">A cancellationToken to trigger stop capturing.</param>
        /// <param name="burstMode">When enabled, burst mode will increase the rate at which images are taken, at the expense of quality.</param>
        /// <returns>The awaitable Task.</returns>
        public async Task TakePictureTimeout(ICaptureHandler handler, MMALEncoding encodingType, MMALEncoding pixelFormat, CancellationToken cancellationToken, bool burstMode = false)
        {
            if (burstMode)
            {
                this.Camera.StillPort.SetParameter(MMALParametersCamera.MMAL_PARAMETER_CAMERA_BURST_CAPTURE, true);
            }

            using (var imgEncoder = new MMALImageEncoder(handler))
            using (var renderer = new MMALNullSinkComponent())
            {
                this.ConfigureCameraSettings();

                imgEncoder.ConfigureOutputPort(encodingType, pixelFormat, 90);

                // Create our component pipeline.
                this.Camera.StillPort.ConnectTo(imgEncoder);
                this.Camera.PreviewPort.ConnectTo(renderer);

                // Camera warm up time
                await Task.Delay(2000).ConfigureAwait(false);

                while (!cancellationToken.IsCancellationRequested)
                {
                    await this.ProcessAsync(this.Camera.StillPort).ConfigureAwait(false);
                }
            }
        }
        
        /// <summary>
        /// Self-contained method for capturing timelapse images.
        /// An MMALImageEncoder component will be created and attached to the still port.
        /// </summary>
        /// <param name="handler">The image capture handler to apply to the encoder component.</param>
        /// <param name="encodingType">The image encoding type e.g. JPEG, BMP.</param>
        /// <param name="pixelFormat">The pixel format to use with the encoder e.g. I420 (YUV420).</param>
        /// <param name="timelapse">A Timelapse object which specifies the timeout and rate at which images should be taken.</param>
        /// <returns>The awaitable Task.</returns>
        /// <exception cref="ArgumentNullException"/>
        public async Task TakePictureTimelapse(ICaptureHandler handler, MMALEncoding encodingType, MMALEncoding pixelFormat, Timelapse timelapse)
        {
            int interval = 0;

            if (timelapse == null)
            {
                throw new ArgumentNullException(nameof(timelapse), "Timelapse object null. This must be initialized for Timelapse mode");
            }

            using (var imgEncoder = new MMALImageEncoder(handler))
            using (var renderer = new MMALNullSinkComponent())
            {
                this.ConfigureCameraSettings();

                imgEncoder.ConfigureOutputPort(encodingType, pixelFormat, 90);

                // Create our component pipeline.
                this.Camera.StillPort.ConnectTo(imgEncoder);
                this.Camera.PreviewPort.ConnectTo(renderer);

                // Camera warm up time
                await Task.Delay(2000).ConfigureAwait(false);

                while (!timelapse.CancellationToken.IsCancellationRequested)
                {
                    switch (timelapse.Mode)
                    {
                        case TimelapseMode.Millisecond:
                            interval = timelapse.Value;
                            break;
                        case TimelapseMode.Second:
                            interval = timelapse.Value * 1000;
                            break;
                        case TimelapseMode.Minute:
                            interval = (timelapse.Value * 60) * 1000;
                            break;
                    }

                    await Task.Delay(interval).ConfigureAwait(false);

                    MMALLog.Logger.Info($"Preparing to take picture. Resolution: {imgEncoder.Width} x {imgEncoder.Height}. " +
                                        $"Encoder: {encodingType.EncodingName}. Pixel Format: {pixelFormat.EncodingName}.");

                    await this.ProcessAsync(this.Camera.StillPort).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Helper method to begin processing image data. Starts the Camera port and awaits until processing is complete.
        /// Cleans up resources upon finish.
        /// </summary>
        /// <param name="cameraPort">The camera port which image data is coming from.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for a task to complete.</param>
        /// <returns>The awaitable Task.</returns>
        public async Task ProcessAsync(OutputPortBase cameraPort, CancellationToken cancellationToken = default(CancellationToken))
        {
            var handlerComponents = this.PopulateProcessingList();
            
            if (handlerComponents.Count == 0)
            {
                await this.ProcessRawAsync(cameraPort, cancellationToken);
                return;
            }
            
            List<Task> tasks = new List<Task>();
            
            // Enable all connections associated with these components
            foreach (var component in handlerComponents)
            {
                component.EnableConnections();
                component.ForceStopProcessing = false;

                foreach (var port in component.ProcessingPorts.Values)
                {
                    port.Trigger = false;
                    if (port.ConnectedReference == null)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            while (!port.Trigger)
                            {
                                await Task.Delay(50).ConfigureAwait(false);
                            }
                        }, cancellationToken));
                        
                        port.Start();
                    }
                }
            }
            
            // We now begin capturing on the camera, processing will commence based on the pipeline configured.
            this.StartCapture(cameraPort);
            
            if (cancellationToken == CancellationToken.None)
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            else
            {
                await Task.WhenAny(Task.WhenAll(tasks), cancellationToken.AsTask()).ConfigureAwait(false);

                foreach (var component in handlerComponents)
                {
                    component.ForceStopProcessing = true;
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            
            // Disable the image encoder output port.
            foreach (var component in handlerComponents)
            {
                foreach (var port in component.ProcessingPorts.Values)
                {
                    // Apply any final processing on each component
                    port.Handler?.PostProcess();
                    
                    if (port.ConnectedReference == null)
                    {
                        port.DisablePort();
                    }
                }
                
                component.CleanPortPools();
                component.DisableConnections();
            }

            this.StopCapture(cameraPort);
        }
        
        /// <summary>
        /// Prints the currently configured component pipeline to the console window.
        /// </summary>
        public void PrintPipeline()
        {
            MMALLog.Logger.Info("Current pipeline:");
            MMALLog.Logger.Info(string.Empty);

            this.Camera.PrintComponent();

            foreach (var component in this.DownstreamComponents)
            {
                component.PrintComponent();
            }
        }

        /// <summary>
        /// Disables processing on the camera component.
        /// </summary>
        public void DisableCamera()
        {
            this.Camera.DisableComponent();
        }

        /// <summary>
        /// Enables processing on the camera component
        /// </summary>
        public void EnableCamera()
        {
            this.Camera.EnableComponent();
        }

        /// <summary>
        /// Initialises the camera component ready for operation. This method can also be called if you want to change
        /// configuration settings in <see cref="MMALCameraConfig"/>.
        /// </summary>
        /// <returns>The camera instance.</returns>
        public MMALCamera ConfigureCameraSettings()
        {            
            this.Camera.Initialise();                               
            return this;
        }

        /// <summary>
        /// Enables the annotation feature which will produce a textual overlay on produced frames.
        /// </summary>
        /// <returns>The camera instance.</returns>
        public MMALCamera EnableAnnotation()
        {
            this.Camera.SetAnnotateSettings();
            return this;
        }

        /// <summary>
        /// Disables the annotation feature.
        /// </summary>
        /// <returns>The camera instance.</returns>
        public MMALCamera DisableAnnotation()
        {
            this.Camera.DisableAnnotate();
            return this;
        }

        /// <summary>
        /// Creates an overlay renderer that is able to render an overlay from a static image source.
        /// </summary>
        /// <param name="parent">The parent renderer which is being used to overlay onto the display.</param>
        /// <param name="config">The configuration for rendering a static preview overlay.</param>
        /// <param name="source">A reference to the current stream being used in the overlay.</param>
        /// <returns>The created <see cref="MMALOverlayRenderer"/> object.</returns>
        public MMALOverlayRenderer AddOverlay(MMALVideoRenderer parent, PreviewOverlayConfiguration config, byte[] source)
            => new MMALOverlayRenderer(parent, config, source);
        
        /// <summary>
        /// Cleans up any unmanaged resources. It is intended for this method to be run when no more activity is to be done on the camera.
        /// </summary>
        public void Cleanup()
        {
            MMALLog.Logger.Debug("Destroying final components");

            var tempList = new List<MMALDownstreamComponent>(this.DownstreamComponents);

            tempList.ForEach(c => c.Dispose());
            this.Camera.Dispose();

            BcmHost.bcm_host_deinit();
        }

        /// <summary>
        /// Acts as an isolated processor specifically used when capturing raw frames from the camera component.
        /// </summary>
        /// <param name="cameraPort">The camera component port (still or video).</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The awaitable task.</returns>
        private async Task ProcessRawAsync(OutputPortBase cameraPort,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cameraPort.Trigger = false;
            
            var t = Task.Run(async () =>
            {
                while (!cameraPort.Trigger)
                {
                    await Task.Delay(50).ConfigureAwait(false);
                }
            }, cancellationToken);
            
            cameraPort.DisablePort();
            cameraPort.Start();
                
            this.StartCapture(cameraPort);
            await t.ConfigureAwait(false);
            
            cameraPort.Handler?.PostProcess();
            this.StopCapture(cameraPort);
            this.Camera.CleanPortPools();
        }
        
        /// <summary>
        /// Helper method to check the Renderer component status. If a Renderer has not been initialized, a warning will
        /// be shown to the user.
        /// </summary>
        private void CheckPreviewComponentStatus()
        {
            // Create connections
            if (this.Camera.PreviewPort.ConnectedReference == null)
            {
                MMALLog.Logger.Warn("Preview port does not have a Render component configured. Resulting image will be affected.");
            }
        }

        private List<MMALDownstreamComponent> PopulateProcessingList()
        {
            var list = new List<MMALDownstreamComponent>();
            var initialStillDownstream = this.Camera.StillPort.ConnectedReference?.DownstreamComponent;
            var initialVideoDownstream = this.Camera.VideoPort.ConnectedReference?.DownstreamComponent;
            var initialPreviewDownstream = this.Camera.PreviewPort.ConnectedReference?.DownstreamComponent;

            if (initialStillDownstream != null)
            {
                this.FindComponents(initialStillDownstream, list);
            }
            
            if (initialVideoDownstream != null)
            {
                this.FindComponents(initialVideoDownstream, list);
            }

            if (initialPreviewDownstream != null)
            {
                this.FindComponents(initialPreviewDownstream, list);
            }

            return list;
        }

        private void FindComponents(MMALDownstreamComponent downstream, List<MMALDownstreamComponent> list)
        {
            if (downstream.Outputs.Count == 0)
            {
                return;
            }

            if (downstream.Outputs.Count == 1 && downstream.Outputs[0].ConnectedReference == null)
            {
                list.Add(downstream);
                return;
            }

            if (downstream.GetType().BaseType == typeof(MMALDownstreamHandlerComponent))
            {
                list.Add((MMALDownstreamHandlerComponent)downstream);
            }

            foreach (var output in downstream.Outputs)
            {
                if (output.ConnectedReference != null)
                {
                    this.FindComponents(output.ConnectedReference.DownstreamComponent, list);
                }
            }
        }
    }
}
