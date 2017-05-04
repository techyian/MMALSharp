using MMALSharp.Components;
using MMALSharp.Native;
using MMALSharp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MMALSharp.Handlers;

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
        public List<MMALEncoderBase> Encoders { get; set; }

        /// <summary>
        /// Reference to the Preview component to be used by the camera component
        /// </summary>
        public MMALRendererBase Preview { get; set; }

        /// <summary>
        /// Reference to the Video splitter component which attaches to the Camera's video output port
        /// </summary>
        public MMALSplitterComponent Splitter { get; set; }

        private static readonly Lazy<MMALCamera> lazy = new Lazy<MMALCamera>(() => new MMALCamera());

        public static MMALCamera Instance => lazy.Value;

        private MMALCamera()
        {
            BcmHost.bcm_host_init();

            this.Camera = new MMALCameraComponent();
            this.Encoders = new List<MMALEncoderBase>();
        }

        /// <summary>
        /// Begin capture on one of the camera's output ports.
        /// </summary>
        /// <param name="port">An output port of the camera component</param>
        public void StartCapture(MMALPortImpl port)
        {            
            if (port == this.Camera.StillPort || this.Encoders.Any(c => c.Enabled))
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
            if (port == this.Camera.StillPort || this.Encoders.Any(c => c.Enabled))
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
            if(port.Trigger.CurrentCount > 0)
            {
                port.Trigger.Signal();
            }            
        }

        /// <summary>
        /// Record video for a specified amount of time. 
        /// </summary>        
        /// <param name="connPort">Port the encoder is connected to</param>                
        /// <param name="timeout">A timeout to stop the video capture</param>
        /// <param name="split">Used for Segmented video mode</param>
        /// <returns>The awaitable Task</returns>
        public async Task TakeVideo(MMALPortImpl connPort, DateTime? timeout = null, Split split = null)
        {                            
            var encoder = this.Encoders.Where(c => c.Connection != null && c.Connection.OutputPort == connPort).FirstOrDefault();

            if (encoder == null || encoder.GetType() != typeof(MMALVideoEncoder))
            {
                throw new PiCameraError("No video encoder currently attached to output port specified");
            }
                
            if (!encoder.Connection.Enabled)
            {
                encoder.Connection.Enable();
            }
                                        
            if(split != null && !MMALCameraConfig.InlineHeaders)
            {
                Helpers.PrintWarning("Inline headers not enabled. Split mode not supported when this is disabled.");
                split = null;
            }

            this.CheckPreviewComponentStatus();
            
            try
            {
                Console.WriteLine($"Preparing to take video. Resolution: {MMALCameraConfig.VideoResolution.Width} x {MMALCameraConfig.VideoResolution.Height}. Encoder: {encoder.EncodingType.EncodingName}. Pixel Format: {encoder.PixelFormat.EncodingName}.");
                                
                ((MMALVideoPort)encoder.Outputs.ElementAt(0)).Timeout = timeout;
                ((MMALVideoEncoder)encoder).Split = split;

                await BeginProcessing(encoder, encoder.Connection, this.Camera.VideoPort, 0);
                
            }
            finally
            {
                encoder.Handler.PostProcess();
            }            
        }

        /// <summary>
        /// Capture raw image data directly from the Camera component - this method does not use an Image encoder.
        /// </summary>
        /// <returns>The awaitable Task</returns>
        public async Task TakeRawPicture(ICaptureHandler handler)
        {
            var encoder = this.Encoders.Where(c => c.Connection != null && c.Connection.OutputPort == this.Camera.StillPort).FirstOrDefault();

            if (encoder != null)
            {
                throw new PiCameraError("A connection was found to the Camera still port. No encoder should be connected to the Camera's still port for raw capture.");
            }
            if (handler == null)
            {
                throw new PiCameraError("No handler specified");
            }

            this.Camera.Handler = handler;

            this.CheckPreviewComponentStatus();

            //Enable the image encoder output port.            
            try
            {
                Console.WriteLine($"Preparing to take picture - Resolution: {MMALCameraConfig.StillResolution.Width} x {MMALCameraConfig.StillResolution.Height}");

                await BeginProcessing(this.Camera, null, this.Camera.StillPort, 0);
            }
            finally
            {
                this.Camera.Handler.PostProcess();
                this.Camera.Handler.Dispose();
            }
        }

        /// <summary>
        /// Captures a single image from the output port specified. Expects an MMALImageEncoder to be attached.
        /// </summary>                
        /// <param name="connPort">The port our encoder is attached to</param>       
        /// <param name="rawBayer">Include raw bayer metadeta in the capture</param>        
        /// <param name="useExif">Specify whether to include EXIF tags in the capture</param>
        /// <param name="exifTags">Custom EXIF tags to use in the capture</param>
        /// <returns>The awaitable Task</returns>
        public async Task TakePicture(MMALPortImpl connPort, bool rawBayer = false, bool useExif = true, params ExifTag[] exifTags)
        {
            if (connPort == null)
            {
                throw new PiCameraError("The port an Image encoder is attached to has not been specified.");
            }

            //Find the encoder/decoder which is connected to the output port specified.
            var encoder = this.Encoders.Where(c => c.Connection != null && c.Connection.OutputPort == connPort).FirstOrDefault();
            
            if (encoder == null || encoder.GetType() != typeof(MMALImageEncoder))
            {
                throw new PiCameraError("No image encoder currently attached to output port specified");
            }
                
            if (!encoder.Connection.Enabled)
            {
                encoder.Connection.Enable();
            }
                
            if (useExif)
            {
                ((MMALImageEncoder)encoder).AddExifTags(exifTags);
            }

            this.CheckPreviewComponentStatus();

            if (rawBayer)
            {
                this.Camera.StillPort.SetRawCapture(true);
            }
                
            if (MMALCameraConfig.EnableAnnotate)
            {
                encoder.AnnotateImage();
            }
            
            //Enable the image encoder output port.            
            try
            {
                Console.WriteLine($"Preparing to take picture. Resolution: {MMALCameraConfig.StillResolution.Width} x {MMALCameraConfig.StillResolution.Height}. Encoder: {encoder.EncodingType.EncodingName}. Pixel Format: {encoder.PixelFormat.EncodingName}.");

                await BeginProcessing(encoder, encoder.Connection, this.Camera.StillPort, 0);
            }
            finally
            {
                encoder.Handler.PostProcess();
            }            
        }

        /// <summary>
        /// Takes images until the moment specified in the timeout parameter has been met.
        /// </summary>
        /// <param name="connPort">The port our encoder is attached to</param>
        /// <param name="timeout">Take images until this timeout is hit</param>       
        /// <param name="rawBayer">Include raw bayer metadeta in the capture</param>        
        /// <param name="useExif">Specify whether to include EXIF tags in the capture</param>
        /// <param name="exifTags">Custom EXIF tags to use in the capture</param>
        /// <returns>The awaitable Task</returns>
        public async Task TakePictureTimeout(MMALPortImpl connPort, DateTime timeout, bool rawBayer = false, bool useExif = true, params ExifTag[] exifTags)
        {            
            while (DateTime.Now.CompareTo(timeout) < 0)
            {                             
                await TakePicture(connPort, rawBayer, useExif, exifTags);                
            }
        }

        /// <summary>
        /// Takes a timelapse image. You can specify the interval between each image taken and also when the operation should finish.
        /// </summary>
        /// <param name="connPort">The port our encoder is attached to</param>
        /// <param name="tl">Specifies settings for the Timelapse</param>       
        /// <param name="rawBayer">Include raw bayer metadeta in the capture</param>        
        /// <param name="useExif">Specify whether to include EXIF tags in the capture</param>
        /// <param name="exifTags">Custom EXIF tags to use in the capture</param>
        /// <returns>The awaitable Task</returns>
        public async Task TakePictureTimelapse(MMALPortImpl connPort, Timelapse tl, bool rawBayer = false, bool useExif = true, params ExifTag[] exifTags)
        {           
            int interval = 0;

            if(tl == null)
            {
                throw new PiCameraError("Timelapse object null. This must be initialized for Timelapse mode");
            }
            
            while (DateTime.Now.CompareTo(tl.Timeout) < 0)
            {
                switch (tl.Mode)
                {
                    case TimelapseMode.Millisecond:
                        interval = tl.Value;
                        break;
                    case TimelapseMode.Second:
                        interval = tl.Value * 1000;
                        break;
                    case TimelapseMode.Minute:
                        interval = (tl.Value * 60) * 1000;
                        break;
                }

                await Task.Delay(interval);
                
                await TakePicture(connPort, rawBayer, useExif, exifTags);                            
            }
        }

        /// <summary>
        /// Helper method to begin processing image data. Starts the Camera port and awaits until processing is complete.
        /// Cleans up resources upon finish.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="connection"></param>
        /// <param name="cameraPort"></param>
        /// <param name="outputPort"></param>
        /// <returns>The awaitable Task</returns>
        private async Task BeginProcessing(MMALComponentBase component, MMALConnectionImpl connection, MMALPortImpl cameraPort, int outputPort)
        {
            component.Start(outputPort, component.ManagedCallback);

            this.StartCapture(cameraPort);

            //Wait until the process is complete.
            component.Outputs.ElementAt(outputPort).Trigger = new Nito.AsyncEx.AsyncCountdownEvent(1);
            await component.Outputs.ElementAt(outputPort).Trigger.WaitAsync();

            this.StopCapture(cameraPort);

            //Disable the image encoder output port.
            component.Stop(outputPort);

            //Close open connections.
            connection?.Disable();
            
            component.CleanPortPools();
        }

        /// <summary>
        /// Helper method to create a new preview component
        /// </summary>
        /// <param name="renderer">The renderer type</param>
        /// <returns>The static Camera instance</returns>
        public MMALCamera CreatePreviewComponent(MMALRendererBase renderer)
        {
            if (this.Preview != null)
            {
                this.Preview?.Connection.Disable();
                this.Preview?.Connection.Destroy();
                this.Preview.Dispose();
            }

            this.Preview = renderer;
            this.Preview.CreateConnection(this.Camera.PreviewPort);
            return this;
        }

        /// <summary>
        /// Helper method to create a splitter component
        /// </summary>
        /// <returns>The static Camera instance</returns>
        public MMALCamera CreateSplitterComponent()
        {
            this.Splitter = new MMALSplitterComponent();
            return this;
        }

        /// <summary>
        /// Provides a facility to attach an encoder/decoder component to an upstream component's output port
        /// </summary>
        /// <param name="encoder">The encoder component to attach to the output port</param>
        /// <param name="outputPort">The output port to attach to</param>
        /// <returns>The static Camera instance</returns>
        public MMALCamera AddEncoder(MMALEncoderBase encoder, MMALPortImpl outputPort)
        {            
            if (MMALCameraConfig.Debug)
            {
                Console.WriteLine("Adding encoder");
            }
            
            this.RemoveEncoder(outputPort);

            encoder.CreateConnection(outputPort);
            
            this.Encoders.Add(encoder);
            return this;
        }

        /// <summary>
        /// Remove an encoder component from an output port
        /// </summary>
        /// <param name="outputPort">The output port we are removing an encoder component from</param>
        /// <returns>The static Camera instance</returns>
        public MMALCamera RemoveEncoder(MMALPortImpl outputPort)
        {            
            var enc = this.Encoders.Where(c => c.Connection != null && c.Connection.OutputPort == outputPort).FirstOrDefault();

            if (enc != null)
            {
                if (MMALCameraConfig.Debug)
                {
                    Console.WriteLine("Removing encoder");
                }
                    
                enc.Connection.Destroy();
                enc.Dispose();
                this.Encoders.Remove(enc);
            }                            
            return this;
        }
        
        /// <summary>
        /// Disables processing on the camera component
        /// </summary>
        public void DisableCamera()
        {
            this.Encoders.ForEach(c => c.DisableComponent());
            this.Preview?.DisableComponent();
            this.Camera.DisableComponent();
        }

        /// <summary>
        /// Enables processing on the camera component
        /// </summary>
        public void EnableCamera()
        {
            this.Encoders.ForEach(c => c.EnableComponent());
            this.Preview?.EnableComponent();
            this.Camera.EnableComponent();
        }

        /// <summary>
        /// Configures the camera component. This method applies configuration settings and initialises the components required
        /// for capturing images.
        /// </summary>
        /// <returns>The static Camera instance</returns>
        public MMALCamera ConfigureCamera()
        {
            if (MMALCameraConfig.Debug)
            {
                Console.WriteLine("Configuring camera parameters.");
            }
                
            this.DisableCamera();
            
            this.SetSaturation(MMALCameraConfig.Saturation);
            this.SetSharpness(MMALCameraConfig.Sharpness);
            this.SetContrast(MMALCameraConfig.Contrast);
            this.SetBrightness(MMALCameraConfig.Brightness);
            this.SetISO(MMALCameraConfig.ISO);
            this.SetVideoStabilisation(MMALCameraConfig.VideoStabilisation);
            this.SetExposureCompensation(MMALCameraConfig.ExposureCompensation);
            this.SetExposureMode(MMALCameraConfig.ExposureMode);
            this.SetExposureMeteringMode(MMALCameraConfig.ExposureMeterMode);
            this.SetAwbMode(MMALCameraConfig.AwbMode);
            this.SetAwbGains(MMALCameraConfig.AwbGainsR, MMALCameraConfig.AwbGainsB);
            this.SetImageFx(MMALCameraConfig.ImageFx);
            this.SetColourFx(MMALCameraConfig.ColourFx);
            this.SetRotation(MMALCameraConfig.Rotation);
            this.SetShutterSpeed(MMALCameraConfig.ShutterSpeed);
            this.SetStatsPass(MMALCameraConfig.StatsPass);
            this.SetDRC(MMALCameraConfig.DrcLevel);
            this.SetFlips(MMALCameraConfig.Flips);
            this.SetZoom(MMALCameraConfig.ROI);
            
            this.EnableCamera();

            return this;
        }
           
        /// <summary>
        /// Helper method to check the Renderer component status. If a Renderer has not been initialized, a warning will
        /// be shown to the user. If a Renderer has been created but a connection has not been initialized, this will be 
        /// done automatically for the user.
        /// </summary>
        private void CheckPreviewComponentStatus()
        {
            //Create connections
            if (this.Preview == null)
            {
                Helpers.PrintWarning("Preview port does not have a Render component configured. Resulting image will be affected.");
            }
            else
            {
                if (this.Preview.Connection == null)
                    this.Preview.CreateConnection(this.Camera.PreviewPort);
            }
        }

        /// <summary>
        /// Cleans up any unmanaged resources. It is intended for this method to be run when no more activity is to be done on the camera.
        /// </summary>
        public void Cleanup()
        {
            if (MMALCameraConfig.Debug)
            {
                Console.WriteLine("Destroying final components");
            }

            this.Encoders.ForEach(c => c.Dispose());
            this.Preview?.Dispose();
            this.Camera?.Dispose();

            BcmHost.bcm_host_deinit();
        }
                 
    }
    
}
