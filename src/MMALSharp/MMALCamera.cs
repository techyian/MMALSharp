// <copyright file="MMALCamera.cs" company="Techyian">
// Copyright (c) Techyian. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp
{
    /// <summary>
    /// This class provides an interface to the Raspberry Pi camera module. 
    /// </summary>
    public sealed class MMALCamera
    {
        /// <summary>
        /// Reference to the camera component
        /// </summary>
        public MMALCameraComponent Camera { get; set; }

        /// <summary>
        /// List of all encoders currently in the pipeline
        /// </summary>
        public List<MMALDownstreamComponent> DownstreamComponents { get; set; }

        private static readonly Lazy<MMALCamera> lazy = new Lazy<MMALCamera>(() => new MMALCamera());

        public static MMALCamera Instance => lazy.Value;

        private MMALCamera()
        {
            BcmHost.bcm_host_init();

            MMALLog.ConfigureLogConfig();

            this.Camera = new MMALCameraComponent();
            this.DownstreamComponents = new List<MMALDownstreamComponent>();
        }

        /// <summary>
        /// Begin capture on one of the camera's output ports.
        /// </summary>
        /// <param name="port">An output port of the camera component</param>
        public void StartCapture(MMALPortImpl port)
        {
            if (port == this.Camera.StillPort || port == this.Camera.VideoPort)
            {
                port.SetImageCapture(true);
            }
        }

        /// <summary>
        /// Stop capture on one of the camera's output ports
        /// </summary>
        /// <param name="port">An output port of the camera component</param>
        public void StopCapture(MMALPortImpl port)
        {
            if (port == this.Camera.StillPort || port == this.Camera.VideoPort)
            {
                port.SetImageCapture(false);
            }
        }

        /// <summary>
        /// Force capture to stop on a port (Still or Video)
        /// </summary>
        /// <param name="port">The capture port</param>
        public void ForceStop(MMALPortImpl port)
        {
            if (port.Trigger.CurrentCount > 0)
            {
                port.Trigger.Signal();
            }
        }

        /// <summary>
        /// Self-contained method for recording H.264 video for a specified amount of time. Records at 30fps, 25Mb/s at the highest quality.
        /// </summary>
        /// <param name="handler">The video capture handler to apply to the encoder.</param>
        /// <param name="timeout">A timeout to stop the video capture</param>
        /// <param name="split">Used for Segmented video mode</param>
        /// <returns>The awaitable Task</returns>
        public async Task TakeVideo(VideoStreamCaptureHandler handler, DateTime? timeout = null, Split split = null)
        {
            if (split != null && !MMALCameraConfig.InlineHeaders)
            {
                MMALLog.Logger.Warn("Inline headers not enabled. Split mode not supported when this is disabled.");
                split = null;
            }

            using (var vidEncoder = new MMALVideoEncoder(handler, new MMAL_RATIONAL_T(30, 1), timeout, split))
            using (var renderer = new MMALVideoRenderer())
            {
                vidEncoder.ConfigureOutputPort(0, MMALEncoding.H264, MMALEncoding.I420, 10, 25000000);

                // Create our component pipeline.
                this.Camera.VideoPort.ConnectTo(vidEncoder);
                this.Camera.PreviewPort.ConnectTo(renderer);
                this.ConfigureCameraSettings();

                MMALLog.Logger.Info($"Preparing to take video. Resolution: {vidEncoder.Width} x {vidEncoder.Height}. " +
                                    $"Encoder: {vidEncoder.Outputs[0].EncodingType.EncodingName}. Pixel Format: {vidEncoder.Outputs[0].PixelFormat.EncodingName}.");

                // Camera warm up time
                await Task.Delay(2000);

                await this.BeginProcessing(this.Camera.VideoPort);
            }
        }

        /// <summary>
        /// Self-contained method to capture raw image data directly from the Camera component - this method does not use an Image encoder.
        /// </summary>
        /// <param name="handler">The image capture handler to use to save image.</param>
        /// <returns>The awaitable Task</returns>
        public async Task TakeRawPicture(ICaptureHandler handler)
        {
            if (this.Camera.StillPort.ConnectedReference != null)
            {
                throw new PiCameraError("A connection was found to the Camera still port. No encoder should be connected to the Camera's still port for raw capture.");
            }

            if (handler == null)
            {
                throw new PiCameraError("No handler specified");
            }

            this.Camera.Handler = handler;

            this.CheckPreviewComponentStatus();

            // Enable the image encoder output port.
            try
            {
                MMALLog.Logger.Info($"Preparing to take raw picture - Resolution: {MMALCameraConfig.StillResolution.Width} x {MMALCameraConfig.StillResolution.Height}. " +
                                  $"Encoder: {MMALCameraConfig.StillEncoding.EncodingName}. Pixel Format: {MMALCameraConfig.StillSubFormat.EncodingName}.");

                // Camera warm up time
                await Task.Delay(2000);

                // Enable processing on the camera still port.
                this.Camera.EnableConnections();

                this.Camera.Start(this.Camera.StillPort, new Action<MMALBufferImpl, MMALPortBase>(this.Camera.ManagedOutputCallback));
                this.Camera.StillPort.Trigger = new Nito.AsyncEx.AsyncCountdownEvent(1);

                this.StartCapture(this.Camera.StillPort);

                // Wait until the process is complete.
                await this.Camera.StillPort.Trigger.WaitAsync();

                // Stop capturing on the camera still port.
                this.StopCapture(this.Camera.StillPort);

                this.Camera.Stop(MMALCameraComponent.MMALCameraStillPort);

                // Close open connections and clean port pools.
                this.Camera.DisableConnections();

                this.Camera.CleanPortPools();
            }
            finally
            {
                this.Camera.Handler.Dispose();
            }
        }

        /// <summary>
        /// Self-contained method for capturing a single image from the camera still port.
        /// An MMALImageEncoder component will be created and attached to the still port.
        /// </summary>
        /// <param name="handler">The image capture handler to apply to the encoder component</param>
        /// <param name="encodingType">The image encoding type e.g. JPEG, BMP</param>
        /// <param name="pixelFormat">The pixel format to use with the encoder e.g. I420 (YUV420)</param>
        /// <returns>The awaitable Task</returns>
        public async Task TakePicture(ImageStreamCaptureHandler handler, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            using (var imgEncoder = new MMALImageEncoder(handler))
            using (var renderer = new MMALNullSinkComponent())
            {
                imgEncoder.ConfigureOutputPort(0, encodingType, pixelFormat, 90);

                // Create our component pipeline.
                this.Camera.StillPort.ConnectTo(imgEncoder);
                this.Camera.PreviewPort.ConnectTo(renderer);
                this.ConfigureCameraSettings();

                // Enable the image encoder output port.
                MMALLog.Logger.Info($"Preparing to take picture. Resolution: {imgEncoder.Width} x {imgEncoder.Height}. " +
                                    $"Encoder: {encodingType.EncodingName}. Pixel Format: {pixelFormat.EncodingName}.");

                // Camera warm up time
                await Task.Delay(2000);

                await this.BeginProcessing(this.Camera.StillPort);
            }
        }

        /// <summary>
        /// Self-contained method for capturing a continual images from the camera still port for a specified period of time.
        /// An MMALImageEncoder component will be created and attached to the still port.
        /// </summary>
        /// <param name="handler">The image capture handler to apply to the encoder component</param>
        /// <param name="encodingType">The image encoding type e.g. JPEG, BMP</param>
        /// <param name="pixelFormat">The pixel format to use with the encoder e.g. I420 (YUV420)</param>
        /// <param name="timeout">The DateTime which capturing should stop</param>
        /// <param name="burstMode">When enabled, burst mode will increase the rate at which images are taken, at the expense of quality</param>
        /// <returns>The awaitable Task</returns>
        public async Task TakePictureTimeout(ImageStreamCaptureHandler handler, MMALEncoding encodingType, MMALEncoding pixelFormat, DateTime timeout, bool burstMode = false)
        {
            if (burstMode)
            {
                this.Camera.StillPort.SetParameter(MMALParametersCamera.MMAL_PARAMETER_CAMERA_BURST_CAPTURE, true);
            }

            while (DateTime.Now.CompareTo(timeout) < 0)
            {
                await this.TakePicture(handler, encodingType, pixelFormat);
            }
        }

        /// <summary>
        /// Self-contained method for capturing timelapse images.
        /// An MMALImageEncoder component will be created and attached to the still port.
        /// </summary>
        /// <param name="handler">The image capture handler to apply to the encoder component</param>
        /// <param name="encodingType">The image encoding type e.g. JPEG, BMP</param>
        /// <param name="pixelFormat">The pixel format to use with the encoder e.g. I420 (YUV420)</param>
        /// <param name="timelapse">A Timelapse object which specifies the timeout and rate at which images should be taken</param>
        /// <returns>The awaitable Task</returns>
        public async Task TakePictureTimelapse(ImageStreamCaptureHandler handler, MMALEncoding encodingType, MMALEncoding pixelFormat, Timelapse timelapse)
        {
            int interval = 0;

            if (timelapse == null)
            {
                throw new PiCameraError("Timelapse object null. This must be initialized for Timelapse mode");
            }

            while (DateTime.Now.CompareTo(timelapse.Timeout) < 0)
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

                await Task.Delay(interval);

                await this.TakePicture(handler, encodingType, pixelFormat);
            }
        }

        /// <summary>
        /// Helper method to begin processing image data. Starts the Camera port and awaits until processing is complete.
        /// Cleans up resources upon finish.
        /// </summary>
        /// <param name="cameraPort">The camera port which image data is coming from</param>
        /// <returns>The awaitable Task</returns>
        public async Task BeginProcessing(MMALPortImpl cameraPort)
        {
            var handlerComponents = this.PopulateProcessingList();

            // Enable all connections associated with these components
            foreach (var component in handlerComponents)
            {
                component.EnableConnections();

                foreach (var portNum in component.ProcessingPorts)
                {
                    if (component.Outputs[portNum].ConnectedReference == null)
                    {
                        component.Start(portNum, new Action<MMALBufferImpl, MMALPortBase>(component.ManagedOutputCallback));
                        component.Outputs[portNum].Trigger = new Nito.AsyncEx.AsyncCountdownEvent(1);
                    }
                }
            }

            // We now begin capturing on the camera, processing will commence based on the pipeline configured.
            this.StartCapture(cameraPort);

            List<Task> tasks = new List<Task>();

            // Wait until the process is complete.
            foreach (var component in handlerComponents)
            {
                foreach (var portNum in component.ProcessingPorts)
                {
                    if (component.Outputs[portNum].ConnectedReference == null)
                    {
                        tasks.Add(component.Outputs[portNum].Trigger.WaitAsync());
                    }
                }
            }

            await Task.WhenAll(tasks.ToArray());

            this.StopCapture(cameraPort);

            // If taking raw image, the camera component will hold the handler
            this.Camera.Handler?.PostProcess();

            // Disable the image encoder output port.
            foreach (var component in handlerComponents)
            {
                // Apply any final processing on each component
                component.Handler?.PostProcess();

                foreach (var portNum in component.ProcessingPorts)
                {
                    if (component.Outputs[portNum].ConnectedReference == null)
                    {
                        component.Stop(portNum);
                    }
                }

                // Close open connections.
                component.DisableConnections();

                component.CleanPortPools();
            }
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
        /// Disables processing on the camera component
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
        /// This applies the configuration settings against the camera such as Saturation, Contrast etc.
        /// </summary>
        /// <returns>The camera instance</returns>
        public MMALCamera ConfigureCameraSettings()
        {
            MMALLog.Logger.Debug("Configuring camera parameters.");

            this.DisableCamera();
            this.Camera.SetCameraParameters();
            this.Camera.InitialiseStill();
            this.Camera.InitialiseVideo();
            this.Camera.InitialisePreview();
            this.EnableCamera();

            return this;
        }

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
