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
using System.Timers;
using static MMALSharp.Native.MMALParameters;
using static MMALSharp.MMALCallerHelper;

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
                
        #region Configuration Properties
                
        /// <summary>
        /// Configure the sharpness of the image
        /// </summary>
        public double Sharpness
        {
            get
            {
                return this.GetSharpness();
            }
            set
            {
                MMALCameraConfigImpl.Config.Sharpness = value;
                this.ConfigureCamera();                                
            }
        }

        /// <summary>
        /// Configure the contrast of the image
        /// </summary>        
        public double Contrast
        {
            get
            {
                return this.GetContrast();
            }
            set
            {
                MMALCameraConfigImpl.Config.Contrast = value;
                this.ConfigureCamera();
            }
        }

        /// <summary>
        /// Configure the brightness of the image
        /// </summary>
        public double Brightness
        {
            get
            {
                return this.GetBrightness();
            }
            set
            {              
                MMALCameraConfigImpl.Config.Brightness = value;
                this.ConfigureCamera();
            }
        }

        /// <summary>
        /// Configure the saturation of the image
        /// </summary>       
        public double Saturation
        {
            get
            {
                return this.GetSaturation();
            }
            set
            {              
                MMALCameraConfigImpl.Config.Saturation = value;
                this.ConfigureCamera();
            }
        }

        /// <summary>
        /// Configure the light sensitivity of the sensor
        /// </summary>      
        public int ISO
        {
            get
            {
                return this.GetISO();
            }
            set
            {               
                MMALCameraConfigImpl.Config.ISO = value;
                this.ConfigureCamera();
            }
        }

        /// <summary>
        /// Enable video stabilisation
        /// </summary>   
        public bool VideoStabilisation
        {
            get
            {
                return this.GetVideoStabilisation();
            }
            set
            {             
                MMALCameraConfigImpl.Config.VideoStabilisation = value;                
                this.ConfigureCamera();
            }
        }

        /// <summary>
        /// Configure the exposure compensation of the camera. Doing so will produce a lighter/darker image beyond the recommended exposure.
        /// </summary>
        public int ExposureCompensation
        {
            get
            {
                return this.GetExposureCompensation();
            }
            set
            {            
                MMALCameraConfigImpl.Config.ExposureCompensation = value;                
                this.ConfigureCamera();
            }
        }

        /// <summary>
        /// Configure the exposure mode used by the camera
        /// </summary>        
        public MMAL_PARAM_EXPOSUREMODE_T ExposureMode
        {
            get
            {
                return this.GetExposureMode();
            }
            set
            {               
                MMALCameraConfigImpl.Config.ExposureMode = value;                
                this.ConfigureCamera();
            }
        }

        /// <summary>
        /// Configure the exposure metering mode to be used by the camera. The metering mode determines how the camera measures exposure.
        /// 
        /// Spot metering (MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_SPOT):
        /// 
        /// With spot metering, the camera will only measure a very small area of the scene and ignores everything else.
        /// On the Raspberry Pi camera, this will be the very centre of the image. 
        /// 
        /// Average metering (MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE):
        /// 
        /// Using this metering mode, the camera will use the light information coming from the entire scene. It does not focus on any particular
        /// area of the scene.
        /// 
        /// Matrix metering (MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_MATRIX):
        /// 
        /// Matrix metering works by dividing the entire frame into multiple "zones" which are then analysed on an individual basis for light and dark tones.
        /// 
        /// 
        /// Sources:
        /// https://photographylife.com/understanding-metering-modes
        /// https://en.wikipedia.org/wiki/Metering_mode#Spot_metering
        /// 
        /// </summary>        
        public MMAL_PARAM_EXPOSUREMETERINGMODE_T ExposureMeterMode
        {
            get
            {
                return this.GetExposureMeteringMode();
            }
            set
            {              
                MMALCameraConfigImpl.Config.ExposureMeterMode = value;                
                this.ConfigureCamera();
            }
        }
                
        /// <summary>
        /// Configure the Auto White Balance to be used by the camera
        /// </summary>
        public MMAL_PARAM_AWBMODE_T AwbMode
        {
            get
            {
                return this.GetAwbMode();
            }
            set
            {               
                MMALCameraConfigImpl.Config.AwbMode = value;                
                this.ConfigureCamera();
            }
        }
                
        /// <summary>
        /// Configure any image effects to be used by the camera
        /// </summary>
        public MMAL_PARAM_IMAGEFX_T ImageEffect
        {
            get
            {
                return this.GetImageFx();
            }
            set
            {              
                MMALCameraConfigImpl.Config.ImageEffect = value;
                this.ConfigureCamera();
            }
        }
            
        /// <summary>
        /// Specify the rotation of the image, this value should be multiples of 90
        /// </summary>                    
        public int Rotation
        {
            get
            {
                return this.GetRotation();
            }
            set
            {              
                MMALCameraConfigImpl.Config.Rotation = value;
                this.ConfigureCamera();                
            }
        }
         
        /// <summary>
        /// Specify whether the image should be flipped
        /// </summary>                       
        public MMAL_PARAM_MIRROR_T Flips
        {
            get
            {
                return this.GetFlips();
            }
            set
            {                
                MMALCameraConfigImpl.Config.Flips = value;
                this.ConfigureCamera();                
            }
        }
        
        /// <summary>
        /// Configure the shutter speed of the camera. This value is the length of time
        /// that the sensor is exposed to light. 
        /// </summary>                        
        public int ShutterSpeed
        {
            get
            {
                return this.GetShutterSpeed();
            }
            set
            {               
                MMALCameraConfigImpl.Config.ShutterSpeed = value;
                this.ConfigureCamera();                
            }
        }

        #endregion

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

            if (MMALCameraConfigImpl.Config == null)
                MMALCameraConfigImpl.Config = new MMALCameraConfig();
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
        /// Captures a single image from the camera's still port. Initializes a standalone MMALImageEncoder using the provided encodingType and quality.
        /// </summary>
        /// <typeparam name="T"></typeparam>        
        /// <param name="handler"></param>
        /// <param name="encodingType"></param>
        /// <param name="quality"></param>
        /// <param name="useExif"></param>
        /// <param name="exifTags"></param>
        /// <returns></returns>
        public async Task TakeSinglePicture<T>(ICaptureHandler<T> handler, int encodingType = 0, int quality = 0, bool useExif = true, bool raw = false, params ExifTag[] exifTags)
        {
            Console.WriteLine("Preparing to take picture");
            var camPreviewPort = this.Camera.PreviewPort;
            var camVideoPort = this.Camera.VideoPort;
            var camStillPort = this.Camera.StillPort;
            
            using (var encoder = new MMALImageEncoder(encodingType, quality))
            {
                if (useExif)
                    ((MMALImageEncoder)encoder).AddExifTags((MMALImageEncoder)encoder, exifTags);

                //Create connections
                if (this.Preview == null)
                    Helpers.PrintWarning("Preview port does not have a Render component configured. Resulting image will be affected.");
                else
                {
                    if (this.Preview.Connection == null)
                        this.Preview.CreateConnection(camPreviewPort);
                }

                if(this.Encoders.Any(c => c?.Connection.OutputPort == this.Camera.StillPort))
                {
                    var enc = this.Encoders.Where(c => c.Connection.OutputPort == this.Camera.StillPort).FirstOrDefault();
                    this.Encoders.Remove(enc);
                    if(enc.Connection != null)
                        enc.Connection.Destroy();
                }

                encoder.CreateConnection(camStillPort);

                if (raw)
                    camStillPort.SetRawCapture(true);

                if (MMALCameraConfigImpl.Config.EnableAnnotate)
                    encoder.AnnotateImage();

                int outputPort = 0;

                //Enable the image encoder output port.
                encoder.Start(outputPort, encoder.ManagedCallback, handler.Process);

                this.StartCapture(camStillPort);
                
                //Wait until the process is complete.
                await encoder.Outputs.ElementAt(outputPort).Trigger.WaitAsync();

                this.StopCapture(camStillPort);

                //Close open connections.
                encoder.CloseConnection();
                
                //Disable the image encoder output port.
                encoder.Stop(outputPort);

                handler.PostProcess();
            }            
        }


        /// <summary>
        /// Captures a single image from the output port specified. Expects an MMALImageEncoder to be attached.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="outputPort"></param>
        /// <param name="handler"></param>
        /// <param name="encodingType"></param>
        /// <param name="quality"></param>
        /// <param name="useExif"></param>
        /// <param name="exifTags"></param>
        /// <returns></returns>
        public async Task TakePicture<T>(MMALPortImpl connPort, int outputPort, ICaptureHandler<T> handler, bool useExif = true, bool raw = false, params ExifTag[] exifTags)
        {          
            
            Console.WriteLine("Preparing to take picture");
            var camPreviewPort = this.Camera.PreviewPort;
            var camVideoPort = this.Camera.VideoPort;
            var camStillPort = this.Camera.StillPort;

            //Find the encoder/decoder which is connected to the output port specified.
            var encoder = this.Encoders.Where(c => c?.Connection.OutputPort == connPort).FirstOrDefault();
            
            if (encoder == null || encoder.GetType() != typeof(MMALImageEncoder))
                throw new PiCameraError("No image encoder currently attached to output port specified");
                        
            if (useExif)
                ((MMALImageEncoder)encoder).AddExifTags((MMALImageEncoder)encoder, exifTags);

            //Create connections
            if (this.Preview == null)
                Helpers.PrintWarning("Preview port does not have a Render component configured. Resulting image will be affected.");
            else
            {
                if (this.Preview.Connection == null)
                    this.Preview.CreateConnection(camPreviewPort);
            }
                        
            if (raw)                
                camStillPort.SetRawCapture(true);

            if (MMALCameraConfigImpl.Config.EnableAnnotate)
                encoder.AnnotateImage();
            
            //Enable the image encoder output port.
            encoder.Start(outputPort, encoder.ManagedCallback, handler.Process);

            this.StartCapture(camStillPort);

            //Wait until the process is complete.
            await encoder.Outputs.ElementAt(0).Trigger.WaitAsync();

            this.StopCapture(camStillPort);

            //Close open connections.
            //encoder.CloseConnection();
            //this.Preview.CloseConnection();

            //Disable the image encoder output port.
            encoder.Stop(outputPort);           
        }

        /// <summary>
        /// Takes a number of images as specified by the iterations parameter.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="extension"></param>
        /// <param name="encodingType"></param>
        /// <param name="quality"></param>
        /// <param name="iterations"></param>
        /// <param name="useExif"></param>
        /// <param name="exifTags"></param>
        public async Task TakePictureIterative(MMALPortImpl connPort, int outputPort, string directory, string extension, int iterations, bool useExif = true, bool raw = false, params ExifTag[] exifTags)
        {            
            for (int i = 0; i < iterations; i++)
            {
                var filename = (directory.EndsWith("/") ? directory : directory + "/") + DateTime.Now.ToString("dd-MMM-yy HH-mm-ss") + (extension.StartsWith(".") ? extension : "." + extension);

                using (var fs = File.Create(filename))
                {
                    await TakePicture(connPort, outputPort, new StreamCaptureResult(fs), useExif, raw, exifTags);
                }                    
            }                        
        }

        /// <summary>
        /// Takes images until the moment specified in the timeout parameter has been met.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="extension"></param>
        /// <param name="encodingType"></param>
        /// <param name="quality"></param>
        /// <param name="timeout"></param>
        /// <param name="useExif"></param>
        /// <param name="exifTags"></param>
        public async Task TakePictureTimeout(MMALPortImpl connPort, int outputPort, string directory, string extension, DateTime timeout, bool useExif = true, bool raw = false, params ExifTag[] exifTags)
        {
            while (DateTime.Now.CompareTo(timeout) < 0)
            {
                var filename = (directory.EndsWith("/") ? directory : directory + "/") + DateTime.Now.ToString("dd-MMM-yy HH-mm-ss") + (extension.StartsWith(".") ? extension : "." + extension);

                using (var fs = File.Create(filename))
                {
                    await TakePicture(connPort, outputPort, new StreamCaptureResult(fs), useExif, raw, exifTags);
                }                    
            }
        }

        /// <summary>
        /// Takes a timelapse image. You can specify the interval between each image taken and also when the operation should finish.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="extension"></param>
        /// <param name="encodingType"></param>
        /// <param name="quality"></param>
        /// <param name="tl"></param>
        /// <param name="timeout"></param>
        /// <param name="useExif"></param>
        /// <param name="raw"></param>
        /// <param name="exifTags"></param>
        /// <returns></returns>
        public async Task TakePictureTimelapse(MMALPortImpl connPort, int outputPort, string directory, string extension, Timelapse tl, DateTime timeout, bool useExif = true, bool raw = false, params ExifTag[] exifTags)
        {
            int interval = 0;

            while (DateTime.Now.CompareTo(timeout) < 0)
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

                using (var fs = File.Create(filename))
                {
                    await TakePicture(connPort, outputPort, new StreamCaptureResult(fs), useExif, raw, exifTags);
                }                    
            }
        }

        public MMALCamera CreatePreviewComponent(MMALRendererBase renderer)
        {            
            if(this.Preview != null)
            {
                this.Preview?.Connection.Disable();
                this.Preview?.Connection.Destroy();
            }

            this.Preview = renderer;
            this.Preview.CreateConnection(this.Camera.PreviewPort);
            return this;
        }

        public MMALCamera CreateSplitterComponent()
        {
            this.Splitter = new MMALSplitterComponent();
            return this;
        }
        
        /// <summary>
        /// Provides a facility to attach an encoder/decoder component to an upstream component's output port
        /// </summary>
        /// <param name="encoder"></param>
        /// <param name="outputPort"></param>
        /// <returns></returns>
        public MMALCamera AddEncoder(MMALEncoderBase encoder, MMALPortImpl outputPort)
        {
            var enc = this.Encoders.Where(c => c?.Connection.OutputPort == outputPort).FirstOrDefault();

            if(enc != null)
            {
                //Dispose of encoder that's connected to this output port and remove from list of encoders.
                enc.DisableComponent();
                enc.Dispose();
                this.Encoders.Remove(enc);
            }

            encoder.CreateConnection(outputPort);
            this.Encoders.Add(encoder);
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
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine("Configuring camera parameters.");

            this.DisableCamera();
            
            this.SetSaturation(MMALCameraConfigImpl.Config.Saturation);
            this.SetSharpness(MMALCameraConfigImpl.Config.Sharpness);
            this.SetContrast(MMALCameraConfigImpl.Config.Contrast);
            this.SetBrightness(MMALCameraConfigImpl.Config.Brightness);
            this.SetISO(MMALCameraConfigImpl.Config.ISO);
            this.SetVideoStabilisation(MMALCameraConfigImpl.Config.VideoStabilisation);
            this.SetExposureCompensation(MMALCameraConfigImpl.Config.ExposureCompensation);
            this.SetExposureMode(MMALCameraConfigImpl.Config.ExposureMode);
            this.SetExposureMeteringMode(MMALCameraConfigImpl.Config.ExposureMeterMode);
            this.SetAwbMode(MMALCameraConfigImpl.Config.AwbMode);
            this.SetAwbGains(MMALCameraConfigImpl.Config.AwbGainsR, MMALCameraConfigImpl.Config.AwbGainsB);
            this.SetImageFx(MMALCameraConfigImpl.Config.ImageEffect);
            this.SetColourFx(MMALCameraConfigImpl.Config.Effects);
            this.SetRotation(MMALCameraConfigImpl.Config.Rotation);
            this.SetShutterSpeed(MMALCameraConfigImpl.Config.ShutterSpeed);
            this.SetStatsPass(MMALCameraConfigImpl.Config.StatsPass);
            this.SetDRC(MMALCameraConfigImpl.Config.DrcLevel);
            this.SetFlips(MMALCameraConfigImpl.Config.Flips);
            this.SetCrop(MMALCameraConfigImpl.Config.Crop);
            
            this.EnableCamera();

            return this;
        }
                
        public void Dispose()
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine("Destroying final components");
            if (this.Camera != null)
                this.Camera.Dispose();
            this.Encoders.ForEach(c => c.Dispose());

            if (this.Preview != null)
                this.Preview.Dispose();
            BcmHost.bcm_host_deinit();
        }
    }




}
