using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharPicam
{
    public static class MMALPortHelper
    {
        public static Dictionary<int, string> ReflectedParameterHelper = new Dictionary<int, string>()
        {
            {   MMALParametersCamera.MMAL_PARAMETER_ALGORITHM_CONTROL,              "AlgorithmControl"},
            {   MMALParametersAudio.MMAL_PARAMETER_AUDIO_LATENCY_TARGET,           "AudioLatencyTarget"},
            {   MMALParametersCamera.MMAL_PARAMETER_AWB_MODE,                       "AWBMode"},
            {   MMALParametersCommon.MMAL_PARAMETER_BUFFER_REQUIREMENTS,            "BufferRequirements"},
            {   MMALParametersCamera.MMAL_PARAMETER_CAMERA_CLOCKING_MODE,           "CameraClockingMode"},
            {   MMALParametersCamera.MMAL_PARAMETER_CAMERA_CONFIG,                  "CameraConfig"},
            {   MMALParametersCamera.MMAL_PARAMETER_CAMERA_INTERFACE,               "CameraInterface"},
            {   MMALParametersCamera.MMAL_PARAMETER_CAMERA_RX_CONFIG,               "CameraRXConfig"},
            {   MMALParametersCamera.MMAL_PARAMETER_CAMERA_RX_TIMING,               "CameraRXTiming"},
            {   MMALParametersCamera.MMAL_PARAMETER_CAMERA_SETTINGS,                "CameraSettings"},
            {   MMALParametersCamera.MMAL_PARAMETER_CAMERA_USE_CASE,                "CameraUseCase"},
            {   MMALParametersCamera.MMAL_PARAMETER_CAPTURE_MODE,                   "CaptureMode"},
            {   MMALParametersCamera.MMAL_PARAMETER_CAPTURE_STATUS,                 "CaptureStatus"},
            {   MMALParametersCommon.MMAL_PARAMETER_CHANGE_EVENT_REQUEST,           "ChangeEventRequest"},
            {   MMALParametersClock.MMAL_PARAMETER_CLOCK_DISCONT_THRESHOLD,        "DiscontThreshold"},
            {   MMALParametersClock.MMAL_PARAMETER_CLOCK_LATENCY,                  "ClockLatency"},
            {   MMALParametersClock.MMAL_PARAMETER_CLOCK_REQUEST_THRESHOLD,        "ClockRequestThreshold"},
            {   MMALParametersClock.MMAL_PARAMETER_CLOCK_UPDATE_THRESHOLD,         "ClockUpdateThreshold"},
            {   MMALParametersCamera.MMAL_PARAMETER_COLOUR_EFFECT,                  "ColourFX"},
            {   MMALParametersCommon.MMAL_PARAMETER_CORE_STATISTICS,                "CoreStatistics"},
            {   MMALParametersCamera.MMAL_PARAMETER_CUSTOM_AWB_GAINS,               "AWBGains"},
            {   MMALParametersVideo.MMAL_PARAMETER_DISPLAYREGION,                  "DisplayRegion"},
            {   MMALParametersCamera.MMAL_PARAMETER_DYNAMIC_RANGE_COMPRESSION,      "DRC"},
            {   MMALParametersCamera.MMAL_PARAMETER_EXIF,                           "Exif"},
            {   MMALParametersCamera.MMAL_PARAMETER_EXP_METERING_MODE,              "ExposureMeteringMode"},
            {   MMALParametersCamera.MMAL_PARAMETER_EXPOSURE_MODE,                  "ExposureMode"},
            {   MMALParametersCamera.MMAL_PARAMETER_FIELD_OF_VIEW,                  "FieldOfView"},
            {   MMALParametersCamera.MMAL_PARAMETER_FLASH,                          "Flash"},
            {   MMALParametersCamera.MMAL_PARAMETER_FLASH_SELECT,                   "FlashSelect"},
            {   MMALParametersCamera.MMAL_PARAMETER_FLICKER_AVOID,                  "FlickerAvoid"},
            {   MMALParametersCamera.MMAL_PARAMETER_FOCUS,                          "Focus"},
            {   MMALParametersCamera.MMAL_PARAMETER_FOCUS_REGIONS,                  "FocusRegions"},
            {   MMALParametersCamera.MMAL_PARAMETER_FOCUS_STATUS,                   "FocusStatus"},
            {   MMALParametersCamera.MMAL_PARAMETER_FPS_RANGE,                      "FPSRange"},
            {   MMALParametersCamera.MMAL_PARAMETER_IMAGE_EFFECT,                   "ImageFX"},
            {   MMALParametersCamera.MMAL_PARAMETER_IMAGE_EFFECT_PARAMETERS,        "ImageFXParameters"},
            {   MMALParametersCamera.MMAL_PARAMETER_INPUT_CROP,                     "InputCrop"},
            {   MMALParametersCommon.MMAL_PARAMETER_LOGGING,                        "Logging"},
            {   MMALParametersCommon.MMAL_PARAMETER_MEM_USAGE,                      "MemUsage"},
            {   MMALParametersVideo.MMAL_PARAMETER_NALUNITFORMAT,                  "VideoNALUnitFormat"},
            {   MMALParametersCamera.MMAL_PARAMETER_MIRROR,                         "Mirror"},
            {   MMALParametersCamera.MMAL_PARAMETER_PRIVACY_INDICATOR,              "PrivacyIndicator"},
            {   MMALParametersVideo.MMAL_PARAMETER_PROFILE,                        "VideoProfile"},
            {   MMALParametersVideo.MMAL_PARAMETER_RATECONTROL,                    "VideoRateControl"},
            {   MMALParametersCamera.MMAL_PARAMETER_REDEYE,                         "RedEye"},
            {   MMALParametersCommon.MMAL_PARAMETER_SEEK,                           "Seek"},
            {   MMALParametersCamera.MMAL_PARAMETER_SENSOR_INFORMATION,             "SensorInformation"},
            {   MMALParametersCommon.MMAL_PARAMETER_STATISTICS,                     "Statistics"},
            {   MMALParametersCamera.MMAL_PARAMETER_STEREOSCOPIC_MODE,              "StereoscopicMode"},
            {   MMALParametersCommon.MMAL_PARAMETER_SUPPORTED_ENCODINGS,            "Encoding"},
            {   MMALParametersVideo.MMAL_PARAMETER_SUPPORTED_PROFILES,             "VideoProfile"},
            {   MMALParametersCamera.MMAL_PARAMETER_THUMBNAIL_CONFIGURATION,        "ThumbnailConfig"},
            {   MMALParametersCommon.MMAL_PARAMETER_URI,                            "URI"},
            {   MMALParametersCamera.MMAL_PARAMETER_USE_STC,                        "CameraSTCMode"},
            {   MMALParametersVideo.MMAL_PARAMETER_VIDEO_EEDE_ENABLE,              "VideoEEDEEnable"},
            {   MMALParametersVideo.MMAL_PARAMETER_VIDEO_EEDE_LOSSRATE,            "VideoEEDELossRate"},
            {   MMALParametersVideo.MMAL_PARAMETER_VIDEO_ENCODE_RC_MODEL,          "VideoEncodeRCModel"},
            {   MMALParametersVideo.MMAL_PARAMETER_VIDEO_INTERLACE_TYPE,           "VideoInterlaceType"},
            {   MMALParametersVideo.MMAL_PARAMETER_VIDEO_INTRA_REFRESH,            "VideoIntraRefresh"},
            {   MMALParametersVideo.MMAL_PARAMETER_VIDEO_LEVEL_EXTENSION,          "VideoLevelExtension"},
            {   MMALParametersVideo.MMAL_PARAMETER_VIDEO_RENDER_STATS,             "VideoRenderStats"},
            {   MMALParametersCamera.MMAL_PARAMETER_ZERO_SHUTTER_LAG,               "ZeroShutterLag"},
            {   MMALParametersCamera.MMAL_PARAMETER_ZOOM,                           "ScaleFactor"}
        };

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
        public MMAL_ES_FORMAT_T Format
        {
            get
            {
                return *(*Ptr).format;
            }
        }

        public CountdownEvent CountdownEvent = new CountdownEvent(1);
        public Action<MMALBufferImpl> Callback { get; set; }
        public MMALPort.MMAL_PORT_BH_CB_T NativeCallback { get; set; }

        public MMALPortImpl(MMAL_PORT_T* ptr)
        {
            this.Ptr = ptr;
            this.Comp = (*ptr).component;
            this.Name = Marshal.PtrToStringAnsi((IntPtr)((*ptr).name));          
        }

        public void EnablePort(Action<MMALBufferImpl> callback)
        {            
            this.Callback = callback;

            this.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(NativePortCallback);

            if (!Enabled)
                Console.WriteLine("Enabling port.");
                MMALCheck(MMALPort.mmal_port_enable(this.Ptr, this.NativeCallback), "Unable to enable port.");
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

        public void SendBuffer(MMAL_BUFFER_HEADER_T* buffer)
        {
            MMALCheck(MMALPort.mmal_port_send_buffer(this.Ptr, buffer), "Unable to send buffer header.");
        }

        public dynamic GetParameter(int key)
        {
            try
            {
                var t = MMALPortHelper.ParameterHelper.Where(c => c.Key == key).FirstOrDefault();

                switch (t.Value.Name)
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
            catch
            {
                Console.WriteLine("Unable to get port parameter");
                return null;
            }                  
        }

        public void SetParameter(int key, dynamic value)
        {
            Console.WriteLine("Setting parameter");
            try
            {
                var t = MMALPortHelper.ParameterHelper.Where(c => c.Key == key).FirstOrDefault();

                switch (t.Value.Name)
                {
                    case "MMAL_PARAMETER_BOOLEAN_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_boolean(this.Ptr, (uint)key, (int)value), "Unable to get boolean value");
                        break;
                    case "MMAL_PARAMETER_UINT64_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_uint64(this.Ptr, (uint)key, (ulong)value), "Unable to get ulong value");
                        break;
                    case "MMAL_PARAMETER_INT64_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_int64(this.Ptr, (uint)key, (long)value), "Unable to get long value");
                        break;
                    case "MMAL_PARAMETER_UINT32_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_uint32(this.Ptr, (uint)key, (uint)value), "Unable to get uint value");
                        break;
                    case "MMAL_PARAMETER_INT32_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_int32(this.Ptr, (uint)key, (int)value), "Unable to get int value");
                        break;
                    case "MMAL_PARAMETER_RATIONAL_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_rational(this.Ptr, (uint)key, (MMAL_RATIONAL_T)value), "Unable to get rational value");
                        break;
                    case "MMAL_PARAMETER_STRING_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_string(this.Ptr, (uint)key, (string)value), "Unable to get rational value");
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch
            {
                SetGenericParameter(key, value);
            }
        }

        public void SetCameraConfig(MMAL_PARAMETER_CAMERA_CONFIG_T value)
        {
            //MMALCheck(MMALPort.mmal_port_parameter_set(this.Ptr, &value.hdr), "Unable to set parameter.");
        }

        public void SetGenericParameter(int key, dynamic value)
        {            
            Console.WriteLine("Id " + value.hdr.id);

            Console.WriteLine("Enabled " + this.Enabled);
            Console.WriteLine("Num " + this.BufferNum);
            Console.WriteLine("Size " + this.BufferSize);

            Console.WriteLine("Setting generic parameter");
            
            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_HEADER_T>());

            var val = value;
            var hdr = (MMAL_PARAMETER_HEADER_T)val.hdr;
            Marshal.StructureToPtr(hdr, ptr, false);
                        
            Console.WriteLine("About to set parameter");
                        
            MMALCheck(MMALPort.mmal_port_parameter_set(this.Ptr, ref hdr), "Unable to set parameter.");
            Marshal.FreeHGlobal(ptr);
        }

        public void NativePortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {            
            var bufferImpl = new MMALBufferImpl(buffer);

            Console.WriteLine("Inside native port callback");

            Console.WriteLine("Passed port address " + ((IntPtr)port).ToString());

            bufferImpl.Release();
            
            this.Callback(bufferImpl);

            //CountdownEvent.Signal();
        }

    }
}
