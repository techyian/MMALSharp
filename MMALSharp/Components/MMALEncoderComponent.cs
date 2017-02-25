using MMALSharp.Handlers;
using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static MMALSharp.MMALParameterHelpers;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a base class for all encoder components
    /// </summary>
    public abstract unsafe class MMALEncoderBase : MMALDownstreamComponent
    {        
        protected MMALEncoderBase(string encoderName) : base(encoderName) { }
        
        /// <summary>
        /// Initializes the encoder component to allow processing to commence. Creates the same format between input/output port.
        /// </summary>
        public override void Initialize()
        {
            var input = this.Inputs.ElementAt(0);
            var output = this.Outputs.ElementAt(0);

            input.ShallowCopy(output);
        }

        /// <summary>
        /// Annotates the image with various text as specified in the MMALCameraConfig class.
        /// </summary>
        internal unsafe void AnnotateImage()
        {
            if (MMALCameraConfigImpl.Config.Annotate != null)
            {
                if (MMALCameraConfigImpl.Config.Debug)
                    Console.WriteLine("Setting annotate");
                                                               
                StringBuilder sb = new StringBuilder();

                var showShutter = 0;
                var showAnalogGain = 0;
                var showLens = 0;
                var showCaf = 0;
                var showMotion = 0;
                var showFrame = 0;
                var enableTextBackground = 0;
                var textSize = (byte)0;
                var customTextColor = 0;
                var customTextY = (byte)0;
                var customTextU = (byte)0;
                var customTextV = (byte)0;
                var customBackgroundColor = 0;
                var customBackgroundY = (byte)0;
                var customBackgroundU = (byte)0;
                var customBackgroundV = (byte)0;
                
                if (!string.IsNullOrEmpty(MMALCameraConfigImpl.Config.Annotate.CustomText))
                    sb.Append(MMALCameraConfigImpl.Config.Annotate.CustomText + " ");

                if (MMALCameraConfigImpl.Config.Annotate.ShowTimeText)
                    sb.Append(DateTime.Now.ToString("HH:mm") + " ");

                if (MMALCameraConfigImpl.Config.Annotate.ShowDateText)
                    sb.Append(DateTime.Now.ToString("dd/MM/yyyy") + " ");

                if (MMALCameraConfigImpl.Config.Annotate.ShowShutterSettings)
                    showShutter = 1;

                if (MMALCameraConfigImpl.Config.Annotate.ShowGainSettings)
                    showAnalogGain = 1;

                if (MMALCameraConfigImpl.Config.Annotate.ShowLensSettings)
                    showLens = 1;

                if (MMALCameraConfigImpl.Config.Annotate.ShowCafSettings)
                    showCaf = 1;

                if (MMALCameraConfigImpl.Config.Annotate.ShowMotionSettings)
                    showMotion = 1;

                if (MMALCameraConfigImpl.Config.Annotate.ShowFrameNumber)
                    showFrame = 1;

                if (MMALCameraConfigImpl.Config.Annotate.ShowBlackBackground)
                    enableTextBackground = 1;

                textSize = Convert.ToByte(MMALCameraConfigImpl.Config.Annotate.TextSize);

                if (MMALCameraConfigImpl.Config.Annotate.TextColour != -1)
                {
                    customTextColor = 1;
                    customTextY = Convert.ToByte((MMALCameraConfigImpl.Config.Annotate.TextColour & 0xff));
                    customTextU = Convert.ToByte(((MMALCameraConfigImpl.Config.Annotate.TextColour >> 8) & 0xff));
                    customTextV = Convert.ToByte(((MMALCameraConfigImpl.Config.Annotate.TextColour >> 16) & 0xff));
                }
                
                if (MMALCameraConfigImpl.Config.Annotate.BgColour != -1)
                {
                    customBackgroundColor = 1;
                    customBackgroundY = Convert.ToByte((MMALCameraConfigImpl.Config.Annotate.BgColour & 0xff));
                    customBackgroundU = Convert.ToByte(((MMALCameraConfigImpl.Config.Annotate.BgColour >> 8) & 0xff));
                    customBackgroundV = Convert.ToByte(((MMALCameraConfigImpl.Config.Annotate.BgColour >> 16) & 0xff));
                }
                
                string t = sb.ToString() + char.MinValue;
                                
                var text = Encoding.ASCII.GetBytes(t);
                                
                MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T str = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_ANNOTATE, (Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>() + (t.Length))),
                                                                                                    1, showShutter, showAnalogGain, showLens,
                                                                                                    showCaf, showMotion, showFrame, enableTextBackground,
                                                                                                    customBackgroundColor, customBackgroundY, customBackgroundU, customBackgroundV, (byte)0, customTextColor,
                                                                                                    customTextY, customTextU, customTextV, textSize, text);
                
                var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>());
                Marshal.StructureToPtr(str, ptr, false);

                MMALCheck(MMALPort.mmal_port_parameter_set(MMALCamera.Instance.Camera.Control.Ptr, (MMAL_PARAMETER_HEADER_T*)ptr), "Unable to set annotate");

                Marshal.FreeHGlobal(ptr);
            }
        }

    }
        
    /// <summary>
    /// Represents a video encoder component
    /// </summary>
    public unsafe class MMALVideoEncoder : MMALEncoderBase
    {
        /// <summary>
        /// The encoding type that the video encoder should use
        /// </summary>
        public int EncodingType { get; set; } = MMALEncodings.MMAL_ENCODING_H264;

        public int Bitrate { get; set; } = 17000000;

        public int Framerate { get; set; } = 30;

        public MMALVideoEncoder(int encodingType, int bitrate, int framerate) : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_ENCODER)
        {
            this.EncodingType = encodingType;
            this.Bitrate = bitrate;
        }

        public MMALVideoEncoder() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_ENCODER)
        {            
        }

        public override void ManagedCallback(MMALBufferImpl buffer, MMALPortBase port)
        {


            base.ManagedCallback(buffer, port);
        }

    }

    /// <summary>
    /// Represents a video decoder component
    /// </summary>
    public unsafe class MMALVideoDecoder : MMALEncoderBase
    {
        public MMALVideoDecoder() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_DECODER)
        {
            this.Initialize();
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }
        
    }

    /// <summary>
    /// Represents an image encoder component
    /// </summary>
    public unsafe class MMALImageEncoder : MMALEncoderBase
    {
        public const int MaxExifPayloadLength = 128;

        /// <summary>
        /// The encoding type that the image encoder should use
        /// </summary>
        public int EncodingType { get; set; } = MMALEncodings.MMAL_ENCODING_JPEG;

        /// <summary>
        /// The quality of the JPEG image
        /// </summary>
        public int Quality { get; set; } = 90;

        public MMALImageEncoder(int encodingType, int quality) : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_ENCODER)
        {
            if(encodingType > 0)
                this.EncodingType = encodingType;
            if(quality > 0)
                this.Quality = quality;
            this.Initialize();
        }

        public MMALImageEncoder() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_ENCODER)
        {           
            this.Initialize();
        }
        
        public override void Initialize()
        {
            base.Initialize();
            var input = this.Inputs.ElementAt(0);
            var output = this.Outputs.ElementAt(0);
                        
            output.Ptr->Format->Encoding = this.EncodingType;
            output.Ptr->BufferNum = Math.Max(output.Ptr->BufferNumRecommended, output.Ptr->BufferNumMin);
            output.Ptr->BufferSize = Math.Max(output.Ptr->BufferSizeRecommended, output.Ptr->BufferSizeMin);

            output.Commit();
                        
            if (this.EncodingType == MMALEncodings.MMAL_ENCODING_JPEG)
                SetParameter(MMALParametersCamera.MMAL_PARAMETER_JPEG_Q_FACTOR, this.Quality, output.Ptr);
        }
        
        /// <summary>
        /// Adds EXIF tags to the resulting image
        /// </summary>
        /// <param name="encoder"></param>
        /// <param name="exifTags"></param>                     
        internal void AddExifTags(MMALImageEncoder encoder, params ExifTag[] exifTags)
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

            defaultTags.ForEach(c => encoder.AddExifTag(c));

            if ((defaultTags.Count + exifTags.Length) > 32)
                throw new PiCameraError("Maximum number of EXIF tags exceeded.");

            //Add user defined tags.                 
            foreach (ExifTag tag in exifTags)
            {
                encoder.AddExifTag(tag);
            }
        }

        /// <summary>
        /// Provides a facility to add an EXIF tag to the image. 
        /// </summary>
        /// <param name="exifTag"></param>
        internal unsafe void AddExifTag(ExifTag exifTag)
        {
            this.SetDisableExif(false);            
            var formattedExif = exifTag.Key + "=" + exifTag.Value + char.MinValue;
          
            if (formattedExif.Length > MaxExifPayloadLength)
                throw new PiCameraError("EXIF payload greater than allowed max.");
            
            var bytes = Encoding.ASCII.GetBytes(formattedExif);
            
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_EXIF_T>() + (bytes.Length - 1));

            var str = new MMAL_PARAMETER_EXIF_T(new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_EXIF,
                                                (Marshal.SizeOf<MMAL_PARAMETER_EXIF_T_DUMMY>() + (bytes.Length - 1))
                                                                            ), 0, 0, 0, bytes);

            Marshal.StructureToPtr(str, ptr, false);

            MMALCheck(MMALPort.mmal_port_parameter_set(this.Outputs.ElementAt(0).Ptr, (MMAL_PARAMETER_HEADER_T*)ptr), string.Format("Unable to set EXIF {0}", formattedExif));

            Marshal.FreeHGlobal(ptr);
        }

    }

    /// <summary>
    /// Represents an image decoder component
    /// </summary>
    public unsafe class MMALImageDecoder : MMALEncoderBase
    {
        public MMALImageDecoder() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_DECODER)
        {
            this.Initialize();
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

    }


}
