// <copyright file="MMALCameraConfig.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;
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
        public static int Sharpness { get; set; }

        /// <summary>
        /// Configure the contrast of the image.
        /// </summary>  
        public static int Contrast { get; set; }

        /// <summary>
        /// Configure the brightness of the image.
        /// </summary>
        public static int Brightness { get; set; } = 50;

        /// <summary>
        /// Configure the saturation of the image.
        /// </summary>  
        public static int Saturation { get; set; }

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
        /// Requires exposure mode to be left at "auto" (or at least not "off"). Used to set the analog gain value directly on the sensor.
        /// <para>
        /// Floating point value from 1.0 to 8.0 for the OV5647 sensor on Camera Module V1, and 1.0 to 12.0 for the IMX219 sensor on Camera Module V2 and the IMX447 on the HQ Camera.
        /// </para>
        /// </summary>
        public static double AnalogGain { get; set; }

        /// <summary>
        /// Requires exposure mode to be left at "auto" (or at least not "off"). Used to set the digital gain value applied by the ISP.
        /// <para>
        /// Floating point value from 1.0 to 255.0, but values over about 4.0 will produce overexposed images.
        /// </para>
        /// </summary>
        public static double DigitalGain { get; set; }

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

        /// <summary>
        /// The camera encoding type.
        /// </summary>
        public static MMALEncoding Encoding { get; set; } = MMALEncoding.OPAQUE;

        /// <summary>
        /// User requested number of buffers to be applied to the camera ports.
        /// </summary>
        public static int UserBufferNum { get; set; }

        /// <summary>
        /// User requested buffer size to be applied to the camera ports.
        /// </summary>
        public static int UserBufferSize { get; set; }

        /// <summary>
        /// The camera encoding sub format.
        /// </summary>
        public static MMALEncoding EncodingSubFormat { get; set; } = MMALEncoding.I420;

        /// <summary>
        /// The working resolution of the camera.
        /// </summary>
        public static Resolution Resolution { get; set; } = Resolution.As720p;

        /// <summary>
        /// The working framerate of the camera.
        /// </summary>
        public static double Framerate { get; set; } = 30;

        /*
         * -----------------------------------------------------------------------------------------------------------
         * Camera video port specific properties.
         * -----------------------------------------------------------------------------------------------------------
        */

        /// <summary>
        /// Enable video stabilisation. The purpose of video stabilisation is to filter the video frame
        /// however this comes at a cost of frames being cropped by a small amount to compensate.
        /// </summary> 
        public static bool VideoStabilisation { get; set; } = false;

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
        /// The working video color space, specific to the camera's video port. 
        /// </summary>
        public static MMALEncoding VideoColorSpace { get; set; }

        /*
         * -----------------------------------------------------------------------------------------------------------
         * Camera still port specific properties.
         * -----------------------------------------------------------------------------------------------------------
        */

        /// <summary>
        /// Indicates whether the camera's still port should feature Burst Mode, i.e. MMAL_PARAMETER_CAMERA_BURST_CAPTURE.
        /// When enabled, burst mode will increase the rate at which images are taken, at the expense of quality.
        /// </summary>
        public static bool StillBurstMode { get; set; }
    }
}
