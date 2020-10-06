// <copyright file="MMALCamera.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Config;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using MMALSharp.Ports.Outputs;
using MMALSharp.Processors.Motion;

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
        public MMALCameraComponent Camera { get; }
        
        private MMALCamera()
        {
            BcmHost.bcm_host_init();

            this.Camera = new MMALCameraComponent();
        }

        /// <summary>
        /// Begin capture on one of the camera's output ports.
        /// </summary>
        /// <param name="port">An output port of the camera component.</param>
        public void StartCapture(IOutputPort port)
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
        public void StopCapture(IOutputPort port)
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
        public void ForceStop(IOutputPort port)
        {
            Task.Run(() =>
            {
                port.Trigger.SetResult(true);
            });
        }

        /// <summary>
        /// Self-contained method for recording raw video frames directly from the camera's video port.
        /// Uses the encoding and pixel format as set in <see cref="MMALCameraConfig.Encoding"/> and <see cref="MMALCameraConfig.EncodingSubFormat"/>.
        /// </summary>
        /// <param name="handler">The video capture handler to apply to the encoder.</param>
        /// <param name="cancellationToken">A cancellationToken to signal when to stop video capture.</param>        
        /// <returns>The awaitable Task.</returns>
        public async Task TakeRawVideo(IVideoCaptureHandler handler, CancellationToken cancellationToken)
        {            
            using (var splitter = new MMALSplitterComponent())
            using (var renderer = new MMALVideoRenderer())
            {
                this.ConfigureCameraSettings();

                var splitterOutputConfig = new MMALPortConfig(MMALCameraConfig.Encoding, MMALCameraConfig.EncodingSubFormat);

                // Force port type to SplitterVideoPort to prevent resolution from being set against splitter component.
                splitter.ConfigureOutputPort<SplitterVideoPort>(0, splitterOutputConfig, handler);

                // Create our component pipeline.
                this.Camera.VideoPort.ConnectTo(splitter);
                this.Camera.PreviewPort.ConnectTo(renderer);

                MMALLog.Logger.LogInformation($"Preparing to take raw video. Resolution: {this.Camera.VideoPort.Resolution.Width} x {this.Camera.VideoPort.Resolution.Height}. " +
                                    $"Encoder: {MMALCameraConfig.Encoding.EncodingName}. Pixel Format: {MMALCameraConfig.EncodingSubFormat.EncodingName}.");

                // Camera warm up time
                await Task.Delay(2000).ConfigureAwait(false);
                await this.ProcessAsync(this.Camera.VideoPort, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Self-contained method for recording H.264 video for a specified amount of time. Records at 30fps, 25Mb/s at the highest quality.
        /// </summary>
        /// <param name="handler">The video capture handler to apply to the encoder.</param>
        /// <param name="cancellationToken">A cancellationToken to signal when to stop video capture.</param>
        /// <param name="split">Used for Segmented video mode.</param>
        /// <returns>The awaitable Task.</returns>
        public async Task TakeVideo(IVideoCaptureHandler handler, CancellationToken cancellationToken, Split split = null)
        {
            if (split != null && !MMALCameraConfig.InlineHeaders)
            {
                MMALLog.Logger.LogWarning("Inline headers not enabled. Split mode not supported when this is disabled.");
                split = null;
            }

            using (var vidEncoder = new MMALVideoEncoder())
            using (var renderer = new MMALVideoRenderer())
            {
                this.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(MMALEncoding.H264, MMALEncoding.I420, 10, MMALVideoEncoder.MaxBitrateLevel4, split: split);

                vidEncoder.ConfigureOutputPort(portConfig, handler);

                // Create our component pipeline.
                this.Camera.VideoPort.ConnectTo(vidEncoder);
                this.Camera.PreviewPort.ConnectTo(renderer);

                MMALLog.Logger.LogInformation($"Preparing to take video. Resolution: {this.Camera.VideoPort.Resolution.Width} x {this.Camera.VideoPort.Resolution.Height}. " +
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
        public async Task TakeRawPicture(IOutputCaptureHandler handler)
        {
            if (this.Camera.StillPort.ConnectedReference != null)
            {
                throw new PiCameraError("A connection was found to the Camera still port. No encoder should be connected to the Camera's still port for raw capture.");
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            using (var renderer = new MMALNullSinkComponent())
            {
                this.ConfigureCameraSettings(handler);
                this.Camera.PreviewPort.ConnectTo(renderer);
                
                MMALLog.Logger.LogInformation($"Preparing to take raw picture - Resolution: {this.Camera.StillPort.Resolution.Width} x {this.Camera.StillPort.Resolution.Height}. " +
                                  $"Encoder: {MMALCameraConfig.Encoding.EncodingName}. Pixel Format: {MMALCameraConfig.EncodingSubFormat.EncodingName}.");

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
        public async Task TakePicture(IOutputCaptureHandler handler, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            using (var imgEncoder = new MMALImageEncoder())
            using (var renderer = new MMALNullSinkComponent())
            {
                this.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(encodingType, pixelFormat, 90);

                imgEncoder.ConfigureOutputPort(portConfig, handler);

                // Create our component pipeline.
                this.Camera.StillPort.ConnectTo(imgEncoder);
                this.Camera.PreviewPort.ConnectTo(renderer);
                
                MMALLog.Logger.LogInformation($"Preparing to take picture. Resolution: {this.Camera.StillPort.Resolution.Width} x {this.Camera.StillPort.Resolution.Height}. " +
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
        public async Task TakePictureTimeout(IFileStreamCaptureHandler handler, MMALEncoding encodingType, MMALEncoding pixelFormat, CancellationToken cancellationToken, bool burstMode = false)
        {
            if (burstMode)
            {
                MMALCameraConfig.StillBurstMode = true;                
            }

            using (var imgEncoder = new MMALImageEncoder())
            using (var renderer = new MMALNullSinkComponent())
            {
                this.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(encodingType, pixelFormat, 90);

                imgEncoder.ConfigureOutputPort(portConfig, handler);

                // Create our component pipeline.
                this.Camera.StillPort.ConnectTo(imgEncoder);
                this.Camera.PreviewPort.ConnectTo(renderer);

                // Camera warm up time
                await Task.Delay(2000).ConfigureAwait(false);

                while (!cancellationToken.IsCancellationRequested)
                {
                    await this.ProcessAsync(this.Camera.StillPort).ConfigureAwait(false);

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        handler.NewFile();
                    }
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
        public async Task TakePictureTimelapse(IFileStreamCaptureHandler handler, MMALEncoding encodingType, MMALEncoding pixelFormat, Timelapse timelapse)
        {
            int interval = 0;

            if (timelapse == null)
            {
                throw new ArgumentNullException(nameof(timelapse), "Timelapse object null. This must be initialized for Timelapse mode");
            }

            using (var imgEncoder = new MMALImageEncoder())
            using (var renderer = new MMALNullSinkComponent())
            {
                this.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(encodingType, pixelFormat, 90);

                imgEncoder.ConfigureOutputPort(portConfig, handler);

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

                    MMALLog.Logger.LogInformation($"Preparing to take picture. Resolution: {MMALCameraConfig.Resolution.Width} x {MMALCameraConfig.Resolution.Height}. " +
                                        $"Encoder: {encodingType.EncodingName}. Pixel Format: {pixelFormat.EncodingName}.");

                    await this.ProcessAsync(this.Camera.StillPort).ConfigureAwait(false);

                    if (!timelapse.CancellationToken.IsCancellationRequested)
                    {
                        handler.NewFile();
                    }
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
        public async Task ProcessAsync(IOutputPort cameraPort, CancellationToken cancellationToken = default(CancellationToken))
        {
            var handlerComponents = this.PopulateProcessingList();
            
            if (handlerComponents.Count == 0)
            {
                await this.ProcessRawAsync(cameraPort, cancellationToken);
                return;
            }
            
            var tasks = new List<Task>();
           
            // Enable all connections associated with these components
            foreach (var component in handlerComponents)
            {
                component.ForceStopProcessing = false;

                foreach (var port in component.ProcessingPorts.Values)
                {
                    if (port.ConnectedReference == null)
                    {
                        port.Start();
                        tasks.Add(port.Trigger.Task);
                    }
                }

                component.EnableConnections();
            }

            this.Camera.SetShutterSpeed(MMALCameraConfig.ShutterSpeed);

            // Prepare arguments for the annotation-refresh task
            var ctsRefreshAnnotation = new CancellationTokenSource();
            var refreshInterval = (int)(MMALCameraConfig.Annotate?.RefreshRate ?? 0);
            
            if (!(MMALCameraConfig.Annotate?.ShowDateText ?? false) && !(MMALCameraConfig.Annotate?.ShowTimeText ?? false))
            {
                refreshInterval = 0;
            }

            // We now begin capturing on the camera, processing will commence based on the pipeline configured.
            this.StartCapture(cameraPort);
            
            if (cancellationToken == CancellationToken.None)
            {
                await Task.WhenAny(
                    Task.WhenAll(tasks),
                    RefreshAnnotations(refreshInterval, ctsRefreshAnnotation.Token)).ConfigureAwait(false);

                ctsRefreshAnnotation.Cancel();
            }
            else
            {
                await Task.WhenAny(
                    Task.WhenAll(tasks),
                    RefreshAnnotations(refreshInterval, ctsRefreshAnnotation.Token),
                    cancellationToken.AsTask()).ConfigureAwait(false);

                ctsRefreshAnnotation.Cancel();

                foreach (var component in handlerComponents)
                {
                    component.ForceStopProcessing = true;
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }

            this.StopCapture(cameraPort);

            // Cleanup each connected downstream component.
            foreach (var component in handlerComponents)
            {
                foreach (var port in component.ProcessingPorts.Values)
                {
                    if (port.ConnectedReference == null)
                    {
                        port.DisablePort();
                    }
                }
                
                component.CleanPortPools();
                component.DisableConnections();
            }                        
        }
        
        /// <summary>
        /// Prints the currently configured component pipeline to the console window.
        /// </summary>
        public void PrintPipeline()
        {
            MMALLog.Logger.LogInformation("Current pipeline:");
            MMALLog.Logger.LogInformation(string.Empty);

            this.Camera.PrintComponent();

            foreach (var component in MMALBootstrapper.DownstreamComponents)
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
        /// <param name="stillCaptureHandler">Optional output capture handler for use with raw image capture.</param>
        /// <param name="videoCaptureHandler">Optional output capture handler for use with raw video capture.</param>
        /// <returns>The camera instance.</returns>
        public MMALCamera ConfigureCameraSettings(IOutputCaptureHandler stillCaptureHandler = null, IOutputCaptureHandler videoCaptureHandler = null)
        {            
            this.Camera.Initialise(stillCaptureHandler, videoCaptureHandler);
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
        /// Call to enable motion detection.
        /// </summary>
        /// <param name="handler">The motion capture handler.</param>
        /// <param name="config">The motion configuration object.</param>
        /// <param name="onDetect">The callback when motion is detected.</param>
        /// <returns>The camera instance.</returns>
        public MMALCamera WithMotionDetection(IMotionCaptureHandler handler, MotionConfig config, Action onDetect)
        {
            MMALCameraConfig.InlineMotionVectors = true;
            handler.ConfigureMotionDetection(config, onDetect);
            return this;
        }

        /// <summary>
        /// Cleans up any unmanaged resources. It is intended for this method to be run when no more activity is to be done on the camera.
        /// </summary>
        public void Cleanup()
        {
            MMALLog.Logger.LogDebug("Destroying final components");

            var tempList = new List<MMALDownstreamComponent>(MMALBootstrapper.DownstreamComponents);

            tempList.ForEach(c => c.Dispose());
            this.Camera.Dispose();

            BcmHost.bcm_host_deinit();
        }

        /// <summary>
        /// Periodically invokes <see cref="MMALCameraComponentExtensions.SetAnnotateSettings(MMALCameraComponent)"/> to update date/time annotations.
        /// </summary>
        /// <param name="msInterval">Update frequency in milliseconds, or 0 to disable.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for a task to complete.</param>
        /// <returns>The awaitable Task.</returns>
        private async Task RefreshAnnotations(int msInterval, CancellationToken cancellationToken)
        {
            try
            {
                if (msInterval == 0)
                {
                    await Task.Delay(Timeout.Infinite, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(msInterval, cancellationToken).ConfigureAwait(false);
                        this.Camera.SetAnnotateSettings();
                    }
                }
            }
            catch (OperationCanceledException)
            { // disregard token cancellation
            }
        }

        /// <summary>
        /// Acts as an isolated processor specifically used when capturing raw frames from the camera component.
        /// </summary>
        /// <param name="cameraPort">The camera component port (still or video).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The awaitable task.</returns>
        private async Task ProcessRawAsync(IOutputPort cameraPort,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (cancellationToken.Register(() => 
            {
                // this callback will be executed when token is cancelled
                cameraPort.Trigger.SetResult(true);
            }))
            {
                cameraPort.DisablePort();
                cameraPort.Start();

                this.StartCapture(cameraPort);
                await cameraPort.Trigger.Task.ConfigureAwait(false);
                
                this.StopCapture(cameraPort);
                this.Camera.CleanPortPools();
            }
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
                MMALLog.Logger.LogWarning("Preview port does not have a Render component configured. Resulting image will be affected.");
            }
        }

        private List<IDownstreamComponent> PopulateProcessingList()
        {
            var list = new List<IDownstreamComponent>();
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

        private void FindComponents(IDownstreamComponent downstream, List<IDownstreamComponent> list)
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
