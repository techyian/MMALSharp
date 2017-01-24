using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp
{
    internal unsafe static class MMALParameterHelpers
    {
        public static Dictionary<int, Type> ParameterHelper = new Dictionary<int, Type>
        {
            { MMALParametersCamera.MMAL_PARAMETER_ANTISHAKE,                      typeof(MMAL_PARAMETER_BOOLEAN_T) },
            { MMALParametersCamera.MMAL_PARAMETER_BRIGHTNESS,                     typeof(MMAL_PARAMETER_RATIONAL_T) },
            { MMALParametersCommon.MMAL_PARAMETER_BUFFER_FLAG_FILTER,             typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersCamera.MMAL_PARAMETER_CAMERA_BURST_CAPTURE,           typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCamera.MMAL_PARAMETER_CAMERA_CUSTOM_SENSOR_CONFIG,    typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersCamera.MMAL_PARAMETER_CAMERA_MIN_ISO,                 typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersCamera.MMAL_PARAMETER_CAMERA_NUM,                     typeof(MMAL_PARAMETER_INT32_T)},
            { MMALParametersCamera.MMAL_PARAMETER_CAPTURE_EXPOSURE_COMP,          typeof(MMAL_PARAMETER_INT32_T)},
            { MMALParametersCamera.MMAL_PARAMETER_CAPTURE,                        typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCamera.MMAL_PARAMETER_CAPTURE_STATS_PASS,             typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersClock.MMAL_PARAMETER_CLOCK_ACTIVE,                    typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersClock.MMAL_PARAMETER_CLOCK_ENABLE_BUFFER_INFO,        typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersClock.MMAL_PARAMETER_CLOCK_FRAME_RATE,                typeof(MMAL_PARAMETER_RATIONAL_T)},
            { MMALParametersClock.MMAL_PARAMETER_CLOCK_SCALE,                     typeof(MMAL_PARAMETER_RATIONAL_T)},
            { MMALParametersClock.MMAL_PARAMETER_CLOCK_TIME,                      typeof(MMAL_PARAMETER_INT64_T)},
            { MMALParametersCamera.MMAL_PARAMETER_CONTRAST,                       typeof(MMAL_PARAMETER_RATIONAL_T)},
            { MMALParametersCamera.MMAL_PARAMETER_DPF_CONFIG,                     typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersCamera.MMAL_PARAMETER_ENABLE_RAW_CAPTURE,             typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCamera.MMAL_PARAMETER_EXIF_DISABLE,                   typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCamera.MMAL_PARAMETER_EXPOSURE_COMP,                  typeof(MMAL_PARAMETER_INT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_EXTRA_BUFFERS,                   typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersCamera.MMAL_PARAMETER_FLASH_REQUIRED,                 typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCamera.MMAL_PARAMETER_FRAME_RATE,                     typeof(MMAL_PARAMETER_RATIONAL_T)},
            { MMALParametersVideo.MMAL_PARAMETER_INTRAPERIOD,                     typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersCamera.MMAL_PARAMETER_ISO,                            typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersCamera.MMAL_PARAMETER_JPEG_ATTACH_LOG,                typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCamera.MMAL_PARAMETER_JPEG_Q_FACTOR,                  typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersCommon.MMAL_PARAMETER_LOCKSTEP_ENABLE,                typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_MB_ROWS_PER_SLICE,               typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_MINIMISE_FRAGMENTATION,          typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCommon.MMAL_PARAMETER_NO_IMAGE_PADDING,               typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCommon.MMAL_PARAMETER_POWERMON_ENABLE,                typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCamera.MMAL_PARAMETER_ROTATION,                       typeof(MMAL_PARAMETER_INT32_T)},
            { MMALParametersCamera.MMAL_PARAMETER_SATURATION,                     typeof(MMAL_PARAMETER_RATIONAL_T)},
            { MMALParametersCamera.MMAL_PARAMETER_SHARPNESS,                      typeof(MMAL_PARAMETER_RATIONAL_T)},
            { MMALParametersCamera.MMAL_PARAMETER_SHUTTER_SPEED,                  typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersCamera.MMAL_PARAMETER_STILLS_DENOISE,                 typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCamera.MMAL_PARAMETER_SW_SATURATION_DISABLE,          typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCamera.MMAL_PARAMETER_SW_SHARPEN_DISABLE,             typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCommon.MMAL_PARAMETER_SYSTEM_TIME,                    typeof(MMAL_PARAMETER_UINT64_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ALIGN_HORIZ,               typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ALIGN_VERT,                typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_BIT_RATE,                  typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersCamera.MMAL_PARAMETER_VIDEO_DENOISE,                  typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_DROPPABLE_PFRAMES,         typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_FRAME_LIMIT_BITS,   typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INITIAL_QUANT,      typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INLINE_HEADER,      typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INLINE_VECTORS,     typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_MAX_QUANT,          typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_MIN_QUANT,          typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_PEAK_RATE,          typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_QP_P,               typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_RC_SLICE_DQUANT,    typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_SEI_ENABLE,         typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_SPS_TIMINGS,        typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_FRAME_RATE,                typeof(MMAL_PARAMETER_RATIONAL_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_IMMUTABLE_INPUT,           typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_INTERPOLATE_TIMESTAMPS,    typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_MAX_NUM_CALLBACKS,         typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_REQUEST_I_FRAME,           typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCamera.MMAL_PARAMETER_VIDEO_STABILISATION,            typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY,                      typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCamera.MMAL_PARAMETER_JPEG_RESTART_INTERVAL,          typeof(MMAL_PARAMETER_UINT32_T)},
        };

        public static dynamic GetParameter(int key, MMAL_PORT_T* ptr)
        {
            try
            {
                var t = ParameterHelper.Where(c => c.Key == key).FirstOrDefault();

                switch (t.Value.Name)
                {
                    case "MMAL_PARAMETER_BOOLEAN_T":
                        int boolVal = 0;
                        MMALCheck(MMALUtil.mmal_port_parameter_get_boolean(ptr, (uint)key, ref boolVal), "Unable to get boolean value");
                        return (boolVal == 1) ? true : false;                        
                    case "MMAL_PARAMETER_UINT64_T":
                        ulong ulongVal = 0UL;
                        MMALCheck(MMALUtil.mmal_port_parameter_get_uint64(ptr, (uint)key, ref ulongVal), "Unable to get ulong value");
                        return ulongVal;
                    case "MMAL_PARAMETER_INT64_T":
                        long longVal = 0U;
                        MMALCheck(MMALUtil.mmal_port_parameter_get_int64(ptr, (uint)key, ref longVal), "Unable to get long value");
                        return longVal;
                    case "MMAL_PARAMETER_UINT32_T":
                        uint uintVal = 0U;
                        MMALCheck(MMALUtil.mmal_port_parameter_get_uint32(ptr, (uint)key, ref uintVal), "Unable to get uint value");
                        return uintVal;
                    case "MMAL_PARAMETER_INT32_T":
                        int intVal = 0;
                        MMALCheck(MMALUtil.mmal_port_parameter_get_int32(ptr, (uint)key, ref intVal), "Unable to get int value");
                        return intVal;
                    case "MMAL_PARAMETER_RATIONAL_T":
                        MMAL_RATIONAL_T ratVal = new MMAL_RATIONAL_T();
                        MMALCheck(MMALUtil.mmal_port_parameter_get_rational(ptr, (uint)key, ref ratVal), "Unable to get rational value");
                        return (double)ratVal.num / ratVal.den;                        
                    default:
                        throw new NotSupportedException();
                }
            }
            catch
            {
                Console.WriteLine("Unable to get port parameter");
                return null;
            }
        }

        public static void SetParameter(int key, dynamic value, MMAL_PORT_T* ptr)
        {
            Console.WriteLine("Setting parameter");
            try
            {
                var t = ParameterHelper.Where(c => c.Key == key).FirstOrDefault();

                switch (t.Value.Name)
                {
                    case "MMAL_PARAMETER_BOOLEAN_T":
                        int i = (bool)value ? 1 : 0;                        
                        MMALCheck(MMALUtil.mmal_port_parameter_set_boolean(ptr, (uint)key, i), "Unable to set boolean value");
                        break;
                    case "MMAL_PARAMETER_UINT64_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_uint64(ptr, (uint)key, (ulong)value), "Unable to set ulong value");
                        break;
                    case "MMAL_PARAMETER_INT64_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_int64(ptr, (uint)key, (long)value), "Unable to set long value");
                        break;
                    case "MMAL_PARAMETER_UINT32_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_uint32(ptr, (uint)key, (uint)value), "Unable to set uint value");
                        break;
                    case "MMAL_PARAMETER_INT32_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_int32(ptr, (uint)key, (int)value), "Unable to set int value");
                        break;
                    case "MMAL_PARAMETER_RATIONAL_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_rational(ptr, (uint)key, (MMAL_RATIONAL_T)value), "Unable to set rational value");
                        break;
                    case "MMAL_PARAMETER_STRING_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_string(ptr, (uint)key, (string)value), "Unable to set rational value");
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch
            {
                Console.WriteLine("Something went wrong whilst setting parameter " + key);
            }
        }

        



    }
}
