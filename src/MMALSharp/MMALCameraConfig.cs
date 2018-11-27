// <copyright file="MMALCameraConfig.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Config;
using MMALSharp.Native;

namespace MMALSharp
{
    /// <summary>
    /// Provides a rich set of camera/sensor related configuration. Call <see cref="MMALCamera.ConfigureCameraSettings"/> to apply changes.
    /// </summary>
    public static class MMALCameraConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether to enable verbose debug output for many operations.
        /// </summary>
        public static bool Debug { get; set; }

        /// <summary>
        /// Manually set the camera sensor mode to force certain attributes.
        /// See: 
        /// https://github.com/techyian/MMALSharp/wiki/OmniVision-OV5647-Camera-Module
        /// https://github.com/techyian/MMALSharp/wiki/Sony-IMX219-Camera-Module
        /// </summary>
        public static MMALSensorMode SensorMode { get; set; }

        /// <summary>
        /// Configure the sharpness of the image.
        /// </summary>
        public static double Sharpness { get; set; }

        /// <summary>
        /// Configure the contrast of the image.
        /// </summary>  
        public static double Contrast { get; set; }

        /// <summary>
        /// Configure the brightness of the image.
        /// </summary>
        public static double Brightness { get; set; } = 50;

        /// <summary>
        /// Configure the saturation of the image.
        /// </summary>  
        public static double Saturation { get; set; }

        /// <summary>
        /// Configure the light sensitivity of the sensor.
        /// </summary> 
        public static int ISO { get; set; }

        /// <summary>
        /// Configure the exposure compensation of the camera. Doing so will produce a lighter/darker image beyond the recommended exposure.
        /// </summary>
        public static int ExposureCompensation { get; set; }

        /// <summary>
        /// Configure the exposure mode used by the camera. 
        /// </summary>  
        public static MMAL_PARAM_EXPOSUREMODE_T ExposureMode { get; set; } = MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_AUTO;

        /// <summary>
        /// Configure the exposure metering mode to be used by the camera. The metering mode determines how the camera measures exposure.       
        /// </summary> 
        public static MMAL_PARAM_EXPOSUREMETERINGMODE_T ExposureMeterMode { get; set; } = MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE;

        /// <summary>
        /// Configure the Auto White Balance to be used by the camera.
        /// </summary>
        public static MMAL_PARAM_AWBMODE_T AwbMode { get; set; } = MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_AUTO;

        /// <summary>
        /// Configure any image effects to be used by the camera       
        /// </summary>
        public static MMAL_PARAM_IMAGEFX_T ImageFx { get; set; } = MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_NONE;

        /// <summary>
        /// Allows a user to change the colour of an image, e.g. U = 128, V = 128 will result in a greyscale (monochrome) image.
        /// </summary>
        public static ColourEffects ColourFx { get; set; }

        /// <summary>
        /// Specify the rotation of the image, possible values are 0, 90, 180, 270.
        /// </summary>   
        public static int Rotation { get; set; }

        /// <summary>
        /// Specify whether the image should be flipped.
        /// </summary>                       
        public static MMAL_PARAM_MIRROR_T Flips { get; set; } = MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_NONE;

        /// <summary>
        /// Zoom in on an image to focus on a region of interest.
        /// </summary>
        public static Zoom ROI { get; set; }

        /// <summary>
        /// Changing the shutter speed alters how long the sensor is exposed to light (in microseconds).
        /// <para />
        /// There's currently an upper limit of approximately 6.000.000µs (6.000ms, 6s), past which operation is undefined. 8MP Sony sensor supports 8s max shutter speed.
        /// </summary>
        public static int ShutterSpeed { get; set; }

        /// <summary>
        /// Adjust auto white balance 'red' gains.
        /// </summary>
        public static double AwbGainsR { get; set; }

        /// <summary>
        /// Adjust auto white balance 'blue' gains.
        /// </summary>
        public static double AwbGainsB { get; set; }

        /// <summary>
        /// Adjust dynamic range compression.
        /// 
        /// DRC changes the images by increasing the range of dark areas, and decreasing the brighter areas. This can improve the image in low light areas.        
        /// </summary>
        public static MMAL_PARAMETER_DRC_STRENGTH_T DrcLevel { get; set; } = MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_OFF;

        /// <summary>
        /// Displays the exposure, analogue and digital gains, and AWB settings used.
        /// </summary>
        public static bool StatsPass { get; set; }

        /// <summary>
        /// Allows fine tuning of annotation options.
        /// </summary>
        public static AnnotateImage Annotate { get; set; }

        /// <summary>
        /// Adjust Stereoscopic mode - only supported with Raspberry Pi Compute module.
        /// </summary>
        public static StereoMode StereoMode { get; set; } = new StereoMode();

        /// <summary>
        /// Enable to receive event request callbacks.
        /// </summary>
        public static bool SetChangeEventRequest { get; set; }

        /// <summary>
        /// Specify the Presentation timestamp (PTS) mode.
        /// </summary>
        public static MMAL_PARAMETER_CAMERA_CONFIG_TIMESTAMP_MODE_T ClockMode { get; set; } =
            MMAL_PARAMETER_CAMERA_CONFIG_TIMESTAMP_MODE_T.MMAL_PARAM_TIMESTAMP_MODE_RESET_STC;

