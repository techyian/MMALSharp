using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MMALSharp.Native.MMALParameters;

namespace MMALSharp
{
    public static class MMALCameraConfigImpl
    {
        public static MMALCameraConfig Config { get; set; }
    }

    public class MMALCameraConfig
    {        
        public bool Debug { get; set; }
        public double Sharpness { get; set; }
        public double Contrast { get; set; }
        public double Brightness { get; set; } = 50;
        public double Saturation { get; set; }
        public int ISO { get; set; }
        public bool VideoStabilisation { get; set; } = true;
        public int ExposureCompensation { get; set; } = (int)MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_AUTO;
        public MMAL_PARAM_EXPOSUREMODE_T ExposureMode { get; set; } = MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_AUTO;
        public MMAL_PARAM_EXPOSUREMETERINGMODE_T ExposureMeterMode { get; set; } = MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE;
        public MMAL_PARAM_AWBMODE_T AwbMode { get; set; } = MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_OFF;
        public MMAL_PARAM_IMAGEFX_T ImageEffect { get; set; } = MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_NONE;
        public ColourEffects Effects { get; set; } = new ColourEffects();
        public int Rotation { get; set; }
        public MMAL_PARAM_MIRROR_T Flips { get; set; } = MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_NONE;
        public Crop Crop { get; set; } = new Crop();
        public int ShutterSpeed { get; set; }
        public int AwbGainsR { get; set; }
        public int AwbGainsB { get; set; }
        public MMAL_PARAMETER_DRC_STRENGTH_T DrcLevel { get; set; } = MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_OFF;
        public bool StatsPass { get; set; }
        public bool EnableAnnotate { get; set; }    
        public AnnotateImage Annotate { get; set; }
        public StereoMode StereoMode { get; set; } = new StereoMode();
        public bool SetChangeEventRequest { get; set; }

        // Camera preview port specific properties
        public int PreviewEncoding { get; set; } = MMALEncodings.MMAL_ENCODING_OPAQUE;

        private int previewWidth;
        public int PreviewWidth {
            get
            {
                return this.previewWidth;
            }
            set
            {
                this.previewWidth = MMALUtil.VCOS_ALIGN_UP(value, 32);
            }                
        }

        private int previewHeight;
        public int PreviewHeight
        {
            get
            {
                return this.previewHeight;
            }
            set
            {
                this.previewHeight = MMALUtil.VCOS_ALIGN_UP(value, 16);
            }
        }

        // Camera video port specific properties


        // Camera still port specific properties
        public int StillEncoding { get; set; } = MMALEncodings.MMAL_ENCODING_OPAQUE;
        public int StillEncodingSubFormat { get; set; } = MMALEncodings.MMAL_ENCODING_I420;

        private int stillWidth;
        public int StillWidth
        {
            get
            {
                return this.stillWidth;
            }
            set
            {
                this.stillWidth = MMALUtil.VCOS_ALIGN_UP(value, 32);
            }
        }

        private int stillHeight;
        public int StillHeight
        {
            get
            {
                return this.stillHeight;
            }
            set
            {
                this.stillHeight = MMALUtil.VCOS_ALIGN_UP(value, 16);
            }
        }        
    }
    
    public class ColourEffects
    {
        public int Enable { get; set; }
        public uint U { get; set; } = 128;
        public uint V { get; set; } = 128;
        
    }

    public class Crop
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }

    public class StereoMode
    {
        public MMAL_STEREOSCOPIC_MODE_T Mode { get; set; } = MMAL_STEREOSCOPIC_MODE_T.MMAL_STEREOSCOPIC_MODE_NONE;
        public int Decimate { get; set; }
        public int SwapEyes { get; set; }
    }
    
    public class ExifTag
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Timelapse
    {
        public TimelapseMode Mode { get; set; }
        public int Value { get; set; }
    }

    public class AnnotateImage
    {
        public string CustomText { get; set; }
        public int TextSize { get; set; }
        public int TextColour { get; set; } = -1;
        public int BgColour { get; set; } = -1;
        public bool ShowShutterSettings { get; set; }
        public bool ShowGainSettings { get; set; }
        public bool ShowLensSettings { get; set; }
        public bool ShowCafSettings { get; set; }
        public bool ShowMotionSettings { get; set; }
        public bool ShowFrameNumber { get; set; }
        public bool ShowBlackBackground { get; set; }
        public bool ShowDateText { get; set; }
        public bool ShowTimeText { get; set; }
    }

    public enum TimelapseMode
    {
        Millisecond,
        Second,
        Minute
    }

}
