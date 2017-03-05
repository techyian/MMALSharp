using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// Enable video stabilisation
        /// </summary> 
        public static bool VideoStabilisation { get; set; } = true;

        /// <summary>
        /// Configure the exposure compensation of the camera. Doing so will produce a lighter/darker image beyond the recommended exposure.
        /// </summary>
        public static MMAL_PARAM_EXPOSUREMODE_T ExposureCompensation { get; set; } = MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_AUTO;

        /// <summary>
        /// Configure the exposure mode used by the camera
        /// </summary>  
        public static MMAL_PARAM_EXPOSUREMODE_T ExposureMode { get; set; } = MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_AUTO;

        /// <summary>
        /// Configure the exposure metering mode to be used by the camera. The metering mode determines how the camera measures exposure.
        /// 
        /// Spot metering (MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_SPOT):
        /// 
        /// With spot metering, the camera will only measure a very small area of the scene and ignores everything else.
        /// On the Raspberry Pi camera, this will be the very centre of the image. 
        /// 
        /// Average metering (MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE):
        /// 
        /// Using this metering mode, the camera will use the light information coming from the entire scene. It does not focus on any particular
        /// area of the scene.
        /// 
        /// Matrix metering (MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_MATRIX):
        /// 
        /// Matrix metering works by dividing the entire frame into multiple "zones" which are then analysed on an individual basis for light and dark tones.
        /// 
        /// 
        /// Sources:
        /// https://photographylife.com/understanding-metering-modes
        /// https://en.wikipedia.org/wiki/Metering_mode#Spot_metering
        /// 
        /// </summary> 
        public static MMAL_PARAM_EXPOSUREMETERINGMODE_T ExposureMeterMode { get; set; } = MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE;

        /// <summary>
        /// Configure the Auto White Balance to be used by the camera
        /// </summary>
        public static MMAL_PARAM_AWBMODE_T AwbMode { get; set; } = MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_AUTO;

        /// <summary>
        /// Configure any image effects to be used by the camera
        /// </summary>
        public static MMAL_PARAM_IMAGEFX_T ImageEffect { get; set; } = MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_NONE;

        /// <summary>
        /// Allows a user to change the colour of an image, e.g. U = 128, V = 128 will result in a greyscale image.
        /// </summary>
        public static ColourEffects Effects { get; set; } = new ColourEffects();

        /// <summary>
        /// Specify the rotation of the image, this value should be multiples of 90
        /// </summary>   
        public static int Rotation { get; set; }

        /// <summary>
        /// Specify whether the image should be flipped
        /// </summary>                       
        public static MMAL_PARAM_MIRROR_T Flips { get; set; } = MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_NONE;

        /// <summary>
        /// Crop an image to focus on a region of interest.
        /// </summary>
        public static Crop Crop { get; set; } = new Crop();

        /// <summary>
        /// Changing the shutter speed alters how long the sensor is exposed to light.
        /// </summary>
        public static int ShutterSpeed { get; set; }

        /// <summary>
        /// Adjust auto white balance 'red' gains
        /// </summary>
        public static int AwbGainsR { get; set; }

        /// <summary>
        /// Adjust auto white balance 'blue' gains
        /// </summary>
        public static int AwbGainsB { get; set; }

        /// <summary>
        /// Adjust dynamic range compression
        /// </summary>
        public static MMAL_PARAMETER_DRC_STRENGTH_T DrcLevel { get; set; } = MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_OFF;

        /// <summary>
        /// Enable an extra statistics pass to be made when capturing images
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

        // Camera preview port specific properties
        public static int PreviewEncoding { get; set; } = MMALEncodings.MMAL_ENCODING_OPAQUE;

        private static int previewWidth;
        public static int PreviewWidth {
            get
            {
                return MMALCameraConfig.previewWidth;
            }
            set
            {
                MMALCameraConfig.previewWidth = MMALUtil.VCOS_ALIGN_UP(value, 32);
            }                
        }

        private static int previewHeight;
        public static int PreviewHeight
        {
            get
            {
                return MMALCameraConfig.previewHeight;
            }
            set
            {
                MMALCameraConfig.previewHeight = MMALUtil.VCOS_ALIGN_UP(value, 16);
            }
        }

        // Camera video port specific properties
        public static int VideoEncoding { get; set; } = MMALEncodings.MMAL_ENCODING_OPAQUE;
        public static int VideoSubformat { get; set; } = MMALEncodings.MMAL_ENCODING_I420;
        private static int videoWidth;
        public static int VideoWidth
        {
            get
            {
                return MMALCameraConfig.videoWidth;
            }
            set
            {
                MMALCameraConfig.videoWidth = MMALUtil.VCOS_ALIGN_UP(value, 32);
            }
        }

        private static int videoHeight;
        public static int VideoHeight
        {
            get
            {
                return MMALCameraConfig.videoHeight;
            }
            set
            {
                MMALCameraConfig.videoHeight = MMALUtil.VCOS_ALIGN_UP(value, 16);
            }
        }

        // Camera still port specific properties
        public static int StillEncoding { get; set; } = MMALEncodings.MMAL_ENCODING_OPAQUE;
        public static int StillEncodingSubFormat { get; set; } = MMALEncodings.MMAL_ENCODING_I420;

        private static int stillWidth;
        public static int StillWidth
        {
            get
            {
                return MMALCameraConfig.stillWidth;
            }
            set
            {
                MMALCameraConfig.stillWidth = MMALUtil.VCOS_ALIGN_UP(value, 32);
            }
        }

        private static int stillHeight;
        public static int StillHeight
        {
            get
            {
                return MMALCameraConfig.stillHeight;
            }
            set
            {
                MMALCameraConfig.stillHeight = MMALUtil.VCOS_ALIGN_UP(value, 16);
            }
        }

        public static MMALParametersVideo.MMAL_VIDEO_RATECONTROL_T RateControl { get; set; } = MMALParametersVideo.MMAL_VIDEO_RATECONTROL_T.MMAL_VIDEO_RATECONTROL_DEFAULT;

        public static int IntraPeriod { get; set; } = -1;

        public static Quantisation QuantisationParameter { get; set; } = new Quantisation();

        public static MMALParametersVideo.MMAL_VIDEO_PROFILE_T VideoProfile { get; set; } = MMALParametersVideo.MMAL_VIDEO_PROFILE_T.MMAL_VIDEO_PROFILE_H264_HIGH;

        public static MMALParametersVideo.MMAL_VIDEO_LEVEL_T VideoLevel { get; set; } = MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_4;

        public static bool ImmutableInput { get; set; } = true;

        public static bool InlineHeaders { get; set; }

        public static bool InlineMotionVectors { get; set; }

        public static MMALParametersVideo.MMAL_VIDEO_INTRA_REFRESH_T IntraRefresh { get; set; } = MMALParametersVideo.MMAL_VIDEO_INTRA_REFRESH_T.MMAL_VIDEO_INTRA_REFRESH_DISABLED;

        /// <summary>
        /// Reloads Camera configuration settings
        /// </summary>
        public static void Reload()
        {
            MMALCamera.Instance.ConfigureCamera();
        }

    }
    
    public class ColourEffects
    {
        public int Enable { get; set; }
        public uint U { get; set; } = 128;
        public uint V { get; set; } = 128;
        
    }

    public class Quantisation
    {
        public int Initial { get; set; }
        public int Min { get; set; }        
        public int Max { get; set; }
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
