// <copyright file="MMALParameters.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace MMALSharp.Native
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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

        public byte[] Data { get; }

        public MMAL_PARAMETER_BYTES_T(MMAL_PARAMETER_HEADER_T hdr, byte[] data)
        {
            this.Hdr = hdr;
            this.Data = data;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CHANGE_EVENT_REQUEST_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public int ChangeId { get; }

        public int Enable { get; }

        public MMAL_PARAMETER_CHANGE_EVENT_REQUEST_T(MMAL_PARAMETER_HEADER_T hdr, int changeId, int enable)
        {
            this.Hdr = hdr;
            this.ChangeId = changeId;
            this.Enable = enable;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_BUFFER_REQUIREMENTS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public int BufferNumMin { get; }

        public int BufferSizeMin { get; }

        public int BufferAlignmentMin { get; }

        public int BufferNumRecommended { get; }

        public int BufferSizeRecommended { get; }

        public MMAL_PARAMETER_BUFFER_REQUIREMENTS_T(MMAL_PARAMETER_HEADER_T hdr, int bufferNumMin, int bufferSizeMin, int bufferAlignmentMin, int bufferNumRecommended, int bufferSizeRecommended)
        {
            this.Hdr = hdr;
            this.BufferNumMin = bufferNumMin;
            this.BufferSizeMin = bufferSizeMin;
            this.BufferAlignmentMin = bufferAlignmentMin;
            this.BufferNumRecommended = bufferNumRecommended;
            this.BufferSizeRecommended = bufferSizeRecommended;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_SEEK_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public long Offset { get; }

        public uint Flags { get; }

        public MMAL_PARAMETER_SEEK_T(MMAL_PARAMETER_HEADER_T hdr, long offset, uint flags)
        {
            this.Hdr = hdr;
            this.Offset = offset;
            this.Flags = flags;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_STATISTICS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public uint BufferCount { get; }

        public uint FrameCount { get; }

        public uint FramesSkipped { get; }

        public uint FramesDiscarded { get; }

        public uint EosSeen { get; }

        public uint MaximumFrameBytes { get; }

        public uint TotalBytes { get; }

        public uint CorruptMacroBlocks { get; }

        public MMAL_PARAMETER_STATISTICS_T(MMAL_PARAMETER_HEADER_T hdr, uint bufferCount, uint frameCount, uint framesSkipped, 
                                           uint framesDiscarded, uint eosSeen, uint maximumFrameBytes, uint totalBytes, 
                                           uint corruptMacroblocks)
        {
            this.Hdr = hdr;
            this.BufferCount = bufferCount;
            this.FrameCount = frameCount;
            this.FramesSkipped = framesSkipped;
            this.FramesDiscarded = framesDiscarded;
            this.EosSeen = eosSeen;
            this.MaximumFrameBytes = maximumFrameBytes;
            this.TotalBytes = totalBytes;
            this.CorruptMacroBlocks = corruptMacroblocks;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CORE_STATISTICS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_CORE_STATS_DIR Dir { get; }

        public int Reset { get; }

        public MMAL_CORE_STATISTICS_T Stats { get; }

        public MMAL_PARAMETER_CORE_STATISTICS_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CORE_STATS_DIR dir, int reset,
                                                MMAL_CORE_STATISTICS_T stats)
        {
            this.Hdr = hdr;
            this.Dir = dir;
            this.Reset = reset;
            this.Stats = stats;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_MEM_USAGE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public int PoolMemAllocSize { get; }

        public MMAL_PARAMETER_MEM_USAGE_T(MMAL_PARAMETER_HEADER_T hdr, int poolMemAllocSize)
        {
            this.Hdr = hdr;
            this.PoolMemAllocSize = poolMemAllocSize;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_LOGGING_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public uint Set { get; }

        public uint Clear { get; }

        public MMAL_PARAMETER_LOGGING_T(MMAL_PARAMETER_HEADER_T hdr, uint set, uint clear)
        {
            this.Hdr = hdr;
            this.Set = set;
            this.Clear = clear;
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

        public const int MMAL_MAX_IMAGEFX_PARAMETERS = 6;

        public const int MMAL_PARAMETER_CAMERA_INFO_MAX_CAMERAS = 4;
        public const int MMAL_PARAMETER_CAMERA_INFO_MAX_FLASHES = 2;
        public const int MMAL_PARAMETER_CAMERA_INFO_MAX_STR_LEN = 16;

        public const int MMAL_CAMERA_ANNOTATE_MAX_TEXT_LEN = 32;
        public const int MMAL_CAMERA_ANNOTATE_MAX_TEXT_LEN_V2 = 256;
        public const int MMAL_CAMERA_ANNOTATE_MAX_TEXT_LEN_V3 = 256;        
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

        public int Width { get; }

        public int Height { get; }

        public int Quality { get; }

        public MMAL_PARAMETER_THUMBNAIL_CONFIG_T(MMAL_PARAMETER_HEADER_T hdr, int width, int height, int quality)
        {
            this.Hdr = hdr;
            this.Width = width;
            this.Height = height;
            this.Quality = quality;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MMAL_PARAMETER_EXIF_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        private byte[] data;

        public int KeyLen { get; }

        public int ValueOffset { get; }

        public int ValueLen { get; }

        public byte[] Data => this.data;

        public MMAL_PARAMETER_EXIF_T(MMAL_PARAMETER_HEADER_T hdr, int keylen, int valueOffset, int valueLen, byte[] data)
        {
            this.Hdr = hdr;
            this.KeyLen = keylen;
            this.ValueOffset = valueOffset;
            this.ValueLen = valueLen;
            this.data = data;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MMAL_PARAMETER_EXIF_T_DUMMY
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public int KeyLen { get; }

        public int ValueOffset { get; }

        public int ValueLen { get; }

        public byte Data { get; }

        public MMAL_PARAMETER_EXIF_T_DUMMY(MMAL_PARAMETER_HEADER_T hdr, int keylen, int valueOffset, int valueLen, byte data)
        {
            this.Hdr = hdr;
            this.KeyLen = keylen;
            this.ValueOffset = valueOffset;
            this.ValueLen = valueLen;
            this.Data = data;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_EXPOSUREMODE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAM_EXPOSUREMODE_T Value { get; }

        public MMAL_PARAMETER_EXPOSUREMODE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_EXPOSUREMODE_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_EXPOSUREMETERINGMODE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAM_EXPOSUREMETERINGMODE_T Value { get; }

        public MMAL_PARAMETER_EXPOSUREMETERINGMODE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_EXPOSUREMETERINGMODE_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_AWBMODE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAM_AWBMODE_T Value { get; }

        public MMAL_PARAMETER_AWBMODE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_AWBMODE_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_IMAGEFX_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAM_IMAGEFX_T Value { get; }

        public MMAL_PARAMETER_IMAGEFX_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_IMAGEFX_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_IMAGEFX_PARAMETERS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAM_IMAGEFX_T Effect { get; }

        public uint NumEffectParams { get; }

        public uint[] EffectParameter { get; }

        public MMAL_PARAMETER_IMAGEFX_PARAMETERS_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_IMAGEFX_T effect, uint numEffectParams, uint[] effectParameter)
        {
            this.Hdr = hdr;
            this.Effect = effect;
            this.NumEffectParams = numEffectParams;
            this.EffectParameter = effectParameter;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_COLOURFX_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public int Enable { get; }

        public int U { get; }

        public int V { get; }

        public MMAL_PARAMETER_COLOURFX_T(MMAL_PARAMETER_HEADER_T hdr, int enable, int u, int v)
        {
            this.Hdr = hdr;
            this.Enable = enable;
            this.U = u;
            this.V = v;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_STC_MODE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_CAMERA_STC_MODE_T Value { get; }

        public MMAL_PARAMETER_CAMERA_STC_MODE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CAMERA_STC_MODE_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FLICKERAVOID_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAM_FLICKERAVOID_T Value { get; }

        public MMAL_PARAMETER_FLICKERAVOID_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_FLICKERAVOID_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FLASH_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAM_FLASH_T Value { get; }

        public MMAL_PARAMETER_FLASH_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_FLASH_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_REDEYE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAM_REDEYE_T Value { get; }

        public MMAL_PARAMETER_REDEYE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_REDEYE_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FOCUS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAM_FOCUS_T Value { get; }

        public MMAL_PARAMETER_FOCUS_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_FOCUS_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAPTURE_STATUS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAM_CAPTURE_STATUS_T Value { get; }

        public MMAL_PARAMETER_CAPTURE_STATUS_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_CAPTURE_STATUS_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FOCUS_STATUS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAM_FOCUS_STATUS_T Value { get; }

        public MMAL_PARAMETER_FOCUS_STATUS_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_FOCUS_STATUS_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FACE_TRACK_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAM_FACE_TRACK_MODE_T Mode { get; }

        public uint MaxRegions { get; }

        public uint Frames { get; }

        public uint Quality { get; }

        public MMAL_PARAMETER_FACE_TRACK_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_FACE_TRACK_MODE_T mode,
                                           uint maxRegions, uint frames, uint quality)
        {
            this.Hdr = hdr;
            this.Mode = mode;
            this.MaxRegions = maxRegions;
            this.Frames = frames;
            this.Quality = quality;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FACE_TRACK_FACE_T
    {
        public int FaceId { get; }

        public int Score { get; }

        public MMAL_RECT_T FaceRect { get; }

        public MMAL_RECT_T[] EyeRect { get; }

        public MMAL_RECT_T MouthRect { get; }

        public MMAL_PARAMETER_FACE_TRACK_FACE_T(int faceId, int score, MMAL_RECT_T faceRect, MMAL_RECT_T[] eyeRect, MMAL_RECT_T mouthRect)
        {
            this.FaceId = faceId;
            this.Score = score;
            this.FaceRect = faceRect;
            this.EyeRect = eyeRect;
            this.MouthRect = mouthRect;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FACE_TRACK_RESULTS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public uint NumFaces { get; }

        public uint FrameWidth { get; }

        public uint FrameHeight { get; }

        public MMAL_PARAMETER_FACE_TRACK_FACE_T[] Faces { get; }

        public MMAL_PARAMETER_FACE_TRACK_RESULTS_T(MMAL_PARAMETER_HEADER_T hdr, uint numFaces, uint frameWidth, uint frameHeight,
                                                   MMAL_PARAMETER_FACE_TRACK_FACE_T[] faces)
        {
            this.Hdr = hdr;
            this.NumFaces = numFaces;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.Faces = faces;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_CAMERA_CONFIG_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public int MaxStillsW { get; }

        public int MaxStillsH { get; }

        public int StillsYUV422 { get; }

        public int OneShotStills { get; }

        public int MaxPreviewVideoW { get; }

        public int MaxPreviewVideoH { get; }

        public int NumPreviewVideoFrames { get; }

        public int StillsCaptureCircularBufferHeight { get; }

        public int FastPreviewResume { get; }

        public MMAL_PARAMETER_CAMERA_CONFIG_TIMESTAMP_MODE_T UseSTCTimestamp { get; }

        public MMAL_PARAMETER_CAMERA_CONFIG_T(MMAL_PARAMETER_HEADER_T hdr, int maxStillsW, int maxStillsH, int stillsYUV422, 
                                              int oneShotStills, int maxPreviewVideoW, int maxPreviewVideoH, int numPreviewVideoFrames,
                                              int stillsCaptureCircularBufferHeight, int fastPreviewResume,
                                              MMAL_PARAMETER_CAMERA_CONFIG_TIMESTAMP_MODE_T useSTCTimestamp)
        {
            this.Hdr = hdr;
            this.MaxStillsW = maxStillsW;
            this.MaxStillsH = maxStillsH;
            this.StillsYUV422 = stillsYUV422;
            this.OneShotStills = oneShotStills;
            this.MaxPreviewVideoW = maxPreviewVideoW;
            this.MaxPreviewVideoH = maxPreviewVideoH;
            this.NumPreviewVideoFrames = numPreviewVideoFrames;
            this.StillsCaptureCircularBufferHeight = stillsCaptureCircularBufferHeight;
            this.FastPreviewResume = fastPreviewResume;
            this.UseSTCTimestamp = useSTCTimestamp;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_INFO_CAMERA_T
    {
        public int PortId { get; }

        public int MaxWidth { get; }

        public int MaxHeight { get; }

        public int LensPresent { get; }

        public MMAL_PARAMETER_CAMERA_INFO_CAMERA_T(int portId, int maxWidth, int maxHeight, int lensPresent)
        {
            this.PortId = portId;
            this.MaxWidth = maxWidth;
            this.MaxHeight = maxHeight;
            this.LensPresent = lensPresent;            
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MMAL_PARAMETER_CAMERA_INFO_CAMERA_V2_T
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        private string cameraName;

        public int PortId { get; }

        public int MaxWidth { get; }

        public int MaxHeight { get; }

        public int LensPresent { get; }

        public string CameraName => cameraName;

        public MMAL_PARAMETER_CAMERA_INFO_CAMERA_V2_T(int portId, int maxWidth, int maxHeight, int lensPresent, string cameraName)
        {
            this.PortId = portId;
            this.MaxWidth = maxWidth;
            this.MaxHeight = maxHeight;
            this.LensPresent = lensPresent;
            this.cameraName = cameraName;                 
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_INFO_FLASH_T
    {
        public MMAL_PARAMETER_CAMERA_INFO_FLASH_TYPE_T FlashType { get; }

        public MMAL_PARAMETER_CAMERA_INFO_FLASH_T(MMAL_PARAMETER_CAMERA_INFO_FLASH_TYPE_T flashType)
        {
            this.FlashType = flashType;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_INFO_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        private MMAL_PARAMETER_CAMERA_INFO_CAMERA_T[] cameras;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        private MMAL_PARAMETER_CAMERA_INFO_FLASH_T[] flashes;
        
        public int NumCameras { get; }

        public int NumFlashes { get; }

        public MMAL_PARAMETER_CAMERA_INFO_CAMERA_T[] Cameras => cameras;
        public MMAL_PARAMETER_CAMERA_INFO_FLASH_T[] Flashes => flashes;

        public MMAL_PARAMETER_CAMERA_INFO_T(MMAL_PARAMETER_HEADER_T hdr, int numCameras, int numFlashes, MMAL_PARAMETER_CAMERA_INFO_CAMERA_T[] cameras,
                                            MMAL_PARAMETER_CAMERA_INFO_FLASH_T[] flashes)
        {
            this.Hdr = hdr;
            this.NumCameras = numCameras;
            this.NumFlashes = numFlashes;
            this.cameras = cameras;
            this.flashes = flashes;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_INFO_V2_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 4)]
        private MMAL_PARAMETER_CAMERA_INFO_CAMERA_V2_T[] cameras;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 2)]
        private MMAL_PARAMETER_CAMERA_INFO_FLASH_T[] flashes;
        
        public int NumCameras { get; }

        public int NumFlashes { get; }

        public MMAL_PARAMETER_CAMERA_INFO_CAMERA_V2_T[] Cameras => cameras;
        public MMAL_PARAMETER_CAMERA_INFO_FLASH_T[] Flashes => flashes;

        public MMAL_PARAMETER_CAMERA_INFO_V2_T(MMAL_PARAMETER_HEADER_T hdr, int numCameras, int numFlashes, MMAL_PARAMETER_CAMERA_INFO_CAMERA_V2_T[] cameras,
                                            MMAL_PARAMETER_CAMERA_INFO_FLASH_T[] flashes)
        {
            this.Hdr = hdr;
            this.NumCameras = numCameras;
            this.NumFlashes = numFlashes;
            this.cameras = cameras;
            this.flashes = flashes;
        }
    }
        
    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAPTUREMODE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAMETER_CAPTUREMODE_MODE_T Mode { get; }

        public MMAL_PARAMETER_CAPTUREMODE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAMETER_CAPTUREMODE_MODE_T mode)
        {
            this.Hdr = hdr;
            this.Mode = mode;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FOCUS_REGION_T
    {
        public MMAL_RECT_T Rect { get; }

        public int Weight { get; }

        public int Mask { get; }

        public MMAL_PARAMETER_FOCUS_REGION_TYPE_T Type { get; }

        public MMAL_PARAMETER_FOCUS_REGION_T(MMAL_RECT_T rect, int weight, int mask, MMAL_PARAMETER_FOCUS_REGION_TYPE_T type)
        {
            this.Rect = rect;
            this.Weight = weight;
            this.Mask = mask;
            this.Type = type;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FOCUS_REGIONS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public uint NumRegions { get; }

        public int LockToFaces { get; }

        public MMAL_PARAMETER_FOCUS_REGION_T[] Regions { get; }

        public MMAL_PARAMETER_FOCUS_REGIONS_T(MMAL_PARAMETER_HEADER_T hdr, uint numRegions, int lockToFaces, MMAL_PARAMETER_FOCUS_REGION_T[] regions)
        {
            this.Hdr = hdr;
            this.NumRegions = numRegions;
            this.LockToFaces = lockToFaces;
            this.Regions = regions;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_INPUT_CROP_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_RECT_T Rect { get; }

        public MMAL_PARAMETER_INPUT_CROP_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_RECT_T rect)
        {
            this.Hdr = hdr;
            this.Rect = rect;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_SENSOR_INFORMATION_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_RATIONAL_T FNumber { get; }

        public MMAL_RATIONAL_T FocalLength { get; }

        public uint ModelId { get; }

        public uint ManufacturerId { get; }

        public uint Revision { get; }

        public MMAL_PARAMETER_SENSOR_INFORMATION_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_RATIONAL_T fNumber, MMAL_RATIONAL_T focalLength,
                                                   uint modelId, uint manufacturerId, uint revision)
        {
            this.Hdr = hdr;
            this.FNumber = fNumber;
            this.FocalLength = focalLength;
            this.ModelId = modelId;
            this.ManufacturerId = manufacturerId;
            this.Revision = revision;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FLASH_SELECT_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAMETER_CAMERA_INFO_FLASH_TYPE_T FlashType { get; }

        public MMAL_PARAMETER_FLASH_SELECT_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAMETER_CAMERA_INFO_FLASH_TYPE_T flashType)
        {
            this.Hdr = hdr;
            this.FlashType = flashType;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FIELD_OF_VIEW_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_RATIONAL_T FovH { get; }

        public MMAL_RATIONAL_T FovV { get; }

        public MMAL_PARAMETER_FIELD_OF_VIEW_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_RATIONAL_T fovH, MMAL_RATIONAL_T fovV)
        {
            this.Hdr = hdr;
            this.FovH = fovH;
            this.FovV = fovV;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_DRC_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAMETER_DRC_STRENGTH_T Strength { get; }

        public MMAL_PARAMETER_DRC_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAMETER_DRC_STRENGTH_T strength)
        {
            this.Hdr = hdr;
            this.Strength = strength;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_ALGORITHM_CONTROL_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_T Algorithm { get; }

        public int Enabled { get; }

        public MMAL_PARAMETER_ALGORITHM_CONTROL_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAMETER_ALGORITHM_CONTROL_ALGORITHMS_T algorithm,
                                                  int enabled)
        {
            this.Hdr = hdr;
            this.Algorithm = algorithm;
            this.Enabled = enabled;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_USE_CASE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAM_CAMERA_USE_CASE_T UseCase { get; }

        public MMAL_PARAMETER_CAMERA_USE_CASE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_CAMERA_USE_CASE_T useCase)
        {
            this.Hdr = hdr;
            this.UseCase = useCase;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_FPS_RANGE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_RATIONAL_T FpsLow { get; }

        public MMAL_RATIONAL_T FpsHigh { get; }

        public MMAL_PARAMETER_FPS_RANGE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_RATIONAL_T fpsLow, MMAL_RATIONAL_T fpsHigh)
        {
            this.Hdr = hdr;
            this.FpsLow = fpsLow;
            this.FpsHigh = fpsHigh;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_ZEROSHUTTERLAG_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
        public int ZeroShutterLagMode { get; }

        public int ConcurrentCapture { get; }

        public MMAL_PARAMETER_ZEROSHUTTERLAG_T(MMAL_PARAMETER_HEADER_T hdr, int zeroShutterLagMode, int concurrentCapture)
        {
            this.Hdr = hdr;
            this.ZeroShutterLagMode = zeroShutterLagMode;
            this.ConcurrentCapture = concurrentCapture;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_AWB_GAINS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_RATIONAL_T RGain { get; }

        public MMAL_RATIONAL_T BGain { get; }

        public MMAL_PARAMETER_AWB_GAINS_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_RATIONAL_T rGain, MMAL_RATIONAL_T bGain)
        {
            this.Hdr = hdr;
            this.RGain = rGain;
            this.BGain = bGain;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_SETTINGS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public int Exposure { get; }

        public MMAL_RATIONAL_T AnalogGain { get; }

        public MMAL_RATIONAL_T DigitalGain { get; }

        public MMAL_RATIONAL_T AwbRedGain { get; }

        public MMAL_RATIONAL_T AwbBlueGain { get; }

        public int FocusPosition { get; }

        public MMAL_PARAMETER_CAMERA_SETTINGS_T(MMAL_PARAMETER_HEADER_T hdr, int exposure, MMAL_RATIONAL_T analogGain, 
                                                MMAL_RATIONAL_T digitalGain, MMAL_RATIONAL_T awbRedGain, MMAL_RATIONAL_T awbBlueGain,
                                                int focusPosition)
        {
            this.Hdr = hdr;
            this.Exposure = exposure;
            this.AnalogGain = analogGain;
            this.DigitalGain = digitalGain;
            this.AwbRedGain = awbRedGain;
            this.AwbBlueGain = awbBlueGain;
            this.FocusPosition = focusPosition;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_PRIVACY_INDICATOR_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAM_PRIVACY_INDICATOR_T Mode { get; }

        public MMAL_PARAMETER_PRIVACY_INDICATOR_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_PRIVACY_INDICATOR_T mode)
        {
            this.Hdr = hdr;
            this.Mode = mode;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_ANNOTATE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public int Enable { get; }

        public string Text { get; }

        public int ShowShutter { get; }

        public int ShowAnalogGain { get; }

        public int ShowLens { get; }

        public int ShowCaf { get; }

        public int ShowMotion { get; }

        public MMAL_PARAMETER_CAMERA_ANNOTATE_T(MMAL_PARAMETER_HEADER_T hdr, int enable, string text,
                                                int showShutter, int showAnalogGain, int showLens, int showCaf, int showMotion)
        {
            this.Hdr = hdr;
            this.Enable = enable;
            this.Text = text;
            this.ShowShutter = showShutter;
            this.ShowAnalogGain = showAnalogGain;
            this.ShowLens = showLens;
            this.ShowCaf = showCaf;
            this.ShowMotion = showMotion;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_ANNOTATE_V2_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public int Enable { get; }

        public string Text { get; }

        public int ShowShutter { get; }

        public int ShowAnalogGain { get; }

        public int ShowLens { get; }

        public int ShowCaf { get; }

        public int ShowMotion { get; }

        public MMAL_PARAMETER_CAMERA_ANNOTATE_V2_T(MMAL_PARAMETER_HEADER_T hdr, int enable, string text,
                                                int showShutter, int showAnalogGain, int showLens, int showCaf, int showMotion)
        {
            this.Hdr = hdr;
            this.Enable = enable;
            this.Text = text;
            this.ShowShutter = showShutter;
            this.ShowAnalogGain = showAnalogGain;
            this.ShowLens = showLens;
            this.ShowCaf = showCaf;
            this.ShowMotion = showMotion;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        private byte[] text;
        
        public int Enable { get; }

        public int ShowShutter { get; }

        public int ShowAnalogGain { get; }

        public int ShowLens { get; }

        public int ShowCaf { get; }

        public int ShowMotion { get; }

        public int ShowFrameNum { get; }

        public int EnableTextBackground { get; }

        public int CustomBackgroundColor { get; }

        public byte CustomBackgroundY { get; }

        public byte CustomBackgroundU { get; }

        public byte CustomBackgroundV { get; }

        public byte Dummy1 { get; }

        public int CustomTextColor { get; }

        public byte CustomTextY { get; }

        public byte CustomTextU { get; }

        public byte CustomTextV { get; }

        public byte TextSize { get; }

        public byte[] Text => text;
        
        public MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(MMAL_PARAMETER_HEADER_T hdr, int enable, int showShutter, int showAnalogGain, int showLens, 
                                                   int showCaf, int showMotion, int showFrameNum, int enableTextBackground, int customBackgroundColor,
                                                   byte customBackgroundY, byte customBackgroundU, byte customBackgroundV, byte dummy1,
                                                   int customTextColor, byte customTextY, byte customTextU, byte customTextV, byte textSize,
                                                   byte[] text)
        {
            this.Hdr = hdr;
            this.Enable = enable;
            this.text = text;
            this.ShowShutter = showShutter;
            this.ShowAnalogGain = showAnalogGain;
            this.ShowLens = showLens;
            this.ShowCaf = showCaf;
            this.ShowMotion = showMotion;
            this.ShowFrameNum = showFrameNum;
            this.EnableTextBackground = enableTextBackground;
            this.CustomBackgroundColor = customBackgroundColor;
            this.CustomBackgroundY = customBackgroundY;
            this.CustomBackgroundU = customBackgroundU;
            this.CustomBackgroundV = customBackgroundV;
            this.Dummy1 = dummy1;
            this.CustomTextColor = customTextColor;
            this.CustomTextY = customTextY;
            this.CustomTextU = customTextU;
            this.CustomTextV = customTextV;
            this.TextSize = textSize;
            this.text = text;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_STEREOSCOPIC_MODE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_STEREOSCOPIC_MODE_T Mode { get; }

        public int Decimate { get; }

        public int SwapEyes { get; }

        public MMAL_PARAMETER_STEREOSCOPIC_MODE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_STEREOSCOPIC_MODE_T mode,
                                                  int decimate, int swapEyes)
        {
            this.Hdr = hdr;
            this.Mode = mode;
            this.Decimate = decimate;
            this.SwapEyes = swapEyes;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_INTERFACE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_CAMERA_INTERFACE_T Mode { get; }

        public MMAL_PARAMETER_CAMERA_INTERFACE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CAMERA_INTERFACE_T mode)
        {
            this.Hdr = hdr;
            this.Mode = mode;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_CLOCKING_MODE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_CAMERA_CLOCKING_MODE_T Mode { get; }

        public MMAL_PARAMETER_CAMERA_CLOCKING_MODE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CAMERA_CLOCKING_MODE_T mode)
        {
            this.Hdr = hdr;
            this.Mode = mode;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_RX_CONFIG_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_CAMERA_RX_CONFIG_DECODE Decode { get; }

        public MMAL_CAMERA_RX_CONFIG_ENCODE Encode { get; }

        public MMAL_CAMERA_RX_CONFIG_UNPACK Unpack { get; }

        public MMAL_CAMERA_RX_CONFIG_PACK Pack { get; }

        public uint DataLanes { get; }

        public uint EncodeBlockLength { get; }

        public uint EmbeddedDataLanes { get; }

        public uint ImageId { get; }

        public MMAL_PARAMETER_CAMERA_RX_CONFIG_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CAMERA_RX_CONFIG_DECODE decode,
                                                 MMAL_CAMERA_RX_CONFIG_ENCODE encode, MMAL_CAMERA_RX_CONFIG_UNPACK unpack,
                                                 MMAL_CAMERA_RX_CONFIG_PACK pack,
                                                 uint dataLanes, uint encodeBlockLength, uint embeddedDataLines, uint imageId)
        {
            this.Hdr = hdr;
            this.Decode = decode;
            this.Encode = encode;
            this.Unpack = unpack;
            this.Pack = pack;
            this.DataLanes = dataLanes;
            this.EncodeBlockLength = encodeBlockLength;
            this.EmbeddedDataLanes = embeddedDataLines;
            this.ImageId = imageId;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CAMERA_RX_TIMING_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public uint Timing1 { get; }

        public uint Timing2 { get; }

        public uint Timing3 { get; }

        public uint Timing4 { get; }

        public uint Timing5 { get; }

        public uint Term1 { get; }

        public uint Term2 { get; }

        public uint CpiTiming1 { get; }

        public uint CpiTiming2 { get; }

        public MMAL_PARAMETER_CAMERA_RX_TIMING_T(MMAL_PARAMETER_HEADER_T hdr, uint timing1, uint timing2, uint timing3, uint timing4, 
                                                 uint timing5, uint term1, uint term2, uint cpiTiming1, uint cpiTiming2)
        {
            this.Hdr = hdr;
            this.Timing1 = timing1;
            this.Timing2 = timing2;
            this.Timing3 = timing3;
            this.Timing4 = timing4;
            this.Timing5 = timing5;
            this.Term1 = term1;
            this.Term2 = term2;
            this.CpiTiming1 = cpiTiming1;
            this.CpiTiming2 = cpiTiming2;
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

        public uint Set { get; }

        public uint DisplayNum { get; }

        public int Fullscreen { get; }

        public MMALParametersVideo.MMAL_DISPLAYTRANSFORM_T Transform { get; }

        public MMAL_RECT_T DestRect { get; }

        public MMAL_RECT_T SrcRect { get; }

        public int NoAspect { get; }

        public MMALParametersVideo.MMAL_DISPLAYMODE_T Mode { get; }

        public int PixelX { get; }

        public int PixelY { get; }

        public int Layer { get; }

        public int CopyrightRequired { get; }

        public int Alpha { get; }

        public MMAL_DISPLAYREGION_T(MMAL_PARAMETER_HEADER_T hdr, uint set, uint displayNum, int fullscreen,
                                    MMALParametersVideo.MMAL_DISPLAYTRANSFORM_T transform, MMAL_RECT_T destRect, MMAL_RECT_T srcRect,
                                    int noAspect, MMALParametersVideo.MMAL_DISPLAYMODE_T mode, int pixelX, int pixelY,
                                    int layer, int copyrightRequired, int alpha)
        {
            this.Hdr = hdr;
            this.Set = set;
            this.DisplayNum = displayNum;
            this.Fullscreen = fullscreen;
            this.Transform = transform;
            this.DestRect = destRect;
            this.SrcRect = srcRect;
            this.NoAspect = noAspect;
            this.Mode = mode;
            this.PixelX = pixelX;
            this.PixelY = pixelY;
            this.Layer = layer;
            this.CopyrightRequired = copyrightRequired;
            this.Alpha = alpha;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_PROFILE_S
    {
        public MMALParametersVideo.MMAL_VIDEO_PROFILE_T Profile { get; }

        public MMALParametersVideo.MMAL_VIDEO_LEVEL_T Level { get; }

        public MMAL_PARAMETER_VIDEO_PROFILE_S(MMALParametersVideo.MMAL_VIDEO_PROFILE_T profile, MMALParametersVideo.MMAL_VIDEO_LEVEL_T level)
        {
            this.Profile = profile;
            this.Level = level;
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

        public uint RcModel { get; }

        public MMAL_PARAMETER_VIDEO_ENCODE_RC_MODEL_T(MMAL_PARAMETER_HEADER_T hdr, uint rcModel)
        {
            this.Hdr = hdr;
            this.RcModel = rcModel;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_RATECONTROL_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

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

        public MMALParametersVideo.MMAL_VIDEO_RATECONTROL_T Control { get; }

        public MMAL_PARAMETER_VIDEO_RATECONTROL_T(MMAL_PARAMETER_HEADER_T hdr, MMALParametersVideo.MMAL_VIDEO_RATECONTROL_T control)
        {
            this.Hdr = hdr;
            this.Control = control;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_ENCODER_H264_MB_INTRA_MODES_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMALParametersVideo.MMAL_VIDEO_ENCODE_H264_MB_INTRA_MODES_T MbMode { get; }

        public MMAL_PARAMETER_VIDEO_ENCODER_H264_MB_INTRA_MODES_T(MMAL_PARAMETER_HEADER_T hdr, MMALParametersVideo.MMAL_VIDEO_ENCODE_H264_MB_INTRA_MODES_T mbMode)
        {
            this.Hdr = hdr;
            this.MbMode = mbMode;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_NALUNITFORMAT_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMALParametersVideo.MMAL_VIDEO_NALUNITFORMAT_T Format { get; }

        public MMAL_PARAMETER_VIDEO_NALUNITFORMAT_T(MMAL_PARAMETER_HEADER_T hdr, MMALParametersVideo.MMAL_VIDEO_NALUNITFORMAT_T format)
        {
            this.Hdr = hdr;
            this.Format = format;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_LEVEL_EXTENSION_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public uint CustomMaxMbps { get; }

        public uint CustomMaxFs { get; }

        public uint CustomMaxBrAndCpb { get; }

        public MMAL_PARAMETER_VIDEO_LEVEL_EXTENSION_T(MMAL_PARAMETER_HEADER_T hdr, uint customMaxMbps, uint customMaxFs, uint customMaxBrAndCpb)
        {
            this.Hdr = hdr;
            this.CustomMaxMbps = customMaxMbps;
            this.CustomMaxFs = customMaxFs;
            this.CustomMaxBrAndCpb = customMaxBrAndCpb;           
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_INTRA_REFRESH_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

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

        public MMALParametersVideo.MMAL_VIDEO_INTRA_REFRESH_T RefreshMode { get; }

        public int AirMbs { get; }

        public int AirRef { get; }

        public int CirMbs { get; }

        public int PirMbs { get; }

        public MMAL_PARAMETER_VIDEO_INTRA_REFRESH_T(MMAL_PARAMETER_HEADER_T hdr, MMALParametersVideo.MMAL_VIDEO_INTRA_REFRESH_T refreshMode,
                                                    int airMbs, int airRef, int cirMbs, int pirMbs)
        {
            this.Hdr = hdr;
            this.RefreshMode = refreshMode;
            this.AirMbs = airMbs;
            this.AirRef = airRef;
            this.CirMbs = cirMbs;
            this.PirMbs = pirMbs;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_EEDE_ENABLE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public int Enable { get; }

        public MMAL_PARAMETER_VIDEO_EEDE_ENABLE_T(MMAL_PARAMETER_HEADER_T hdr, int enable)
        {
            this.Hdr = hdr;
            this.Enable = enable;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_EEDE_LOSSRATE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public uint LossRate { get; }

        public MMAL_PARAMETER_VIDEO_EEDE_LOSSRATE_T(MMAL_PARAMETER_HEADER_T hdr, uint lossRate)
        {
            this.Hdr = hdr;
            this.LossRate = lossRate;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_DRM_INIT_INFO_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public uint CurrentTime { get; }

        public uint TicksPerSec { get; }

        public uint[] Lhs { get; }

        public MMAL_PARAMETER_VIDEO_DRM_INIT_INFO_T(MMAL_PARAMETER_HEADER_T hdr, uint currentTime, uint ticksPerSec, uint[] lhs)
        {
            this.Hdr = hdr;
            this.CurrentTime = currentTime;
            this.TicksPerSec = ticksPerSec;
            this.Lhs = lhs;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_DRM_PROTECT_BUFFER_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public uint SizeWanted { get; }

        public uint Protect { get; }

        public uint MemHandle { get; }

        public IntPtr PhysAddr { get; }

        public MMAL_PARAMETER_VIDEO_DRM_PROTECT_BUFFER_T(MMAL_PARAMETER_HEADER_T hdr, uint sizeWanted, uint protect, uint memHandle,
                                                         IntPtr physAddr)
        {
            this.Hdr = hdr;
            this.SizeWanted = sizeWanted;
            this.Protect = protect;
            this.MemHandle = memHandle;
            this.PhysAddr = physAddr;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_RENDER_STATS_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public int Valid { get; }

        public uint Match { get; }

        public uint Period { get; }

        public uint Phase { get; }

        public uint PixelClockNominal { get; }

        public uint HvsStatus { get; }

        public uint[] Dummy { get; }

        public MMAL_PARAMETER_VIDEO_RENDER_STATS_T(MMAL_PARAMETER_HEADER_T hdr, int valid, uint match, uint period, 
                                                   uint phase, uint pixelClockNominal, uint hvsStatus, uint[] dummy)
        {
            this.Hdr = hdr;
            this.Valid = valid;
            this.Match = match;
            this.Period = period;
            this.Phase = phase;
            this.PixelClockNominal = pixelClockNominal;
            this.HvsStatus = hvsStatus;
            this.Dummy = dummy;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_VIDEO_INTERLACE_TYPE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMALParametersVideo.MMAL_INTERLACE_TYPE_T EMode { get; }

        public int BRepeatFirstField { get; }

        public MMAL_PARAMETER_VIDEO_INTERLACE_TYPE_T(MMAL_PARAMETER_HEADER_T hdr, MMALParametersVideo.MMAL_INTERLACE_TYPE_T eMode,
                                                     int bRepeatFirstField)
        {
            this.Hdr = hdr;
            this.EMode = eMode;
            this.BRepeatFirstField = bRepeatFirstField;
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

        public int Enable { get; }

        public uint Filter { get; }

        public uint Target { get; }

        public uint Shift { get; }

        public int SpeedFactor { get; }

        public int InterFactor { get; }

        public int AdjCap { get; }

        public MMAL_PARAMETER_AUDIO_LATENCY_TARGET_T(MMAL_PARAMETER_HEADER_T hdr, int enable, uint filter, uint target, uint shift,
                                                     int speedFactor, int interFactor, int adjCap)
        {
            this.Hdr = hdr;
            this.Enable = enable;
            this.Filter = filter;
            this.Target = target;
            this.Shift = shift;
            this.SpeedFactor = speedFactor;
            this.InterFactor = interFactor;
            this.AdjCap = adjCap;
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

        public MMAL_CLOCK_UPDATE_THRESHOLD_T Value { get; }

        public MMAL_PARAMETER_CLOCK_UPDATE_THRESHOLD_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CLOCK_UPDATE_THRESHOLD_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CLOCK_DISCONT_THRESHOLD_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_CLOCK_DISCONT_THRESHOLD_T Value { get; }

        public MMAL_PARAMETER_CLOCK_DISCONT_THRESHOLD_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CLOCK_DISCONT_THRESHOLD_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CLOCK_REQUEST_THRESHOLD_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_CLOCK_REQUEST_THRESHOLD_T Value { get; }

        public MMAL_PARAMETER_CLOCK_REQUEST_THRESHOLD_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CLOCK_REQUEST_THRESHOLD_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CLOCK_LATENCY_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_CLOCK_LATENCY_T Value { get; }

        public MMAL_PARAMETER_CLOCK_LATENCY_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_CLOCK_LATENCY_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
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

        // @waveform80 The following two components aren't in the MMAL headers, but do exist
        public const string MMAL_COMPONENT_DEFAULT_NULL_SINK = "vc.null_sink";
        public const string MMAL_COMPONENT_DEFAULT_RESIZER = "vc.ril.resize";
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_UINT64_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public ulong Value { get; }

        public MMAL_PARAMETER_UINT64_T(MMAL_PARAMETER_HEADER_T hdr, ulong value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_INT64_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public long Value { get; }

        public MMAL_PARAMETER_INT64_T(MMAL_PARAMETER_HEADER_T hdr, long value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_UINT32_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public uint Value { get; }

        public MMAL_PARAMETER_UINT32_T(MMAL_PARAMETER_HEADER_T hdr, uint value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_INT32_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public int Value { get; }

        public MMAL_PARAMETER_INT32_T(MMAL_PARAMETER_HEADER_T hdr, int value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_RATIONAL_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_RATIONAL_T Value { get; }

        public MMAL_PARAMETER_RATIONAL_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_RATIONAL_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_BOOLEAN_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public int Value { get; }

        public MMAL_PARAMETER_BOOLEAN_T(MMAL_PARAMETER_HEADER_T hdr, int value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_STRING_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public string Value { get; }

        public MMAL_PARAMETER_STRING_T(MMAL_PARAMETER_HEADER_T hdr, string value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_SCALEFACTOR_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public uint ScaleX { get; }

        public uint ScaleY { get; }

        public MMAL_PARAMETER_SCALEFACTOR_T(MMAL_PARAMETER_HEADER_T hdr, uint scaleX, uint scaleY)
        {
            this.Hdr = hdr;
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PARAMETER_MIRROR_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public MMAL_PARAM_MIRROR_T Value { get; }

        public MMAL_PARAMETER_MIRROR_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_PARAM_MIRROR_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_URI_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public string Value { get; }

        public MMAL_PARAMETER_URI_T(MMAL_PARAMETER_HEADER_T hdr, string value)
        {
            this.Hdr = hdr;
            this.Value = value;
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

        public MMAL_RATIONAL_T Value { get; }

        public MMAL_PARAMETER_FRAME_RATE_T(MMAL_PARAMETER_HEADER_T hdr, MMAL_RATIONAL_T value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CONFIGFILE_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public uint Value { get; }

        public MMAL_PARAMETER_CONFIGFILE_T(MMAL_PARAMETER_HEADER_T hdr, uint value)
        {
            this.Hdr = hdr;
            this.Value = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PARAMETER_CONFIGFILE_CHUNK_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;

        public uint Size { get; }

        public uint Offset { get; }

        public string Data { get; }

        public MMAL_PARAMETER_CONFIGFILE_CHUNK_T(MMAL_PARAMETER_HEADER_T hdr, uint size, uint offset, string data)
        {
            this.Hdr = hdr;
            this.Size = size;
            this.Offset = offset;
            this.Data = data;
        }
    }
}
