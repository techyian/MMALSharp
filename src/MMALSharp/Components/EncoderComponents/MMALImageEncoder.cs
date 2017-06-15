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

        public MMALImageEncoder(ICaptureHandler handler) : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_ENCODER, handler)
        {
            this.Inputs = new List<MMALPortImpl>();
            for (int i = 0; i < this.Ptr->InputNum; i++)
            {
                this.Inputs.Add(new MMALStillPort(&(*this.Ptr->Input[i]), this, PortType.Input));
            }

            this.Outputs = new List<MMALPortImpl>();
            for (int i = 0; i < this.Ptr->OutputNum; i++)
            {
                this.Outputs.Add(new MMALStillPort(&(*this.Ptr->Output[i]), this, PortType.Output));
            }

            this.InputPort = this.Inputs.ElementAt(0);
            this.OutputPort = this.Outputs.ElementAt(0);

            
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

            var bytes = Encoding.ASCII.GetBytes(formattedExif);

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_EXIF_T>() + (bytes.Length - 1));

            var str = new MMAL_PARAMETER_EXIF_T(new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_EXIF,
                (Marshal.SizeOf<MMAL_PARAMETER_EXIF_T_DUMMY>() + (bytes.Length - 1))
            ), 0, 0, 0, bytes);

            Marshal.StructureToPtr(str, ptr, false);

            MMALCheck(MMALPort.mmal_port_parameter_set(this.Outputs.ElementAt(0).Ptr, (MMAL_PARAMETER_HEADER_T*)ptr), $"Unable to set EXIF {formattedExif}");

            Marshal.FreeHGlobal(ptr);
        }
        
    }

    public class MMALImageEncoderConverter : MMALImageEncoder, IMMALConvert
    {
        public unsafe MMALImageEncoderConverter(TransformStreamCaptureHandler handler) : base(handler)
        {
            this.Inputs = new List<MMALPortImpl>();
            for (int i = 0; i < this.Ptr->InputNum; i++)
            {
                this.Inputs.Add(new MMALStillConvertPort(&(*this.Ptr->Input[i]), this, PortType.Input));
            }

            this.Outputs = new List<MMALPortImpl>();
            for (int i = 0; i < this.Ptr->OutputNum; i++)
            {
                this.Outputs.Add(new MMALStillConvertPort(&(*this.Ptr->Output[i]), this, PortType.Output));
            }

            this.InputPort = this.Inputs.ElementAt(0);
            this.OutputPort = this.Outputs.ElementAt(0);
        }

        /// <summary>
        /// Encodes/decodes user provided image data
        /// </summary>
        /// <param name="outputPort">The output port to begin processing on. Usually will be 0.</param>
        /// <returns>An awaitable task</returns>
        public virtual async Task Convert(int outputPort = 0)
        {
            //Enable both input and output ports. Ports should have been pre-configured by user prior to this point.
            this.Start(this.InputPort, this.ManagedInputCallback);
            this.Start(this.OutputPort, new Action<MMALBufferImpl, MMALPortBase>(this.ManagedOutputCallback));

            this.EnableComponent();

            //Wait until the process is complete.
            this.Outputs.ElementAt(outputPort).Trigger = new Nito.AsyncEx.AsyncCountdownEvent(1);
            await this.Outputs.ElementAt(outputPort).Trigger.WaitAsync();

            this.DisableComponent();

            this.CleanPortPools();
        }


    }
    
}
