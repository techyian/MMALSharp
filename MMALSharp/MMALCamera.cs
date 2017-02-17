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
        /// <param name="useExif"></param>
        /// <param name="exifTags"></param>
        /// <returns></returns>
        public async Task<T> TakePicture<T>(ICaptureHandler<T> handler, uint encodingType, uint quality, bool useExif = true, bool raw = false, params ExifTag[] exifTags)
        {            
            Console.WriteLine("Preparing to take picture");
            var camPreviewPort = this.Camera.PreviewPort;
            var camVideoPort = this.Camera.VideoPort;
            var camStillPort = this.Camera.StillPort;

            using (var encoder = CreateImageEncoder(encodingType, quality))
            {
                if (useExif)
                    AddExifTags(encoder, exifTags);

                //Create connections
                if (this.Preview.Connection != null)
                    this.Preview.CreateConnection(camPreviewPort);
                encoder.CreateConnection(camStillPort);

                if (raw)                
                    camStillPort.SetRawCapture(true);

                if (MMALCameraConfigImpl.Config.EnableAnnotate)
                    AnnotateImage();

                //Enable the image encoder output port.
                encoder.Start();

                this.StartCapture(camStillPort);

                //Wait until the process is complete.
                await encoder.Outputs.ElementAt(0).Trigger.WaitAsync();

                this.StopCapture(camStillPort);

                //Close open connections.
                encoder.CloseConnection();
                this.Preview.CloseConnection();

                //Disable the image encoder output port.
                encoder.Outputs.ElementAt(0).DisablePort();

                //Return the data to the client in whatever format requested.
                return handler.Process(encoder.Storage);
            }           
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
        public async Task TakePictureIterative(string directory, string extension, uint encodingType, uint quality, int iterations, bool useExif = true, bool raw = false, params ExifTag[] exifTags)
        {            
            for(int i = 0; i < iterations; i++)
            {
                var filename = (directory.EndsWith("/") ? directory : directory + "/") + DateTime.Now.ToString("dd-MMM-yy HH-mm-ss") + (extension.StartsWith(".") ? extension : "." + extension);                
                await TakePicture(new FileCaptureHandler(filename), encodingType, quality, useExif, raw, exifTags);
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
        public async Task TakePictureTimeout(string directory, string extension, uint encodingType, uint quality, DateTime timeout, bool useExif = true, bool raw = false, params ExifTag[] exifTags)
        {
            while(DateTime.Now.CompareTo(timeout) < 0)
            {
                var filename = (directory.EndsWith("/") ? directory : directory + "/") + DateTime.Now.ToString("dd-MMM-yy HH-mm-ss") + (extension.StartsWith(".") ? extension : "." + extension);
                await TakePicture(new FileCaptureHandler(filename), encodingType, quality, useExif, raw, exifTags);
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
        public async Task TakePictureTimelapse(string directory, string extension, uint encodingType, uint quality, Timelapse tl, DateTime timeout, bool useExif = true, bool raw = false, params ExifTag[] exifTags)
        {
            int interval = 0;

            while(DateTime.Now.CompareTo(timeout) < 0)
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
                await TakePicture(new FileCaptureHandler(filename), encodingType, quality, useExif, raw, exifTags);
            }
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
            if(MMALCameraConfigImpl.Config.Debug)
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
                
        public void AddExifTags(MMALImageEncoder encoder, params ExifTag[] exifTags)
        {
            //Add the same defaults as per Raspistill.c
            List<ExifTag> defaultTags = new List<ExifTag>
            {                
                new ExifTag { Key = "IFD0.Model", Value = "RP_" + this.Camera.CameraInfo.SensorName },
                new ExifTag { Key = "IFD0.Make", Value = "RaspberryPi" },
                new ExifTag { Key = "EXIF.DateTimeDigitized", Value = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss") },
                new ExifTag { Key = "EXIF.DateTimeOriginal", Value = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss") },
                new ExifTag { Key = "IFD0.DateTime", Value = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss") }
            };

            defaultTags.ForEach(c => encoder.AddExifTag(c));

            if ((defaultTags.Count + exifTags.Length) > 32)
                throw new PiCameraError("Maximum number of EXIF tags exceeded.");

            //Add user defined tags.                 
            foreach(ExifTag tag in exifTags)
            {
                encoder.AddExifTag(tag);
            }
        }
        
        public unsafe void AnnotateImage()
        {
            if(MMALCameraConfigImpl.Config.Annotate != null)
            {
                Console.WriteLine("Setting annotate");
                MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T str = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T();
                str.hdr = new MMAL_PARAMETER_HEADER_T((uint)MMALParametersCamera.MMAL_PARAMETER_ANNOTATE, (uint)Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>());
                str.enable = 1;

                StringBuilder sb = new StringBuilder();

                if (!string.IsNullOrEmpty(MMALCameraConfigImpl.Config.Annotate.CustomText))
                {
                    sb.Append(MMALCameraConfigImpl.Config.Annotate.CustomText + " ");
                }

                if(MMALCameraConfigImpl.Config.Annotate.ShowTimeText)
                {
                    sb.Append(DateTime.Now.ToString("HH:mm") + " ");
                }

                if (MMALCameraConfigImpl.Config.Annotate.ShowDateText)
                {
                    sb.Append(DateTime.Now.ToString("dd/MM/yyyy") + " ");
                }

                if (MMALCameraConfigImpl.Config.Annotate.ShowShutterSettings)
                    str.showShutter = 1;

                if (MMALCameraConfigImpl.Config.Annotate.ShowGainSettings)
                    str.showAnalogGain = 1;

                if (MMALCameraConfigImpl.Config.Annotate.ShowLensSettings)
                    str.showLens = 1;

                if (MMALCameraConfigImpl.Config.Annotate.ShowCafSettings)
                    str.showCaf = 1;

                if (MMALCameraConfigImpl.Config.Annotate.ShowMotionSettings)
                    str.showMotion = 1;

                if (MMALCameraConfigImpl.Config.Annotate.ShowFrameNumber)
                    str.showFrameNum = 1;

                if (MMALCameraConfigImpl.Config.Annotate.ShowBlackBackground)
                    str.enableTextBackground = 1;

                str.textSize = Convert.ToByte(MMALCameraConfigImpl.Config.Annotate.TextSize);
                
                if(MMALCameraConfigImpl.Config.Annotate.TextColour != -1)
                {
                    str.customTextColor = 1;
                    str.customTextY = Convert.ToByte((MMALCameraConfigImpl.Config.Annotate.TextColour & 0xff));
                    str.customTextU = Convert.ToByte(((MMALCameraConfigImpl.Config.Annotate.TextColour >> 8) & 0xff));
                    str.customTextV = Convert.ToByte(((MMALCameraConfigImpl.Config.Annotate.TextColour >> 16 ) & 0xff));
                }
                else
                {
                    str.customTextColor = 0;
                }

                if (MMALCameraConfigImpl.Config.Annotate.BgColour != -1)
                {
                    str.customBackgroundColor = 1;
                    str.customBackgroundY = Convert.ToByte((MMALCameraConfigImpl.Config.Annotate.BgColour & 0xff));
                    str.customBackgroundU = Convert.ToByte(((MMALCameraConfigImpl.Config.Annotate.BgColour >> 8) & 0xff));
                    str.customBackgroundV = Convert.ToByte(((MMALCameraConfigImpl.Config.Annotate.BgColour >> 16) & 0xff));
                }
                else
                {
                    str.customBackgroundColor = 0;
                }

                string t = sb.ToString() + char.MinValue;

                var text = Encoding.ASCII.GetBytes(t);

                str.text = text;
                str.hdr = new MMAL_PARAMETER_HEADER_T((uint) MMALParametersCamera.MMAL_PARAMETER_ANNOTATE, (uint) (Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>() + (t.Length)));

                var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>());
                Marshal.StructureToPtr(str, ptr, false);

                MMALCheck(MMALPort.mmal_port_parameter_set(this.Camera.Control.Ptr, (MMAL_PARAMETER_HEADER_T*)ptr), "Unable to set annotate");

                Marshal.FreeHGlobal(ptr);
            }
        }

        public void Dispose()
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine("Destroying final components");
            if(this.Camera != null)
                this.Camera.Dispose();
            this.Encoders.ForEach(c => c.Dispose());

            if(this.Preview != null)
                this.Preview.Dispose();
            BcmHost.bcm_host_deinit();
        }
    }




}
