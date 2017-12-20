using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Common.Handlers;
using MMALSharp.Components.EncoderComponents;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents an image encoder component
    /// </summary>
    public unsafe class MMALImageEncoder : MMALEncoderBase
    {
        public const int MaxExifPayloadLength = 128;

        private int _width;
        private int _height;

        public override int Width
        {
            get
            {
                if (_width == 0)
                {
                    return MMALCameraConfig.StillResolution.Width;
                }
                return _width;
            }
            set { _width = value; }
        }

        public override int Height
        {
            get
            {
                if (_height == 0)
                {
                    return MMALCameraConfig.StillResolution.Height;
                }
                return _height;
            }
            set { _height = value; }
        }

        /// <summary>
        /// When enabled, raw bayer metadata will be included in JPEG still captures
        /// </summary>
        public bool RawBayer { get; set; }

        /// <summary>
        /// When enabled, EXIF metadata will be included in image stills
        /// </summary>
        public bool UseExif { get; set; }

        /// <summary>
        /// Custom list of user defined EXIF metadata
        /// </summary>
        public ExifTag[] ExifTags { get; set; }
        
        public MMALImageEncoder(ICaptureHandler handler, bool rawBayer = false, bool useExif = true, params ExifTag[] exifTags) : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_ENCODER, handler)
        {
            this.RawBayer = rawBayer;
            this.UseExif = useExif;
            this.ExifTags = exifTags;
        }

        internal override void InitialiseOutputPort(int outputPort)
        {
            this.Outputs[outputPort] = new MMALStillPort(&(*this.Ptr->Output[outputPort]), this, PortType.Output);
        }

        public override void ConfigureOutputPort(int outputPort, MMALEncoding encodingType, MMALEncoding pixelFormat, int quality, int bitrate = 0)
        {            
            base.ConfigureOutputPort(outputPort, encodingType, pixelFormat, quality, bitrate);

            if (this.RawBayer)
            {
                MMALCamera.Instance.Camera.StillPort.SetRawCapture(true);
            }
            if (this.UseExif)
            {
                this.AddExifTags(this.ExifTags);
            }
            if (MMALCameraConfig.EnableAnnotate)
            {
                this.AnnotateImage();
            }
        }

        /// <summary>
        /// Adds EXIF tags to the resulting image
        /// </summary>        
        /// <param name="exifTags">A list of user defined EXIF tags</param>                     
        internal void AddExifTags(params ExifTag[] exifTags)
        {
            //Add the same defaults as per Raspistill.c
            List<ExifTag> defaultTags = new List<ExifTag>
            {
                new ExifTag { Key = "IFD0.Model", Value = "RP_" + MMALCamera.Instance.Camera.CameraInfo.SensorName },
                new ExifTag { Key = "IFD0.Make", Value = "RaspberryPi" },
                new ExifTag { Key = "EXIF.DateTimeDigitized", Value = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss") },
                new ExifTag { Key = "EXIF.DateTimeOriginal", Value = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss") },
                new ExifTag { Key = "IFD0.DateTime", Value = DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss") }
            };

            defaultTags.ForEach(c => this.AddExifTag(c));

            if ((defaultTags.Count + exifTags.Length) > 32)
            {
                throw new PiCameraError("Maximum number of EXIF tags exceeded.");
            }

            //Add user defined tags.                 
            foreach (ExifTag tag in exifTags)
            {
                this.AddExifTag(tag);
            }
        }

        /// <summary>
        /// Provides a facility to add an EXIF tag to the image. 
        /// </summary>
        /// <param name="exifTag">The EXIF tag to add to</param>
        internal void AddExifTag(ExifTag exifTag)
        {
            this.SetDisableExif(false);
            var formattedExif = exifTag.Key + "=" + exifTag.Value + char.MinValue;

            if (formattedExif.Length > MaxExifPayloadLength)
            {
                throw new PiCameraError("EXIF payload greater than allowed max.");
            }

            var arr = new byte[128];
                        
            var bytes = Encoding.ASCII.GetBytes(formattedExif);

            Array.Copy(bytes, arr, bytes.Length);

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_EXIF_T>() + (arr.Length - 1));
            
            var str = new MMAL_PARAMETER_EXIF_T(new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_EXIF,
                (Marshal.SizeOf<MMAL_PARAMETER_EXIF_T_DUMMY>() + (arr.Length - 1))
            ), 0, 0, 0, arr);

            Marshal.StructureToPtr(str, ptr, false);

            MMALCheck(MMALPort.mmal_port_parameter_set(this.Outputs[0].Ptr, (MMAL_PARAMETER_HEADER_T*)ptr), $"Unable to set EXIF {formattedExif}");

            Marshal.FreeHGlobal(ptr);
        }

        public override void PrintComponent()
        {
            base.PrintComponent();
            MMALLog.Logger.Info($"    Width: {this.Width}. Height: {this.Height}");
        }
    }

    public class MMALImageEncoderConverter : MMALImageEncoder, IMMALConvert
    {
        public unsafe MMALImageEncoderConverter(TransformStreamCaptureHandler handler) : base(handler)
        {
        }

        /// <summary>
        /// Encodes/decodes user provided image data
        /// </summary>
        /// <param name="outputPort">The output port to begin processing on. Usually will be 0.</param>
        /// <returns>An awaitable task</returns>
        public virtual async Task Convert(int outputPort = 0)
        {
            //Enable both input and output ports. Ports should have been pre-configured by user prior to this point.
            this.Start(this.Inputs[0], this.ManagedInputCallback);
            this.Start(this.Outputs[outputPort], new Action<MMALBufferImpl, MMALPortBase>(this.ManagedOutputCallback));

            this.EnableComponent();

            //Wait until the process is complete.
            this.Outputs[outputPort].Trigger = new Nito.AsyncEx.AsyncCountdownEvent(1);
            await this.Outputs[outputPort].Trigger.WaitAsync();

            this.DisableComponent();

            this.CleanPortPools();
        }


    }
    
}
