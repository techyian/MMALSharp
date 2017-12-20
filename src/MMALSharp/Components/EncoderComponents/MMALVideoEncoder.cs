using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a video encoder component
    /// </summary>
    public sealed unsafe class MMALVideoEncoder : MMALEncoderBase
    {
        public int Bitrate { get; set; }

        public int Framerate { get; set; }

        public int Level { get; set; } = (int)MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_4;

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

        private int _width;
        private int _height;

        public override int Width
        {
            get
            {
                if (_width == 0)
                {
                    return MMALCameraConfig.VideoResolution.Width;
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
                    return MMALCameraConfig.VideoResolution.Height;
                }
                return _height;
            }
            set { _height = value; }
        }

        public DateTime? Timeout { get; set; }
        
        public MMALVideoEncoder(ICaptureHandler handler, MMAL_RATIONAL_T framerate, DateTime? timeout = null, Split split = null) : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_ENCODER, handler)
        {
            this.Framerate = framerate.Num / framerate.Den;
            this.Split = split;
            this.Timeout = timeout;
        }

        /// <summary>
        /// Call to configure changes on a Downstream video output port.
        /// </summary>
        /// <param name="outputPort">The output port we are configuring</param>
        /// <param name="encodingType">The encoding type this output port will send data in</param>
        /// <param name="pixelFormat">The pixel format this output port will send data in</param>
        /// <param name="quality">Quantisation parameter - quality. When using this setting, set bitrate 0 and set this for variable bitrate</param>
        /// <param name="bitrate">The bitrate we are sending data at</param>
        public override void ConfigureOutputPort(int outputPort, MMALEncoding encodingType, MMALEncoding pixelFormat, int quality, int bitrate = 0)
        {
            base.ConfigureOutputPort(outputPort, encodingType, pixelFormat, quality, bitrate);

            ((MMALVideoPort)this.Outputs[outputPort]).Timeout = this.Timeout;
            this.Outputs[outputPort].Ptr->BufferSize = 512 * 1024;
            this.Quality = quality;
            this.Bitrate = bitrate;
        
            if (this.Outputs[outputPort].EncodingType == MMALEncoding.H264)
            {
                this.ConfigureIntraPeriod(outputPort);

                this.ConfigureVideoProfile(outputPort);

                this.ConfigureInlineHeaderFlag(outputPort);

                this.ConfigureInlineVectorsFlag(outputPort);

                this.ConfigureIntraRefresh(outputPort);

                this.ConfigureQuantisationParameter(outputPort);
            }
            
            this.ConfigureImmutableInput(outputPort);
            this.ConfigureBitrate(outputPort);
        }

        /// <summary>
        /// Delegate to process the buffer header containing image data
        /// </summary>
        /// <param name="buffer">The buffer header we're currently processing</param>
        /// <param name="port">The port we're currently processing on</param>
        public override void ManagedOutputCallback(MMALBufferImpl buffer, MMALPortBase port)
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

            base.ManagedOutputCallback(buffer, port);
        }

        internal override void InitialiseOutputPort(int outputPort)
        {
            this.Outputs[outputPort] = new MMALVideoPort(&(*this.Ptr->Output[outputPort]), this, PortType.Output);
        }

        internal void ConfigureBitrate(int outputPort)
        {
            if (this.Outputs[outputPort].EncodingType == MMALEncoding.H264)
            {
                List<VideoLevel> levelList = null;

                if (MMALCameraConfig.VideoProfile == MMALParametersVideo.MMAL_VIDEO_PROFILE_T.MMAL_VIDEO_PROFILE_H264_HIGH)
                {
                    levelList = H264VideoLevel.GetHighLevelLimits();
                }
                else if (MMALCameraConfig.VideoProfile == MMALParametersVideo.MMAL_VIDEO_PROFILE_T.MMAL_VIDEO_PROFILE_H264_HIGH10)
                {
                    levelList = H264VideoLevel.GetHigh10LevelLimits();
                }
                else
                {
                    levelList = H264VideoLevel.GetNormalLevelLimits();
                }

                var level = levelList.Where(c => c.Level == MMALCameraConfig.VideoLevel).First();

                if (this.Bitrate > level.Maxbitrate)
                {
                    throw new PiCameraError("Bitrate requested exceeds maximum for selected Video Level and Profile");
                }

            }
            else if (this.Outputs[outputPort].EncodingType == MMALEncoding.MJPEG)
            {
                if (this.Bitrate > MaxBitrateMJPEG)
                {
                    MMALLog.Logger.Warn("Bitrate too high: Reducing to 25MBit/s");
                    this.Bitrate = MaxBitrateMJPEG;
                }
            }
        }

        internal void ConfigureRateControl(int outputPort)
        {
            MMAL_PARAMETER_VIDEO_RATECONTROL_T param = new MMAL_PARAMETER_VIDEO_RATECONTROL_T(new MMAL_PARAMETER_HEADER_T(MMALParametersVideo.MMAL_PARAMETER_RATECONTROL, Marshal.SizeOf<MMAL_PARAMETER_VIDEO_RATECONTROL_T>()), MMALCameraConfig.RateControl);
            MMALCheck(MMALPort.mmal_port_parameter_set(this.Outputs[outputPort].Ptr, param.HdrPtr), "Unable to set ratecontrol.");
        }

        internal void ConfigureIntraPeriod(int outputPort)
        {
            if (this.Outputs[outputPort].EncodingType == MMALEncoding.H264 && MMALCameraConfig.IntraPeriod != -1)
            {
                this.Outputs[outputPort].SetParameter(MMALParametersVideo.MMAL_PARAMETER_INTRAPERIOD, MMALCameraConfig.IntraPeriod);
            }
        }

        internal void ConfigureQuantisationParameter(int outputPort)
        {
            this.Outputs[outputPort].SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INITIAL_QUANT, Quality);
            this.Outputs[outputPort].SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_MIN_QUANT, Quality);
            this.Outputs[outputPort].SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_MAX_QUANT, Quality);
        }

        internal void ConfigureVideoProfile(int outputPort)
        {
            var macroblocks = (MMALCameraConfig.VideoResolution.Width >> 4) * (MMALCameraConfig.VideoResolution.Height >> 4);
            var macroblocksPSec = macroblocks * this.Framerate;

            List<VideoLevel> videoLevels = H264VideoLevel.GetNormalLevelLimits();

            var level = videoLevels.Where(c => c.Level == MMALCameraConfig.VideoLevel).First();

            if (macroblocks > level.MacroblocksLimit)
            {
                throw new PiCameraError("Resolution exceeds macroblock limit for selected profile and level.");
            }

            if (macroblocksPSec > level.MacroblocksPerSecLimit)
            {
                throw new PiCameraError("Resolution exceeds macroblocks/s limit for selected profile and level.");
            }

            MMAL_PARAMETER_VIDEO_PROFILE_S p = new MMAL_PARAMETER_VIDEO_PROFILE_S(MMALCameraConfig.VideoProfile, MMALCameraConfig.VideoLevel);

            MMAL_PARAMETER_VIDEO_PROFILE_S[] arr = new MMAL_PARAMETER_VIDEO_PROFILE_S[1] { p };

            MMAL_PARAMETER_VIDEO_PROFILE_T param = new MMAL_PARAMETER_VIDEO_PROFILE_T(new MMAL_PARAMETER_HEADER_T(MMALParametersVideo.MMAL_PARAMETER_PROFILE, Marshal.SizeOf<MMAL_PARAMETER_VIDEO_PROFILE_T>()), arr);

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(param));

            Marshal.StructureToPtr(param, ptr, false);

            MMALCheck(MMALPort.mmal_port_parameter_set(this.Outputs[outputPort].Ptr, (MMAL_PARAMETER_HEADER_T*)ptr), "Unable to set video profile.");

            Marshal.FreeHGlobal(ptr);
        }

        internal void ConfigureImmutableInput(int outputPort)
        {
            this.Inputs[0].SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_IMMUTABLE_INPUT, MMALCameraConfig.ImmutableInput);
        }

        internal void ConfigureInlineHeaderFlag(int outputPort)
        {
            this.Outputs[outputPort].SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INLINE_HEADER, MMALCameraConfig.InlineHeaders);
        }

        internal void ConfigureInlineVectorsFlag(int outputPort)
        {
            if (this.Outputs[outputPort].EncodingType == MMALEncoding.H264)
            {
                this.Outputs[outputPort].SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INLINE_VECTORS, MMALCameraConfig.InlineMotionVectors);
            }
        }

        internal void ConfigureIntraRefresh(int outputPort)
        {
            MMAL_PARAMETER_VIDEO_INTRA_REFRESH_T param = new MMAL_PARAMETER_VIDEO_INTRA_REFRESH_T(new MMAL_PARAMETER_HEADER_T(MMALParametersVideo.MMAL_PARAMETER_VIDEO_INTRA_REFRESH, Marshal.SizeOf<MMAL_PARAMETER_VIDEO_INTRA_REFRESH_T>()), MMALParametersVideo.MMAL_VIDEO_INTRA_REFRESH_T.MMAL_VIDEO_INTRA_REFRESH_BOTH, 0, 0, 0, 0);

            int airMbs = 0, airRef = 0, cirMbs = 0, pirMbs = 0;

            try
            {
                MMALCheck(MMALPort.mmal_port_parameter_get(this.Outputs[outputPort].Ptr, param.HdrPtr), "Unable to set video profile.");
                airMbs = param.AirMbs;
                airRef = param.AirRef;
                cirMbs = param.CirMbs;
                pirMbs = param.PirMbs;
            }
            catch
            {
            }

            param = new MMAL_PARAMETER_VIDEO_INTRA_REFRESH_T(new MMAL_PARAMETER_HEADER_T(MMALParametersVideo.MMAL_PARAMETER_VIDEO_INTRA_REFRESH, Marshal.SizeOf<MMAL_PARAMETER_VIDEO_INTRA_REFRESH_T>()), MMALCameraConfig.IntraRefresh, airMbs, airRef, cirMbs, pirMbs);

            MMALCheck(MMALPort.mmal_port_parameter_set(this.Outputs[outputPort].Ptr, param.HdrPtr), "Unable to set video intra refresh.");

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

        public override void PrintComponent()
        {
            base.PrintComponent();
            MMALLog.Logger.Info($"    Width: {this.Width}. Height: {this.Height}");
        }

        private class H264VideoLevel
        {
            public static List<VideoLevel> GetNormalLevelLimits()
            {
                var videoLevels = new List<VideoLevel>
                {
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_1, 1485, 99, 64000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_1b, 1485, 99, 128000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_11, 3000, 396, 192000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_12, 6000, 396, 384000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_13, 11880, 396, 768000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_2, 11880, 396, 2000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_21, 19800, 792, 4000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_22, 20250, 1620, 4000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_3, 40500, 1620, 10000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_31, 108000, 3600, 14000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_32, 216000, 5120, 20000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_4, 245760, 8192, 20000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_41, 245760, 8192, 50000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_42, 522240, 8704, 50000000)
                };

                return videoLevels;
            }

            public static List<VideoLevel> GetHighLevelLimits()
            {
                var videoLevels = new List<VideoLevel>
                {
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_1, 1485, 99, 80000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_1b, 1485, 99, 160000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_11, 3000, 396, 240000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_12, 6000, 396, 480000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_13, 11880, 396, 960000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_2, 11880, 396, 2500000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_21, 19800, 792, 5000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_22, 20250, 1620, 5000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_3, 40500, 1620, 12500000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_31, 108000, 3600, 17500000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_32, 216000, 5120, 25000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_4, 245760, 8192, 25000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_41, 245760, 8192, 62500000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_42, 522240, 8704, 62500000)
                };


                return videoLevels;
            }

            public static List<VideoLevel> GetHigh10LevelLimits()
            {
                var videoLevels = new List<VideoLevel>
                {
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_1, 1485, 99, 192000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_1b, 1485, 99, 384000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_11, 3000, 396, 576000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_12, 6000, 396, 1152000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_13, 11880, 396, 2304000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_2, 11880, 396, 6000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_21, 19800, 792, 12000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_22, 20250, 1620, 12000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_3, 40500, 1620, 30000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_31, 108000, 3600, 42000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_32, 216000, 5120, 60000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_4, 245760, 8192, 60000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_41, 245760, 8192, 150000000),
                    new VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_42, 522240, 8704, 150000000)
                };

                return videoLevels;
            }
        }

        private class VideoLevel
        {
            public MMALParametersVideo.MMAL_VIDEO_LEVEL_T Level { get; set; }
            public int MacroblocksPerSecLimit { get; set; }
            public int MacroblocksLimit { get; set; }
            public int Maxbitrate { get; set; }

            public VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T level, int mcbps, int mcb, int bitrate)
            {
                this.Level = level;
                this.MacroblocksPerSecLimit = mcbps;
                this.MacroblocksLimit = mcb;
                this.Maxbitrate = bitrate;
            }
        }
    }
}
