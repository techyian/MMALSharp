using MMALSharp.Native;
using System;
using static MMALSharp.Native.MMALParameters;

namespace MMALSharp
{
    
    public static class MMALCameraConfig
    {        
        public static bool Debug { get; set; }

        /// <summary>
        /// Configure the sharpness of the image
        /// </summary>
        public static double Sharpness { get; set; }

        /// <summary>
        /// Configure the contrast of the image
        /// </summary>  
        public static double Contrast { get; set; }

        /// <summary>
        /// Configure the brightness of the image
        /// </summary>
        public static double Brightness { get; set; } = 50;

        /// <summary>
        /// Configure the saturation of the image
        /// </summary>  
        public static double Saturation { get; set; }

        /// <summary>
        /// Configure the light sensitivity of the sensor
        /// </summary> 
        public static int ISO { get; set; }
        
        /// <summary>
        /// Configure the exposure compensation of the camera. Doing so will produce a lighter/darker image beyond the recommended exposure.
        /// </summary>
        public static int ExposureCompensation { get; set; }

        /// <summary>
        /// Configure the exposure mode used by the camera       
        /// </summary>  
        public static MMAL_PARAM_EXPOSUREMODE_T ExposureMode { get; set; } = MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_AUTO;

        /// <summary>
        /// Configure the exposure metering mode to be used by the camera. The metering mode determines how the camera measures exposure.       
        /// </summary> 
        public static MMAL_PARAM_EXPOSUREMETERINGMODE_T ExposureMeterMode { get; set; } = MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE;

        /// <summary>
        /// Configure the Auto White Balance to be used by the camera        
        /// </summary>
        public static MMAL_PARAM_AWBMODE_T AwbMode { get; set; } = MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_AUTO;

        /// <summary>
        /// Configure any image effects to be used by the camera       
        /// </summary>
        public static MMAL_PARAM_IMAGEFX_T ImageFx { get; set; } = MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_NONE;

        /// <summary>
        /// Allows a user to change the colour of an image, e.g. U = 128, V = 128 will result in a greyscale (monochrome) image.
        /// </summary>
        public static ColourEffects ColourFx { get; set; } = new ColourEffects();

        /// <summary>
        /// Specify the rotation of the image, possible values are 0, 90, 180, 270
        /// </summary>   
        public static int Rotation { get; set; }

        /// <summary>
        /// Specify whether the image should be flipped
        /// </summary>                       
        public static MMAL_PARAM_MIRROR_T Flips { get; set; } = MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_NONE;

        /// <summary>
        /// Zoom in on an image to focus on a region of interest.
        /// </summary>
        public static Zoom ROI { get; set; } = new Zoom();

        /// <summary>
        /// Changing the shutter speed alters how long the sensor is exposed to light (in microseconds).
        /// 
        /// There's currently an upper limit of approximately 6000000us (6000ms, 6s), past which operation is undefined.
        /// 
        /// </summary>
        public static int ShutterSpeed { get; set; }

        /// <summary>
        /// Adjust auto white balance 'red' gains
        /// </summary>
        public static double AwbGainsR { get; set; }

        /// <summary>
        /// Adjust auto white balance 'blue' gains
        /// </summary>
        public static double AwbGainsB { get; set; }

        /// <summary>
        /// Adjust dynamic range compression
        /// 
        /// DRC changes the images by increasing the range of dark areas, and decreasing the brighter areas. This can improve the image in low light areas.        
        /// </summary>
        public static MMAL_PARAMETER_DRC_STRENGTH_T DrcLevel { get; set; } = MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_OFF;

        /// <summary>
        /// Displays the exposure, analogue and digital gains, and AWB settings used.
        /// </summary>
        public static bool StatsPass { get; set; }

        /// <summary>
        /// Enable annotation of captured images
        /// </summary>
        public static bool EnableAnnotate { get; set; }    
        
        /// <summary>
        /// Allows fine tuning of annotation options
        /// </summary>
        public static AnnotateImage Annotate { get; set; }

        /// <summary>
        /// Adjust Stereoscopic mode - only supported with Raspberry Pi Compute module
        /// </summary>
        public static StereoMode StereoMode { get; set; } = new StereoMode();

        /// <summary>
        /// Enable to receive event request callbacks
        /// </summary>
        public static bool SetChangeEventRequest { get; set; }

        /// <summary>
        /// Specify the Presentation timestamp (PTS) mode.
        /// </summary>
        public static MMAL_PARAMETER_CAMERA_CONFIG_TIMESTAMP_MODE_T ClockMode { get; set; } =
            MMAL_PARAMETER_CAMERA_CONFIG_TIMESTAMP_MODE_T.MMAL_PARAM_TIMESTAMP_MODE_RESET_STC;

        /*
         * -----------------------------------------------------------------------------------------------------------
         * Camera preview port specific properties
         * -----------------------------------------------------------------------------------------------------------
        */

