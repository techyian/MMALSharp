using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharPicam
{
    public static class MMALPortHelper
    {
        public static Dictionary<int, Type> ParameterHelper = new Dictionary<int, Type> {
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
            { MMALParametersClock.MMAL_PARAMETER_CLOCK_ACTIVE,                   typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersClock.MMAL_PARAMETER_CLOCK_ENABLE_BUFFER_INFO,       typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersClock.MMAL_PARAMETER_CLOCK_FRAME_RATE,               typeof(MMAL_PARAMETER_RATIONAL_T)},
            { MMALParametersClock.MMAL_PARAMETER_CLOCK_SCALE,                    typeof(MMAL_PARAMETER_RATIONAL_T)},
            { MMALParametersClock.MMAL_PARAMETER_CLOCK_TIME,                     typeof(MMAL_PARAMETER_INT64_T)},
            { MMALParametersCamera.MMAL_PARAMETER_CONTRAST,                       typeof(MMAL_PARAMETER_RATIONAL_T)},
            { MMALParametersCamera.MMAL_PARAMETER_DPF_CONFIG,                     typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersCamera.MMAL_PARAMETER_ENABLE_RAW_CAPTURE,             typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCamera.MMAL_PARAMETER_EXIF_DISABLE,                   typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCamera.MMAL_PARAMETER_EXPOSURE_COMP,                  typeof(MMAL_PARAMETER_INT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_EXTRA_BUFFERS,                  typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersCamera.MMAL_PARAMETER_FLASH_REQUIRED,                 typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCamera.MMAL_PARAMETER_FRAME_RATE,                     typeof(MMAL_PARAMETER_RATIONAL_T)}, //actually MMAL_PARAMETER_FRAME_RATE_T but this only contains a rational anyway...                    
            { MMALParametersVideo.MMAL_PARAMETER_INTRAPERIOD,                    typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersCamera.MMAL_PARAMETER_ISO,                            typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersCamera.MMAL_PARAMETER_JPEG_ATTACH_LOG,                typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCamera.MMAL_PARAMETER_JPEG_Q_FACTOR,                  typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersCommon.MMAL_PARAMETER_LOCKSTEP_ENABLE,                typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_MB_ROWS_PER_SLICE,              typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_MINIMISE_FRAGMENTATION,         typeof(MMAL_PARAMETER_BOOLEAN_T)},
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
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ALIGN_HORIZ,              typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ALIGN_VERT,               typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_BIT_RATE,                 typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersCamera.MMAL_PARAMETER_VIDEO_DENOISE,                  typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_DROPPABLE_PFRAMES,        typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_FRAME_LIMIT_BITS,  typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INITIAL_QUANT,     typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INLINE_HEADER,     typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_INLINE_VECTORS,    typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_MAX_QUANT,         typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_MIN_QUANT,         typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_PEAK_RATE,         typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_QP_P,              typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_RC_SLICE_DQUANT,   typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_SEI_ENABLE,        typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_SPS_TIMINGS,       typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_FRAME_RATE,               typeof(MMAL_PARAMETER_RATIONAL_T)}, //# actually MMAL_PARAMETER_FRAME_RATE_T but this only contains a rational anyway...
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_IMMUTABLE_INPUT,          typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_INTERPOLATE_TIMESTAMPS,   typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_MAX_NUM_CALLBACKS,        typeof(MMAL_PARAMETER_UINT32_T)},
            { MMALParametersVideo.MMAL_PARAMETER_VIDEO_REQUEST_I_FRAME,          typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCamera.MMAL_PARAMETER_VIDEO_STABILISATION,            typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY,                      typeof(MMAL_PARAMETER_BOOLEAN_T)},
            { MMALParametersCamera.MMAL_PARAMETER_JPEG_RESTART_INTERVAL,          typeof(MMAL_PARAMETER_UINT32_T)},   
        };
            

    }

    public unsafe class MMALPortImpl : MMALObject
    {
        public MMAL_PORT_T* Ptr { get; set; }
        public MMAL_COMPONENT_T* Comp { get; set; }
        public string Name { get; set; }
        public bool Enabled {
            get {
                return (*Ptr).isEnabled == 1;
            }
        }
        public uint BufferNumMin {
            get {
                return (*Ptr).bufferNumMin;
            }
        }
        public uint BufferSizeMin {
            get {
                return (*Ptr).bufferSizeMin;
            }
        }
        public uint BufferAlignmentMin {
            get {
                return (*Ptr).bufferAlignmentMin;
            }
        }
        public uint BufferNumRecommended {
            get {
                return (*Ptr).bufferNumRecommended;
            }
        }
        public uint BufferSizeRecommended {
            get {
                return (*Ptr).bufferSizeRecommended;
            }
        }
        public uint BufferNum {
            get {
                return (*Ptr).bufferNum;
            }
            set {
                Ptr->bufferNum = value;
            }
        }
        public uint BufferSize {
            get {
                return (*Ptr).bufferSize;
            }
            set {
                Ptr->bufferSize = value;
            }
        }

        public Action<MMALBufferImpl> Callback { get; set; }
        
        public MMALPortImpl(MMAL_PORT_T* ptr)
        {
            this.Ptr = ptr;
            this.Comp = (*ptr).component;
            this.Name = Marshal.PtrToStringAnsi((IntPtr)((*ptr).name));          
        }

        public void EnablePort(Action<MMALBufferImpl> callback)
        {            
            this.Callback = callback;

            var nativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(NativePortCallback);

            if (!Enabled)
                MMALCheck(MMALPort.mmal_port_enable(this.Ptr, nativeCallback), "Unable to enable port.");
        }

        public void DisablePort()
        {
            if(Enabled)
                MMALCheck(MMALPort.mmal_port_disable(this.Ptr), "Unable to disable port.");
        }

        public void Commit()
        {
            MMALCheck(MMALPort.mmal_port_format_commit(this.Ptr), "Unable to commit port changes.");
        }

        public void ShallowCopy(MMAL_ES_FORMAT_T* destination)
        {
            MMALFormat.mmal_format_copy(destination, (*this.Ptr).format);
        }

        public void FullCopy(MMAL_ES_FORMAT_T* destination)
        {
            MMALFormat.mmal_format_full_copy(destination, (*this.Ptr).format);
        }

        public dynamic GetParameter(int key)
        {
            var t = MMALPortHelper.ParameterHelper.Where(c => c.Key == key).FirstOrDefault();
            
            switch(t.Value.Name)
            {
                case "MMAL_PARAMETER_BOOLEAN_T":
                    int boolVal = 0;
                    MMALCheck(MMALUtil.mmal_port_parameter_get_boolean(this.Ptr, (uint)key, ref boolVal), "Unable to get boolean value");
                    return boolVal;
                case "MMAL_PARAMETER_UINT64_T":
                    ulong ulongVal = 0UL;
                    MMALCheck(MMALUtil.mmal_port_parameter_get_uint64(this.Ptr, (uint)key, ref ulongVal), "Unable to get ulong value");
                    return ulongVal;
                case "MMAL_PARAMETER_INT64_T":
                    long longVal = 0U;
                    MMALCheck(MMALUtil.mmal_port_parameter_get_int64(this.Ptr, (uint)key, ref longVal), "Unable to get long value");
                    return longVal;
                case "MMAL_PARAMETER_UINT32_T":
                    uint uintVal = 0U;
                    MMALCheck(MMALUtil.mmal_port_parameter_get_uint32(this.Ptr, (uint)key, ref uintVal), "Unable to get uint value");
                    return uintVal;
                case "MMAL_PARAMETER_INT32_T":
                    int intVal = 0;
                    MMALCheck(MMALUtil.mmal_port_parameter_get_int32(this.Ptr, (uint)key, ref intVal), "Unable to get int value");
                    return intVal;
                case "MMAL_PARAMETER_RATIONAL_T":
                    MMAL_RATIONAL_T ratVal = new MMAL_RATIONAL_T();
                    MMALCheck(MMALUtil.mmal_port_parameter_get_rational(this.Ptr, (uint)key, ref ratVal), "Unable to get rational value");
                    return ratVal;
                default:
                    throw new NotSupportedException();
            }            
        }

        public void SetParameter(int key, dynamic value)
        {
            var t = MMALPortHelper.ParameterHelper.Where(c => c.Key == key).FirstOrDefault();

            switch (t.Value.Name)
            {
                case "MMAL_PARAMETER_BOOLEAN_T":                    
                    MMALCheck(MMALUtil.mmal_port_parameter_set_boolean(this.Ptr, (uint)key, (int) value), "Unable to get boolean value");
                    break;
                case "MMAL_PARAMETER_UINT64_T":                    
                    MMALCheck(MMALUtil.mmal_port_parameter_set_uint64(this.Ptr, (uint)key, (ulong) value), "Unable to get ulong value");
                    break;
                case "MMAL_PARAMETER_INT64_T":                    
                    MMALCheck(MMALUtil.mmal_port_parameter_set_int64(this.Ptr, (uint)key, (long) value), "Unable to get long value");
                    break;
                case "MMAL_PARAMETER_UINT32_T":                    
                    MMALCheck(MMALUtil.mmal_port_parameter_set_uint32(this.Ptr, (uint)key, (uint) value), "Unable to get uint value");
                    break;
                case "MMAL_PARAMETER_INT32_T":                    
                    MMALCheck(MMALUtil.mmal_port_parameter_set_int32(this.Ptr, (uint)key, (int) value), "Unable to get int value");
                    break;
                case "MMAL_PARAMETER_RATIONAL_T":                    
                    MMALCheck(MMALUtil.mmal_port_parameter_set_rational(this.Ptr, (uint)key, (MMAL_RATIONAL_T) value), "Unable to get rational value");
                    break;
                case "MMAL_PARAMETER_STRING_T":
                    MMALCheck(MMALUtil.mmal_port_parameter_set_string(this.Ptr, (uint)key, (string) value), "Unable to get rational value");
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public void NativePortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            var bufferImpl = new MMALBufferImpl(buffer);

            this.Callback(bufferImpl);
        }

    }
}
