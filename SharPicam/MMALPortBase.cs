using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SharPicam.MMALCallerHelper;

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

        
    }

    public unsafe class MMALPortBase : MMALObject
    {
        public MMAL_PORT_T* Ptr { get; set; }
        public MMAL_COMPONENT_T* Comp { get; set; }
        public MMALComponentBase ComponentReference { get; set; }
        public string Name { get; set; }
        public string ObjName { get; set; }
        public bool Enabled
        {
            get
            {
                return this.Ptr->isEnabled == 1;
            }
        }
        public uint BufferNumMin
        {
            get
            {
                return this.Ptr->bufferNumMin;
            }
        }
        public uint BufferSizeMin
        {
            get
            {
                return this.Ptr->bufferSizeMin;
            }
        }
        public uint BufferAlignmentMin
        {
            get
            {
                return this.Ptr->bufferAlignmentMin;
            }
        }
        public uint BufferNumRecommended
        {
            get
            {
                return this.Ptr->bufferNumRecommended;
            }
        }
        public uint BufferSizeRecommended
        {
            get
            {
                return this.Ptr->bufferSizeRecommended;
            }
        }
        public uint BufferNum
        {
            get
            {
                return this.Ptr->bufferNum;
            }
            set
            {
                this.Ptr->bufferNum = value;
            }
        }
        public uint BufferSize
        {
            get
            {
                return this.Ptr->bufferSize;
            }
            set
            {
                this.Ptr->bufferSize = value;
            }
        }
        public MMAL_ES_FORMAT_T Format
        {
            get
            {
                return *this.Ptr->format;
            }
        }

        public AutoResetEvent ResetEvent = new AutoResetEvent(false);
        public int Triggered = 0;
        public Action<MMALBufferImpl> Callback { get; set; }
        public MMALPort.MMAL_PORT_BH_CB_T NativeCallback { get; set; }

        public MMALPortBase(MMAL_PORT_T* ptr, MMALComponentBase comp)
        {
            this.Ptr = ptr;
            this.Comp = ptr->component;
            this.Name = Marshal.PtrToStringAnsi((IntPtr)(this.Ptr->name));
            this.ComponentReference = comp;
        }


        public void EnablePort(Action<MMALBufferImpl> callback)
        {
            this.Callback = callback;

            this.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(NativePortCallback);

            IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);
            
            Console.WriteLine("Enabling port.");

            if (callback == null)
            {
                Console.WriteLine("Callback null");
                MMALCheck(MMALPort.mmal_port_enable(this.Ptr, IntPtr.Zero), "Unable to enable port.");
            }                
            else
                MMALCheck(MMALPort.mmal_port_enable(this.Ptr, ptrCallback), "Unable to enable port.");

        }

        public void DisablePort()
        {
            if (Enabled)
                MMALCheck(MMALPort.mmal_port_disable(this.Ptr), "Unable to disable port.");
        }

        public void Commit()
        {
            MMALCheck(MMALPort.mmal_port_format_commit(this.Ptr), "Unable to commit port changes.");
        }

        public void ShallowCopy(MMAL_ES_FORMAT_T* destination)
        {
            MMALFormat.mmal_format_copy(destination, this.Ptr->format);
        }

        public void FullCopy(MMAL_ES_FORMAT_T* destination)
        {
            MMALFormat.mmal_format_full_copy(destination, this.Ptr->format);
        }

        public void SendBuffer(MMAL_BUFFER_HEADER_T* buffer)
        {
            MMALCheck(MMALPort.mmal_port_send_buffer(this.Ptr, buffer), "Unable to send buffer header.");
        }
            
        public virtual void NativePortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {            
        }

    }
}
