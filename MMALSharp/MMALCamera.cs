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
        internal MMALCameraComponent Camera { get; set; }
        internal MMALEncoderComponent Encoder { get; set; }
        internal MMALNullSinkComponent NullSink { get; set; }
        internal List<MMALConnectionImpl> Connections { get; set; }
        
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
            this.Encoder = new MMALEncoderComponent();
            this.NullSink = new MMALNullSinkComponent();            
        }

        public T TakePicture<T>(ICaptureHandler<T> handler)
        {
            Console.WriteLine("Preparing to take picture");
            var previewPort = this.Camera.PreviewPort;
            var videoPort = this.Camera.VideoPort;
            var stillPort = this.Camera.StillPort;

            var encInput = this.Encoder.Inputs.ElementAt(0);                        
            var encOutput = this.Encoder.Outputs.ElementAt(0);

            encOutput.Storage = null;
                        
            var nullSinkInputPort = this.NullSink.Inputs.ElementAt(0);

            //Create connections
            var nullSinkConnection = MMALConnectionImpl.CreateConnection(previewPort, nullSinkInputPort);
            var encConection = MMALConnectionImpl.CreateConnection(stillPort, encInput);

            if (this.Connections == null)
                this.Connections = new List<MMALConnectionImpl>();

            this.Connections.Add(encConection);
            this.Connections.Add(nullSinkConnection);
                                    
            encOutput.EnablePort(this.Camera.CameraBufferCallback);
            
            Console.WriteLine("Attempt capture");
                        
            stillPort.SetImageCapture(true);

            encOutput.Trigger.Wait();
                                   
            this.Camera.StopCapture();            

            return handler.Process(encOutput.Storage);
        }
        
        public void DisableCamera()
        {
            this.Encoder.DisableComponent();
            this.NullSink.DisableComponent();
            this.Camera.DisableComponent();
        }

        public void EnableCamera()
        {
            this.Encoder.EnableComponent();
            this.NullSink.EnableComponent();
            this.Camera.EnableComponent();
        }

        public MMALCamera ConfigureCamera()
        {
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
            this.Encoder.Initialize();
            this.NullSink.Initialize();

            this.EnableCamera();

            return this;
        }
        
        public void Dispose()
        {
            //Close any connections we have open.
            Console.WriteLine("Closing connections.");
            foreach (var conn in this.Connections)
            {
                if (conn.Enabled)
                    conn.Disable();
                conn.Destroy();
            }
            
            Console.WriteLine("Disabling ports and destroying components");
            this.Camera.Dispose();
            this.Encoder.Dispose();
            this.NullSink.Dispose();
            BcmHost.bcm_host_deinit();
        }
    }
}
