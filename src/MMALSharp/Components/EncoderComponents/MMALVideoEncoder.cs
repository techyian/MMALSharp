// <copyright file="MMALVideoEncoder.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using MMALSharp.Callbacks;
using MMALSharp.Common.Utility;
using MMALSharp.Config;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;
using static MMALSharp.MMALNativeExceptionHelper;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a video encoder component
    /// </summary>
    public unsafe class MMALVideoEncoder : MMALEncoderBase
    {
        /// <summary>
        /// Signifies the max bitrate supported for MJPEG (25Mbits/s)
        /// </summary>
        public const int MaxBitrateMJPEG = 25000000;

        /// <summary>
        /// Signifies the max bitrate supported for H.264 Level 4 (25Mbits/s)
        /// </summary>
        public const int MaxBitrateLevel4 = 25000000; // 25Mbits/s

        /// <summary>
        /// Signifies the max bitrate supported for H.264 Level 4.2 (62.5Mbits/s)
        /// </summary>
        public const int MaxBitrateLevel42 = 62500000; // 62.5Mbits/s
        
        /// <summary>
        /// The working H.264 level.
        /// </summary>
        public int Level { get; set; } = (int)MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_4;

        /// <summary>
        /// Quality of the encoder output. Valid property for both H264 and MJPEG encoders.
        /// H264 encoding - High: 10 Low: 40 Average: 20-25.
        /// MJPEG encoding - Uses same quality scale as JPEG encoder (Lowest: 1  Highest: 100).
        /// </summary>
        public int Quality { get; set; }
        
        /// <summary>
        /// Object containing properties used to determine when we should perform a file split.
        /// </summary>
        public Split Split { get; }

        /// <summary>
        /// States the time we last did a file split.
        /// </summary>
        public DateTime? LastSplit { get; set; }

        /// <summary>
        /// Property to indicate whether on the next callback we should split. This is used so that we can request an I-Frame from the camera
        /// and this can be applied on the next run to the newly created file.
        /// </summary>
        public bool PrepareSplit { get; set; }
        
        /// <summary>
        /// Creates a new instance of <see cref="MMALVideoEncoder"/>.
        /// </summary>
        /// <param name="handler">The capture handler.</param>
        /// <param name="split">Configure this component to split into multiple files.</param>
        public MMALVideoEncoder(ICaptureHandler handler, Split split = null)
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_ENCODER)
        {
            this.Split = split;
            this.Inputs.Add(new InputPort((IntPtr)(&(*this.Ptr->Input[0])), this, PortType.Input, Guid.NewGuid()));
            this.Outputs.Add(new VideoPort((IntPtr)(&(*this.Ptr->Output[0])), this, PortType.Output, Guid.NewGuid(), handler));
        }
        
        /// <inheritdoc />
        public override MMALDownstreamComponent ConfigureOutputPort(int outputPort, MMALPortConfig config)
        {
            base.ConfigureOutputPort(outputPort, config);
            
            if (MMALCameraConfig.VideoColorSpace != null &&
                MMALCameraConfig.VideoColorSpace.EncType == MMALEncoding.EncodingType.ColorSpace)
            {
                this.Outputs[outputPort].VideoColorSpace = MMALCameraConfig.VideoColorSpace;
            }
            
            if (this.Outputs[outputPort].GetType() == typeof(VideoPort) || this.Outputs[outputPort].GetType().IsSubclassOf(typeof(VideoPort)))
            {
                ((VideoPort)this.Outputs[outputPort]).Timeout = config.Timeout;
            }

            this.Outputs[outputPort].Ptr->BufferNum = Math.Max(this.Outputs[outputPort].Ptr->BufferNumRecommended, 3);
            this.Outputs[outputPort].Ptr->BufferSize = 512 * 1024;
            this.Quality = config.Quality;
            
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

            this.RegisterOutputCallback(new VideoOutputCallbackHandler(this.Outputs[outputPort]));
            
            return this;
        }
        
        internal DateTime CalculateSplit()
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

        internal void ConfigureBitrate(int outputPort)
        {
            if (this.Outputs[outputPort].EncodingType == MMALEncoding.H264)
            {
                List<VideoLevel> levelList = null;

                if (MMALCameraConfig.VideoProfile == MMALParametersVideo.MMAL_VIDEO_PROFILE_T.MMAL_VIDEO_PROFILE_H264_HIGH)
                {
                    levelList = GetHighLevelLimits();
                }
                else if (MMALCameraConfig.VideoProfile == MMALParametersVideo.MMAL_VIDEO_PROFILE_T.MMAL_VIDEO_PROFILE_H264_HIGH10)
                {
                    levelList = GetHigh10LevelLimits();
                }
                else
                {
                    levelList = GetNormalLevelLimits();
                }

                var level = levelList.Where(c => c.Level == MMALCameraConfig.VideoLevel).First();

                if (this.Outputs[outputPort].Bitrate > level.Maxbitrate)
                {
                    throw new PiCameraError("Bitrate requested exceeds maximum for selected Video Level and Profile");
                }
            }
            else if (this.Outputs[outputPort].EncodingType == MMALEncoding.MJPEG)
            {
                if (this.Outputs[outputPort].Bitrate > MaxBitrateMJPEG)
                {
                    MMALLog.Logger.Warn("Bitrate too high: Reducing to 25MBit/s");
                    this.Outputs[outputPort].Bitrate = MaxBitrateMJPEG;
                }
            }

            this.Outputs[outputPort].Ptr->Format->Bitrate = this.Outputs[outputPort].Bitrate;
            this.Outputs[outputPort].Ptr->Format->Es->Video.FrameRate = new MMAL_RATIONAL_T(0, 1);
            this.Outputs[outputPort].Commit();

            this.Outputs[outputPort].Ptr->Format->Bitrate = this.Outputs[outputPort].Bitrate;
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
            this.Outputs[outputPort].SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INITIAL_QUANT, this.Quality);
            this.Outputs[outputPort].SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_MIN_QUANT, this.Quality);
            this.Outputs[outputPort].SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_MAX_QUANT, this.Quality);
        }

        internal void ConfigureVideoProfile(int outputPort)
        {
            var macroblocks = (MMALCameraConfig.VideoResolution.Width >> 4) * (MMALCameraConfig.VideoResolution.Height >> 4);
            var macroblocksPSec = macroblocks * (MMALCameraConfig.VideoFramerate.Num / MMALCameraConfig.VideoFramerate.Den);

            List<VideoLevel> videoLevels = GetNormalLevelLimits();

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

            try
            {
                MMALCheck(
                    MMALPort.mmal_port_parameter_set(this.Outputs[outputPort].Ptr, (MMAL_PARAMETER_HEADER_T*)ptr),
                    "Unable to set video profile.");
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
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
        
        private List<VideoLevel> GetNormalLevelLimits()
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

        private List<VideoLevel> GetHighLevelLimits()
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

        private List<VideoLevel> GetHigh10LevelLimits()
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

    /// <summary>
    /// A class to describe a H.264 level mode.
    /// </summary>
    public class VideoLevel
    {
        /// <summary>
        /// The MMAL level enum.
        /// </summary>
        public MMALParametersVideo.MMAL_VIDEO_LEVEL_T Level { get; set; }

        /// <summary>
        /// The max macroblocks per second limit for this level.
        /// </summary>
        public int MacroblocksPerSecLimit { get; set; }

        /// <summary>
        /// The max macroblocks limit for this level.
        /// </summary>
        public int MacroblocksLimit { get; set; }

        /// <summary>
        /// The max bitrate for this level.
        /// </summary>
        public int Maxbitrate { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="VideoLevel"/>.
        /// </summary>
        /// <param name="level">The MMAL level enum.</param>
        /// <param name="mcbps">The max macroblocks per second value.</param>
        /// <param name="mcb">The max macroblocks value.</param>
        /// <param name="bitrate">The max bitrate value.</param>
        public VideoLevel(MMALParametersVideo.MMAL_VIDEO_LEVEL_T level, int mcbps, int mcb, int bitrate)
        {
            this.Level = level;
            this.MacroblocksPerSecLimit = mcbps;
            this.MacroblocksLimit = mcb;
            this.Maxbitrate = bitrate;
        }
    }
}
