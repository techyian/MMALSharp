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
        public MMAL_PARAM_AWBMODE_T AwbMode { get; set; } = MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_AUTO;
        public MMAL_PARAM_IMAGEFX_T ImageEffect { get; set; } = MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_NONE;
        public ColourEffects Effects { get; set; } = new ColourEffects();
        public int Rotation { get; set; }
        public MMAL_PARAM_MIRROR_T Flips { get; set; } = MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_NONE;
        public ROI ROI { get; set; } = new ROI();
        public int ShutterSpeed { get; set; }
        public int AwbGainsR { get; set; }
        public int AwbGainsB { get; set; }
        public MMAL_PARAMETER_DRC_STRENGTH_T DrcLevel { get; set; } = MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_OFF;
        public int StatsPass { get; set; }
        public int EnableAnnotate { get; set; }
        public int AnnotateTextSize { get; set; }
        public int AnnotateTextColour { get; set; } = -1;
        public int AnnotateBgColour { get; set; } = -1;
        public StereoMode StereoMode { get; set; } = new StereoMode();
        public bool SetChangeEventRequest { get; set; }

        //Camera preview port specific properties
        public uint PreviewEncoding { get; set; } = MMALEncodings.MMAL_ENCODING_OPAQUE;
        public uint PreviewWidth { get; set; }
        public uint PreviewHeight { get; set; }

        //Camera video port specific properties


        //Camera still port specific properties
        public uint StillEncoding { get; set; } = MMALEncodings.MMAL_ENCODING_OPAQUE;
        public uint StillEncodingSubFormat { get; set; } = MMALEncodings.MMAL_ENCODING_I420;        
        public uint StillWidth { get; set; }
        public uint StillHeight { get; set; }

    }
    
    public class ColourEffects
    {
        public int Enable { get; set; }
        public uint U { get; set; } = 128;
        public uint V { get; set; } = 128;
        
    }

    public class ROI
    {
        public double X { get; set; } = 0.0;
        public double Y { get; set; } = 1.0;        
    }

    public class StereoMode
    {
        public int Mode { get; set; } = (int)MMAL_STEREOSCOPIC_MODE_T.MMAL_STEREOSCOPIC_MODE_NONE;
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

    public enum TimelapseMode
    {
        Millisecond,
        Second,
        Minute
    }

}
