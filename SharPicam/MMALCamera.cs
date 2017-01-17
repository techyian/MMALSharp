using SharPicam.Components;
using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharPicam
{
    public class MMALCamera : IDisposable
    {
        public MMALCameraComponent Camera { get; set; }
        public MMALEncoderComponent Encoder { get; set; }
        public MMALNullSinkComponent NullSink { get; set; }

        #region Configuration Properties

        private int _sharpness;
        public int Sharpness {
            get { return this._sharpness; }
            set
            {
                this.Camera.SetSharpness(value);
                this._sharpness = value;
            }
        }

        private int _contrast;
        public int Contrast {
            get { return this._contrast; }
            set
            {
                this.Camera.SetContrast(value);
                this._contrast = value;
            }
        }

        private int _brightness;
        public int Brightness {
            get { return this._brightness; }
            set
            {
                this.Camera.SetBrightness(value);
                this._brightness = value;
            }
        }

        private int _saturation;
        public int Saturation {
            get { return this._saturation; }
            set
            {
                this.Camera.SetSaturation(value);
                this._saturation = value;
            }
        }

        private int _iso;
        public int ISO {
            get { return this._iso; }
            set
            {
                this.Camera.SetISO(value);
                this._iso = value;
            }
        }

        private bool _videoStabilisation;
        public bool VideoStabilisation {
            get { return this._videoStabilisation; }
            set
            {
                this.Camera.SetVideoStabilisation(value);
                this._videoStabilisation = value;
            }
        }

        private int _exposureCompensation;
        public int ExposureCompensation {
            get { return this._exposureCompensation; }
            set
            {
                this.Camera.SetExposureCompensation(value);
                this._exposureCompensation = value;
            }
        }

        private MMAL_PARAM_EXPOSUREMODE_T _exposureMode;
        public MMAL_PARAM_EXPOSUREMODE_T ExposureMode {
            get { return this._exposureMode; }
            set
            {
                this.Camera.SetExposureMode(value);
                this._exposureMode = value;
            }
        }

        private MMAL_PARAM_EXPOSUREMETERINGMODE_T _exposureMeterMode;
        public MMAL_PARAM_EXPOSUREMETERINGMODE_T ExposureMeterMode {
            get { return this._exposureMeterMode; }
            set
            {
                this.Camera.SetExposureMeteringMode(value);
                this._exposureMeterMode = value;
            }
        }

        private MMAL_PARAM_AWBMODE_T _awbMode;
        public MMAL_PARAM_AWBMODE_T AwbMode {
            get { return this._awbMode; }
            set
            {
                this.Camera.SetAwbMode(value);
                this._awbMode = value;
            }
        }

        private MMAL_PARAM_IMAGEFX_T _imageEffect;
        public MMAL_PARAM_IMAGEFX_T ImageEffect {
            get { return this._imageEffect; }
            set
            {
                this.Camera.SetImageFx(value);
                this._imageEffect = value;
            }
        }

        private int _rotation;
        public int Rotation {
            get { return this._rotation; }
            set
            {
                this.Camera.SetRotation(value);
                this._rotation = value;
            }
        }

        private Tuple<bool, bool> _flips;
        public Tuple<bool, bool> Flips {
            get { return this._flips; }
            set
            {
                this.Camera.SetFlips(value.Item1, value.Item2);
            }
        }

        private int _shutterSpeed;
        public int ShutterSpeed {
            get { return this._shutterSpeed; }
            set
            {
                this.Camera.SetShutterSpeed(value);
                this._shutterSpeed = value;
            }
        }

        #endregion


        public MMALCamera()
        {
            BcmHost.bcm_host_init();
            this.Camera = new MMALCameraComponent();
            this.Encoder = new MMALEncoderComponent();
            this.NullSink = new MMALNullSinkComponent();            
        }
                
        public async Task TakePicture(string filename)
        {
            await Task.Factory.StartNew(() => {

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

                //Should ideally have this running on a timer in case something goes wrong with the callbacks.
                while (encOutput.Triggered == 0) ;
                
                encOutput.DisablePort();
                
                Console.WriteLine("Triggered flag has been set.");

                File.WriteAllBytes(filename, encOutput.Storage);
                
                encOutput.Storage = null;
            });                                              
        }

        public void Dispose()
        {            
            this.Camera.Dispose();
            BcmHost.bcm_host_deinit();
        }
    }
}