        /*
         * -----------------------------------------------------------------------------------------------------------
         * Camera preview port specific properties.
         * -----------------------------------------------------------------------------------------------------------
        */

        /// <summary>
        /// The encoding type to use with the Preview renderer.
        /// </summary>
        public static MMALEncoding PreviewEncoding { get; set; } = MMALEncoding.OPAQUE;

        /// <summary>
        /// The pixel format to use with the Preview renderer.
        /// </summary>
        public static MMALEncoding PreviewSubformat { get; set; } = MMALEncoding.I420;

        /*
         * -----------------------------------------------------------------------------------------------------------
         * Camera video port specific properties.
         * -----------------------------------------------------------------------------------------------------------
        */

        /// <summary>
        /// The encoding type to use for Video captures.
        /// </summary>
        public static MMALEncoding VideoEncoding { get; set; } = MMALEncoding.OPAQUE;

        /// <summary>
        /// The pixel format to use for Video captures.
        /// </summary>
        public static MMALEncoding VideoSubformat { get; set; } = MMALEncoding.I420;

        /// <summary>
        /// The Resolution to use for Video captures.
        /// </summary>
        public static Resolution VideoResolution { get; set; } = Resolution.As1080p;

        /// <summary>
        /// Enable video stabilisation.
        /// </summary> 
        public static bool VideoStabilisation { get; set; } = true;

        /// <summary>
        /// Used to force behaviour of frame rate control.
        /// </summary>
        public static MMALParametersVideo.MMAL_VIDEO_RATECONTROL_T RateControl { get; set; } = MMALParametersVideo.MMAL_VIDEO_RATECONTROL_T.MMAL_VIDEO_RATECONTROL_DEFAULT;

        /// <summary>
        /// Specifies the number of frames after which a new I-frame is inserted in to the stream.
        /// </summary>
        public static int IntraPeriod { get; set; } = -1;

        /// <summary>
        /// Represents the H.264 video profile you wish to use.
        /// </summary>
        public static MMALParametersVideo.MMAL_VIDEO_PROFILE_T VideoProfile { get; set; } = MMALParametersVideo.MMAL_VIDEO_PROFILE_T.MMAL_VIDEO_PROFILE_H264_HIGH;

        /// <summary>
        /// Represents the H.264 video level you wish to use.
        /// </summary>
        public static MMALParametersVideo.MMAL_VIDEO_LEVEL_T VideoLevel { get; set; } = MMALParametersVideo.MMAL_VIDEO_LEVEL_T.MMAL_VIDEO_LEVEL_H264_4;

        /// <summary>
        /// Requires the video encoder not to modify the input images. 
        /// </summary>
        /// <remarks>http://www.jvcref.com/files/PI/documentation/ilcomponents/prop.html#OMX_IndexParamBrcmImmutableInput</remarks>
        public static bool ImmutableInput { get; set; } = true;

        /// <summary>
        /// Force the stream to include PPS and SPS headers on every I-frame. Needed for certain 
        /// streaming cases e.g. Apple HLS.
        /// </summary>
        public static bool InlineHeaders { get; set; }

        /// <summary>
        /// Enable output of motion vectors. 
        /// See https://www.raspberrypi.org/forums/viewtopic.php?t=85867 for use case.
        /// </summary>
        public static bool InlineMotionVectors { get; set; }

        /// <summary>
        /// Sets the intra refresh period (GoP) rate for the recorded video.
        /// </summary>
        public static MMALParametersVideo.MMAL_VIDEO_INTRA_REFRESH_T IntraRefresh { get; set; } = MMALParametersVideo.MMAL_VIDEO_INTRA_REFRESH_T.MMAL_VIDEO_INTRA_REFRESH_DISABLED;

        /// <summary>
        /// Specifies the frames per second to record
        /// </summary>
        public static MMAL_RATIONAL_T VideoFramerate { get; set; } = new MMAL_RATIONAL_T(30, 1);

        /// <summary>
        /// The working video color space, specific to video ports.
        /// </summary>
        public static MMALEncoding VideoColorSpace { get; set; }

        /*
         * -----------------------------------------------------------------------------------------------------------
         * Camera still port specific properties.
         * -----------------------------------------------------------------------------------------------------------
        */

        /// <summary>
        /// The encoding type to use for Still captures.
        /// </summary>
        public static MMALEncoding StillEncoding { get; set; } = MMALEncoding.OPAQUE;

        /// <summary>
        /// The pixel format to use for Still captures. Irrelavent when not using OPAQUE encoding.
        /// </summary>
        public static MMALEncoding StillSubFormat { get; set; } = MMALEncoding.I420;

        /// <summary>
        /// The Resolution to use for Still captures.
        /// </summary>
        public static Resolution StillResolution { get; set; } = Resolution.As5MPixel;

        /// <summary>
        /// The frame rate to use for Still captures.
        /// </summary>
        public static MMAL_RATIONAL_T StillFramerate { get; set; } = new MMAL_RATIONAL_T(0, 1);
    }
}
