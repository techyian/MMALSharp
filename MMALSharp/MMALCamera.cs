using MMALSharp.Components;
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
    public class MMALCamera : IDisposable
    {        
        public MMALCameraComponent Camera { get; set; }
        public MMALEncoderComponent Encoder { get; set; }
        public MMALNullSinkComponent NullSink { get; set; }
        private MMALCameraConfig Config { get; set; }

        #region Configuration Properties
                
        public double Sharpness
        {
            get
            {
                return this.GetSharpness();
            }
            set
            {
                this.DisableCamera();
                this.Config.Sharpness = value;
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
                this.DisableCamera();
                this.Config.Contrast = value;
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
                this.DisableCamera();
                this.Config.Brightness = value;
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
                this.DisableCamera();
                this.Config.Saturation = value;
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
                this.DisableCamera();
                this.Config.ISO = value;
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
                this.DisableCamera();
                this.Config.VideoStabilisation = value;                
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
                this.DisableCamera();
                this.Config.ExposureCompensation = value;                
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
                this.DisableCamera();
                this.Config.ExposureMode = value;                
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
                this.DisableCamera();
                this.Config.ExposureMeterMode = value;                
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
                this.DisableCamera();
                this.Config.AwbMode = value;                
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
                this.DisableCamera();
                this.Config.ImageEffect = value;
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
                this.DisableCamera();
                this.Config.Rotation = value;
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
                this.DisableCamera();
                this.Config.Flips = value;
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
                this.DisableCamera();
                this.Config.ShutterSpeed = value;
                this.ConfigureCamera();                
            }
        }
        
        #endregion

        public MMALCamera(MMALCameraConfig config)
        {
            this.Config = config;

            BcmHost.bcm_host_init();            
            this.Camera = new MMALCameraComponent();
            this.Encoder = new MMALEncoderComponent();
            this.NullSink = new MMALNullSinkComponent();            
        }

        public void TakePicture(string filename)
        {
            var previewPort = this.Camera.PreviewPort;
            var videoPort = this.Camera.VideoPort;
            var stillPort = this.Camera.StillPort;

            var encInput = this.Encoder.Inputs.ElementAt(0);
                        
            var encOutput = this.Encoder.Outputs.ElementAt(0);
            encOutput.Storage = null;

            var nullSinkInputPort = this.NullSink.Inputs.ElementAt(0);
            var nullSinkConnection = MMALConnectionImpl.CreateConnection(previewPort, nullSinkInputPort);

            var encConection = MMALConnectionImpl.CreateConnection(stillPort, encInput);

            encOutput.EnablePort(this.Camera.CameraBufferCallback);

            var length = this.Encoder.BufferPool.Queue.QueueLength();

            for (int i = 0; i < length; i++)
            {
                var buffer = this.Encoder.BufferPool.Queue.GetBuffer();
                encOutput.SendBuffer(buffer);
            }

            Console.WriteLine("Attempt capture");
                        
            stillPort.SetImageCapture(true);

            encOutput.Trigger.Wait();
                        
            File.WriteAllBytes(filename, encOutput.Storage);
                        
            encOutput.Storage = null;

            this.Camera.StopCapture();
            


        }
                
        public async Task TakePictureAsync(string filename)
        {
            await Task.Run(async () => {

                var previewPort = this.Camera.PreviewPort;
                var videoPort = this.Camera.VideoPort;
                var stillPort = this.Camera.StillPort;
                
                var encInput = this.Encoder.Inputs.ElementAt(0);
                var encOutput = this.Encoder.Outputs.ElementAt(0);
                encOutput.Storage = null;

                var nullSinkInputPort = this.NullSink.Inputs.ElementAt(0);
                var nullSinkConnection = MMALConnectionImpl.CreateConnection(previewPort, nullSinkInputPort);

                var encConection = MMALConnectionImpl.CreateConnection(stillPort, encInput);

                encOutput.EnablePort(this.Camera.CameraBufferCallback);
                                
                var length = this.Encoder.BufferPool.Queue.QueueLength();
                
                for (int i = 0; i < length; i++)
                {
                    var buffer = this.Encoder.BufferPool.Queue.GetBuffer();
                    encOutput.SendBuffer(buffer);
                }

                Console.WriteLine("Attempt capture");
                stillPort.SetImageCapture(true);

                encOutput.TokenSource = new CancellationTokenSource();

                await Task.Delay(30000, encOutput.TokenSource.Token).ContinueWith(c =>
                {                    
                    encOutput.DisablePort();
                                        
                    File.WriteAllBytes(filename, encOutput.Storage);

                    encOutput.Storage = null;

                    nullSinkConnection.Destroy();
                    encConection.Destroy();
                });
                
                
            });                                              
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
            this.SetSaturation(this.Config.Saturation);
            this.SetSharpness(this.Config.Sharpness);
            this.SetContrast(this.Config.Contrast);
            this.SetBrightness(this.Config.Brightness);
            this.SetISO(this.Config.ISO);
            this.SetVideoStabilisation(this.Config.VideoStabilisation);
            this.SetExposureCompensation(this.Config.ExposureCompensation);
            this.SetExposureMode(this.Config.ExposureMode);
            this.SetExposureMeteringMode(this.Config.ExposureMeterMode);
            this.SetAwbMode(this.Config.AwbMode);
            this.SetAwbGains(this.Config.AwbGainsR, this.Config.AwbGainsB);
            this.SetImageFx(this.Config.ImageEffect);
            this.SetColourFx(this.Config.Effects);
            this.SetRotation(this.Config.Rotation);
            this.SetShutterSpeed(this.Config.ShutterSpeed);

            this.Camera.Initialize();
            this.Encoder.Initialize();
            this.NullSink.Initialize();

            EnableCamera();

            return this;
        }
        
        public void Dispose()
        {            
            //this.Camera.Dispose();
            //this.Encoder.Dispose();
            this.NullSink.Dispose();
            BcmHost.bcm_host_deinit();
        }
    }
}