        public static MMALEncoding PreviewEncoding { get; set; } = MMALEncoding.OPAQUE;

        public static MMALEncoding PreviewSubformat { get; set; } = MMALEncoding.I420;
        
        /*
         * -----------------------------------------------------------------------------------------------------------
         * Camera video port specific properties
         * -----------------------------------------------------------------------------------------------------------
        */

        public static MMALEncoding VideoEncoding { get; set; } = MMALEncoding.OPAQUE;

        public static MMALEncoding VideoSubformat { get; set; } = MMALEncoding.I420;

        public static Resolution VideoResolution { get; set; } = Resolution.As1080p;

        /// <summary>
        /// Enable video stabilisation
        /// </summary> 
        public static bool VideoStabilisation { get; set; } = true;

        public static MMALParametersVideo.MMAL_VIDEO_RATECONTROL_T RateControl { get; set; } = MMALParametersVideo.MMAL_VIDEO_RATECONTROL_T.MMAL_VIDEO_RATECONTROL_DEFAULT;

        public static int IntraPeriod { get; set; } = -1;
                
        public static MMALParametersVideo.MMAL_VIDEO_PROFILE_T VideoProfile { get; set; } = MMALParametersVideo.MMAL_VIDEO_PROFILE_T.MMAL_VIDEO_PROFILE_H264_HIGH;

        public static MMALParametersVideo.MMAL_VIDEO_LEVEL_T VideoLevel { get; set; } = MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_4;

        public static bool ImmutableInput { get; set; } = true;

        public static bool InlineHeaders { get; set; }

        public static bool InlineMotionVectors { get; set; }

        public static MMALParametersVideo.MMAL_VIDEO_INTRA_REFRESH_T IntraRefresh { get; set; } = MMALParametersVideo.MMAL_VIDEO_INTRA_REFRESH_T.MMAL_VIDEO_INTRA_REFRESH_DISABLED;

        public static MMAL_RATIONAL_T VideoFramerate { get; set; } = new MMAL_RATIONAL_T(30, 1);

        /*
         * -----------------------------------------------------------------------------------------------------------
         * Camera still port specific properties
         * -----------------------------------------------------------------------------------------------------------
        */
        public static MMALEncoding StillEncoding { get; set; } = MMALEncoding.OPAQUE;

        public static MMALEncoding StillSubFormat { get; set; } = MMALEncoding.I420;

        public static Resolution StillResolution { get; set; } = Resolution.As5MPixel;

        public static MMAL_RATIONAL_T StillFramerate { get; set; } = new MMAL_RATIONAL_T(0, 1);
               

        /// <summary>
        /// Reloads Camera configuration settings
        /// </summary>
        public static void Reload()
        {
            MMALCamera.Instance.Camera.SetCameraParameters();
        }

    }
    
    public class ColourEffects
    {
        public bool Enable { get; set; }
        public int U { get; set; } = 128;
        public int V { get; set; } = 128;
        
    }
    
    public class Zoom
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
        public DateTime Timeout { get; set; }
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

    public class Split
    {
        public int Value { get; set; }
        public TimelapseMode Mode { get; set; }        
    }

    public enum TimelapseMode
    {
        Millisecond,
        Second,
        Minute
    }

    public class Resolution : IComparable<Resolution>
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Resolution(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        /*
         * 4:3 Aspect ratios 
        */
        public static Resolution As8MPixel => new Resolution(3264, 2448);
        
        public static Resolution As7MPixel => new Resolution(3072, 2304);
        
        public static Resolution As6MPixel => new Resolution(3032, 2008);
        
        public static Resolution As5MPixel => new Resolution(2560, 1920);
        
        public static Resolution As4MPixel => new Resolution(2240, 1680);
        
        public static Resolution As3MPixel => new Resolution(2048, 1536);
        
        public static Resolution As2MPixel => new Resolution(1600, 1200);
        
        public static Resolution As1MPixel => new Resolution(1280, 960);
        
        public static Resolution As03MPixel => new Resolution(640, 480);
        
        /*
         * 16:9 Aspect ratios 
        */
        public static Resolution As720p => new Resolution(1280, 720);
        
        public static Resolution As1080p => new Resolution(1920, 1080);
        
        public static Resolution As1440p => new Resolution(2560, 1440);

        /// <summary>
        /// Compares this Resolution instance against the Resolution passed in. 
        /// </summary>
        /// <param name="res"></param>
        /// <returns>0 if width and height are same. 1 if source width is greater than target. -1 if target greater than source.</returns>
        public int CompareTo(Resolution res)
        {
            if (this.Width == res.Width && this.Height == res.Height)
            {
                return 0;
            }
            if (this.Width == res.Width && this.Height > res.Height)
            {
                return 1;
            }
            if (this.Width == res.Width && this.Height < res.Height)
            {
                return -1;
            }
            
            if (this.Width > res.Width)
                return 1;

            return -1;
        }
    }

}
