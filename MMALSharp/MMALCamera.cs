using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static MMALSharp.Native.MMALParameters;

namespace MMALSharp
{
    public sealed class MMALCamera : IDisposable
    {
        public MMALCameraComponent Camera { get; set; }
        public List<MMALEncoderBase> Encoders { get; set; }            
        public MMALRendererBase Preview { get; set; }
                
        #region Configuration Properties
                
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

        public MMALCamera(MMALCameraConfig config)
        {
            MMALCameraConfigImpl.Config = config;

            BcmHost.bcm_host_init();            
            this.Camera = new MMALCameraComponent();
            this.Encoders = new List<MMALEncoderBase>();
            this.Preview = new MMALNullSinkComponent();            
        }

        public void StartCapture(MMALPortImpl port)
        {
            if(port == this.Camera.StillPort || this.Encoders.Any(c => c.Enabled))
                port.SetImageCapture(true);
        }

        public void StopCapture(MMALPortImpl port)
        {
            if (port == this.Camera.StillPort || this.Encoders.Any(c => c.Enabled))
                port.SetImageCapture(false);
        }

        /// <summary>
        /// Captures a single image from the camera still port and processes it using the Image Encoder component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="encodingType"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        public T TakePicture<T>(ICaptureHandler<T> handler, uint encodingType, uint quality)
        {
            Console.WriteLine("Preparing to take picture");
            var camPreviewPort = this.Camera.PreviewPort;
            var camVideoPort = this.Camera.VideoPort;
            var camStillPort = this.Camera.StillPort;

            var encoder = CreateImageEncoder(encodingType, quality);
                      
            //Create connections
            this.Preview.CreateConnection(camPreviewPort);
            encoder.CreateConnection(camStillPort);

            //Enable the image encoder output port.
            encoder.Start();
            
            Console.WriteLine("Attempt capture");

            this.StartCapture(camStillPort);

            encoder.Outputs.ElementAt(0).Trigger.Wait();

            this.StopCapture(camStillPort);
            
            return handler.Process(encoder.Storage);
        }

        public void TakePictureIterative(FileCaptureHandler handler, uint encodingType, uint quality, int iterations, DateTime timeout)
        {

        }

        public MMALImageEncoder CreateImageEncoder(uint encodingType, uint quality)
        {            
            return new MMALImageEncoder(encodingType, quality);
        }
        
        public void DisableCamera()
        {
            this.Encoders.ForEach(c => c.DisableComponent());
            this.Preview.DisableComponent();
            this.Camera.DisableComponent();
        }

        public void EnableCamera()
        {
            this.Encoders.ForEach(c => c.EnableComponent());
            this.Preview.EnableComponent();
            this.Camera.EnableComponent();
        }

        public MMALCamera ConfigureCamera()
        {
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

            this.Camera.Initialize();
            this.Encoders.ForEach(c => c.Initialize());
            this.Preview.Initialize();

            this.EnableCamera();

            return this;
        }
        
        public void Dispose()
        {            
            Console.WriteLine("Disabling ports and destroying components");
            if(this.Camera != null)
                this.Camera.Dispose();
            this.Encoders.ForEach(c => c.Dispose());

            if(this.Preview != null)
                this.Preview.Dispose();
            BcmHost.bcm_host_deinit();
        }
    }
}
