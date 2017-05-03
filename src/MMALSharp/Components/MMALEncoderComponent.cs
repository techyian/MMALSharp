using MMALSharp.Handlers;
using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
        public MMALEncoding EncodingType { get; set; }
        public MMALEncoding PixelFormat { get; set; }
        public MMALPortImpl InputPort { get; set; }
        public MMALPortImpl OutputPort { get; set; }

        protected MMALEncoderBase(string encoderName, MMALEncoding encodingType, MMALEncoding pixelFormat, ICaptureHandler handler) : base(encoderName, handler)
        {
            if(encodingType == null || pixelFormat == null || handler == null)
            {
                throw new PiCameraError("Please configure the encoder component correctly.");
            }

            if (pixelFormat.EncType != MMALEncoding.EncodingType.PixelFormat)
            {
                throw new PiCameraError("Invalid pixel format selected");
            }

            this.EncodingType = encodingType;
            this.PixelFormat = pixelFormat;

            this.Inputs.ElementAt(0).ShallowCopy(this.Outputs.ElementAt(0));
        }
        
        /// <summary>
        /// Annotates the image with various text as specified in the MMALCameraConfig class.
        /// </summary>
        internal void AnnotateImage()
        {
            if (MMALCameraConfig.Annotate != null)
            {
                if (MMALCameraConfig.Debug)
                    Console.WriteLine("Setting annotate");
                                                               
                var sb = new StringBuilder();

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
                                
                var str = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_ANNOTATE, (Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>() + (t.Length))),
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
        
        public override void Dispose()
        {                        
            if (MMALCameraConfig.Debug)
            {
                Console.WriteLine("Removing encoder");
            }
            
            this.Connection?.Destroy();
            
            MMALCamera.Instance.Encoders.Remove(this);

            //Remove any unmanaged resources held by the capture handler.
            this.Handler?.Dispose();

            base.Dispose();
        }
    }
        
    /// <summary>
    /// Represents a video encoder component
    /// </summary>
    public sealed unsafe class MMALVideoEncoder : MMALEncoderBase
    {        
        public int Bitrate { get; set; }

        public int Framerate { get; set; }

        public int Level { get; set; } = (int)MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_4;

        /// <summary>
        /// Quality of the encoder output. Valid property for both H264 and MJPEG encoders.
        /// H264 encoding - High: 10 Low: 40 Average: 20-25
        /// MJPEG encoding - Uses same quality scale as JPEG encoder (Lowest: 1  Highest: 100)
        /// </summary>
        public int Quality { get; set; }
                
        public const int MaxBitrateMJPEG = 25000000; // 25Mbits/s
        public const int MaxBitrateLevel4 = 25000000; // 25Mbits/s
        public const int MaxBitrateLevel42 = 62500000; // 62.5Mbits/s

        /// <summary>
        /// Object containing properties used to determine when we should perform a file split.
        /// </summary>
        public Split Split { get; set; }

        /// <summary>
        /// States the time we last did a file split.
        /// </summary>
        public DateTime? LastSplit { get; set; }

        /// <summary>
        /// Property to indicate whether on the next callback we should split. This is used so that we can request an I-Frame from the camera
        /// and this can be applied on the next run to the newly created file.
        /// </summary>
        public bool PrepareSplit { get; set; }

        public MMALVideoEncoder(ICaptureHandler handler, MMALEncoding encodingType, MMALEncoding pixelFormat, int bitrate, int quality, int framerate) : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_ENCODER, encodingType, pixelFormat, handler)
        {
            this.Quality = quality;
            this.Bitrate = bitrate;
            this.Framerate = framerate;
            
            if (this.EncodingType.EncType != MMALEncoding.EncodingType.Video)
            {
                throw new PiCameraError("Unsupported format.");
            }

            this.Inputs = new List<MMALPortImpl>();
            for (int i = 0; i < this.Ptr->InputNum; i++)
            {
                this.Inputs.Add(new MMALVideoPort(&(*this.Ptr->Input[i]), this));
            }

            this.Outputs = new List<MMALPortImpl>();
            for (int i = 0; i < this.Ptr->OutputNum; i++)
            {
                this.Outputs.Add(new MMALVideoPort(&(*this.Ptr->Output[i]), this));
            }
            
            this.InputPort = this.Inputs.ElementAt(0);
            this.OutputPort = this.Outputs.ElementAt(0);
            
            this.OutputPort.Ptr->Format->Encoding = this.EncodingType.EncodingVal;
            this.OutputPort.Ptr->Format->EncodingVariant = this.PixelFormat.EncodingVal;

            this.ConfigureBitrate();

            this.OutputPort.Ptr->Format->Bitrate = this.Bitrate;

            this.OutputPort.Ptr->BufferSize = 512 * 1024;
            this.OutputPort.Ptr->BufferNum = this.OutputPort.BufferNumMin;

            var vFormat = new MMAL_VIDEO_FORMAT_T(MMALCameraConfig.VideoResolution.Width,
                                                  MMALCameraConfig.VideoResolution.Height,
                                                  new MMAL_RECT_T(0, 0, MMALCameraConfig.VideoResolution.Width, MMALCameraConfig.VideoResolution.Height),
                                                  new MMAL_RATIONAL_T(0, 1),
                                                  this.OutputPort.Ptr->Format->Es->Video.Par,
                                                  this.OutputPort.Ptr->Format->Es->Video.ColorSpace);

            MMALCamera.Instance.Camera.VideoPort.Ptr->Format->Es->Video = vFormat;

            this.OutputPort.Commit();

            if (this.EncodingType == MMALEncoding.MMAL_ENCODING_H264)
            {
                this.ConfigureIntraPeriod();

                this.ConfigureVideoProfile();

                this.ConfigureInlineHeaderFlag();

                this.ConfigureInlineVectorsFlag();

                this.ConfigureIntraRefresh();
            }

            this.ConfigureQuantisationParameter();
            this.ConfigureImmutableInput();
        }
        
        public MMALVideoEncoder(ICaptureHandler handler, int bitrate, int quality, int framerate) : this(handler, MMALEncoding.MMAL_ENCODING_H264, MMALEncoding.MMAL_ENCODING_I420, bitrate, quality, framerate) { }

        public MMALVideoEncoder(ICaptureHandler handler) : this(handler, MMALEncoding.MMAL_ENCODING_H264, MMALEncoding.MMAL_ENCODING_I420, MaxBitrateLevel4, 10, 25) { }
        
        /// <summary>
        /// Delegate to process the buffer header containing image data
        /// </summary>
        /// <param name="buffer">The buffer header we're currently processing</param>
        /// <param name="port">The port we're currently processing on</param>
        public override void ManagedCallback(MMALBufferImpl buffer, MMALPortBase port)
        {            
            if (this.PrepareSplit && buffer.Properties.Any(c => c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CONFIG))
            {
                ((VideoStreamCaptureHandler)this.Handler).Split();
                this.LastSplit = DateTime.Now;
                this.PrepareSplit = false;
            }

            //Ensure that if we need to split then this is done before processing the buffer data.
            if (this.Split != null)
            {
                if (!this.LastSplit.HasValue)
                {
                    this.LastSplit = DateTime.Now;
                }
                    
                if (DateTime.Now.CompareTo(this.CalculateSplit()) > 0)
                {                    
                    this.PrepareSplit = true;
                    port.SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_REQUEST_I_FRAME, true);                    
                }
            }

            base.ManagedCallback(buffer, port);            
        }

        internal void ConfigureBitrate()
        {
            if(this.EncodingType == MMALEncoding.MMAL_ENCODING_H264)
            {
                List<VideoLevel> levelList = null;
                                
                if (MMALCameraConfig.VideoProfile == MMAL_VIDEO_PROFILE_T.MMAL_VIDEO_PROFILE_H264_HIGH)
                {
                    levelList = VideoLevel.GetHighLevelLimits();                    
                }
                else if (MMALCameraConfig.VideoProfile == MMAL_VIDEO_PROFILE_T.MMAL_VIDEO_PROFILE_H264_HIGH10)
                {
                    levelList = VideoLevel.GetHigh10LevelLimits();                   
                }
                else
                {
                    levelList = VideoLevel.GetNormalLevelLimits();                    
                }

                var level = levelList.Where(c => c.Level == MMALCameraConfig.VideoLevel).First();

                if (this.Bitrate > level.Maxbitrate)
                {
                    throw new PiCameraError("Bitrate requested exceeds maximum for selected Video Level and Profile");
                }

            }            
            else if (this.EncodingType == MMALEncoding.MMAL_ENCODING_MJPEG)
            {
                if (this.Bitrate > MaxBitrateMJPEG)
                {
                    Helpers.PrintWarning("Bitrate too high: Reducing to 25MBit/s");
                    this.Bitrate = MaxBitrateMJPEG;
                }
            }                                    
        }

        internal void ConfigureRateControl()
        {
            MMAL_PARAMETER_VIDEO_RATECONTROL_T param = new MMAL_PARAMETER_VIDEO_RATECONTROL_T(new MMAL_PARAMETER_HEADER_T(MMALParametersVideo.MMAL_PARAMETER_RATECONTROL, Marshal.SizeOf<MMAL_PARAMETER_VIDEO_RATECONTROL_T>()), MMALCameraConfig.RateControl);
            MMALCheck(MMALPort.mmal_port_parameter_set(this.OutputPort.Ptr, param.HdrPtr), "Unable to set ratecontrol.");
        }

        internal void ConfigureIntraPeriod()
        {
            if(this.EncodingType == MMALEncoding.MMAL_ENCODING_H264 && MMALCameraConfig.IntraPeriod != -1)
            {
                this.OutputPort.SetParameter(MMALParametersVideo.MMAL_PARAMETER_INTRAPERIOD, MMALCameraConfig.IntraPeriod);
            }                
        }

        internal void ConfigureQuantisationParameter()
        {
            if (this.EncodingType == MMALEncoding.MMAL_ENCODING_H264)
            {
                this.OutputPort.SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INITIAL_QUANT, Quality);
                this.OutputPort.SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_MIN_QUANT, Quality);
                this.OutputPort.SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_MAX_QUANT, Quality);
            }
        }

        internal void ConfigureVideoProfile()
        {
            var macroblocks = (MMALCameraConfig.VideoResolution.Width >> 4) * (MMALCameraConfig.VideoResolution.Height >> 4);
            var macroblocksPSec = macroblocks * this.Framerate;

            List<VideoLevel> videoLevels = VideoLevel.GetNormalLevelLimits();
            
            var level = videoLevels.Where(c => c.Level == MMALCameraConfig.VideoLevel).First();

            if(macroblocks > level.MacroblocksLimit)
            {
                throw new PiCameraError("Resolution exceeds macroblock limit for selected profile and level.");
            }

            if(macroblocksPSec > level.MacroblocksPerSecLimit)
            {
                throw new PiCameraError("Resolution exceeds macroblocks/s limit for selected profile and level.");
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
            if (this.EncodingType == MMALEncoding.MMAL_ENCODING_H264)
            {
                this.OutputPort.SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INLINE_VECTORS, MMALCameraConfig.InlineMotionVectors);
            }                                       
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

        private DateTime CalculateSplit()
        {
            DateTime tempDt = new DateTime(this.LastSplit.Value.Ticks);
            switch (this.Split.Mode)
            {
                case TimelapseMode.Millisecond:
                    return tempDt.AddMilliseconds(this.Split.Value);
                case TimelapseMode.Second:
                    return tempDt.AddSeconds(this.Split.Value);
                case TimelapseMode.Minute:
                    return tempDt.AddMinutes(this.Split.Value);
                default:
                    return tempDt.AddMinutes(this.Split.Value);
            }
        }
        
        class VideoLevel
        {
            public MMAL_VIDEO_LEVEL_T Level { get; set; }
            public int MacroblocksPerSecLimit { get; set; }
            public int MacroblocksLimit { get; set; }
            public int Maxbitrate { get; set; }

            public VideoLevel(MMAL_VIDEO_LEVEL_T level, int mcbps, int mcb, int bitrate)
            {
                this.Level = level;
                this.MacroblocksPerSecLimit = mcbps;
                this.MacroblocksLimit = mcb;
                this.Maxbitrate = bitrate;
            }

            public static List<VideoLevel> GetNormalLevelLimits()
            {
                var videoLevels = new List<VideoLevel>
                {
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_1, 1485, 99, 64000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_1b, 1485, 99, 128000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_11, 3000, 396, 192000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_12, 6000, 396, 384000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_13, 11880, 396, 768000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_2, 11880, 396, 2000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_21, 19800, 792, 4000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_22, 20250, 1620, 4000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_3, 40500, 1620, 10000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_31, 108000, 3600, 14000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_32, 216000, 5120, 20000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_4, 245760, 8192, 20000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_41, 245760, 8192, 50000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_42, 522240, 8704, 50000000)
                };
                
                return videoLevels;
            }

            public static List<VideoLevel> GetHighLevelLimits()
            {
                var videoLevels = new List<VideoLevel>
                {
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_1, 1485, 99, 80000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_1b, 1485, 99, 160000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_11, 3000, 396, 240000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_12, 6000, 396, 480000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_13, 11880, 396, 960000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_2, 11880, 396, 2500000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_21, 19800, 792, 5000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_22, 20250, 1620, 5000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_3, 40500, 1620, 12500000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_31, 108000, 3600, 17500000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_32, 216000, 5120, 25000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_4, 245760, 8192, 25000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_41, 245760, 8192, 62500000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_42, 522240, 8704, 62500000)
                };


                return videoLevels;
            }

            public static List<VideoLevel> GetHigh10LevelLimits()
            {
                var videoLevels = new List<VideoLevel>
                {
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_1, 1485, 99, 192000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_1b, 1485, 99, 384000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_11, 3000, 396, 576000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_12, 6000, 396, 1152000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_13, 11880, 396, 2304000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_2, 11880, 396, 6000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_21, 19800, 792, 12000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_22, 20250, 1620, 12000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_3, 40500, 1620, 30000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_31, 108000, 3600, 42000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_32, 216000, 5120, 60000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_4, 245760, 8192, 60000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_41, 245760, 8192, 150000000),
                    new VideoLevel(MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_42, 522240, 8704, 150000000)
                };
                
                return videoLevels;
            }
        }       
    }

    /// <summary>
    /// Represents a video decoder component
    /// </summary>
    public sealed unsafe class MMALVideoDecoder : MMALEncoderBase
    {
        public MMALVideoDecoder(ICaptureHandler handler, MMALEncoding encodingType, MMALEncoding pixelFormat) : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_DECODER, encodingType, pixelFormat, handler)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Represents an image encoder component
    /// </summary>
    public sealed unsafe class MMALImageEncoder : MMALEncoderBase
    {
        public const int MaxExifPayloadLength = 128;
                
        /// <summary>
        /// The quality of the JPEG image
        /// </summary>
        public int Quality { get; set; } = 90;

        public MMALImageEncoder(ICaptureHandler handler, MMALEncoding encodingType, MMALEncoding pixelFormat, int quality) : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_ENCODER, encodingType, pixelFormat, handler)
        {
            if(quality > 0)
            {
                this.Quality = quality;
            }

            if (this.EncodingType.EncType != MMALEncoding.EncodingType.Image)
            {
                throw new PiCameraError("Unsupported format.");
            }

            this.Inputs = new List<MMALPortImpl>();
            for (int i = 0; i < this.Ptr->InputNum; i++)
            {
                this.Inputs.Add(new MMALStillPort(&(*this.Ptr->Input[i]), this));
            }

            this.Outputs = new List<MMALPortImpl>();
            for (int i = 0; i < this.Ptr->OutputNum; i++)
            {
                this.Outputs.Add(new MMALStillPort(&(*this.Ptr->Output[i]), this));
            }
            
            this.InputPort = this.Inputs.ElementAt(0);
            this.OutputPort = this.Outputs.ElementAt(0);
            
            this.OutputPort.Ptr->Format->Encoding = this.EncodingType.EncodingVal;
            this.OutputPort.Ptr->Format->EncodingVariant = this.PixelFormat.EncodingVal;

            this.OutputPort.Ptr->BufferNum = Math.Max(this.OutputPort.Ptr->BufferNumRecommended, this.OutputPort.Ptr->BufferNumMin);
            this.OutputPort.Ptr->BufferSize = Math.Max(this.OutputPort.Ptr->BufferSizeRecommended, this.OutputPort.Ptr->BufferSizeMin);

            this.OutputPort.Commit();

            if (this.EncodingType == MMALEncoding.MMAL_ENCODING_JPEG)
            {
                this.OutputPort.SetParameter(MMALParametersCamera.MMAL_PARAMETER_JPEG_Q_FACTOR, this.Quality);
            }
        }

        public MMALImageEncoder(ICaptureHandler handler, int quality) : this(handler, MMALEncoding.MMAL_ENCODING_JPEG, MMALEncoding.MMAL_ENCODING_I420, quality) { }

        public MMALImageEncoder(ICaptureHandler handler) : this(handler, MMALEncoding.MMAL_ENCODING_JPEG, MMALEncoding.MMAL_ENCODING_I420, 0) { }            
        
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

    /// <summary>
    /// Represents an image decoder component
    /// </summary>
    public sealed unsafe class MMALImageDecoder : MMALEncoderBase
    {
        public MMALImageDecoder(ICaptureHandler handler, MMALEncoding encodingType, MMALEncoding pixelFormat) : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_DECODER, encodingType, pixelFormat, handler)
        {
            throw new NotImplementedException();
        }
        
    }


}
