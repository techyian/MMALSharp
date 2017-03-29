using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MMALSharp
{    
    /// <summary>
    /// This class provides an interface to the Raspberry Pi camera module. 
    /// </summary>
    public sealed class MMALCamera : IDisposable
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

        private static readonly MMALCamera instance = new MMALCamera();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static MMALCamera()
        {
        }

        private MMALCamera()
        {
            BcmHost.bcm_host_init();

            this.Camera = new MMALCameraComponent();
            this.Encoders = new List<MMALEncoderBase>();
        }

        public static MMALCamera Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Begin capture on one of the camera's output ports.
        /// </summary>
        /// <param name="port"></param>
        public void StartCapture(MMALPortImpl port)
        {            
            if (port == this.Camera.StillPort || this.Encoders.Any(c => c.Enabled))
                port.SetImageCapture(true);
        }

        /// <summary>
        /// Stop capture on one of the camera's output ports
        /// </summary>
        /// <param name="port"></param>
        public void StopCapture(MMALPortImpl port)
        {
            if (port == this.Camera.StillPort || this.Encoders.Any(c => c.Enabled))
                port.SetImageCapture(false);
        }
        
        /// <summary>
        /// Force capture to stop on a port (Still or Video)
        /// </summary>
        /// <param name="port">The capture port</param>
        public void ForceStop(MMALPortImpl port)
        {
            port.Trigger.Signal();
        }        

        /// <summary>
        /// Record video for a specified amount of time. 
        /// </summary>        
        /// <param name="connPort">Port the encoder is connected to</param>        
        /// <param name="handler">The capture handler for this capture method</param>
        /// <param name="timeout">A timeout to stop the video capture</param>
        /// <param name="split">Used for Segmented video mode</param>
        /// <returns></returns>
        public async Task TakeVideo(MMALPortImpl connPort, ICaptureHandler handler, DateTime? timeout = null, Split split = null)
        {
            if (handler == null)
            {
                throw new PiCameraError("Handler cannot be null");
            }
                
            var encoder = this.Encoders.Where(c => c.Connection != null && c.Connection.OutputPort == connPort).FirstOrDefault();

            if (encoder == null || encoder.GetType() != typeof(MMALVideoEncoder))
            {
                throw new PiCameraError("No video encoder currently attached to output port specified");
            }
                
            encoder.Handler = handler;
            
            if (!encoder.Connection.Enabled)
            {
                encoder.Connection.Enable();
            }
                                        
            if(split != null && !MMALCameraConfig.InlineHeaders)
            {
                Helpers.PrintWarning("Inline headers not enabled. Split mode not supported when this is disabled.");
                split = null;
            }

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

            var outputPort = 0;

            //Enable the video encoder output port.
            encoder.Start(outputPort, encoder.ManagedCallback);         
                                       
            encoder.Outputs.ElementAt(outputPort).Trigger = new Nito.AsyncEx.AsyncCountdownEvent(1);
            
            ((MMALVideoPort)encoder.Outputs.ElementAt(outputPort)).Timeout = timeout;
            ((MMALVideoEncoder)encoder).Split = split;

            this.StartCapture(this.Camera.VideoPort);
                        
            await encoder.Outputs.ElementAt(outputPort).Trigger.WaitAsync();
                                      
            //Wait until the process is complete.            
            this.StopCapture(this.Camera.VideoPort);

            //Disable the image encoder output port.
            encoder.Stop(outputPort);

            //Close open connections.
            encoder.Connection.Disable();
            encoder.CleanEncoderPorts();

            encoder.Handler.PostProcess();
        }

        /// <summary>
        /// Captures a single image from the camera's still port. 
        /// Initializes a standalone MMALImageEncoder using the provided encodingType and quality.        
        /// </summary>        
        /// <param name="handler">The capture handler for this capture method</param>
        /// <param name="encodingType">The image encoding type</param>
        /// <param name="quality">Quality of the image (valid only for JPEG)</param>
        /// <param name="raw">Include raw bayer metadeta in the capture</param>        
        /// <param name="useExif">Specify whether to include EXIF tags in the capture</param>
        /// <param name="exifTags">Custom EXIF tags to use in the capture</param>
        /// <returns></returns>
        public async Task TakeSinglePicture(ICaptureHandler handler, int encodingType = 0, int quality = 0, bool raw = false, bool useExif = true, params ExifTag[] exifTags)
        {            
            var camPreviewPort = this.Camera.PreviewPort;
            var camVideoPort = this.Camera.VideoPort;
            var camStillPort = this.Camera.StillPort;

            if (handler == null)
            {
                throw new PiCameraError("Handler cannot be null");
            }

            if (this.Encoders.Any(c => c.Connection != null && c.Connection.OutputPort == this.Camera.StillPort && c.GetType() == typeof(MMALImageEncoder)))
            {
                //Reuse if an Image encoder is already connected to the Still camera port
                await TakePicture(this.Camera.StillPort, this.Camera.StillPort, handler, useExif, raw, exifTags);
            }
            else
            {
                Console.WriteLine("Preparing to take picture");
                using (var encoder = new MMALImageEncoder(encodingType, quality))
                {
                    if (useExif)
                    {
                        ((MMALImageEncoder)encoder).AddExifTags(exifTags);
                    }
                        
                    //Create connections
                    if (this.Preview == null)
                    {
                        Helpers.PrintWarning("Preview port does not have a Render component configured. Resulting image will be affected.");
                    }                        
                    else
                    {
                        if (this.Preview.Connection == null)
                        {
                            this.Preview.CreateConnection(camPreviewPort);
                        }                            
                    }
                    
                    encoder.Handler = handler;

                    encoder.CreateConnection(camStillPort);

                    if (raw)
                    {
                        camStillPort.SetRawCapture(true);
                    }
                        
                    if (MMALCameraConfig.EnableAnnotate)
                    {
                        encoder.AnnotateImage();
                    }
                        
                    int outputPort = 0;

                    //Enable the image encoder output port.
                    encoder.Start(outputPort, encoder.ManagedCallback);

                    this.StartCapture(camStillPort);

                    //Wait until the process is complete.
                    encoder.Outputs.ElementAt(outputPort).Trigger = new Nito.AsyncEx.AsyncCountdownEvent(1);
                    await encoder.Outputs.ElementAt(outputPort).Trigger.WaitAsync();

                    this.StopCapture(camStillPort);

                    //Disable the image encoder output port.
                    encoder.Stop(outputPort);

                    //Close open connections.                
                    encoder.Connection.Destroy();

                    encoder.Handler.PostProcess();
                }
            }                        
        }

        /// <summary>
        /// Captures a single image from the output port specified. Expects an MMALImageEncoder to be attached.
        /// </summary>        
        /// <param name="cameraPort">The port that is currently capturing images (Still or Video)</param>
        /// <param name="connPort">The port our encoder is attached to</param>
        /// <param name="handler">The handler to use for this capture method</param>
        /// <param name="raw">Include raw bayer metadeta in the capture</param>        
        /// <param name="useExif">Specify whether to include EXIF tags in the capture</param>
        /// <param name="exifTags">Custom EXIF tags to use in the capture</param>
        /// <returns></returns>
        public async Task TakePicture(MMALPortImpl cameraPort, MMALPortImpl connPort, ICaptureHandler handler, bool raw = false, bool useExif = true, params ExifTag[] exifTags)
        {
            Console.WriteLine("Preparing to take picture");
            
            //Find the encoder/decoder which is connected to the output port specified.
            var encoder = this.Encoders.Where(c => c.Connection != null && c.Connection.OutputPort == connPort).FirstOrDefault();

            if (encoder == null || encoder.GetType() != typeof(MMALImageEncoder))
                throw new PiCameraError("No image encoder currently attached to output port specified");

            if (handler == null)
            {
                throw new PiCameraError("Handler cannot be null");
            }

            encoder.Handler = handler;

            if (!encoder.Connection.Enabled)
            {
                encoder.Connection.Enable();
            }
                
            if (useExif)
            {
                ((MMALImageEncoder)encoder).AddExifTags(exifTags);
            }
                
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

            if (raw)
            {
                this.Camera.StillPort.SetRawCapture(true);
            }
                
            if (MMALCameraConfig.EnableAnnotate)
            {
                encoder.AnnotateImage();
            }
                
            var outputPort = 0;

            //Enable the image encoder output port.
            encoder.Start(outputPort, encoder.ManagedCallback);

            this.StartCapture(cameraPort);

            //Wait until the process is complete.
            encoder.Outputs.ElementAt(outputPort).Trigger = new Nito.AsyncEx.AsyncCountdownEvent(1);
            await encoder.Outputs.ElementAt(outputPort).Trigger.WaitAsync();

            this.StopCapture(cameraPort);

            //Disable the image encoder output port.
            encoder.Stop(outputPort);

            //Close open connections.
            encoder.Connection.Disable();
            encoder.CleanEncoderPorts();

            encoder.Handler.PostProcess();
        }

        /// <summary>
        /// Takes a number of images as specified by the iterations parameter.
        /// </summary>
        /// <param name="directory">The directory to save our image</param>
        /// <param name="extension">The file extension of our image</param>
        /// <param name="iterations">Number of pictures to take</param>
        /// <param name="encodingType">The encoding type to use</param>
        /// <param name="quality">The quality of the image (Valid for JPEG)</param>
        /// <param name="raw">Include raw bayer metadeta in the capture</param>        
        /// <param name="useExif">Specify whether to include EXIF tags in the capture</param>
        /// <param name="exifTags">Custom EXIF tags to use in the capture</param>
        public async Task TakePictureIterative(string directory, string extension, int iterations, int encodingType = 0, int quality = 0, bool raw = false, bool useExif = true, params ExifTag[] exifTags)
        {
            for (int i = 0; i < iterations; i++)
            {
                var filename = (directory.EndsWith("/") ? directory : directory + "/") + DateTime.Now.ToString("dd-MMM-yy HH-mm-ss") + (extension.StartsWith(".") ? extension : "." + extension);
                                
                await TakeSinglePicture(new StreamCaptureResult(File.Create(filename)), encodingType, quality, raw, useExif, exifTags);                
            }
        }

        /// <summary>
        /// Takes images until the moment specified in the timeout parameter has been met.
        /// </summary>
        /// <param name="directory">The directory to save our image</param>
        /// <param name="extension">The file extension of our image</param>
        /// <param name="timeout">Take images until this timeout is hit</param>
        /// <param name="encodingType">The encoding type to use</param>
        /// <param name="quality">The quality of the image (Valid for JPEG)</param>
        /// <param name="raw">Include raw bayer metadeta in the capture</param>        
        /// <param name="useExif">Specify whether to include EXIF tags in the capture</param>
        /// <param name="exifTags">Custom EXIF tags to use in the capture</param>
        public async Task TakePictureTimeout(string directory, string extension, DateTime timeout, int encodingType = 0, int quality = 0, bool raw = false, bool useExif = true, params ExifTag[] exifTags)
        {
            while (DateTime.Now.CompareTo(timeout) < 0)
            {
                var filename = (directory.EndsWith("/") ? directory : directory + "/") + DateTime.Now.ToString("dd-MMM-yy HH-mm-ss") + (extension.StartsWith(".") ? extension : "." + extension);
                                
                await TakeSinglePicture(new StreamCaptureResult(File.Create(filename)), encodingType, quality, raw, useExif, exifTags);                
            }
        }

        /// <summary>
        /// Takes a timelapse image. You can specify the interval between each image taken and also when the operation should finish.
        /// </summary>
        /// <param name="directory">The directory to save our image</param>
        /// <param name="extension">The file extension of our image</param>
        /// <param name="tl">Specifies settings for the Timelapse</param>
        /// <param name="encodingType">The encoding type to use</param>
        /// <param name="quality">The quality of the image (Valid for JPEG)</param>
        /// <param name="raw">Include raw bayer metadeta in the capture</param>        
        /// <param name="useExif">Specify whether to include EXIF tags in the capture</param>
        /// <param name="exifTags">Custom EXIF tags to use in the capture</param>
        /// <returns></returns>
        public async Task TakePictureTimelapse(string directory, string extension, Timelapse tl, int encodingType = 0, int quality = 0, bool raw = false, bool useExif = true, params ExifTag[] exifTags)
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

                var filename = (directory.EndsWith("/") ? directory : directory + "/") + DateTime.Now.ToString("dd-MMM-yy HH-mm-ss") + (extension.StartsWith(".") ? extension : "." + extension);
                                
                await TakeSinglePicture(new StreamCaptureResult(File.Create(filename)), encodingType, quality, raw, useExif, exifTags);                
            }
        }

        /// <summary>
        /// Helper method to create a new preview component
        /// </summary>
        /// <param name="renderer">The renderer type</param>
        /// <returns></returns>
        public MMALCamera CreatePreviewComponent(MMALRendererBase renderer)
        {
            if (this.Preview != null)
            {
                this.Preview?.Connection.Disable();
                this.Preview?.Connection.Destroy();
            }

            this.Preview = renderer;
            this.Preview.CreateConnection(this.Camera.PreviewPort);
            return this;
        }

        /// <summary>
        /// Helper method to create a splitter component
        /// </summary>
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
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
            this.Preview.DisableComponent();
            this.Camera.DisableComponent();
        }

        /// <summary>
        /// Enables processing on the camera component
        /// </summary>
        public void EnableCamera()
        {
            this.Encoders.ForEach(c => c.EnableComponent());
            this.Preview.EnableComponent();
            this.Camera.EnableComponent();
        }

        /// <summary>
        /// Configures the camera component. This method applies configuration settings and initialises the components required
        /// for capturing images.
        /// </summary>
        /// <returns></returns>
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
            this.SetImageFx(MMALCameraConfig.ImageEffect);
            this.SetColourFx(MMALCameraConfig.Effects);
            this.SetRotation(MMALCameraConfig.Rotation);
            this.SetShutterSpeed(MMALCameraConfig.ShutterSpeed);
            this.SetStatsPass(MMALCameraConfig.StatsPass);
            this.SetDRC(MMALCameraConfig.DrcLevel);
            this.SetFlips(MMALCameraConfig.Flips);
            this.SetCrop(MMALCameraConfig.Crop);
            
            this.EnableCamera();

            return this;
        }
                
        public void Dispose()
        {
            if (MMALCameraConfig.Debug)
            {
                Console.WriteLine("Destroying final components");
            }
                
            this.Encoders.ForEach(c => c.Dispose());

            if (this.Preview != null)
            {
                this.Preview.Dispose();
            }
                
            if (this.Camera != null)
            {
                this.Camera.Dispose();
            }
                                         
            BcmHost.bcm_host_deinit();
        }
    }




}
