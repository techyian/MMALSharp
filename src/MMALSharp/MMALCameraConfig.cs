// <copyright file="MMALCameraConfig.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using System.Threading;
using MMALSharp.Components;
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
        /// The pixel format to use for Still captures.
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

    /// <summary>
    /// Allows a user to adjust the colour of outputted frames.
    /// </summary>
    public struct ColourEffects
    {
        /// <summary>
        /// Enable the Colour Effects functionality.
        /// </summary>
        public bool Enable { get; }

        /// <summary>
        /// The <see cref="Color"/> to use.
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Initialises a new <see cref="ColourEffects"/> struct.
        /// </summary>
        /// <param name="enable">Enable the Colour Effects functionality.</param>
        /// <param name="color">The <see cref="Color"/> to use.</param>
        public ColourEffects(bool enable, Color color)
        {
            Enable = enable;
            Color = color;
        }
    }

    /// <summary>
    /// Allows a user to specify a Region of Interest with Still captures.
    /// </summary>
    public struct Zoom
    {

        /// <summary>
        /// The X coordinate between 0 - 1.0.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// The Y coordinate between 0 - 1.0.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// The Width value between 0 - 1.0.
        /// </summary>
        public double Width { get; }

        /// <summary>
        /// The Height value between 0 - 1.0.
        /// </summary>
        public double Height { get; }

        /// <summary>
        /// Intialises a new <see cref="Zoom"/> struct.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="width">The Width value.</param>
        /// <param name="height">The Height value.</param>
        public Zoom(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }

    /// <summary>
    /// The Stereoscopic mode code has mainly been added for completeness.
    /// It requires a Raspberry Pi Compute Module with two cameras connected.
    /// This functionality has not been tested.
    /// </summary>
    public class StereoMode
    {
        /// <summary>
        /// Gets or sets the stereoscopic mode.
        /// </summary>
        public MMAL_STEREOSCOPIC_MODE_T Mode { get; set; } = MMAL_STEREOSCOPIC_MODE_T.MMAL_STEREOSCOPIC_MODE_NONE;
        /// <summary>
        /// Gets or sets a value indicating whether to half the width and height of a stereoscopic image.
        /// </summary>
        /// <remarks>https://github.com/raspberrypi/userland/blob/master/host_applications/linux/apps/raspicam/RaspiCamControl.c#L204</remarks>
        public int Decimate { get; set; }
        /// <summary>
        /// Gets or sets a value indicating a swap of camera order for stereoscopic mode.
        /// </summary>
        /// <remarks>https://github.com/raspberrypi/userland/blob/master/host_applications/linux/apps/raspicam/RaspiCamControl.c#L205</remarks>
        public int SwapEyes { get; set; }
    }

    /// <summary>
    /// Represents an Exif tag for use with JPEG still captures.
    /// </summary>
    public class ExifTag
    {
        /// <summary>
        /// The Exif key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The Exif value.
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// The <see cref="Timelapse"/> type is for use with Timelapse still captures.
    /// </summary>
    public class Timelapse
    {
        /// <summary>
        /// The timelapse mode.
        /// </summary>
        public TimelapseMode Mode { get; set; }

        /// <summary>
        /// Specifies when timelapse capture should finish.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// How often images should be taken (relates to the <see cref="TimelapseMode"/> chosen).
        /// </summary>
        public int Value { get; set; }
    }

    /// <summary>
    /// The <see cref="AnnotateImage"/> type is for use with the image annotation functionality.
    /// This will produce a textual overlay on image stills depending on the options enabled.
    /// </summary>
    public class AnnotateImage
    {
        /// <summary>
        /// Custom text to overlay on the stills capture.
        /// </summary>
        public string CustomText { get; set; }

        /// <summary>
        /// The text size to use.
        /// </summary>
        public int TextSize { get; set; }

        /// <summary>
        /// The <see cref="Color"/> of the text.
        /// </summary>
        public Color TextColour { get; set; } = Color.Empty;

        /// <summary>
        /// The <see cref="Color"/> of the background. Note: ShowBlackBackground should be enabled
        /// for this to work.
        /// </summary>
        public Color BgColour { get; set; } = Color.Empty;

        /// <summary>
        /// Show shutter settings.
        /// </summary>
        public bool ShowShutterSettings { get; set; }

        /// <summary>
        /// Show gain settings.
        /// </summary>
        public bool ShowGainSettings { get; set; }

        /// <summary>
        /// Show lens settings.
        /// </summary>
        public bool ShowLensSettings { get; set; }

        /// <summary>
        /// Show Continuous Auto Focus settings.
        /// </summary>
        public bool ShowCafSettings { get; set; }

        /// <summary>
        /// Show motion settings.
        /// </summary>
        public bool ShowMotionSettings { get; set; }

        /// <summary>
        /// Show the frame number.
        /// </summary>
        public bool ShowFrameNumber { get; set; }

        /// <summary>
        /// Allows custom background colour to be used.
        /// </summary>
        public bool ShowBlackBackground { get; set; }

        /// <summary>
        /// Show the current date.
        /// </summary>
        public bool ShowDateText { get; set; }

        /// <summary>
        /// Show the current time.
        /// </summary>
        public bool ShowTimeText { get; set; }
    }

    /// <summary>
    /// The <see cref="Split"/> type is used when taking video capture and a user wishes to split
    /// recording into multiple files. 
    /// </summary>
    public class Split
    {
        /// <summary>
        /// How often files should be split.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// The <see cref="TimelapseMode"/> mode to use.
        /// </summary>
        public TimelapseMode Mode { get; set; }
    }

    /// <summary>
    /// The unit of time to use.
    /// </summary>
    public enum TimelapseMode
    {
        /// <summary>
        /// Uses milliseconds as unit of time. One hour equals 3'600'000 milliseconds.
        /// </summary>
        Millisecond,
        /// <summary>
        /// Uses seconds as unit of time. One hour equals 3'600 seconds.
        /// </summary>
        Second,
        /// <summary>
        /// Uses minutes as unit of time. One hour equals 60 minutes.
        /// </summary>
        Minute
    }

    /// <summary>
    /// Exposes properties for width and height. This class is used to specify a resolution for camera and ports.
    /// </summary>
    public struct Resolution : IComparable<Resolution>
    {
        /// <summary>
        /// The width of the <see cref="Resolution"/> object.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The height of the <see cref="Resolution"/> object.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="Resolution"/> class with the specified width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Resolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /*
         * 4:3 Aspect ratios 
        */

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 3264 pixels high and 2448 pixels wide.
        /// </summary>
        public static Resolution As8MPixel => new Resolution(3264, 2448);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 3072 pixels high and 2304 pixels wide.
        /// </summary>
        public static Resolution As7MPixel => new Resolution(3072, 2304);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 3032 pixels high and 2008 pixels wide.
        /// </summary>
        public static Resolution As6MPixel => new Resolution(3032, 2008);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 2560 pixels high and 1920 pixels wide.
        /// </summary>
        public static Resolution As5MPixel => new Resolution(2560, 1920);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 2240 pixels high and 1680 pixels wide.
        /// </summary>
        public static Resolution As4MPixel => new Resolution(2240, 1680);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 2048 pixels high and 1536 pixels wide.
        /// </summary>
        public static Resolution As3MPixel => new Resolution(2048, 1536);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 1600 pixels high and 1200 pixels wide.
        /// </summary>
        public static Resolution As2MPixel => new Resolution(1600, 1200);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 1280 pixels high and 960 pixels wide.
        /// </summary>
        public static Resolution As1MPixel => new Resolution(1280, 960);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 640 pixels high and 480 pixels wide.
        /// </summary>
        public static Resolution As03MPixel => new Resolution(640, 480);

        /*
         * 16:9 Aspect ratios 
        */

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 1280 pixels high and 720 pixels wide.
        /// </summary>
        public static Resolution As720p => new Resolution(1280, 720);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 1920 pixels high and 1080 pixels wide.
        /// </summary>
        public static Resolution As1080p => new Resolution(1920, 1080);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 2560 pixels high and 1440 pixels wide.
        /// </summary>
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

        /// <summary>
        /// Pads a <see cref="Resolution"/> object to the desired width/height.
        /// </summary>
        /// <param name="width">The width to be padded to.</param>
        /// <param name="height">The height to be padded to.</param>
        /// <returns></returns>
        public Resolution Pad(int width = 32, int height = 16)
        {
            return new Resolution(MMALUtil.VCOS_ALIGN_UP(this.Width, width),
                                  MMALUtil.VCOS_ALIGN_UP(this.Height, height));
        }
    }

    /// <summary>
    /// Defines the settings for a <see cref="MMALVideoRenderer"/> component.
    /// </summary>
    public class PreviewConfiguration
    {
        /// <summary>
        /// Indicates whether to use full screen or windowed mode.
        /// </summary>
        public bool FullScreen { get; set; } = true;
        /// <summary>
        /// If set to true, indicates that any display scaling should disregard the aspect ratio of the frame region being displayed.
        /// </summary>
        public bool NoAspect { get; set; }
        /// <summary>
        /// Enable copy protection. 
        /// Note: Doesn't appear to be supported by the firmware.
        /// </summary>
        public bool CopyProtect { get; set; }
        /// <summary>
        /// Specifies where the preview overlay should be drawn on the screen.
        /// </summary>
        public Rectangle PreviewWindow { get; set; } = new Rectangle(0, 0, 1024, 768);
        /// <summary>
        /// Opacity of the preview windows. Value between 1 (fully invisible) - 255 (fully opaque).
        /// Note: If RGBA encoding is used with the preview component then the alpha channel will be ignored.
        /// </summary>
        public int Opacity { get; set; } = 255;
        /// <summary>
        /// Sets the relative depth of the images, with greater values being in front of smaller values.
        /// </summary>
        public int Layer { get; set; } = 2;
        /// <summary>
        /// Indicates whether any flipping or rotation should be used on the overlay.
        /// </summary>
        public MMALParametersVideo.MMAL_DISPLAYTRANSFORM_T DisplayTransform { get; set; }
        /// <summary>
        /// Indicates how the image should be scaled to fit the display.
        /// </summary>
        public MMALParametersVideo.MMAL_DISPLAYMODE_T DisplayMode { get; set; }
    }

    /// <summary>
    /// Defines the settings for a <see cref="MMALOverlayRenderer"/> component.
    /// </summary>
    public class PreviewOverlayConfiguration : PreviewConfiguration
    {
        /// <summary>
        /// Specifies the resolution of the static resource to be used with this Preview Overlay. If this is null then the parent renderer's resolution will be used instead.
        /// </summary>
        public Resolution Resolution { get; set; }
        /// <summary>
        /// The encoding of the static resource. Can be one of the following: YUV, RGB, RGBA, BGR, BGRA.
        /// If left null, we will try to work out the encoding based on the size of the image (3 bytes for RGB, 4 bytes for RGBA).
        /// </summary>
        public MMALEncoding Encoding { get; set; }
    }

}
