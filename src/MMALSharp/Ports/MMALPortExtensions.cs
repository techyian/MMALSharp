using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static MMALSharp.MMALCallerHelper;
using static MMALSharp.Native.MMALParametersCamera;

namespace MMALSharp
{
    public class Parameter
    {
        public int ParamValue { get; set; }
        public Type ParamType { get; set; }
        public string ParamName { get; set; }

        public Parameter(int paramVal, Type paramType, string paramName)
        {
            this.ParamValue = paramVal;
            this.ParamType = paramType;
            this.ParamName = paramName;
        }
    }

    internal static class MMALParameterHelpers
    {
        public static List<Parameter> ParameterHelper = new List<Parameter>
        {
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_ANTISHAKE, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_ANTISHAKE"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_BRIGHTNESS, typeof(MMAL_PARAMETER_RATIONAL_T), "MMAL_PARAMETER_BRIGHTNESS"),
            new Parameter(MMALParametersCommon.MMAL_PARAMETER_BUFFER_FLAG_FILTER, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_BUFFER_FLAG_FILTER"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_CAMERA_BURST_CAPTURE, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_CAMERA_BURST_CAPTURE"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_CAMERA_CUSTOM_SENSOR_CONFIG, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_CAMERA_CUSTOM_SENSOR_CONFIG"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_CAMERA_MIN_ISO, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_CAMERA_MIN_ISO"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_CAMERA_NUM, typeof(MMAL_PARAMETER_INT32_T), "MMAL_PARAMETER_CAMERA_NUM"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_CAPTURE_EXPOSURE_COMP, typeof(MMAL_PARAMETER_INT32_T), "MMAL_PARAMETER_CAPTURE_EXPOSURE_COMP"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_CAPTURE, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_CAPTURE"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_CAPTURE_STATS_PASS, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_CAPTURE_STATS_PASS"),
            new Parameter(MMALParametersClock.MMAL_PARAMETER_CLOCK_ACTIVE, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_CLOCK_ACTIVE"),
            new Parameter(MMALParametersClock.MMAL_PARAMETER_CLOCK_ENABLE_BUFFER_INFO, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_CLOCK_ENABLE_BUFFER_INFO"),
            new Parameter(MMALParametersClock.MMAL_PARAMETER_CLOCK_FRAME_RATE, typeof(MMAL_PARAMETER_RATIONAL_T), "MMAL_PARAMETER_CLOCK_FRAME_RATE"),
            new Parameter(MMALParametersClock.MMAL_PARAMETER_CLOCK_SCALE, typeof(MMAL_PARAMETER_RATIONAL_T), "MMAL_PARAMETER_CLOCK_SCALE"),
            new Parameter(MMALParametersClock.MMAL_PARAMETER_CLOCK_TIME, typeof(MMAL_PARAMETER_INT64_T), "MMAL_PARAMETER_CLOCK_TIME"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_CONTRAST, typeof(MMAL_PARAMETER_RATIONAL_T), "MMAL_PARAMETER_CONTRAST"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_DPF_CONFIG, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_DPF_CONFIG"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_ENABLE_RAW_CAPTURE, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_ENABLE_RAW_CAPTURE"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_EXIF_DISABLE, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_EXIF_DISABLE"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_EXPOSURE_COMP, typeof(MMAL_PARAMETER_INT32_T), "MMAL_PARAMETER_EXPOSURE_COMP"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_EXTRA_BUFFERS, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_EXTRA_BUFFERS"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_FLASH_REQUIRED, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_FLASH_REQUIRED"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_FRAME_RATE, typeof(MMAL_PARAMETER_RATIONAL_T), "MMAL_PARAMETER_FRAME_RATE"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_INTRAPERIOD, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_INTRAPERIOD"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_ISO, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_ISO"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_JPEG_ATTACH_LOG, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_JPEG_ATTACH_LOG"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_JPEG_Q_FACTOR, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_JPEG_Q_FACTOR"),
            new Parameter(MMALParametersCommon.MMAL_PARAMETER_LOCKSTEP_ENABLE, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_LOCKSTEP_ENABLE"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_MB_ROWS_PER_SLICE, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_MB_ROWS_PER_SLICE"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_MINIMISE_FRAGMENTATION, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_MINIMISE_FRAGMENTATION"),
            new Parameter(MMALParametersCommon.MMAL_PARAMETER_NO_IMAGE_PADDING, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_NO_IMAGE_PADDING"),
            new Parameter(MMALParametersCommon.MMAL_PARAMETER_POWERMON_ENABLE, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_POWERMON_ENABLE"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_ROTATION, typeof(MMAL_PARAMETER_INT32_T), "MMAL_PARAMETER_ROTATION"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_SATURATION, typeof(MMAL_PARAMETER_RATIONAL_T), "MMAL_PARAMETER_SATURATION"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_SHARPNESS, typeof(MMAL_PARAMETER_RATIONAL_T), "MMAL_PARAMETER_SHARPNESS"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_SHUTTER_SPEED, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_SHUTTER_SPEED"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_STILLS_DENOISE, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_STILLS_DENOISE"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_SW_SATURATION_DISABLE, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_SW_SATURATION_DISABLE"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_SW_SHARPEN_DISABLE, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_SW_SHARPEN_DISABLE"),
            new Parameter(MMALParametersCommon.MMAL_PARAMETER_SYSTEM_TIME, typeof(MMAL_PARAMETER_UINT64_T), "MMAL_PARAMETER_SYSTEM_TIME"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ALIGN_HORIZ, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_VIDEO_ALIGN_HORIZ"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ALIGN_VERT, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_VIDEO_ALIGN_VERT"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_BIT_RATE, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_VIDEO_BIT_RATE"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_VIDEO_DENOISE, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_VIDEO_DENOISE"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_DROPPABLE_PFRAMES, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_VIDEO_DROPPABLE_PFRAMES"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_FRAME_LIMIT_BITS, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_VIDEO_ENCODE_FRAME_LIMIT_BITS"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INITIAL_QUANT, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_VIDEO_ENCODE_INITIAL_QUANT"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INLINE_HEADER, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_VIDEO_ENCODE_INLINE_HEADER"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INLINE_VECTORS, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_VIDEO_ENCODE_INLINE_VECTORS"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_MAX_QUANT, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_VIDEO_ENCODE_MAX_QUANT"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_MIN_QUANT, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_VIDEO_ENCODE_MIN_QUANT"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_PEAK_RATE, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_VIDEO_ENCODE_PEAK_RATE"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_QP_P, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_VIDEO_ENCODE_QP_P"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_RC_SLICE_DQUANT, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_VIDEO_ENCODE_RC_SLICE_DQUANT"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_SEI_ENABLE, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_VIDEO_ENCODE_SEI_ENABLE"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_SPS_TIMINGS, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_VIDEO_ENCODE_SPS_TIMINGS"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_FRAME_RATE, typeof(MMAL_PARAMETER_RATIONAL_T), "MMAL_PARAMETER_VIDEO_FRAME_RATE"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_IMMUTABLE_INPUT, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_VIDEO_IMMUTABLE_INPUT"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_INTERPOLATE_TIMESTAMPS, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_VIDEO_INTERPOLATE_TIMESTAMPS"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_MAX_NUM_CALLBACKS, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_VIDEO_MAX_NUM_CALLBACKS"),
            new Parameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_REQUEST_I_FRAME, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_VIDEO_REQUEST_I_FRAME"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_VIDEO_STABILISATION, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_VIDEO_STABILISATION"),
            new Parameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, typeof(MMAL_PARAMETER_BOOLEAN_T), "MMAL_PARAMETER_ZERO_COPY"),
            new Parameter(MMALParametersCamera.MMAL_PARAMETER_JPEG_RESTART_INTERVAL, typeof(MMAL_PARAMETER_UINT32_T), "MMAL_PARAMETER_JPEG_RESTART_INTERVAL")
        };
    }

    public static class MMALPortExtensions
    {
        /// <summary>
        /// Provides a facility to get data from the port using the native helper functions
        /// </summary>
        /// <param name="port">The port to get the parameter from</param>
        /// <param name="key">The unique key for the parameter</param>
        /// <returns></returns>
        public static unsafe dynamic GetParameter(this MMALPortBase port, int key)
        {
            var t = MMALParameterHelpers.ParameterHelper.Where(c => c.ParamValue == key).FirstOrDefault();

            if (t == null)
            {
                throw new PiCameraError($"Could not find parameter {key}");
            }

            try
            {
                switch (t.ParamType.Name)
                {
                    case "MMAL_PARAMETER_BOOLEAN_T":
                        int boolVal = 0;
                        MMALCheck(MMALUtil.mmal_port_parameter_get_boolean(port.Ptr, (uint)key, ref boolVal), "Unable to get boolean value");
                        return boolVal == 1;
                    case "MMAL_PARAMETER_UINT64_T":
                        ulong ulongVal = 0UL;
                        MMALCheck(MMALUtil.mmal_port_parameter_get_uint64(port.Ptr, (uint)key, ref ulongVal), "Unable to get ulong value");
                        return ulongVal;
                    case "MMAL_PARAMETER_INT64_T":
                        long longVal = 0U;
                        MMALCheck(MMALUtil.mmal_port_parameter_get_int64(port.Ptr, (uint)key, ref longVal), "Unable to get long value");
                        return longVal;
                    case "MMAL_PARAMETER_UINT32_T":
                        uint uintVal = 0U;
                        MMALCheck(MMALUtil.mmal_port_parameter_get_uint32(port.Ptr, (uint)key, ref uintVal), "Unable to get uint value");
                        return uintVal;
                    case "MMAL_PARAMETER_INT32_T":
                        int intVal = 0;
                        MMALCheck(MMALUtil.mmal_port_parameter_get_int32(port.Ptr, (uint)key, ref intVal), "Unable to get int value");
                        return intVal;
                    case "MMAL_PARAMETER_RATIONAL_T":
                        MMAL_RATIONAL_T ratVal = new MMAL_RATIONAL_T();
                        MMALCheck(MMALUtil.mmal_port_parameter_get_rational(port.Ptr, (uint)key, ref ratVal), "Unable to get rational value");
                        return (double)ratVal.Num / ratVal.Den;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch
            {
                MMALLog.Logger.Warn($"Unable to get port parameter {t.ParamName}");
                throw;
            }
        }

        /// <summary>
        /// Provides a facility to set data on the port using the native helper functions
        /// </summary>
        /// <param name="port">The port we want to set the parameter on</param>
        /// <param name="key">The unique key of the parameter</param>
        /// <param name="value">The value of the parameter</param>
        public static unsafe void SetParameter(this MMALPortBase port, int key, dynamic value)
        {
            var t = MMALParameterHelpers.ParameterHelper.Where(c => c.ParamValue == key).FirstOrDefault();

            if (t == null)
            {
                throw new PiCameraError($"Could not find parameter {key}");
            }

            MMALLog.Logger.Debug($"Setting parameter {t.ParamName}");

            try
            {
                switch (t.ParamType.Name)
                {
                    case "MMAL_PARAMETER_BOOLEAN_T":
                        int i = (bool)value ? 1 : 0;
                        MMALCheck(MMALUtil.mmal_port_parameter_set_boolean(port.Ptr, (uint)key, i), "Unable to set boolean value");
                        break;
                    case "MMAL_PARAMETER_UINT64_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_uint64(port.Ptr, (uint)key, (ulong)value), "Unable to set ulong value");
                        break;
                    case "MMAL_PARAMETER_INT64_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_int64(port.Ptr, (uint)key, (long)value), "Unable to set long value");
                        break;
                    case "MMAL_PARAMETER_UINT32_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_uint32(port.Ptr, (uint)key, (uint)value), "Unable to set uint value");
                        break;
                    case "MMAL_PARAMETER_INT32_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_int32(port.Ptr, (uint)key, (int)value), "Unable to set int value");
                        break;
                    case "MMAL_PARAMETER_RATIONAL_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_rational(port.Ptr, (uint)key, (MMAL_RATIONAL_T)value), "Unable to set rational value");
                        break;
                    case "MMAL_PARAMETER_STRING_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_string(port.Ptr, (uint)key, (string)value), "Unable to set rational value");
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch
            {
                MMALLog.Logger.Warn($"Unable to set port parameter {t.ParamName}");
                throw;
            }
        }

        internal static void SetImageCapture(this MMALPortImpl port, bool enable)
        {
            port.SetParameter(MMAL_PARAMETER_CAPTURE, enable);
        }

        public static bool GetRawCapture(this MMALPortImpl port)
        {
            return port.GetParameter(MMAL_PARAMETER_ENABLE_RAW_CAPTURE);
        }

        internal static void SetRawCapture(this MMALPortImpl port, bool raw)
        {
            port.SetParameter(MMAL_PARAMETER_ENABLE_RAW_CAPTURE, raw);
        }

        internal static unsafe void SetStereoMode(this MMALPortImpl port, StereoMode mode)
        {
            MMAL_PARAMETER_STEREOSCOPIC_MODE_T stereo = new MMAL_PARAMETER_STEREOSCOPIC_MODE_T(new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_STEREOSCOPIC_MODE, Marshal.SizeOf<MMAL_PARAMETER_STEREOSCOPIC_MODE_T>()),
                mode.Mode, mode.Decimate, mode.SwapEyes);

            MMALCheck(MMALPort.mmal_port_parameter_set(port.Ptr, &stereo.hdr), "Unable to set Stereo mode");
        }

        internal static unsafe int[] GetSupportedEncodings(this MMALPortImpl port)
        {
            MMAL_PARAMETER_ENCODING_T encodings =
                new MMAL_PARAMETER_ENCODING_T(
                    new MMAL_PARAMETER_HEADER_T(MMALParametersCommon.MMAL_PARAMETER_SUPPORTED_ENCODINGS,
                        Marshal.SizeOf<MMAL_PARAMETER_ENCODING_T>()), new[]{0});

            MMALCheck(MMALPort.mmal_port_parameter_get(port.Ptr, &encodings.hdr), "Unable to get supported encodings");

            return encodings.Value;
        }

        internal static void CheckSupportedEncoding(this MMALPortImpl port, MMALEncoding encoding)
        {
            var encodings = port.GetSupportedEncodings();

            if (!encodings.Any(c => c == encoding.EncodingVal))
            {
                throw new PiCameraError("Unsupported encoding type for this port");    
            }
        }

        internal static bool RgbOrderFixed(this MMALPortImpl port)
        {
            int newFirmware = 0;
            var encodings = port.GetSupportedEncodings();

            foreach (int enc in encodings)
            {
                if (enc == MMALUtil.MMAL_FOURCC("BGR3"))
                {
                    break;
                }
                if (enc == MMALUtil.MMAL_FOURCC("RGB3"))
                {
                    newFirmware = 1;
                }
            }

            return newFirmware == 1;
        }
    }
}
