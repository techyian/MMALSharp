// <copyright file="MMALParameters.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace MMALSharp.Native
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1132 // Each field should be declared on its own line

    // mmal_parameters_common.h
    public static class MMALParametersCommon
    {
        public const int MMAL_PARAMETER_GROUP_COMMON = 0 << 16;
        public const int MMAL_PARAMETER_GROUP_CAMERA = 1 << 16;
        public const int MMAL_PARAMETER_GROUP_VIDEO = 2 << 16;
        public const int MMAL_PARAMETER_GROUP_AUDIO = 3 << 16;
        public const int MMAL_PARAMETER_GROUP_CLOCK = 4 << 16;
        public const int MMAL_PARAMETER_GROUP_MIRACAST = 5 << 16;
        public const int MMAL_PARAMETER_UNUSED = 0;
        public const int MMAL_PARAMETER_SUPPORTED_ENCODINGS = 1;
        public const int MMAL_PARAMETER_URI = 2;
        public const int MMAL_PARAMETER_CHANGE_EVENT_REQUEST = 3;
        public const int MMAL_PARAMETER_ZERO_COPY = 4;
        public const int MMAL_PARAMETER_BUFFER_REQUIREMENTS = 5;
        public const int MMAL_PARAMETER_STATISTICS = 6;
        public const int MMAL_PARAMETER_CORE_STATISTICS = 7;
        public const int MMAL_PARAMETER_MEM_USAGE = 8;
        public const int MMAL_PARAMETER_BUFFER_FLAG_FILTER = 9;
        public const int MMAL_PARAMETER_SEEK = 10;
        public const int MMAL_PARAMETER_POWERMON_ENABLE = 11;
        public const int MMAL_PARAMETER_LOGGING = 12;
        public const int MMAL_PARAMETER_SYSTEM_TIME = 13;
        public const int MMAL_PARAMETER_NO_IMAGE_PADDING = 14;
        public const int MMAL_PARAMETER_LOCKSTEP_ENABLE = 15;

        public const int MMAL_PARAM_SEEK_FLAG_PRECISE = 0x01;
        public const int MMAL_PARAM_SEEK_FLAG_FORWARD = 0x02;
    }

    public enum MMAL_CORE_STATS_DIR
    {
        MMAL_CORE_STATS_RX,
        MMAL_CORE_STATS_TX
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_HEADER_T
    {
        public int Id { get; set; }

        public int Size { get; set; }

        public MMAL_PARAMETER_HEADER_T(int id, int size)
        {
            this.Id = id;
            this.Size = size;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_BYTES_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        private byte[] data;

        public byte[] Data => data;

        public MMAL_PARAMETER_BYTES_T(MMAL_PARAMETER_HEADER_T hdr, byte[] data)
        {
            this.Hdr = hdr;
            this.data = data;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_CHANGE_EVENT_REQUEST_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private int changeId;
        private int enable;

        public int ChangeId => changeId;
        public int Enable => enable;

        public MMAL_PARAMETER_CHANGE_EVENT_REQUEST_T(MMAL_PARAMETER_HEADER_T hdr, int changeId, int enable)
        {
            this.Hdr = hdr;
            this.changeId = changeId;
            this.enable = enable;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_BUFFER_REQUIREMENTS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        private int bufferNumMin, bufferSizeMin, bufferAlignmentMin, bufferNumRecommended, bufferSizeRecommended;

        public int BufferNumMin => this.bufferNumMin;

        public int BufferSizeMin => this.bufferSizeMin;

        public int BufferAlignmentMin => this.bufferAlignmentMin;

        public int BufferNumRecommended => this.bufferNumRecommended;

        public int BufferSizeRecommended => this.bufferSizeRecommended;

        public MMAL_PARAMETER_BUFFER_REQUIREMENTS_T(MMAL_PARAMETER_HEADER_T hdr, int bufferNumMin, int bufferSizeMin, int bufferAlignmentMin, int bufferNumRecommended, int bufferSizeRecommended)
        {
            this.Hdr = hdr;
            this.bufferNumMin = bufferNumMin;
            this.bufferSizeMin = bufferSizeMin;
            this.bufferAlignmentMin = bufferAlignmentMin;
            this.bufferNumRecommended = bufferNumRecommended;
            this.bufferSizeRecommended = bufferSizeRecommended;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_SEEK_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private long offset;
        private uint flags;

        public long Offset => this.offset;

        public uint Flags => this.flags;

        public MMAL_PARAMETER_SEEK_T(MMAL_PARAMETER_HEADER_T hdr, long offset, uint flags)
        {
            this.Hdr = hdr;
            this.offset = offset;
            this.flags = flags;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_STATISTICS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        private uint bufferCount;
        private uint frameCount;
        private uint framesSkipped;
        private uint framesDiscarded;
        private uint eosSeen;
        private uint maximumFrameBytes;
        private uint totalBytes;
        private uint corruptMacroblocks;

        public uint BufferCount => this.bufferCount;

        public uint FrameCount => this.frameCount;

        public uint FramesSkipped => this.framesSkipped;

        public uint FramesDiscarded => this.framesDiscarded;

        public uint EosSeen => this.eosSeen;

        public uint MaximumFrameBytes => this.maximumFrameBytes;

        public uint TotalBytes => this.totalBytes;

        public uint CorruptMacroBlocks => this.corruptMacroblocks;

        public MMAL_PARAMETER_STATISTICS_T(MMAL_PARAMETER_HEADER_T hdr, uint bufferCount, uint frameCount, uint framesSkipped,
                                           uint framesDiscarded, uint eosSeen, uint maximumFrameBytes, uint totalBytes,
                                           uint corruptMacroblocks)
        {
            this.Hdr = hdr;
            this.bufferCount = bufferCount;
            this.frameCount = frameCount;
            this.framesSkipped = framesSkipped;
            this.framesDiscarded = framesDiscarded;
            this.eosSeen = eosSeen;
            this.maximumFrameBytes = maximumFrameBytes;
            this.totalBytes = totalBytes;
            this.corruptMacroblocks = corruptMacroblocks;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CORE_STATISTICS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        private MMAL_CORE_STATS_DIR dir;
        private int reset;
        private MMAL_CORE_STATISTICS_T stats;

        public MMAL_CORE_STATS_DIR Dir => this.dir;

        public int Reset => this.reset;

        public MMAL_CORE_STATISTICS_T Stats => this.stats;

        public MMAL_PARAMETER_CORE_STATISTICS_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CORE_STATS_DIR dir, int reset,
                                                MMAL_CORE_STATISTICS_T stats)
        {
            this.Hdr = hdr;
            this.dir = dir;
            this.reset = reset;
            this.stats = stats;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_MEM_USAGE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private int poolMemAllocSize;

        public int PoolMemAllocSize => this.poolMemAllocSize;

        public MMAL_PARAMETER_MEM_USAGE_T(MMAL_PARAMETER_HEADER_T hdr, int poolMemAllocSize)
        {
            this.Hdr = hdr;
            this.poolMemAllocSize = poolMemAllocSize;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_LOGGING_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        private uint set;
        private uint clear;

        public uint Set => this.set;

        public uint Clear => this.clear;

        public MMAL_PARAMETER_LOGGING_T(MMAL_PARAMETER_HEADER_T hdr, uint set, uint clear)
        {
            this.Hdr = hdr;
            this.set = set;
            this.clear = clear;
        }
    }

    // mmal_parameters_camera.h
    public static class MMALParametersCamera
    {
        public const int MMAL_PARAMETER_THUMBNAIL_CONFIGURATION = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA;
        public const int MMAL_PARAMETER_CAPTURE_QUALITY = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 1;
        public const int MMAL_PARAMETER_ROTATION = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 2;
        public const int MMAL_PARAMETER_EXIF_DISABLE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 3;
        public const int MMAL_PARAMETER_EXIF = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 4;
        public const int MMAL_PARAMETER_AWB_MODE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 5;
        public const int MMAL_PARAMETER_IMAGE_EFFECT = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 6;
        public const int MMAL_PARAMETER_COLOUR_EFFECT = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 7;
        public const int MMAL_PARAMETER_FLICKER_AVOID = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 8;
        public const int MMAL_PARAMETER_FLASH = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 9;
        public const int MMAL_PARAMETER_REDEYE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 10;
        public const int MMAL_PARAMETER_FOCUS = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 11;
        public const int MMAL_PARAMETER_FOCAL_LENGTHS = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 12;
        public const int MMAL_PARAMETER_EXPOSURE_COMP = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 13;
        public const int MMAL_PARAMETER_ZOOM = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 14;
        public const int MMAL_PARAMETER_MIRROR = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 15;
        public const int MMAL_PARAMETER_CAMERA_NUM = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 16;
        public const int MMAL_PARAMETER_CAPTURE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 17;
        public const int MMAL_PARAMETER_EXPOSURE_MODE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 18;
        public const int MMAL_PARAMETER_EXP_METERING_MODE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 19;
        public const int MMAL_PARAMETER_FOCUS_STATUS = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 20;
        public const int MMAL_PARAMETER_CAMERA_CONFIG = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 21;
        public const int MMAL_PARAMETER_CAPTURE_STATUS = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 22;
        public const int MMAL_PARAMETER_FACE_TRACK = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 23;
        public const int MMAL_PARAMETER_DRAW_BOX_FACES_AND_FOCUS = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 24;
        public const int MMAL_PARAMETER_JPEG_Q_FACTOR = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 25;
        public const int MMAL_PARAMETER_FRAME_RATE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 26;
        public const int MMAL_PARAMETER_USE_STC = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 27;
        public const int MMAL_PARAMETER_CAMERA_INFO = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 28;
        public const int MMAL_PARAMETER_VIDEO_STABILISATION = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 29;
        public const int MMAL_PARAMETER_FACE_TRACK_RESULTS = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 30;
        public const int MMAL_PARAMETER_ENABLE_RAW_CAPTURE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 31;
        public const int MMAL_PARAMETER_DPF_FILE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 32;
        public const int MMAL_PARAMETER_ENABLE_DPF_FILE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 33;
        public const int MMAL_PARAMETER_DPF_FAIL_IS_FATAL = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 34;
        public const int MMAL_PARAMETER_CAPTURE_MODE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 35;
        public const int MMAL_PARAMETER_FOCUS_REGIONS = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 36;
        public const int MMAL_PARAMETER_INPUT_CROP = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 37;
        public const int MMAL_PARAMETER_SENSOR_INFORMATION = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 38;
        public const int MMAL_PARAMETER_FLASH_SELECT = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 39;
        public const int MMAL_PARAMETER_FIELD_OF_VIEW = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 40;
        public const int MMAL_PARAMETER_HIGH_DYNAMIC_RANGE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 41;
        public const int MMAL_PARAMETER_DYNAMIC_RANGE_COMPRESSION = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 42;
        public const int MMAL_PARAMETER_ALGORITHM_CONTROL = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 43;
        public const int MMAL_PARAMETER_SHARPNESS = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 44;
        public const int MMAL_PARAMETER_CONTRAST = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 45;
        public const int MMAL_PARAMETER_BRIGHTNESS = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 46;
        public const int MMAL_PARAMETER_SATURATION = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 47;
        public const int MMAL_PARAMETER_ISO = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 48;
        public const int MMAL_PARAMETER_ANTISHAKE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 49;
        public const int MMAL_PARAMETER_IMAGE_EFFECT_PARAMETERS = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 50;
        public const int MMAL_PARAMETER_CAMERA_BURST_CAPTURE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 51;
        public const int MMAL_PARAMETER_CAMERA_MIN_ISO = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 52;
        public const int MMAL_PARAMETER_CAMERA_USE_CASE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 53;
        public const int MMAL_PARAMETER_CAPTURE_STATS_PASS = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 54;
        public const int MMAL_PARAMETER_CAMERA_CUSTOM_SENSOR_CONFIG = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 55;
        public const int MMAL_PARAMETER_ENABLE_REGISTER_FILE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 56;
        public const int MMAL_PARAMETER_REGISTER_FAIL_IS_FATAL = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 57;
        public const int MMAL_PARAMETER_CONFIGFILE_REGISTERS = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 58;
        public const int MMAL_PARAMETER_CONFIGFILE_CHUNK_REGISTERS = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 59;
        public const int MMAL_PARAMETER_JPEG_ATTACH_LOG = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 60;
        public const int MMAL_PARAMETER_ZERO_SHUTTER_LAG = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 61;
        public const int MMAL_PARAMETER_FPS_RANGE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 62;
        public const int MMAL_PARAMETER_CAPTURE_EXPOSURE_COMP = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 63;
        public const int MMAL_PARAMETER_SW_SHARPEN_DISABLE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 64;
        public const int MMAL_PARAMETER_FLASH_REQUIRED = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 65;
        public const int MMAL_PARAMETER_SW_SATURATION_DISABLE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 66;
        public const int MMAL_PARAMETER_SHUTTER_SPEED = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 67;
        public const int MMAL_PARAMETER_CUSTOM_AWB_GAINS = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 68;
        public const int MMAL_PARAMETER_CAMERA_SETTINGS = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 69;
        public const int MMAL_PARAMETER_PRIVACY_INDICATOR = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 70;
        public const int MMAL_PARAMETER_VIDEO_DENOISE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 71;
        public const int MMAL_PARAMETER_STILLS_DENOISE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 72;
        public const int MMAL_PARAMETER_ANNOTATE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 73;
        public const int MMAL_PARAMETER_STEREOSCOPIC_MODE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 74;
        public const int MMAL_PARAMETER_CAMERA_INTERFACE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 75;
        public const int MMAL_PARAMETER_CAMERA_CLOCKING_MODE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 76;
        public const int MMAL_PARAMETER_CAMERA_RX_CONFIG = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 77;
        public const int MMAL_PARAMETER_CAMERA_RX_TIMING = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 78;
        public const int MMAL_PARAMETER_DPF_CONFIG = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 79;
        public const int MMAL_PARAMETER_JPEG_RESTART_INTERVAL = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 80;
        public const int MMAL_PARAMETER_CAMERA_ISP_BLOCK_OVERRIDE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 81;
        public const int MMAL_PARAMETER_LENS_SHADING_OVERRIDE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 82;
        public const int MMAL_PARAMETER_BLACK_LEVEL = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 83;
        public const int MMAL_PARAMETER_RESIZE_PARAMS = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 84;
        public const int MMAL_PARAMETER_CROP = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 85;
        public const int MMAL_PARAMETER_OUTPUT_SHIFT = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 86;
        public const int MMAL_PARAMETER_CCM_SHIFT = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 87;
        public const int MMAL_PARAMETER_CUSTOM_CCM = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 88;
        public const int MMAL_PARAMETER_ANALOG_GAIN = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 89;
        public const int MMAL_PARAMETER_DIGITAL_GAIN = MMALParametersCommon.MMAL_PARAMETER_GROUP_CAMERA + 90;

        public const int MMAL_MAX_IMAGEFX_PARAMETERS = 6;

        public const int MMAL_PARAMETER_CAMERA_INFO_MAX_CAMERAS = 4;
        public const int MMAL_PARAMETER_CAMERA_INFO_MAX_FLASHES = 2;
        public const int MMAL_PARAMETER_CAMERA_INFO_MAX_STR_LEN = 16;

        public const int MMAL_CAMERA_ANNOTATE_MAX_TEXT_LEN = 32;
        public const int MMAL_CAMERA_ANNOTATE_MAX_TEXT_LEN_V2 = 128;
        public const int MMAL_CAMERA_ANNOTATE_MAX_TEXT_LEN_V3 = 128;
    }

    public enum MMAL_PARAM_EXPOSUREMODE_T
    {
        MMAL_PARAM_EXPOSUREMODE_OFF,
        MMAL_PARAM_EXPOSUREMODE_AUTO,
        MMAL_PARAM_EXPOSUREMODE_NIGHT,
        MMAL_PARAM_EXPOSUREMODE_NIGHTPREVIEW,
        MMAL_PARAM_EXPOSUREMODE_BACKLIGHT,
        MMAL_PARAM_EXPOSUREMODE_SPOTLIGHT,
        MMAL_PARAM_EXPOSUREMODE_SPORTS,
        MMAL_PARAM_EXPOSUREMODE_SNOW,
        MMAL_PARAM_EXPOSUREMODE_BEACH,
        MMAL_PARAM_EXPOSUREMODE_VERYLONG,
        MMAL_PARAM_EXPOSUREMODE_FIXEDFPS,
        MMAL_PARAM_EXPOSUREMODE_ANTISHAKE,
        MMAL_PARAM_EXPOSUREMODE_FIREWORKS,
        MMAL_PARAM_EXPOSUREMODE_MAX = 0x7fffffff
    }

    public enum MMAL_PARAM_EXPOSUREMETERINGMODE_T
    {
        MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE,
        MMAL_PARAM_EXPOSUREMETERINGMODE_SPOT,
        MMAL_PARAM_EXPOSUREMETERINGMODE_BACKLIT,
        MMAL_PARAM_EXPOSUREMETERINGMODE_MATRIX,
        MMAL_PARAM_EXPOSUREMETERINGMODE_MAX = 0x7fffffff
    }

    public enum MMAL_PARAM_AWBMODE_T
    {
        MMAL_PARAM_AWBMODE_OFF,
        MMAL_PARAM_AWBMODE_AUTO,
        MMAL_PARAM_AWBMODE_SUNLIGHT,
        MMAL_PARAM_AWBMODE_CLOUDY,
        MMAL_PARAM_AWBMODE_SHADE,
        MMAL_PARAM_AWBMODE_TUNGSTEN,
        MMAL_PARAM_AWBMODE_FLUORESCENT,
        MMAL_PARAM_AWBMODE_INCANDESCENT,
        MMAL_PARAM_AWBMODE_FLASH,
        MMAL_PARAM_AWBMODE_HORIZON,
        MMAL_PARAM_AWBMODE_GREYWORLD,
        MMAL_PARAM_AWBMODE_MAX = 0x7fffffff
    }

    public enum MMAL_PARAM_IMAGEFX_T
    {
        MMAL_PARAM_IMAGEFX_NONE,
        MMAL_PARAM_IMAGEFX_NEGATIVE,
        MMAL_PARAM_IMAGEFX_SOLARIZE,
        MMAL_PARAM_IMAGEFX_POSTERIZE,
        MMAL_PARAM_IMAGEFX_WHITEBOARD,
        MMAL_PARAM_IMAGEFX_BLACKBOARD,
        MMAL_PARAM_IMAGEFX_SKETCH,
        MMAL_PARAM_IMAGEFX_DENOISE,
        MMAL_PARAM_IMAGEFX_EMBOSS,
        MMAL_PARAM_IMAGEFX_OILPAINT,
        MMAL_PARAM_IMAGEFX_HATCH,
        MMAL_PARAM_IMAGEFX_GPEN,
        MMAL_PARAM_IMAGEFX_PASTEL,
        MMAL_PARAM_IMAGEFX_WATERCOLOUR,
        MMAL_PARAM_IMAGEFX_FILM,
        MMAL_PARAM_IMAGEFX_BLUR,
        MMAL_PARAM_IMAGEFX_SATURATION,
        MMAL_PARAM_IMAGEFX_COLOURSWAP,
        MMAL_PARAM_IMAGEFX_WASHEDOUT,
        MMAL_PARAM_IMAGEFX_POSTERISE,
        MMAL_PARAM_IMAGEFX_COLOURPOINT,
        MMAL_PARAM_IMAGEFX_COLOURBALANCE,
        MMAL_PARAM_IMAGEFX_CARTOON,
        MMAL_PARAM_IMAGEFX_DEINTERLACE_DOUBLE,
        MMAL_PARAM_IMAGEFX_DEINTERLACE_ADV,
        MMAL_PARAM_IMAGEFX_DEINTERLACE_FAST,
        MMAL_PARAM_IMAGEFX_MAX = 0x7fffffff
    }

    public enum MMAL_CAMERA_STC_MODE_T
    {
        MMAL_PARAM_STC_MODE_OFF,
        MMAL_PARAM_STC_MODE_RAW,
        MMAL_PARAM_STC_MODE_COOKED,
        MMAL_PARAM_STC_MODE_MAX = 0x7fffffff
    }

    public enum MMAL_PARAM_FLICKERAVOID_T
    {
        MMAL_PARAM_FLICKERAVOID_OFF,
        MMAL_PARAM_FLICKERAVOID_AUTO,
        MMAL_PARAM_FLICKERAVOID_50HZ,
        MMAL_PARAM_FLICKERAVOID_60HZ,
        MMAL_PARAM_FLICKERAVOID_MAX = 0x7FFFFFFF
    }

    public enum MMAL_PARAM_FLASH_T
    {
        MMAL_PARAM_FLASH_OFF,
        MMAL_PARAM_FLASH_AUTO,
        MMAL_PARAM_FLASH_ON,
        MMAL_PARAM_FLASH_REDEYE,
        MMAL_PARAM_FLASH_FILLIN,
        MMAL_PARAM_FLASH_TORCH,
        MMAL_PARAM_FLASH_MAX = 0x7FFFFFFF
    }

    public enum MMAL_PARAM_REDEYE_T
    {
        MMAL_PARAM_REDEYE_OFF,
        MMAL_PARAM_REDEYE_ON,
        MMAL_PARAM_REDEYE_SIMPLE,
        MMAL_PARAM_REDEYE_MAX = 0x7FFFFFFF
    }

    public enum MMAL_PARAM_FOCUS_T
    {
        MMAL_PARAM_FOCUS_AUTO,
        MMAL_PARAM_FOCUS_AUTO_NEAR,
        MMAL_PARAM_FOCUS_AUTO_MACRO,
        MMAL_PARAM_FOCUS_CAF,
        MMAL_PARAM_FOCUS_CAF_NEAR,
        MMAL_PARAM_FOCUS_FIXED_INFINITY,
        MMAL_PARAM_FOCUS_FIXED_HYPERFOCAL,
        MMAL_PARAM_FOCUS_FIXED_NEAR,
        MMAL_PARAM_FOCUS_FIXED_MACRO,
        MMAL_PARAM_FOCUS_EDOF,
        MMAL_PARAM_FOCUS_CAF_MACRO,
        MMAL_PARAM_FOCUS_CAF_FAST,
        MMAL_PARAM_FOCUS_CAF_NEAR_FAST,
        MMAL_PARAM_FOCUS_CAF_MACRO_FAST,
        MMAL_PARAM_FOCUS_FIXED_CURRENT,
        MMAL_PARAM_FOCUS_MAX = 0x7FFFFFFF
    }

    public enum MMAL_PARAM_CAPTURE_STATUS_T
    {
        MMAL_PARAM_CAPTURE_STATUS_NOT_CAPTURING,
        MMAL_PARAM_CAPTURE_STATUS_CAPTURE_STARTED,
        MMAL_PARAM_CAPTURE_STATUS_CAPTURE_ENDED,
        MMAL_PARAM_CAPTURE_STATUS_MAX = 0x7FFFFFFF
    }

    public enum MMAL_PARAM_FOCUS_STATUS_T
    {
        MMAL_PARAM_FOCUS_STATUS_OFF,
        MMAL_PARAM_FOCUS_STATUS_REQUEST,
        MMAL_PARAM_FOCUS_STATUS_REACHED,
        MMAL_PARAM_FOCUS_STATUS_UNABLE_TO_REACH,
        MMAL_PARAM_FOCUS_STATUS_LOST,
        MMAL_PARAM_FOCUS_STATUS_CAF_MOVING,
        MMAL_PARAM_FOCUS_STATUS_CAF_SUCCESS,
        MMAL_PARAM_FOCUS_STATUS_CAF_FAILED,
        MMAL_PARAM_FOCUS_STATUS_MANUAL_MOVING,
        MMAL_PARAM_FOCUS_STATUS_MANUAL_REACHED,
        MMAL_PARAM_FOCUS_STATUS_CAF_WATCHING,
        MMAL_PARAM_FOCUS_STATUS_CAF_SCENE_CHANGED,
        MMAL_PARAM_FOCUS_STATUS_MAX = 0x7FFFFFFF
    }

    public enum MMAL_PARAM_FACE_TRACK_MODE_T
    {
        MMAL_PARAM_FACE_DETECT_NONE,
        MMAL_PARAM_FACE_DETECT_ON,
        MMAL_PARAM_FACE_DETECT_MAX = 0x7FFFFFFF
    }

    public enum MMAL_PARAMETER_CAMERA_CONFIG_TIMESTAMP_MODE_T
    {
        MMAL_PARAM_TIMESTAMP_MODE_ZERO,
        MMAL_PARAM_TIMESTAMP_MODE_RAW_STC,
        MMAL_PARAM_TIMESTAMP_MODE_RESET_STC,
        MMAL_PARAM_TIMESTAMP_MODE_MAX = 0x7FFFFFFF
    }

    public enum MMAL_PARAMETER_CAMERA_INFO_FLASH_TYPE_T
    {
        MMAL_PARAMETER_CAMERA_INFO_FLASH_TYPE_XENON,
        MMAL_PARAMETER_CAMERA_INFO_FLASH_TYPE_LED,
        MMAL_PARAMETER_CAMERA_INFO_FLASH_TYPE_OTHER,
        MMAL_PARAMETER_CAMERA_INFO_FLASH_TYPE_MAX = 0x7FFFFFFF
    }

    public enum MMAL_PARAMETER_CAPTUREMODE_MODE_T
    {
        MMAL_PARAM_CAPTUREMODE_WAIT_FOR_END,
        MMAL_PARAM_CAPTUREMODE_WAIT_FOR_END_AND_HOLD,
        MMAL_PARAM_CAPTUREMODE_RESUME_VF_IMMEDIATELY
    }

    public enum MMAL_PARAMETER_FOCUS_REGION_TYPE_T
    {
        MMAL_PARAMETER_FOCUS_REGION_TYPE_NORMAL,
        MMAL_PARAMETER_FOCUS_REGION_TYPE_FACE,
        MMAL_PARAMETER_FOCUS_REGION_TYPE_MAX
    }

    public enum MMAL_PARAMETER_DRC_STRENGTH_T
    {
        MMAL_PARAMETER_DRC_STRENGTH_OFF,
        MMAL_PARAMETER_DRC_STRENGTH_LOW,
        MMAL_PARAMETER_DRC_STRENGTH_MEDIUM,
        MMAL_PARAMETER_DRC_STRENGTH_HIGH,
        MMAL_PARAMETER_DRC_STRENGTH_MAX = 0x7fffffff
    }

    public enum MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_T
    {
        MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_FACETRACKING,
        MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_REDEYE_REDUCTION,
        MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_VIDEO_STABILISATION,
        MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_WRITE_RAW,
        MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_VIDEO_DENOISE,
        MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_STILLS_DENOISE,
        MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_TEMPORAL_DENOISE,
        MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_ANTISHAKE,
        MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_IMAGE_EFFECTS,
        MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_DYNAMIC_RANGE_COMPRESSION,
        MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_FACE_RECOGNITION,
        MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_FACE_BEAUTIFICATION,
        MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_SCENE_DETECTION,
        MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_HIGH_DYNAMIC_RANGE,
        MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_MAX = 0x7fffffff
    }

    public enum MMAL_PARAM_CAMERA_USE_CASE_T
    {
        MMAL_PARAM_CAMERA_USE_CASE_UNKNOWN,
        MMAL_PARAM_CAMERA_USE_CASE_STILLS_CAPTURE,
        MMAL_PARAM_CAMERA_USE_CASE_VIDEO_CAPTURE,
        MMAL_PARAM_CAMERA_USE_CASE_MAX = 0x7fffffff
    }

    public enum MMAL_PARAM_PRIVACY_INDICATOR_T
    {
        MMAL_PARAMETER_PRIVACY_INDICATOR_OFF,
        MMAL_PARAMETER_PRIVACY_INDICATOR_ON,
        MMAL_PARAMETER_PRIVACY_INDICATOR_FORCE_ON,
        MMAL_PARAMETER_PRIVACY_INDICATOR_MAX = 0x7fffffff
    }

    public enum MMAL_STEREOSCOPIC_MODE_T
    {
        MMAL_STEREOSCOPIC_MODE_NONE,
        MMAL_STEREOSCOPIC_MODE_SIDE_BY_SIDE,
        MMAL_STEREOSCOPIC_MODE_BOTTOM,
        MMAL_STEREOSCOPIC_MODE_MAX = 0x7fffffff
    }

    public enum MMAL_CAMERA_INTERFACE_T
    {
        MMAL_CAMERA_INTERFACE_CSI2,
        MMAL_CAMERA_INTERFACE_CCP2,
        MMAL_CAMERA_INTERFACE_CPI,
        MMAL_CAMERA_INTERFACE_MAX = 0x7fffffff
    }

    public enum MMAL_CAMERA_CLOCKING_MODE_T
    {
        MMAL_CAMERA_CLOCKING_MODE_STROBE,
        MMAL_CAMERA_CLOCKING_MODE_CLOCK,
        MMAL_CAMERA_CLOCKING_MODE_MAX = 0x7fffffff
    }

    public enum MMAL_CAMERA_RX_CONFIG_DECODE
    {
        MMAL_CAMERA_RX_CONFIG_DECODE_NONE,
        MMAL_CAMERA_RX_CONFIG_DECODE_DPCM8TO10,
        MMAL_CAMERA_RX_CONFIG_DECODE_DPCM7TO10,
        MMAL_CAMERA_RX_CONFIG_DECODE_DPCM6TO10,
        MMAL_CAMERA_RX_CONFIG_DECODE_DPCM8TO12,
        MMAL_CAMERA_RX_CONFIG_DECODE_DPCM7TO12,
        MMAL_CAMERA_RX_CONFIG_DECODE_DPCM6TO12,
        MMAL_CAMERA_RX_CONFIG_DECODE_DPCM10TO14,
        MMAL_CAMERA_RX_CONFIG_DECODE_DPCM8TO14,
        MMAL_CAMERA_RX_CONFIG_DECODE_DPCM12TO16,
        MMAL_CAMERA_RX_CONFIG_DECODE_DPCM10TO16,
        MMAL_CAMERA_RX_CONFIG_DECODE_DPCM8TO16,
        MMAL_CAMERA_RX_CONFIG_DECODE_MAX = 0x7fffffff
    }

    public enum MMAL_CAMERA_RX_CONFIG_ENCODE
    {
        MMAL_CAMERA_RX_CONFIG_ENCODE_NONE,
        MMAL_CAMERA_RX_CONFIG_ENCODE_DPCM10TO8,
        MMAL_CAMERA_RX_CONFIG_ENCODE_DPCM12TO8,
        MMAL_CAMERA_RX_CONFIG_ENCODE_DPCM14TO8,
        MMAL_CAMERA_RX_CONFIG_ENCODE_MAX = 0x7fffffff
    }

    public enum MMAL_CAMERA_RX_CONFIG_UNPACK
    {
        MMAL_CAMERA_RX_CONFIG_UNPACK_NONE,
        MMAL_CAMERA_RX_CONFIG_UNPACK_6,
        MMAL_CAMERA_RX_CONFIG_UNPACK_7,
        MMAL_CAMERA_RX_CONFIG_UNPACK_8,
        MMAL_CAMERA_RX_CONFIG_UNPACK_10,
        MMAL_CAMERA_RX_CONFIG_UNPACK_12,
        MMAL_CAMERA_RX_CONFIG_UNPACK_14,
        MMAL_CAMERA_RX_CONFIG_UNPACK_16,
        MMAL_CAMERA_RX_CONFIG_UNPACK_MAX = 0x7fffffff
    }

    public enum MMAL_CAMERA_RX_CONFIG_PACK
    {
        MMAL_CAMERA_RX_CONFIG_PACK_NONE,
        MMAL_CAMERA_RX_CONFIG_PACK_8,
        MMAL_CAMERA_RX_CONFIG_PACK_10,
        MMAL_CAMERA_RX_CONFIG_PACK_12,
        MMAL_CAMERA_RX_CONFIG_PACK_14,
        MMAL_CAMERA_RX_CONFIG_PACK_16,
        MMAL_CAMERA_RX_CONFIG_PACK_RAW10,
        MMAL_CAMERA_RX_CONFIG_PACK_RAW12,
        MMAL_CAMERA_RX_CONFIG_PACK_MAX = 0x7fffffff
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_THUMBNAIL_CONFIG_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        private int _enable;
        private int _width;
        private int _height;
        private int _quality;

        public bool Enable => _enable == 1;

        public int Width => _width;

        public int Height => _height;

        public int Quality => _quality;

        public MMAL_PARAMETER_THUMBNAIL_CONFIG_T(MMAL_PARAMETER_HEADER_T hdr, bool enable, int width, int height, int quality)
        {
            this.Hdr = hdr;

            _enable = enable ? 1 : 0;
            _width = width;
            _height = height;
            _quality = quality;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MMAL_PARAMETER_EXIF_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        private int keylen;
        private int valueOffset;
        private int valueLen;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        private byte[] data;

        public int KeyLen => this.keylen;

        public int ValueOffset => this.valueOffset;

        public int ValueLen => this.valueLen;

        public byte[] Data => this.data;

        public MMAL_PARAMETER_EXIF_T(MMAL_PARAMETER_HEADER_T hdr, int keylen, int valueOffset, int valueLen, byte[] data)
        {
            this.Hdr = hdr;
            this.keylen = keylen;
            this.valueOffset = valueOffset;
            this.valueLen = valueLen;
            this.data = data;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MMAL_PARAMETER_EXIF_T_DUMMY
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        private int keylen;
        private int valueOffset;
        private int valueLen;
        private byte data;

        public int KeyLen => this.keylen;

        public int ValueOffset => this.valueOffset;

        public int ValueLen => this.valueLen;

        public byte Data => this.data;

        public MMAL_PARAMETER_EXIF_T_DUMMY(MMAL_PARAMETER_HEADER_T hdr, int keylen, int valueOffset, int valueLen, byte data)
        {
            this.Hdr = hdr;
            this.keylen = keylen;
            this.valueOffset = valueOffset;
            this.valueLen = valueLen;
            this.data = data;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_EXPOSUREMODE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        private MMAL_PARAM_EXPOSUREMODE_T value;

        public MMAL_PARAM_EXPOSUREMODE_T Value => this.value;

        public MMAL_PARAMETER_EXPOSUREMODE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_EXPOSUREMODE_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_EXPOSUREMETERINGMODE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        private MMAL_PARAM_EXPOSUREMETERINGMODE_T value;

        public MMAL_PARAM_EXPOSUREMETERINGMODE_T Value => this.value;

        public MMAL_PARAMETER_EXPOSUREMETERINGMODE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_EXPOSUREMETERINGMODE_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_AWBMODE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        private MMAL_PARAM_AWBMODE_T value;

        public MMAL_PARAM_AWBMODE_T Value => this.value;

        public MMAL_PARAMETER_AWBMODE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_AWBMODE_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_IMAGEFX_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_PARAM_IMAGEFX_T value;

        public MMAL_PARAM_IMAGEFX_T Value => value;

        public MMAL_PARAMETER_IMAGEFX_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_IMAGEFX_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_IMAGEFX_PARAMETERS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        private MMAL_PARAM_IMAGEFX_T effect;
        private int numEffectParams;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MMALParametersCamera.MMAL_MAX_IMAGEFX_PARAMETERS)]
        private int[] effectParameter;

        public MMAL_PARAM_IMAGEFX_T Effect => this.effect;

        public int NumEffectParams => this.numEffectParams;

        public int[] EffectParameter => this.effectParameter;

        public MMAL_PARAMETER_IMAGEFX_PARAMETERS_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_IMAGEFX_T effect, int numEffectParams, int[] effectParameter)
        {
            this.Hdr = hdr;
            this.effect = effect;
            this.numEffectParams = numEffectParams;
            this.effectParameter = effectParameter;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_COLOURFX_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        private int enable;
        private int u;
        private int v;

        public int Enable => this.enable;

        public int U => this.u;

        public int V => this.v;

        public MMAL_PARAMETER_COLOURFX_T(MMAL_PARAMETER_HEADER_T hdr, int enable, int u, int v)
        {
            this.Hdr = hdr;
            this.enable = enable;
            this.u = u;
            this.v = v;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_STC_MODE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_CAMERA_STC_MODE_T value;

        public MMAL_CAMERA_STC_MODE_T Value => this.value;

        public MMAL_PARAMETER_CAMERA_STC_MODE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CAMERA_STC_MODE_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FLICKERAVOID_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_PARAM_FLICKERAVOID_T value;

        public MMAL_PARAM_FLICKERAVOID_T Value => value;

        public MMAL_PARAMETER_FLICKERAVOID_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_FLICKERAVOID_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FLASH_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_PARAM_FLASH_T value;

        public MMAL_PARAM_FLASH_T Value => value;

        public MMAL_PARAMETER_FLASH_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_FLASH_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_REDEYE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_PARAM_REDEYE_T value;

        public MMAL_PARAM_REDEYE_T Value => value;

        public MMAL_PARAMETER_REDEYE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_REDEYE_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FOCUS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_PARAM_FOCUS_T value;

        public MMAL_PARAM_FOCUS_T Value => value;

        public MMAL_PARAMETER_FOCUS_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_FOCUS_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAPTURE_STATUS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_PARAM_CAPTURE_STATUS_T value;

        public MMAL_PARAM_CAPTURE_STATUS_T Value => value;

        public MMAL_PARAMETER_CAPTURE_STATUS_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_CAPTURE_STATUS_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FOCUS_STATUS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_PARAM_FOCUS_STATUS_T value;

        public MMAL_PARAM_FOCUS_STATUS_T Value => value;

        public MMAL_PARAMETER_FOCUS_STATUS_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_FOCUS_STATUS_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FACE_TRACK_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_PARAM_FACE_TRACK_MODE_T mode;
        private uint maxRegions, frames, quality;

        public MMAL_PARAM_FACE_TRACK_MODE_T Value => mode;
        public uint MaxRegions => maxRegions;
        public uint Frames => frames;
        public uint Quality => quality;

        public MMAL_PARAMETER_FACE_TRACK_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_FACE_TRACK_MODE_T mode,
                                           uint maxRegions, uint frames, uint quality)
        {
            this.Hdr = hdr;
            this.mode = mode;
            this.maxRegions = maxRegions;
            this.frames = frames;
            this.quality = quality;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FACE_TRACK_FACE_T
    {
        private int faceId, score;
        private MMAL_RECT_T faceRect;
        private MMAL_RECT_T[] eyeRect;
        private MMAL_RECT_T mouthRect;

        public int FaceId => faceId;
        public int Score => score;
        public MMAL_RECT_T FaceRect => faceRect;
        public MMAL_RECT_T[] EyeRect => eyeRect;
        public MMAL_RECT_T MouthRect => mouthRect;

        public MMAL_PARAMETER_FACE_TRACK_FACE_T(int faceId, int score, MMAL_RECT_T faceRect, MMAL_RECT_T[] eyeRect, MMAL_RECT_T mouthRect)
        {
            this.faceId = faceId;
            this.score = score;
            this.faceRect = faceRect;
            this.eyeRect = eyeRect;
            this.mouthRect = mouthRect;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FACE_TRACK_RESULTS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private uint numFaces, frameWidth, frameHeight;
        private MMAL_PARAMETER_FACE_TRACK_FACE_T[] faces;

        public uint NumFaces => numFaces;
        public uint FrameWidth => frameWidth;
        public uint FrameHeight => frameHeight;
        public MMAL_PARAMETER_FACE_TRACK_FACE_T[] Faces => faces;

        public MMAL_PARAMETER_FACE_TRACK_RESULTS_T(MMAL_PARAMETER_HEADER_T hdr, uint numFaces, uint frameWidth, uint frameHeight,
                                                   MMAL_PARAMETER_FACE_TRACK_FACE_T[] faces)
        {
            this.Hdr = hdr;
            this.numFaces = numFaces;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.faces = faces;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_CAMERA_CONFIG_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private int maxStillsW, maxStillsH, stillsYUV422, oneShotStills, maxPreviewVideoW, maxPreviewVideoH, numPreviewVideoFrames,
                    stillsCaptureCircularBufferHeight, fastPreviewResume;
        private MMAL_PARAMETER_CAMERA_CONFIG_TIMESTAMP_MODE_T useSTCTimestamp;

        public int MaxStillsW => maxStillsW;
        public int MaxStillsH => maxStillsH;
        public int StillsYUV422 => stillsYUV422;
        public int OneShotStills => oneShotStills;
        public int MaxPreviewVideoW => maxPreviewVideoW;
        public int MaxPreviewVideoH => maxPreviewVideoH;
        public int NumPreviewVideoFrames => numPreviewVideoFrames;
        public int StillsCaptureCircularBufferHeight => stillsCaptureCircularBufferHeight;
        public int FastPreviewResume => fastPreviewResume;
        public MMAL_PARAMETER_CAMERA_CONFIG_TIMESTAMP_MODE_T UseSTCTimestamp => useSTCTimestamp;

        public MMAL_PARAMETER_CAMERA_CONFIG_T(MMAL_PARAMETER_HEADER_T hdr, int maxStillsW, int maxStillsH, int stillsYUV422,
                                              int oneShotStills, int maxPreviewVideoW, int maxPreviewVideoH, int numPreviewVideoFrames,
                                              int stillsCaptureCircularBufferHeight, int fastPreviewResume,
                                              MMAL_PARAMETER_CAMERA_CONFIG_TIMESTAMP_MODE_T useSTCTimestamp)
        {
            this.Hdr = hdr;
            this.maxStillsW = maxStillsW;
            this.maxStillsH = maxStillsH;
            this.stillsYUV422 = stillsYUV422;
            this.oneShotStills = oneShotStills;
            this.maxPreviewVideoW = maxPreviewVideoW;
            this.maxPreviewVideoH = maxPreviewVideoH;
            this.numPreviewVideoFrames = numPreviewVideoFrames;
            this.stillsCaptureCircularBufferHeight = stillsCaptureCircularBufferHeight;
            this.fastPreviewResume = fastPreviewResume;
            this.useSTCTimestamp = useSTCTimestamp;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_INFO_CAMERA_T
    {
        private int portId, maxWidth, maxHeight, lensPresent;

        public int PortId => portId;
        public int MaxWidth => maxWidth;
        public int MaxHeight => maxHeight;
        public int LensPresent => lensPresent;

        public MMAL_PARAMETER_CAMERA_INFO_CAMERA_T(int portId, int maxWidth, int maxHeight, int lensPresent)
        {
            this.portId = portId;
            this.maxWidth = maxWidth;
            this.maxHeight = maxHeight;
            this.lensPresent = lensPresent;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MMAL_PARAMETER_CAMERA_INFO_CAMERA_V2_T
    {
        private int portId, maxWidth, maxHeight, lensPresent;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        private string cameraName;

        public int PortId => portId;
        public int MaxWidth => maxWidth;
        public int MaxHeight => maxHeight;
        public int LensPresent => lensPresent;
        public string CameraName => cameraName;

        public MMAL_PARAMETER_CAMERA_INFO_CAMERA_V2_T(int portId, int maxWidth, int maxHeight, int lensPresent, string cameraName)
        {
            this.portId = portId;
            this.maxWidth = maxWidth;
            this.maxHeight = maxHeight;
            this.lensPresent = lensPresent;
            this.cameraName = cameraName;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_INFO_FLASH_T
    {
        private MMAL_PARAMETER_CAMERA_INFO_FLASH_TYPE_T flashType;

        public MMAL_PARAMETER_CAMERA_INFO_FLASH_TYPE_T FlashType => flashType;

        public MMAL_PARAMETER_CAMERA_INFO_FLASH_T(MMAL_PARAMETER_CAMERA_INFO_FLASH_TYPE_T flashType)
        {
            this.flashType = flashType;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_INFO_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private int numCameras, numFlashes;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        private MMAL_PARAMETER_CAMERA_INFO_CAMERA_T[] cameras;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        private MMAL_PARAMETER_CAMERA_INFO_FLASH_T[] flashes;

        public int NumCameras => numCameras;
        public int NumFlashes => numFlashes;
        public MMAL_PARAMETER_CAMERA_INFO_CAMERA_T[] Cameras => cameras;
        public MMAL_PARAMETER_CAMERA_INFO_FLASH_T[] Flashes => flashes;

        public MMAL_PARAMETER_CAMERA_INFO_T(MMAL_PARAMETER_HEADER_T hdr, int numCameras, int numFlashes, MMAL_PARAMETER_CAMERA_INFO_CAMERA_T[] cameras,
                                            MMAL_PARAMETER_CAMERA_INFO_FLASH_T[] flashes)
        {
            this.Hdr = hdr;
            this.numCameras = numCameras;
            this.numFlashes = numFlashes;
            this.cameras = cameras;
            this.flashes = flashes;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_INFO_V2_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private int numCameras, numFlashes;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 4)]
        private MMAL_PARAMETER_CAMERA_INFO_CAMERA_V2_T[] cameras;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 2)]
        private MMAL_PARAMETER_CAMERA_INFO_FLASH_T[] flashes;

        public int NumCameras => numCameras;
        public int NumFlashes => numFlashes;
        public MMAL_PARAMETER_CAMERA_INFO_CAMERA_V2_T[] Cameras => cameras;
        public MMAL_PARAMETER_CAMERA_INFO_FLASH_T[] Flashes => flashes;

        public MMAL_PARAMETER_CAMERA_INFO_V2_T(MMAL_PARAMETER_HEADER_T hdr, int numCameras, int numFlashes, MMAL_PARAMETER_CAMERA_INFO_CAMERA_V2_T[] cameras,
                                            MMAL_PARAMETER_CAMERA_INFO_FLASH_T[] flashes)
        {
            this.Hdr = hdr;
            this.numCameras = numCameras;
            this.numFlashes = numFlashes;
            this.cameras = cameras;
            this.flashes = flashes;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAPTUREMODE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_PARAMETER_CAPTUREMODE_MODE_T mode;

        public MMAL_PARAMETER_CAPTUREMODE_MODE_T Mode => mode;

        public MMAL_PARAMETER_CAPTUREMODE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAMETER_CAPTUREMODE_MODE_T mode)
        {
            this.Hdr = hdr;
            this.mode = mode;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FOCUS_REGION_T
    {
        private MMAL_RECT_T rect;
        private int weight, mask;
        private MMAL_PARAMETER_FOCUS_REGION_TYPE_T type;

        public MMAL_RECT_T Rect => rect;
        public int Weight => weight;
        public int Mask => mask;
        public MMAL_PARAMETER_FOCUS_REGION_TYPE_T Type => type;

        public MMAL_PARAMETER_FOCUS_REGION_T(MMAL_RECT_T rect, int weight, int mask, MMAL_PARAMETER_FOCUS_REGION_TYPE_T type)
        {
            this.rect = rect;
            this.weight = weight;
            this.mask = mask;
            this.type = type;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FOCUS_REGIONS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private uint numRegions;
        private int lockToFaces;
        private MMAL_PARAMETER_FOCUS_REGION_T[] regions;

        public uint NumRegions => numRegions;
        public int LockToFaces => lockToFaces;
        public MMAL_PARAMETER_FOCUS_REGION_T[] Regions => regions;

        public MMAL_PARAMETER_FOCUS_REGIONS_T(MMAL_PARAMETER_HEADER_T hdr, uint numRegions, int lockToFaces, MMAL_PARAMETER_FOCUS_REGION_T[] regions)
        {
            this.Hdr = hdr;
            this.numRegions = numRegions;
            this.lockToFaces = lockToFaces;
            this.regions = regions;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_INPUT_CROP_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_RECT_T rect;

        public MMAL_RECT_T Rect => rect;

        public MMAL_PARAMETER_INPUT_CROP_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_RECT_T rect)
        {
            this.Hdr = hdr;
            this.rect = rect;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_SENSOR_INFORMATION_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_RATIONAL_T fNumber, focalLength;
        private uint modelId, manufacturerId, revision;

        public MMAL_RATIONAL_T FNumber => fNumber;
        public MMAL_RATIONAL_T FocalLength => focalLength;
        public uint ModelId => modelId;
        public uint ManufacturerId => manufacturerId;
        public uint Revision => revision;

        public MMAL_PARAMETER_SENSOR_INFORMATION_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_RATIONAL_T fNumber, MMAL_RATIONAL_T focalLength,
                                                   uint modelId, uint manufacturerId, uint revision)
        {
            this.Hdr = hdr;
            this.fNumber = fNumber;
            this.focalLength = focalLength;
            this.modelId = modelId;
            this.manufacturerId = manufacturerId;
            this.revision = revision;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FLASH_SELECT_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_PARAMETER_CAMERA_INFO_FLASH_TYPE_T flashType;

        public MMAL_PARAMETER_CAMERA_INFO_FLASH_TYPE_T FlashType => flashType;

        public MMAL_PARAMETER_FLASH_SELECT_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAMETER_CAMERA_INFO_FLASH_TYPE_T flashType)
        {
            this.Hdr = hdr;
            this.flashType = flashType;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FIELD_OF_VIEW_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_RATIONAL_T fovH, fovV;

        public MMAL_RATIONAL_T FovH => fovH;
        public MMAL_RATIONAL_T FovV => fovV;

        public MMAL_PARAMETER_FIELD_OF_VIEW_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_RATIONAL_T fovH, MMAL_RATIONAL_T fovV)
        {
            this.Hdr = hdr;
            this.fovH = fovH;
            this.fovV = fovV;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_DRC_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_PARAMETER_DRC_STRENGTH_T strength;

        public MMAL_PARAMETER_DRC_STRENGTH_T Strength => strength;

        public MMAL_PARAMETER_DRC_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAMETER_DRC_STRENGTH_T strength)
        {
            this.Hdr = hdr;
            this.strength = strength;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_ALGORITHM_CONTROL_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_T algorithm;
        private int enabled;

        public MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_T Algorithm => algorithm;
        public int Enabled => enabled;

        public MMAL_PARAMETER_ALGORITHM_CONTROL_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_T algorithm,
                                                  int enabled)
        {
            this.Hdr = hdr;
            this.algorithm = algorithm;
            this.enabled = enabled;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_USE_CASE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_PARAM_CAMERA_USE_CASE_T useCase;

        public MMAL_PARAM_CAMERA_USE_CASE_T UseCase => useCase;

        public MMAL_PARAMETER_CAMERA_USE_CASE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_CAMERA_USE_CASE_T useCase)
        {
            this.Hdr = hdr;
            this.useCase = useCase;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FPS_RANGE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_RATIONAL_T fpsLow, fpsHigh;

        public MMAL_RATIONAL_T FpsLow => fpsLow;
        public MMAL_RATIONAL_T FpsHigh => fpsHigh;

        public MMAL_PARAMETER_FPS_RANGE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_RATIONAL_T fpsLow, MMAL_RATIONAL_T fpsHigh)
        {
            this.Hdr = hdr;
            this.fpsLow = fpsLow;
            this.fpsHigh = fpsHigh;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_ZEROSHUTTERLAG_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private int zeroShutterLagMode, concurrentCapture;

        public int ZeroShutterLagMode => zeroShutterLagMode;
        public int ConcurrentCapture => concurrentCapture;

        public MMAL_PARAMETER_ZEROSHUTTERLAG_T(MMAL_PARAMETER_HEADER_T hdr, int zeroShutterLagMode, int concurrentCapture)
        {
            this.Hdr = hdr;
            this.zeroShutterLagMode = zeroShutterLagMode;
            this.concurrentCapture = concurrentCapture;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_AWB_GAINS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_RATIONAL_T rGain, bGain;

        public MMAL_RATIONAL_T RGain => rGain;
        public MMAL_RATIONAL_T BGain => bGain;

        public MMAL_PARAMETER_AWB_GAINS_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_RATIONAL_T rGain, MMAL_RATIONAL_T bGain)
        {
            this.Hdr = hdr;
            this.rGain = rGain;
            this.bGain = bGain;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_SETTINGS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private int exposure;
        private MMAL_RATIONAL_T analogGain, digitalGain, awbRedGain, awbBlueGain;
        private int focusPosition;

        public int Exposure => exposure;
        public MMAL_RATIONAL_T AnalogGain => analogGain;
        public MMAL_RATIONAL_T DigitalGain => digitalGain;
        public MMAL_RATIONAL_T AwbRedGain => awbRedGain;
        public MMAL_RATIONAL_T AwbBlueGain => awbBlueGain;
        public int FocusPosition => focusPosition;

        public MMAL_PARAMETER_CAMERA_SETTINGS_T(MMAL_PARAMETER_HEADER_T hdr, int exposure, MMAL_RATIONAL_T analogGain,
                                                MMAL_RATIONAL_T digitalGain, MMAL_RATIONAL_T awbRedGain, MMAL_RATIONAL_T awbBlueGain,
                                                int focusPosition)
        {
            this.Hdr = hdr;
            this.exposure = exposure;
            this.analogGain = analogGain;
            this.digitalGain = digitalGain;
            this.awbRedGain = awbRedGain;
            this.awbBlueGain = awbBlueGain;
            this.focusPosition = focusPosition;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_PRIVACY_INDICATOR_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_PARAM_PRIVACY_INDICATOR_T mode;

        public MMAL_PARAM_PRIVACY_INDICATOR_T Mode => mode;

        public MMAL_PARAMETER_PRIVACY_INDICATOR_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_PRIVACY_INDICATOR_T mode)
        {
            this.Hdr = hdr;
            this.mode = mode;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_ANNOTATE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private int enable;
        private string text;
        private int showShutter, showAnalogGain, showLens, showCaf, showMotion;

        public int Enable => enable;
        public string Text => text;
        public int ShowShutter => showShutter;
        public int ShowAnalogGain => showAnalogGain;
        public int ShowLens => showLens;
        public int ShowCaf => showCaf;
        public int ShowMotion => showMotion;

        public MMAL_PARAMETER_CAMERA_ANNOTATE_T(MMAL_PARAMETER_HEADER_T hdr, int enable, string text,
                                                int showShutter, int showAnalogGain, int showLens, int showCaf, int showMotion)
        {
            this.Hdr = hdr;
            this.enable = enable;
            this.text = text;
            this.showShutter = showShutter;
            this.showAnalogGain = showAnalogGain;
            this.showLens = showLens;
            this.showCaf = showCaf;
            this.showMotion = showMotion;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_ANNOTATE_V2_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private int enable;
        private string text;
        private int showShutter, showAnalogGain, showLens, showCaf, showMotion;

        public int Enable => enable;
        public string Text => text;
        public int ShowShutter => showShutter;
        public int ShowAnalogGain => showAnalogGain;
        public int ShowLens => showLens;
        public int ShowCaf => showCaf;
        public int ShowMotion => showMotion;

        public MMAL_PARAMETER_CAMERA_ANNOTATE_V2_T(MMAL_PARAMETER_HEADER_T hdr, int enable, string text,
                                                int showShutter, int showAnalogGain, int showLens, int showCaf, int showMotion)
        {
            this.Hdr = hdr;
            this.enable = enable;
            this.text = text;
            this.showShutter = showShutter;
            this.showAnalogGain = showAnalogGain;
            this.showLens = showLens;
            this.showCaf = showCaf;
            this.showMotion = showMotion;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private int _enable;
        private int _showShutter, _showAnalogGain, _showLens, _showCaf, _showMotion, _showFrameNum,
            _enableTextBackground, _customBackgroundColor;
        private byte _customBackgroundY, _customBackgroundU, _customBackgroundV, _dummy1;
        private int _customTextColor;
        private byte _customTextY, _customTextU, _customTextV, _textSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MMALParametersCamera.MMAL_CAMERA_ANNOTATE_MAX_TEXT_LEN_V3)]
        private byte[] _text;

        public int Enable => _enable;
        public int ShowShutter => _showShutter;
        public int ShowAnalogGain => _showAnalogGain;
        public int ShowLens => _showLens;
        public int ShowCaf => _showCaf;
        public int ShowMotion => _showMotion;
        public int ShowFrameNum => _showFrameNum;
        public int EnableTextBackground => _enableTextBackground;
        public int CustomBackgroundColor => _customBackgroundColor;
        public byte CustomBackgroundY => _customBackgroundY;
        public byte CustomBackgroundU => _customBackgroundU;
        public byte CustomBackgroundV => _customBackgroundV;
        public byte Dummy1 => _dummy1;
        public int CustomTextColor => _customTextColor;
        public byte CustomTextY => _customTextY;
        public byte CustomTextU => _customTextU;
        public byte CustomTextV => _customTextV;
        public byte TextSize => _textSize;
        public byte[] Text => _text;

        public MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(MMAL_PARAMETER_HEADER_T hdr, int enable, int showShutter, int showAnalogGain, int showLens,
                                                   int showCaf, int showMotion, int showFrameNum, int enableTextBackground, int customBackgroundColor,
                                                   byte customBackgroundY, byte customBackgroundU, byte customBackgroundV, byte dummy1,
                                                   int customTextColor, byte customTextY, byte customTextU, byte customTextV, byte textSize,
                                                   byte[] text)
        {
            this.Hdr = hdr;

            _enable = enable;
            _text = text;
            _showShutter = showShutter;
            _showAnalogGain = showAnalogGain;
            _showLens = showLens;
            _showCaf = showCaf;
            _showMotion = showMotion;
            _showFrameNum = showFrameNum;
            _enableTextBackground = enableTextBackground;
            _customBackgroundColor = customBackgroundColor;
            _customBackgroundY = customBackgroundY;
            _customBackgroundU = customBackgroundU;
            _customBackgroundV = customBackgroundV;
            _dummy1 = dummy1;
            _customTextColor = customTextColor;
            _customTextY = customTextY;
            _customTextU = customTextU;
            _customTextV = customTextV;
            _textSize = textSize;
            _text = text;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_ANNOTATE_V4_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private int _enable;
        private int _showShutter, _showAnalogGain, _showLens, _showCaf, _showMotion, _showFrameNum,
                    _enableTextBackground, _customBackgroundColor;
        private byte _customBackgroundY, _customBackgroundU, _customBackgroundV, _dummy1;
        private int _customTextColor;
        private byte _customTextY, _customTextU, _customTextV, _textSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MMALParametersCamera.MMAL_CAMERA_ANNOTATE_MAX_TEXT_LEN_V3)]
        private byte[] _text;

        private int _justify, _xOffset, _yOffset;

        public int Enable => _enable;
        public int ShowShutter => _showShutter;
        public int ShowAnalogGain => _showAnalogGain;
        public int ShowLens => _showLens;
        public int ShowCaf => _showCaf;
        public int ShowMotion => _showMotion;
        public int ShowFrameNum => _showFrameNum;
        public int EnableTextBackground => _enableTextBackground;
        public int CustomBackgroundColor => _customBackgroundColor;
        public byte CustomBackgroundY => _customBackgroundY;
        public byte CustomBackgroundU => _customBackgroundU;
        public byte CustomBackgroundV => _customBackgroundV;
        public byte Dummy1 => _dummy1;
        public int CustomTextColor => _customTextColor;
        public byte CustomTextY => _customTextY;
        public byte CustomTextU => _customTextU;
        public byte CustomTextV => _customTextV;
        public byte TextSize => _textSize;
        public byte[] Text => _text;
        public int Justify => _justify;
        public int XOffset => _xOffset;
        public int YOffset => _yOffset;

        public MMAL_PARAMETER_CAMERA_ANNOTATE_V4_T(MMAL_PARAMETER_HEADER_T hdr, int enable, int showShutter, int showAnalogGain, int showLens,
                                                   int showCaf, int showMotion, int showFrameNum, int enableTextBackground, int customBackgroundColor,
                                                   byte customBackgroundY, byte customBackgroundU, byte customBackgroundV, byte dummy1,
                                                   int customTextColor, byte customTextY, byte customTextU, byte customTextV, byte textSize,
                                                   byte[] text, int justify, int xOffset, int yOffset)
        {
            this.Hdr = hdr;

            _enable = enable;
            _text = text;
            _showShutter = showShutter;
            _showAnalogGain = showAnalogGain;
            _showLens = showLens;
            _showCaf = showCaf;
            _showMotion = showMotion;
            _showFrameNum = showFrameNum;
            _enableTextBackground = enableTextBackground;
            _customBackgroundColor = customBackgroundColor;
            _customBackgroundY = customBackgroundY;
            _customBackgroundU = customBackgroundU;
            _customBackgroundV = customBackgroundV;
            _dummy1 = dummy1;
            _customTextColor = customTextColor;
            _customTextY = customTextY;
            _customTextU = customTextU;
            _customTextV = customTextV;
            _textSize = textSize;
            _text = text;
            _justify = justify;
            _xOffset = xOffset;
            _yOffset = yOffset;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_STEREOSCOPIC_MODE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_STEREOSCOPIC_MODE_T mode;
        private int decimate, swapEyes;

        public MMAL_STEREOSCOPIC_MODE_T Mode => mode;
        public int Decimate => decimate;
        public int SwapEyes => swapEyes;

        public MMAL_PARAMETER_STEREOSCOPIC_MODE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_STEREOSCOPIC_MODE_T mode,
                                                  int decimate, int swapEyes)
        {
            this.Hdr = hdr;
            this.mode = mode;
            this.decimate = decimate;
            this.swapEyes = swapEyes;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_INTERFACE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_CAMERA_INTERFACE_T mode;

        public MMAL_CAMERA_INTERFACE_T Mode => mode;

        public MMAL_PARAMETER_CAMERA_INTERFACE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CAMERA_INTERFACE_T mode)
        {
            this.Hdr = hdr;
            this.mode = mode;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_CLOCKING_MODE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_CAMERA_CLOCKING_MODE_T mode;

        public MMAL_CAMERA_CLOCKING_MODE_T Mode => mode;

        public MMAL_PARAMETER_CAMERA_CLOCKING_MODE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CAMERA_CLOCKING_MODE_T mode)
        {
            this.Hdr = hdr;
            this.mode = mode;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_RX_CONFIG_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_CAMERA_RX_CONFIG_DECODE decode;
        private MMAL_CAMERA_RX_CONFIG_ENCODE encode;
        private MMAL_CAMERA_RX_CONFIG_UNPACK unpack;
        private MMAL_CAMERA_RX_CONFIG_PACK pack;
        private uint dataLanes, encodeBlockLength, embeddedDataLines, imageId;

        public MMAL_CAMERA_RX_CONFIG_DECODE Decode => decode;
        public MMAL_CAMERA_RX_CONFIG_ENCODE Encode => encode;
        public MMAL_CAMERA_RX_CONFIG_UNPACK Unpack => unpack;
        public MMAL_CAMERA_RX_CONFIG_PACK Pack => pack;
        public uint DataLanes => dataLanes;
        public uint EncodeBlockLength => encodeBlockLength;
        public uint EmbeddedDataLanes => embeddedDataLines;
        public uint ImageId => imageId;

        public MMAL_PARAMETER_CAMERA_RX_CONFIG_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CAMERA_RX_CONFIG_DECODE decode,
                                                 MMAL_CAMERA_RX_CONFIG_ENCODE encode, MMAL_CAMERA_RX_CONFIG_UNPACK unpack,
                                                 MMAL_CAMERA_RX_CONFIG_PACK pack,
                                                 uint dataLanes, uint encodeBlockLength, uint embeddedDataLines, uint imageId)
        {
            this.Hdr = hdr;
            this.decode = decode;
            this.encode = encode;
            this.unpack = unpack;
            this.pack = pack;
            this.dataLanes = dataLanes;
            this.encodeBlockLength = encodeBlockLength;
            this.embeddedDataLines = embeddedDataLines;
            this.imageId = imageId;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_RX_TIMING_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private uint timing1, timing2, timing3, timing4, timing5, term1, term2, cpiTiming1, cpiTiming2;

        public uint Timing1 => timing1;
        public uint Timing2 => timing2;
        public uint Timing3 => timing3;
        public uint Timing4 => timing4;
        public uint Timing5 => timing5;
        public uint Term1 => term1;
        public uint Term2 => term2;
        public uint CpiTiming1 => cpiTiming1;
        public uint CpiTiming2 => cpiTiming2;

        public MMAL_PARAMETER_CAMERA_RX_TIMING_T(MMAL_PARAMETER_HEADER_T hdr, uint timing1, uint timing2, uint timing3, uint timing4,
                                                 uint timing5, uint term1, uint term2, uint cpiTiming1, uint cpiTiming2)
        {
            this.Hdr = hdr;
            this.timing1 = timing1;
            this.timing2 = timing2;
            this.timing3 = timing3;
            this.timing4 = timing4;
            this.timing5 = timing5;
            this.term1 = term1;
            this.term2 = term2;
            this.cpiTiming1 = cpiTiming1;
            this.cpiTiming2 = cpiTiming2;
        }
    }

    // mmal_parameters_video.h
    public static class MMALParametersVideo
    {
        public const int MMAL_PARAMETER_DISPLAYREGION = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO;
        public const int MMAL_PARAMETER_SUPPORTED_PROFILES = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 1;
        public const int MMAL_PARAMETER_PROFILE = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 2;
        public const int MMAL_PARAMETER_INTRAPERIOD = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 3;
        public const int MMAL_PARAMETER_RATECONTROL = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 4;
        public const int MMAL_PARAMETER_NALUNITFORMAT = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 5;
        public const int MMAL_PARAMETER_MINIMISE_FRAGMENTATION = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 6;
        public const int MMAL_PARAMETER_MB_ROWS_PER_SLICE = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 7;
        public const int MMAL_PARAMETER_VIDEO_LEVEL_EXTENSION = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 8;
        public const int MMAL_PARAMETER_VIDEO_EEDE_ENABLE = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 9;
        public const int MMAL_PARAMETER_VIDEO_EEDE_LOSSRATE = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 10;
        public const int MMAL_PARAMETER_VIDEO_REQUEST_I_FRAME = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 11;
        public const int MMAL_PARAMETER_VIDEO_INTRA_REFRESH = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 12;
        public const int MMAL_PARAMETER_VIDEO_IMMUTABLE_INPUT = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 13;
        public const int MMAL_PARAMETER_VIDEO_BIT_RATE = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 14;
        public const int MMAL_PARAMETER_VIDEO_FRAME_RATE = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 15;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_MIN_QUANT = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 16;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_MAX_QUANT = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 17;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_RC_MODEL = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 18;
        public const int MMAL_PARAMETER_EXTRA_BUFFERS = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 19;
        public const int MMAL_PARAMETER_VIDEO_ALIGN_HORIZ = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 20;
        public const int MMAL_PARAMETER_VIDEO_ALIGN_VERT = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 21;
        public const int MMAL_PARAMETER_VIDEO_DROPPABLE_PFRAMES = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 22;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_INITIAL_QUANT = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 23;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_QP_P = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 24;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_RC_SLICE_DQUANT = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 25;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_FRAME_LIMIT_BITS = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 26;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_PEAK_RATE = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 27;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_H264_DISABLE_CABAC = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 28;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_H264_LOW_LATENCY = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 29;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_H264_AU_DELIMITERS = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 30;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_H264_DEBLOCK_IDC = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 31;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_H264_MB_INTRA_MODE = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 32;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_HEADER_ON_OPEN = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 33;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_PRECODE_FOR_QP = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 34;
        public const int MMAL_PARAMETER_VIDEO_DRM_INIT_INFO = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 35;
        public const int MMAL_PARAMETER_VIDEO_TIMESTAMP_FIFO = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 36;
        public const int MMAL_PARAMETER_VIDEO_DECODE_ERROR_CONCEALMENT = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 37;
        public const int MMAL_PARAMETER_VIDEO_DRM_PROTECT_BUFFER = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 38;
        public const int MMAL_PARAMETER_VIDEO_DECODE_CONFIG_VD3 = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 39;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_H264_VCL_HRD_PARAMETERS = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 40;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_H264_LOW_DELAY_HRD_FLAG = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 41;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_INLINE_HEADER = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 42;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_SEI_ENABLE = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 43;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_INLINE_VECTORS = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 44;
        public const int MMAL_PARAMETER_VIDEO_RENDER_STATS = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 45;
        public const int MMAL_PARAMETER_VIDEO_INTERLACE_TYPE = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 46;
        public const int MMAL_PARAMETER_VIDEO_INTERPOLATE_TIMESTAMPS = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 47;
        public const int MMAL_PARAMETER_VIDEO_ENCODE_SPS_TIMINGS = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 48;
        public const int MMAL_PARAMETER_VIDEO_MAX_NUM_CALLBACKS = MMALParametersCommon.MMAL_PARAMETER_GROUP_VIDEO + 49;

        public enum MMAL_DISPLAYTRANSFORM_T
        {
            MMAL_DISPLAY_ROT0,
            MMAL_DISPLAY_MIRROR_ROT0,
            MMAL_DISPLAY_MIRROR_ROT180,
            MMAL_DISPLAY_ROT180,
            MMAL_DISPLAY_MIRROR_ROT90,
            MMAL_DISPLAY_ROT270,
            MMAL_DISPLAY_ROT90,
            MMAL_DISPLAY_MIRROR_ROT270,
            MMAL_DISPLAY_DUMMY = 0x7FFFFFFF
        }

        public enum MMAL_DISPLAYMODE_T
        {
            /// <summary>
            /// Fill the screen.
            /// </summary>
            MMAL_DISPLAY_MODE_FILL,
            
            /// <summary>
            /// All the source region should be displayed and black bars added if necessary.
            /// </summary>
            MMAL_DISPLAY_MODE_LETTERBOX,
            MMAL_DISPLAY_MODE_DUMMY = 0x7FFFFFFF
        }

        public enum MMAL_DISPLAYSET_T
        {
            MMAL_DISPLAY_SET_NONE = 0,
            MMAL_DISPLAY_SET_NUM = 1,
            MMAL_DISPLAY_SET_FULLSCREEN = 2,
            MMAL_DISPLAY_SET_TRANSFORM = 4,
            MMAL_DISPLAY_SET_DEST_RECT = 8,
            MMAL_DISPLAY_SET_SRC_RECT = 0x10,
            MMAL_DISPLAY_SET_MODE = 0x20,
            MMAL_DISPLAY_SET_PIXEL = 0x40,
            MMAL_DISPLAY_SET_NOASPECT = 0x80,
            MMAL_DISPLAY_SET_LAYER = 0x100,
            MMAL_DISPLAY_SET_COPYPROTECT = 0x200,
            MMAL_DISPLAY_SET_ALPHA = 0x400,
            MMAL_DISPLAY_SET_DUMMY = 0x7FFFFFFF
        }

        public enum MMAL_VIDEO_PROFILE_T
        {
            MMAL_VIDEO_PROFILE_H263_BASELINE,
            MMAL_VIDEO_PROFILE_H263_H320CODING,
            MMAL_VIDEO_PROFILE_H263_BACKWARDCOMPATIBLE,
            MMAL_VIDEO_PROFILE_H263_ISWV2,
            MMAL_VIDEO_PROFILE_H263_ISWV3,
            MMAL_VIDEO_PROFILE_H263_HIGHCOMPRESSION,
            MMAL_VIDEO_PROFILE_H263_INTERNET,
            MMAL_VIDEO_PROFILE_H263_INTERLACE,
            MMAL_VIDEO_PROFILE_H263_HIGHLATENCY,
            MMAL_VIDEO_PROFILE_MP4V_SIMPLE,
            MMAL_VIDEO_PROFILE_MP4V_SIMPLESCALABLE,
            MMAL_VIDEO_PROFILE_MP4V_CORE,
            MMAL_VIDEO_PROFILE_MP4V_MAIN,
            MMAL_VIDEO_PROFILE_MP4V_NBIT,
            MMAL_VIDEO_PROFILE_MP4V_SCALABLETEXTURE,
            MMAL_VIDEO_PROFILE_MP4V_SIMPLEFACE,
            MMAL_VIDEO_PROFILE_MP4V_SIMPLEFBA,
            MMAL_VIDEO_PROFILE_MP4V_BASICANIMATED,
            MMAL_VIDEO_PROFILE_MP4V_HYBRID,
            MMAL_VIDEO_PROFILE_MP4V_ADVANCEDREALTIME,
            MMAL_VIDEO_PROFILE_MP4V_CORESCALABLE,
            MMAL_VIDEO_PROFILE_MP4V_ADVANCEDCODING,
            MMAL_VIDEO_PROFILE_MP4V_ADVANCEDCORE,
            MMAL_VIDEO_PROFILE_MP4V_ADVANCEDSCALABLE,
            MMAL_VIDEO_PROFILE_MP4V_ADVANCEDSIMPLE,
            MMAL_VIDEO_PROFILE_H264_BASELINE,
            MMAL_VIDEO_PROFILE_H264_MAIN,
            MMAL_VIDEO_PROFILE_H264_EXTENDED,
            MMAL_VIDEO_PROFILE_H264_HIGH,
            MMAL_VIDEO_PROFILE_H264_HIGH10,
            MMAL_VIDEO_PROFILE_H264_HIGH422,
            MMAL_VIDEO_PROFILE_H264_HIGH444,
            MMAL_VIDEO_PROFILE_H264_CONSTRAINED_BASELINE,
            MMAL_VIDEO_PROFILE_DUMMY = 0x7FFFFFFF
        }

        public enum MMAL_VIDEO_LEVEL_T
        {
            MMAL_VIDEO_LEVEL_H263_10,
            MMAL_VIDEO_LEVEL_H263_20,
            MMAL_VIDEO_LEVEL_H263_30,
            MMAL_VIDEO_LEVEL_H263_40,
            MMAL_VIDEO_LEVEL_H263_45,
            MMAL_VIDEO_LEVEL_H263_50,
            MMAL_VIDEO_LEVEL_H263_60,
            MMAL_VIDEO_LEVEL_H263_70,
            MMAL_VIDEO_LEVEL_MP4V_0,
            MMAL_VIDEO_LEVEL_MP4V_0b,
            MMAL_VIDEO_LEVEL_MP4V_1,
            MMAL_VIDEO_LEVEL_MP4V_2,
            MMAL_VIDEO_LEVEL_MP4V_3,
            MMAL_VIDEO_LEVEL_MP4V_4,
            MMAL_VIDEO_LEVEL_MP4V_4a,
            MMAL_VIDEO_LEVEL_MP4V_5,
            MMAL_VIDEO_LEVEL_MP4V_6,
            MMAL_VIDEO_LEVEL_H264_1,
            MMAL_VIDEO_LEVEL_H264_1b,
            MMAL_VIDEO_LEVEL_H264_11,
            MMAL_VIDEO_LEVEL_H264_12,
            MMAL_VIDEO_LEVEL_H264_13,
            MMAL_VIDEO_LEVEL_H264_2,
            MMAL_VIDEO_LEVEL_H264_21,
            MMAL_VIDEO_LEVEL_H264_22,
            MMAL_VIDEO_LEVEL_H264_3,
            MMAL_VIDEO_LEVEL_H264_31,
            MMAL_VIDEO_LEVEL_H264_32,
            MMAL_VIDEO_LEVEL_H264_4,
            MMAL_VIDEO_LEVEL_H264_41,
            MMAL_VIDEO_LEVEL_H264_42,
            MMAL_VIDEO_LEVEL_H264_5,
            MMAL_VIDEO_LEVEL_H264_51,
            MMAL_VIDEO_LEVEL_DUMMY = 0x7FFFFFFF
        }

        public enum MMAL_VIDEO_RATECONTROL_T
        {
            MMAL_VIDEO_RATECONTROL_DEFAULT,
            MMAL_VIDEO_RATECONTROL_VARIABLE,
            MMAL_VIDEO_RATECONTROL_CONSTANT,
            MMAL_VIDEO_RATECONTROL_VARIABLE_SKIP_FRAMES,
            MMAL_VIDEO_RATECONTROL_CONSTANT_SKIP_FRAMES,
            MMAL_VIDEO_RATECONTROL_DUMMY = 0x7FFFFFFF
        }

        public enum MMAL_VIDEO_INTRA_REFRESH_T
        {
            MMAL_VIDEO_INTRA_REFRESH_DISABLED = -1,
            MMAL_VIDEO_INTRA_REFRESH_CYCLIC,
            MMAL_VIDEO_INTRA_REFRESH_ADAPTIVE,
            MMAL_VIDEO_INTRA_REFRESH_BOTH,
            MMAL_VIDEO_INTRA_REFRESH_KHRONOSEXTENSIONS = 0x6F000000,
            MMAL_VIDEO_INTRA_REFRESH_VENDORSTARTUNUSED = 0x7F000000,
            MMAL_VIDEO_INTRA_REFRESH_CYCLIC_MROWS = MMAL_VIDEO_INTRA_REFRESH_VENDORSTARTUNUSED,
            MMAL_VIDEO_INTRA_REFRESH_PSEUDO_RAND = MMAL_VIDEO_INTRA_REFRESH_VENDORSTARTUNUSED + 1,
            MMAL_VIDEO_INTRA_REFRESH_MAX = MMAL_VIDEO_INTRA_REFRESH_VENDORSTARTUNUSED + 2,
            MMAL_VIDEO_INTRA_REFRESH_DUMMY = 0x7FFFFFFF
        }

        public enum MMAL_VIDEO_ENCODE_T
        {
            MMAL_VIDEO_ENCODER_RC_MODEL_T = 0,
            MMAL_VIDEO_ENCODER_RC_MODEL_DEFAULT = 0,
            MMAL_VIDEO_ENCODER_RC_MODEL_JVT = MMAL_VIDEO_ENCODER_RC_MODEL_DEFAULT,
            MMAL_VIDEO_ENCODER_RC_MODEL_VOWIFI = MMAL_VIDEO_ENCODER_RC_MODEL_DEFAULT + 1,
            MMAL_VIDEO_ENCODER_RC_MODEL_CBR = MMAL_VIDEO_ENCODER_RC_MODEL_DEFAULT + 2,
            MMAL_VIDEO_ENCODER_RC_MODEL_LAST = MMAL_VIDEO_ENCODER_RC_MODEL_DEFAULT + 3,
            MMAL_VIDEO_ENCODER_RC_MODEL_DUMMY = 0x7FFFFFFF
        }

        public enum MMAL_VIDEO_ENCODE_H264_MB_INTRA_MODES_T
        {
            MMAL_VIDEO_ENCODER_H264_MB_4x4_INTRA,
            MMAL_VIDEO_ENCODER_H264_MB_8x8_INTRA,
            MMAL_VIDEO_ENCODER_H264_MB_16x16_INTRA,
            MMAL_VIDEO_ENCODER_H264_MB_INTRA_DUMMY = 0x7FFFFFFF
        }

        public enum MMAL_VIDEO_NALUNITFORMAT_T
        {
            MMAL_VIDEO_NALUNITFORMAT_STARTCODES,
            MMAL_VIDEO_NALUNITFORMAT_NALUNITPERBUFFER,
            MMAL_VIDEO_NALUNITFORMAT_ONEBYTEINTERLEAVELENGTH,
            MMAL_VIDEO_NALUNITFORMAT_TWOBYTEINTERLEAVELENGTH,
            MMAL_VIDEO_NALUNITFORMAT_FOURBYTEINTERLEAVELENGTH,
            MMAL_VIDEO_NALUNITFORMAT_DUMMY = 0x7FFFFFFF
        }

        public enum MMAL_INTERLACE_TYPE_T
        {
            MMAL_InterlaceProgressive,
            MMAL_InterlaceFieldSingleUpperFirst,
            MMAL_InterlaceFieldSingleLowerFirst,
            MMAL_InterlaceFieldsInterleavedUpperFirst,
            MMAL_InterlaceFieldsInterleavedLowerFirst,
            MMAL_InterlaceMixed,
            MMAL_InterlaceKhronosExtensions = 0x6F000000,
            MMAL_InterlaceVendorStartUnused = 0x7F000000,
            MMAL_InterlaceMax = 0x7FFFFFFF
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_DISPLAYREGION_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private uint set, displayNum;
        private int fullscreen;
        private MMALParametersVideo.MMAL_DISPLAYTRANSFORM_T transform;
        private MMAL_RECT_T destRect, srcRect;
        private int noAspect;
        private MMALParametersVideo.MMAL_DISPLAYMODE_T mode;
        private int pixelX, pixelY;
        private int layer, copyrightRequired;
        private int alpha;

        public uint Set => set;
        public uint DisplayNum => displayNum;
        public int Fullscreen => fullscreen;
        public MMALParametersVideo.MMAL_DISPLAYTRANSFORM_T Transform => transform;
        public MMAL_RECT_T DestRect => destRect;
        public MMAL_RECT_T SrcRect => srcRect;
        public int NoAspect => noAspect;
        public MMALParametersVideo.MMAL_DISPLAYMODE_T Mode => mode;
        public int PixelX => pixelX;
        public int PixelY => pixelY;
        public int Layer => layer;
        public int CopyrightRequired => copyrightRequired;
        public int Alpha => alpha;

        public MMAL_DISPLAYREGION_T(MMAL_PARAMETER_HEADER_T hdr, uint set, uint displayNum, int fullscreen,
                                    MMALParametersVideo.MMAL_DISPLAYTRANSFORM_T transform, MMAL_RECT_T destRect, MMAL_RECT_T srcRect,
                                    int noAspect, MMALParametersVideo.MMAL_DISPLAYMODE_T mode, int pixelX, int pixelY,
                                    int layer, int copyrightRequired, int alpha)
        {
            this.Hdr = hdr;
            this.set = set;
            this.displayNum = displayNum;
            this.fullscreen = fullscreen;
            this.transform = transform;
            this.destRect = destRect;
            this.srcRect = srcRect;
            this.noAspect = noAspect;
            this.mode = mode;
            this.pixelX = pixelX;
            this.pixelY = pixelY;
            this.layer = layer;
            this.copyrightRequired = copyrightRequired;
            this.alpha = alpha;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_PROFILE_S
    {
        private MMALParametersVideo.MMAL_VIDEO_PROFILE_T profile;
        private MMALParametersVideo.MMAL_VIDEO_LEVEL_T level;

        public MMALParametersVideo.MMAL_VIDEO_PROFILE_T Profile => profile;
        public MMALParametersVideo.MMAL_VIDEO_LEVEL_T Level => level;

        public MMAL_PARAMETER_VIDEO_PROFILE_S(MMALParametersVideo.MMAL_VIDEO_PROFILE_T profile, MMALParametersVideo.MMAL_VIDEO_LEVEL_T level)
        {
            this.profile = profile;
            this.level = level;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_PROFILE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        private MMAL_PARAMETER_VIDEO_PROFILE_S[] profile;

        public MMAL_PARAMETER_VIDEO_PROFILE_S[] Profile => profile;

        public MMAL_PARAMETER_VIDEO_PROFILE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAMETER_VIDEO_PROFILE_S[] profile)
        {
            this.Hdr = hdr;
            this.profile = profile;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_ENCODE_RC_MODEL_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private uint rcModel;

        public uint RcModel => rcModel;

        public MMAL_PARAMETER_VIDEO_ENCODE_RC_MODEL_T(MMAL_PARAMETER_HEADER_T hdr, uint rcModel)
        {
            this.Hdr = hdr;
            this.rcModel = rcModel;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_RATECONTROL_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMALParametersVideo.MMAL_VIDEO_RATECONTROL_T control;

        public unsafe MMAL_PARAMETER_HEADER_T* HdrPtr
        {
            get
            {
                fixed (MMAL_PARAMETER_HEADER_T* ptr = &Hdr)
                {
                    return ptr;
                }
            }
        }

        public MMALParametersVideo.MMAL_VIDEO_RATECONTROL_T Control => control;

        public MMAL_PARAMETER_VIDEO_RATECONTROL_T(MMAL_PARAMETER_HEADER_T hdr, MMALParametersVideo.MMAL_VIDEO_RATECONTROL_T control)
        {
            this.Hdr = hdr;
            this.control = control;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_ENCODER_H264_MB_INTRA_MODES_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMALParametersVideo.MMAL_VIDEO_ENCODE_H264_MB_INTRA_MODES_T mbMode;

        public MMALParametersVideo.MMAL_VIDEO_ENCODE_H264_MB_INTRA_MODES_T MbMode => mbMode;

        public MMAL_PARAMETER_VIDEO_ENCODER_H264_MB_INTRA_MODES_T(MMAL_PARAMETER_HEADER_T hdr, MMALParametersVideo.MMAL_VIDEO_ENCODE_H264_MB_INTRA_MODES_T mbMode)
        {
            this.Hdr = hdr;
            this.mbMode = mbMode;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_NALUNITFORMAT_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMALParametersVideo.MMAL_VIDEO_NALUNITFORMAT_T format;

        public MMALParametersVideo.MMAL_VIDEO_NALUNITFORMAT_T Format => format;

        public MMAL_PARAMETER_VIDEO_NALUNITFORMAT_T(MMAL_PARAMETER_HEADER_T hdr, MMALParametersVideo.MMAL_VIDEO_NALUNITFORMAT_T format)
        {
            this.Hdr = hdr;
            this.format = format;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_LEVEL_EXTENSION_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private uint customMaxMbps, customMaxFs, customMaxBrAndCpb;

        public uint CustomMaxMbps => customMaxMbps;
        public uint CustomMaxFs => customMaxFs;
        public uint CustomMaxBrAndCpb => customMaxBrAndCpb;

        public MMAL_PARAMETER_VIDEO_LEVEL_EXTENSION_T(MMAL_PARAMETER_HEADER_T hdr, uint customMaxMbps, uint customMaxFs, uint customMaxBrAndCpb)
        {
            this.Hdr = hdr;
            this.customMaxMbps = customMaxMbps;
            this.customMaxFs = customMaxFs;
            this.customMaxBrAndCpb = customMaxBrAndCpb;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_INTRA_REFRESH_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMALParametersVideo.MMAL_VIDEO_INTRA_REFRESH_T refreshMode;
        private int airMbs, airRef, cirMbs, pirMbs;

        public unsafe MMAL_PARAMETER_HEADER_T* HdrPtr
        {
            get
            {
                fixed (MMAL_PARAMETER_HEADER_T* ptr = &Hdr)
                {
                    return ptr;
                }
            }
        }

        public MMALParametersVideo.MMAL_VIDEO_INTRA_REFRESH_T RefreshMode => refreshMode;
        public int AirMbs => airMbs;
        public int AirRef => airRef;
        public int CirMbs => cirMbs;
        public int PirMbs => pirMbs;

        public MMAL_PARAMETER_VIDEO_INTRA_REFRESH_T(MMAL_PARAMETER_HEADER_T hdr, MMALParametersVideo.MMAL_VIDEO_INTRA_REFRESH_T refreshMode,
                                                    int airMbs, int airRef, int cirMbs, int pirMbs)
        {
            this.Hdr = hdr;
            this.refreshMode = refreshMode;
            this.airMbs = airMbs;
            this.airRef = airRef;
            this.cirMbs = cirMbs;
            this.pirMbs = pirMbs;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_EEDE_ENABLE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private int enable;

        public int Enable => enable;

        public MMAL_PARAMETER_VIDEO_EEDE_ENABLE_T(MMAL_PARAMETER_HEADER_T hdr, int enable)
        {
            this.Hdr = hdr;
            this.enable = enable;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_EEDE_LOSSRATE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private uint lossRate;

        public uint LossRate => lossRate;

        public MMAL_PARAMETER_VIDEO_EEDE_LOSSRATE_T(MMAL_PARAMETER_HEADER_T hdr, uint lossRate)
        {
            this.Hdr = hdr;
            this.lossRate = lossRate;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_DRM_INIT_INFO_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private uint currentTime, ticksPerSec;
        private uint[] lhs;

        public uint CurrentTime => currentTime;
        public uint TicksPerSec => ticksPerSec;
        public uint[] Lhs => lhs;

        public MMAL_PARAMETER_VIDEO_DRM_INIT_INFO_T(MMAL_PARAMETER_HEADER_T hdr, uint currentTime, uint ticksPerSec, uint[] lhs)
        {
            this.Hdr = hdr;
            this.currentTime = currentTime;
            this.ticksPerSec = ticksPerSec;
            this.lhs = lhs;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_DRM_PROTECT_BUFFER_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private uint sizeWanted, protect, memHandle;
        private IntPtr physAddr;

        public uint SizeWanted => sizeWanted;
        public uint Protect => protect;
        public uint MemHandle => memHandle;
        public IntPtr PhysAddr => physAddr;

        public MMAL_PARAMETER_VIDEO_DRM_PROTECT_BUFFER_T(MMAL_PARAMETER_HEADER_T hdr, uint sizeWanted, uint protect, uint memHandle,
                                                         IntPtr physAddr)
        {
            this.Hdr = hdr;
            this.sizeWanted = sizeWanted;
            this.protect = protect;
            this.memHandle = memHandle;
            this.physAddr = physAddr;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_RENDER_STATS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private int valid;
        private uint match, period, phase, pixelClockNominal, hvsStatus;
        private uint[] dummy;

        public int Valid => valid;
        public uint Match => match;
        public uint Period => period;
        public uint Phase => phase;
        public uint PixelClockNominal => pixelClockNominal;
        public uint HvsStatus => hvsStatus;
        public uint[] Dummy => dummy;

        public MMAL_PARAMETER_VIDEO_RENDER_STATS_T(MMAL_PARAMETER_HEADER_T hdr, int valid, uint match, uint period,
                                                   uint phase, uint pixelClockNominal, uint hvsStatus, uint[] dummy)
        {
            this.Hdr = hdr;
            this.valid = valid;
            this.match = match;
            this.period = period;
            this.phase = phase;
            this.pixelClockNominal = pixelClockNominal;
            this.hvsStatus = hvsStatus;
            this.dummy = dummy;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_INTERLACE_TYPE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMALParametersVideo.MMAL_INTERLACE_TYPE_T eMode;
        private int bRepeatFirstField;

        public MMALParametersVideo.MMAL_INTERLACE_TYPE_T EMode => eMode;
        public int BRepeatFirstField => bRepeatFirstField;

        public MMAL_PARAMETER_VIDEO_INTERLACE_TYPE_T(MMAL_PARAMETER_HEADER_T hdr, MMALParametersVideo.MMAL_INTERLACE_TYPE_T eMode,
                                                     int bRepeatFirstField)
        {
            this.Hdr = hdr;
            this.eMode = eMode;
            this.bRepeatFirstField = bRepeatFirstField;
        }
    }

    // mmal_parameters_audio.h
    public static class MMALParametersAudio
    {
        public const int MMAL_PARAMETER_AUDIO_DESTINATION = MMALParametersCommon.MMAL_PARAMETER_GROUP_AUDIO;
        public const int MMAL_PARAMETER_AUDIO_LATENCY_TARGET = MMALParametersCommon.MMAL_PARAMETER_GROUP_AUDIO + 1;
        public const int MMAL_PARAMETER_AUDIO_SOURCE = MMALParametersCommon.MMAL_PARAMETER_GROUP_AUDIO + 2;
        public const int MMAL_PARAMETER_AUDIO_PASSTHROUGH = MMALParametersCommon.MMAL_PARAMETER_GROUP_AUDIO + 3;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_AUDIO_LATENCY_TARGET_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private int enable;
        private uint filter, target, shift;
        private int speedFactor, interFactor, adjCap;

        public int Enable => enable;
        public uint Filter => filter;
        public uint Target => target;
        public uint Shift => shift;
        public int SpeedFactor => speedFactor;
        public int InterFactor => interFactor;
        public int AdjCap => adjCap;

        public MMAL_PARAMETER_AUDIO_LATENCY_TARGET_T(MMAL_PARAMETER_HEADER_T hdr, int enable, uint filter, uint target, uint shift,
                                                     int speedFactor, int interFactor, int adjCap)
        {
            this.Hdr = hdr;
            this.enable = enable;
            this.filter = filter;
            this.target = target;
            this.shift = shift;
            this.speedFactor = speedFactor;
            this.interFactor = interFactor;
            this.adjCap = adjCap;
        }
    }

    // mmal_parameters_clock.h
    public static class MMALParametersClock
    {
        public const int MMAL_PARAMETER_CLOCK_REFERENCE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CLOCK;
        public const int MMAL_PARAMETER_CLOCK_ACTIVE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CLOCK + 1;
        public const int MMAL_PARAMETER_CLOCK_SCALE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CLOCK + 2;
        public const int MMAL_PARAMETER_CLOCK_TIME = MMALParametersCommon.MMAL_PARAMETER_GROUP_CLOCK + 3;
        public const int MMAL_PARAMETER_CLOCK_UPDATE_THRESHOLD = MMALParametersCommon.MMAL_PARAMETER_GROUP_CLOCK + 4;
        public const int MMAL_PARAMETER_CLOCK_DISCONT_THRESHOLD = MMALParametersCommon.MMAL_PARAMETER_GROUP_CLOCK + 5;
        public const int MMAL_PARAMETER_CLOCK_REQUEST_THRESHOLD = MMALParametersCommon.MMAL_PARAMETER_GROUP_CLOCK + 6;
        public const int MMAL_PARAMETER_CLOCK_ENABLE_BUFFER_INFO = MMALParametersCommon.MMAL_PARAMETER_GROUP_CLOCK + 7;
        public const int MMAL_PARAMETER_CLOCK_FRAME_RATE = MMALParametersCommon.MMAL_PARAMETER_GROUP_CLOCK + 8;
        public const int MMAL_PARAMETER_CLOCK_LATENCY = MMALParametersCommon.MMAL_PARAMETER_GROUP_CLOCK + 9;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CLOCK_UPDATE_THRESHOLD_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_CLOCK_UPDATE_THRESHOLD_T value;

        public MMAL_CLOCK_UPDATE_THRESHOLD_T Value => value;

        public MMAL_PARAMETER_CLOCK_UPDATE_THRESHOLD_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CLOCK_UPDATE_THRESHOLD_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CLOCK_DISCONT_THRESHOLD_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_CLOCK_DISCONT_THRESHOLD_T value;

        public MMAL_CLOCK_DISCONT_THRESHOLD_T Value => value;

        public MMAL_PARAMETER_CLOCK_DISCONT_THRESHOLD_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CLOCK_DISCONT_THRESHOLD_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CLOCK_REQUEST_THRESHOLD_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_CLOCK_REQUEST_THRESHOLD_T value;

        public MMAL_CLOCK_REQUEST_THRESHOLD_T Value => value;

        public MMAL_PARAMETER_CLOCK_REQUEST_THRESHOLD_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CLOCK_REQUEST_THRESHOLD_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CLOCK_LATENCY_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_CLOCK_LATENCY_T value;

        public MMAL_CLOCK_LATENCY_T Value => value;

        public MMAL_PARAMETER_CLOCK_LATENCY_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CLOCK_LATENCY_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    // mmal_parameters.h
    public enum MMAL_PARAM_MIRROR_T
    {
        MMAL_PARAM_MIRROR_NONE,
        MMAL_PARAM_MIRROR_VERTICAL,
        MMAL_PARAM_MIRROR_HORIZONTAL,
        MMAL_PARAM_MIRROR_BOTH
    }

    public static class MMALParameters
    {
        public const string MMAL_COMPONENT_DEFAULT_VIDEO_DECODER = "vc.ril.video_decode";
        public const string MMAL_COMPONENT_DEFAULT_VIDEO_ENCODER = "vc.ril.video_encode";
        public const string MMAL_COMPONENT_DEFAULT_VIDEO_RENDERER = "vc.ril.video_render";
        public const string MMAL_COMPONENT_DEFAULT_IMAGE_DECODER = "vc.ril.image_decode";
        public const string MMAL_COMPONENT_DEFAULT_IMAGE_ENCODER = "vc.ril.image_encode";
        public const string MMAL_COMPONENT_DEFAULT_CAMERA = "vc.ril.camera";
        public const string MMAL_COMPONENT_DEFAULT_VIDEO_CONVERTER = "vc.video_convert";
        public const string MMAL_COMPONENT_DEFAULT_SPLITTER = "vc.splitter";
        public const string MMAL_COMPONENT_DEFAULT_SCHEDULER = "vc.scheduler";
        public const string MMAL_COMPONENT_DEFAULT_VIDEO_INJECTER = "vc.video_inject";
        public const string MMAL_COMPONENT_DEFAULT_VIDEO_SPLITTER = "vc.ril.video_splitter";
        public const string MMAL_COMPONENT_DEFAULT_AUDIO_DECODER = "none";
        public const string MMAL_COMPONENT_DEFAULT_AUDIO_RENDERER = "vc.ril.audio_render";
        public const string MMAL_COMPONENT_DEFAULT_MIRACAST = "vc.miracast";
        public const string MMAL_COMPONENT_DEFAULT_CLOCK = "vc.clock";
        public const string MMAL_COMPONENT_DEFAULT_CAMERA_INFO = "vc.camera_info";
        
        // These components are not present in the userland headers but do exist.
        public const string MMAL_COMPONENT_DEFAULT_NULL_SINK = "vc.null_sink";
        public const string MMAL_COMPONENT_DEFAULT_RESIZER = "vc.ril.resize";
        public const string MMAL_COMPONENT_DEFAULT_IMAGE_FX = "vc.ril.image_fx";
        public const string MMAL_COMPONENT_ISP = "vc.ril.isp";
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_UINT64_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private ulong value;

        public ulong Value => value;

        public MMAL_PARAMETER_UINT64_T(MMAL_PARAMETER_HEADER_T hdr, ulong value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_INT64_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private long value;

        public long Value => value;

        public MMAL_PARAMETER_INT64_T(MMAL_PARAMETER_HEADER_T hdr, long value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_UINT32_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private uint value;

        public uint Value => value;

        public MMAL_PARAMETER_UINT32_T(MMAL_PARAMETER_HEADER_T hdr, uint value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_INT32_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private int value;

        public int Value => value;

        public MMAL_PARAMETER_INT32_T(MMAL_PARAMETER_HEADER_T hdr, int value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_RATIONAL_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_RATIONAL_T value;

        public MMAL_RATIONAL_T Value => value;

        public MMAL_PARAMETER_RATIONAL_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_RATIONAL_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_BOOLEAN_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private int value;

        public int Value => value;

        public MMAL_PARAMETER_BOOLEAN_T(MMAL_PARAMETER_HEADER_T hdr, int value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_STRING_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private string value;

        public string Value => value;

        public MMAL_PARAMETER_STRING_T(MMAL_PARAMETER_HEADER_T hdr, string value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_SCALEFACTOR_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private uint scaleX, scaleY;

        public uint ScaleX => scaleX;
        public uint ScaleY => scaleY;

        public MMAL_PARAMETER_SCALEFACTOR_T(MMAL_PARAMETER_HEADER_T hdr, uint scaleX, uint scaleY)
        {
            this.Hdr = hdr;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_MIRROR_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_PARAM_MIRROR_T value;

        public MMAL_PARAM_MIRROR_T Value => value;

        public MMAL_PARAMETER_MIRROR_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_MIRROR_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_URI_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private string value;

        public string Value => value;

        public MMAL_PARAMETER_URI_T(MMAL_PARAMETER_HEADER_T hdr, string value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_ENCODING_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        private int[] value;

        public int[] Value => value;

        public MMAL_PARAMETER_ENCODING_T(MMAL_PARAMETER_HEADER_T hdr, int[] value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FRAME_RATE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private MMAL_RATIONAL_T value;

        public MMAL_RATIONAL_T Value => value;

        public MMAL_PARAMETER_FRAME_RATE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_RATIONAL_T value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CONFIGFILE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private uint value;

        public uint Value => value;

        public MMAL_PARAMETER_CONFIGFILE_T(MMAL_PARAMETER_HEADER_T hdr, uint value)
        {
            this.Hdr = hdr;
            this.value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CONFIGFILE_CHUNK_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        private uint size, offset;
        private string data;

        public uint Size => size;
        public uint Offset => offset;
        public string Data => data;

        public MMAL_PARAMETER_CONFIGFILE_CHUNK_T(MMAL_PARAMETER_HEADER_T hdr, uint size, uint offset, string data)
        {
            this.Hdr = hdr;
            this.size = size;
            this.offset = offset;
            this.data = data;
        }
    }
}