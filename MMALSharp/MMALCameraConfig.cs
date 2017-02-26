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

        /// <summary>
        /// Configure the sharpness of the image
        /// </summary>
        public double Sharpness { get; set; }

        /// <summary>
        /// Configure the contrast of the image
        /// </summary>  
        public double Contrast { get; set; }

        /// <summary>
        /// Configure the brightness of the image
        /// </summary>
        public double Brightness { get; set; } = 50;

        /// <summary>
        /// Configure the saturation of the image
        /// </summary>  
        public double Saturation { get; set; }

        /// <summary>
        /// Configure the light sensitivity of the sensor
        /// </summary> 
        public int ISO { get; set; }

        /// <summary>
        /// Enable video stabilisation
        /// </summary> 
        public bool VideoStabilisation { get; set; } = true;

        /// <summary>
        /// Configure the exposure compensation of the camera. Doing so will produce a lighter/darker image beyond the recommended exposure.
        /// </summary>
        public MMAL_PARAM_EXPOSUREMODE_T ExposureCompensation { get; set; } = MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_AUTO;

        /// <summary>
        /// Configure the exposure mode used by the camera
        /// </summary>  
        public MMAL_PARAM_EXPOSUREMODE_T ExposureMode { get; set; } = MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_AUTO;

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
        public MMAL_PARAM_EXPOSUREMETERINGMODE_T ExposureMeterMode { get; set; } = MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE;

        /// <summary>
        /// Configure the Auto White Balance to be used by the camera
        /// </summary>
        public MMAL_PARAM_AWBMODE_T AwbMode { get; set; } = MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_AUTO;

        /// <summary>
        /// Configure any image effects to be used by the camera
        /// </summary>
        public MMAL_PARAM_IMAGEFX_T ImageEffect { get; set; } = MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_NONE;

        /// <summary>
        /// Allows a user to change the colour of an image, e.g. U = 128, V = 128 will result in a greyscale image.
        /// </summary>
        public ColourEffects Effects { get; set; } = new ColourEffects();

        /// <summary>
        /// Specify the rotation of the image, this value should be multiples of 90
        /// </summary>   
        public int Rotation { get; set; }

        /// <summary>
        /// Specify whether the image should be flipped
        /// </summary>                       
        public MMAL_PARAM_MIRROR_T Flips { get; set; } = MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_NONE;

        /// <summary>
        /// Crop an image to focus on a region of interest.
        /// </summary>
        public Crop Crop { get; set; } = new Crop();

        /// <summary>
        /// Changing the shutter speed alters how long the sensor is exposed to light.
        /// </summary>
        public int ShutterSpeed { get; set; }

        /// <summary>
        /// Adjust auto white balance 'red' gains
        /// </summary>
        public int AwbGainsR { get; set; }

        /// <summary>
        /// Adjust auto white balance 'blue' gains
        /// </summary>
        public int AwbGainsB { get; set; }

        /// <summary>
        /// Adjust dynamic range compression
        /// </summary>
        public MMAL_PARAMETER_DRC_STRENGTH_T DrcLevel { get; set; } = MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_OFF;

        /// <summary>
        /// Enable an extra statistics pass to be made when capturing images
        /// </summary>
        public bool StatsPass { get; set; }

        /// <summary>
        /// Enable annotation of captured images
        /// </summary>
        public bool EnableAnnotate { get; set; }    

        /// <summary>
        /// Allows fine tuning of annotation options
        /// </summary>
        public AnnotateImage Annotate { get; set; }

        /// <summary>
        /// Adjust Stereoscopic mode - only supported with Raspberry Pi Compute module
        /// </summary>
        public StereoMode StereoMode { get; set; } = new StereoMode();

        /// <summary>
        /// Enable to receive event request callbacks
        /// </summary>
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

        /// <summary>
        /// Reloads Camera configuration settings
        /// </summary>
        public void Reload()
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
