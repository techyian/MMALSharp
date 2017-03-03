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
using static MMALSharp.Native.MMALParametersVideo;
using MMALSharp.Utility;

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
            if (MMALCameraConfig.Annotate != null)
            {
                if (MMALCameraConfig.Debug)
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
                
                if (!string.IsNullOrEmpty(MMALCameraConfig.Annotate.CustomText))
                    sb.Append(MMALCameraConfig.Annotate.CustomText + " ");

                if (MMALCameraConfig.Annotate.ShowTimeText)
                    sb.Append(DateTime.Now.ToString("HH:mm") + " ");

                if (MMALCameraConfig.Annotate.ShowDateText)
                    sb.Append(DateTime.Now.ToString("dd/MM/yyyy") + " ");

                if (MMALCameraConfig.Annotate.ShowShutterSettings)
                    showShutter = 1;

                if (MMALCameraConfig.Annotate.ShowGainSettings)
                    showAnalogGain = 1;

                if (MMALCameraConfig.Annotate.ShowLensSettings)
                    showLens = 1;

                if (MMALCameraConfig.Annotate.ShowCafSettings)
                    showCaf = 1;

                if (MMALCameraConfig.Annotate.ShowMotionSettings)
                    showMotion = 1;

                if (MMALCameraConfig.Annotate.ShowFrameNumber)
                    showFrame = 1;

                if (MMALCameraConfig.Annotate.ShowBlackBackground)
                    enableTextBackground = 1;

                textSize = Convert.ToByte(MMALCameraConfig.Annotate.TextSize);

                if (MMALCameraConfig.Annotate.TextColour != -1)
                {
                    customTextColor = 1;
                    customTextY = Convert.ToByte((MMALCameraConfig.Annotate.TextColour & 0xff));
                    customTextU = Convert.ToByte(((MMALCameraConfig.Annotate.TextColour >> 8) & 0xff));
                    customTextV = Convert.ToByte(((MMALCameraConfig.Annotate.TextColour >> 16) & 0xff));
                }
                
                if (MMALCameraConfig.Annotate.BgColour != -1)
                {
                    customBackgroundColor = 1;
                    customBackgroundY = Convert.ToByte((MMALCameraConfig.Annotate.BgColour & 0xff));
                    customBackgroundU = Convert.ToByte(((MMALCameraConfig.Annotate.BgColour >> 8) & 0xff));
                    customBackgroundV = Convert.ToByte(((MMALCameraConfig.Annotate.BgColour >> 16) & 0xff));
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

        internal void CleanEncoderPorts()
        {
            //See if any pools need disposing before destroying component.
            foreach (var port in this.Inputs)
            {
                if (port.BufferPool != null)
                {
                    if (MMALCameraConfig.Debug)
                        Console.WriteLine("Destroying port pool");

                    port.DestroyPortPool();
                    port.BufferPool = null;
                }

            }
            foreach (var port in this.Outputs)
            {
                if (port.BufferPool != null)
                {
                    if (MMALCameraConfig.Debug)
                        Console.WriteLine("Destroying port pool");

                    port.DestroyPortPool();
                    port.BufferPool = null;
                }
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

        public int Level { get; set; } = (int)MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_4;

        public MMALPortImpl InputPort { get; set; }
        public MMALPortImpl OutputPort { get; set; }

        public const int MaxBitrateMJPEG = 25000000; // 25Mbits/s
        public const int MaxBitrateLevel4 = 25000000; // 25Mbits/s
        public const int MaxBitrateLevel42 = 62500000; // 62.5Mbits/s
        
        public MMALVideoEncoder(int encodingType, int bitrate, int framerate) : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_ENCODER)
        {
            if (encodingType > 0)
                this.EncodingType = encodingType;

            if (bitrate > 0)
                this.Bitrate = bitrate;

            if (framerate > 0)
                this.Framerate = framerate;

            this.Initialize();
        }

        public MMALVideoEncoder() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_ENCODER)
        {
            this.InputPort = this.Inputs.ElementAt(0);
            this.OutputPort = this.Outputs.ElementAt(0);
            this.Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            
            this.OutputPort.Ptr->Format->Encoding = this.EncodingType;

            if (this.EncodingType == MMALEncodings.MMAL_ENCODING_H264)
            {
                if(this.Level == (int)MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_4)
                {
                    if(this.Bitrate > MaxBitrateLevel4)
                    {
                        Helpers.PrintWarning("Bitrate too high: Reducing to 25MBit/s");
                        this.Bitrate = MaxBitrateLevel4;
                    }
                }
                else
                {
                    if(this.Bitrate > MaxBitrateLevel42)
                    {
                        Helpers.PrintWarning("Bitrate too high: Reducing to 62.5MBit/s");
                        this.Bitrate = MaxBitrateLevel42;
                    }
                }                
            }
            else if(this.EncodingType == MMALEncodings.MMAL_ENCODING_MJPEG)
            {
                if(this.Bitrate > MaxBitrateMJPEG)
                {
                    Helpers.PrintWarning("Bitrate too high: Reducing to 25MBit/s");
                    this.Bitrate = MaxBitrateMJPEG;
                }
            }

            this.OutputPort.Ptr->Format->Bitrate = this.Bitrate;

            this.OutputPort.Ptr->BufferSize = this.OutputPort.BufferSizeRecommended;
            this.OutputPort.Ptr->BufferNum = this.OutputPort.BufferNumRecommended;

            MMAL_VIDEO_FORMAT_T vFormat = new MMAL_VIDEO_FORMAT_T(MMALCameraConfig.VideoWidth,
                                                                  MMALCameraConfig.VideoHeight,
                                                                  new MMAL_RECT_T(0, 0, MMALCameraConfig.VideoWidth, MMALCameraConfig.VideoHeight),
                                                                  new MMAL_RATIONAL_T(0, 1),
                                                                  this.OutputPort.Ptr->Format->Es->Video.Par,
                                                                  this.OutputPort.Ptr->Format->Es->Video.ColorSpace);

            this.OutputPort.Commit();
                        
            this.ConfigureRateControl();
                        
            this.ConfigureIntraPeriod();
                        
            this.ConfigureQuantisationParameter();
                        
            this.ConfigureVideoProfile();
                        
            this.ConfigureImmutableInput();
                        
            this.ConfigureInlineHeaderFlag();
                        
            this.ConfigureInlineVectorsFlag();
                        
            this.ConfigureIntraRefresh();                 
        }

        internal void ConfigureRateControl()
        {
            MMAL_PARAMETER_VIDEO_RATECONTROL_T param = new MMAL_PARAMETER_VIDEO_RATECONTROL_T(new MMAL_PARAMETER_HEADER_T(MMALParametersVideo.MMAL_PARAMETER_RATECONTROL, Marshal.SizeOf<MMAL_PARAMETER_VIDEO_RATECONTROL_T>()), MMALCameraConfig.RateControl);
            MMALCheck(MMALPort.mmal_port_parameter_set(this.OutputPort.Ptr, param.HdrPtr), "Unable to set ratecontrol.");
        }

        internal void ConfigureIntraPeriod()
        {
            if(this.EncodingType == MMALEncodings.MMAL_ENCODING_H264 && MMALCameraConfig.IntraPeriod != -1)
                this.OutputPort.SetParameter(MMALParametersVideo.MMAL_PARAMETER_INTRAPERIOD, MMALCameraConfig.IntraPeriod);
        }

        internal void ConfigureQuantisationParameter()
        {
            if (this.EncodingType == MMALEncodings.MMAL_ENCODING_H264 && MMALCameraConfig.QuantisationParameter != null)
            {
                this.OutputPort.SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INITIAL_QUANT, MMALCameraConfig.QuantisationParameter.Initial);
                this.OutputPort.SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_MIN_QUANT, MMALCameraConfig.QuantisationParameter.Min);
                this.OutputPort.SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_MAX_QUANT, MMALCameraConfig.QuantisationParameter.Max);
            }
        }

        internal void ConfigureVideoProfile()
        {
            if ((MMALUtil.VCOS_ALIGN_UP(MMALCameraConfig.VideoWidth, 16) >> 4) * (MMALUtil.VCOS_ALIGN_UP(MMALCameraConfig.VideoHeight, 16) >> 4) * this.Framerate > 245760)
            {
                if ((MMALUtil.VCOS_ALIGN_UP(MMALCameraConfig.VideoWidth, 16) >> 4) * (MMALUtil.VCOS_ALIGN_UP(MMALCameraConfig.VideoHeight, 16) >> 4) * this.Framerate <= 522240)
                {
                    Helpers.PrintWarning("Too many macroblocks/s: Increasing H264 Level to 4.2");
                    MMALCameraConfig.VideoLevel = MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_42;                    
                }
                else
                {
                    throw new PiCameraError("Too many macroblocks/s requested");                                        
                }
            }

            MMAL_PARAMETER_VIDEO_PROFILE_S p = new MMAL_PARAMETER_VIDEO_PROFILE_S(MMALCameraConfig.VideoProfile, MMALCameraConfig.VideoLevel);
            
            MMAL_PARAMETER_VIDEO_PROFILE_S[] arr = new MMAL_PARAMETER_VIDEO_PROFILE_S[1] { p };
                        
            MMAL_PARAMETER_VIDEO_PROFILE_T param = new MMAL_PARAMETER_VIDEO_PROFILE_T(new MMAL_PARAMETER_HEADER_T(MMALParametersVideo.MMAL_PARAMETER_PROFILE, Marshal.SizeOf<MMAL_PARAMETER_VIDEO_PROFILE_T>()), arr);

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(param));

            Marshal.StructureToPtr(param, ptr, false);

            MMALCheck(MMALPort.mmal_port_parameter_set(this.OutputPort.Ptr, (MMAL_PARAMETER_HEADER_T*)ptr), "Unable to set video profile.");

            Marshal.FreeHGlobal(ptr);
        }

        internal void ConfigureImmutableInput()
        {
            this.InputPort.SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_IMMUTABLE_INPUT, MMALCameraConfig.ImmutableInput);
        }

        internal void ConfigureInlineHeaderFlag()
        {
            this.OutputPort.SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INLINE_HEADER, MMALCameraConfig.InlineHeaders);
        }

        internal void ConfigureInlineVectorsFlag()
        {
            if (this.EncodingType == MMALEncodings.MMAL_ENCODING_H264)            
                this.OutputPort.SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INLINE_VECTORS, MMALCameraConfig.InlineMotionVectors);            
        }

        internal void ConfigureIntraRefresh()
        {
            MMAL_PARAMETER_VIDEO_INTRA_REFRESH_T param = new MMAL_PARAMETER_VIDEO_INTRA_REFRESH_T(new MMAL_PARAMETER_HEADER_T(MMALParametersVideo.MMAL_PARAMETER_VIDEO_INTRA_REFRESH, Marshal.SizeOf<MMAL_PARAMETER_VIDEO_INTRA_REFRESH_T>()), MMAL_VIDEO_INTRA_REFRESH_T.MMAL_VIDEO_INTRA_REFRESH_BOTH, 0, 0, 0, 0);

            int airMbs = 0, airRef = 0, cirMbs = 0, pirMbs = 0;

            try
            {
                MMALCheck(MMALPort.mmal_port_parameter_get(this.OutputPort.Ptr, param.HdrPtr), "Unable to set video profile.");
                airMbs = param.AirMbs;
                airRef = param.AirRef;
                cirMbs = param.CirMbs;
                pirMbs = param.PirMbs;
            }
            catch
            {                
            }

            param = new MMAL_PARAMETER_VIDEO_INTRA_REFRESH_T(new MMAL_PARAMETER_HEADER_T(MMALParametersVideo.MMAL_PARAMETER_VIDEO_INTRA_REFRESH, Marshal.SizeOf<MMAL_PARAMETER_VIDEO_INTRA_REFRESH_T>()), MMALCameraConfig.IntraRefresh, airMbs, airRef, cirMbs, pirMbs);

            MMALCheck(MMALPort.mmal_port_parameter_set(this.OutputPort.Ptr, param.HdrPtr), "Unable to set video intra refresh.");

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
                output.SetParameter(MMALParametersCamera.MMAL_PARAMETER_JPEG_Q_FACTOR, this.Quality);
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
