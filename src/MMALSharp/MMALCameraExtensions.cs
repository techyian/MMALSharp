// <copyright file="MMALCameraExtensions.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Config;
using MMALSharp.Native;
using MMALSharp.Ports.Controls;
using static MMALSharp.MMALNativeExceptionHelper;
using static MMALSharp.Native.MMALParametersCamera;

namespace MMALSharp
{
#pragma warning disable SA1202

    /// <summary>
    /// Provides extension methods for useful configuration against the Camera component.
    /// </summary>
    public static unsafe class MMALCameraComponentExtensions
    {
        internal static void SetCameraConfig(this MMALCameraComponent camera, MMAL_PARAMETER_CAMERA_CONFIG_T value)
        {
            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Control.Ptr, &value.Hdr), "Unable to set camera config.");
        }

        internal static void SetChangeEventRequest(this IControlPort controlPort, MMAL_PARAMETER_CHANGE_EVENT_REQUEST_T value)
        {
            MMALCheck(MMALPort.mmal_port_parameter_set(controlPort.Ptr, &value.Hdr), "Unable to set camera event request.");
        }

        /// <summary>
        /// States whether the camera control port is configured to display annotation settings.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>True if configured to display annotation settings.</returns>
        public static bool GetIsEnabledAnnotateSettings(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T annotate = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_ANNOTATE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>()),
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &annotate.Hdr), "Unable to get camera annotate settings");

            return annotate.Enable == 1;
        }

        /// <summary>
        /// States whether the annotate feature will display shutter information.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>True if configured to display shutter information.</returns>
        public static bool GetShowShutterAnnotateSettings(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T annotate = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_ANNOTATE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>()),
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &annotate.Hdr), "Unable to get camera annotate settings");

            return annotate.ShowShutter == 1;
        }

        /// <summary>
        /// States whether the annotate feature will display Continuous Auto Focus information.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>True if configured to display Continuous Auto Focus information.</returns>
        public static bool GetShowCafAnnotateSettings(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T annotate = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_ANNOTATE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>()),
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &annotate.Hdr), "Unable to get camera annotate settings");

            return annotate.ShowCaf == 1;
        }

        /// <summary>
        /// States whether the annotate feature will display gain information.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>True if configured to display gain information.</returns>
        public static bool GetShowGainAnnotateSettings(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T annotate = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_ANNOTATE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>()),
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &annotate.Hdr), "Unable to get camera annotate settings");

            return annotate.ShowAnalogGain == 1;
        }

        /// <summary>
        /// States whether the annotate feature will display lens information.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>True if configured to display lens information.</returns>
        public static bool GetShowLensAnnotateSettings(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T annotate = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_ANNOTATE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>()),
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &annotate.Hdr), "Unable to get camera annotate settings");

            return annotate.ShowLens == 1;
        }

        /// <summary>
        /// States whether the annotate feature will display motion information.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>True if configured to display motion information.</returns>
        public static bool GetShowMotionAnnotateSettings(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T annotate = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_ANNOTATE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>()),
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &annotate.Hdr), "Unable to get camera annotate settings");

            return annotate.ShowMotion == 1;
        }

        /// <summary>
        /// States whether the annotate feature will display frame number information.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>True if configured to display frame number information.</returns>
        public static bool GetShowFrameNumberAnnotateSettings(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T annotate = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_ANNOTATE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>()),
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &annotate.Hdr), "Unable to get camera annotate settings");

            return annotate.ShowFrameNum == 1;
        }

        /// <summary>
        /// States whether the annotate feature will use a black background when displaying information.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>True if configured to use a black background when displaying information.</returns>
        public static bool GetShowBlackBackgroundAnnotateSettings(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T annotate = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_ANNOTATE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>()),
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &annotate.Hdr), "Unable to get camera annotate settings");

            return annotate.EnableTextBackground == 1;
        }

        /// <summary>
        /// Get the custom text (if any) that the user has requested to be displayed using the annotate feature.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The custom text specified by the user.</returns>
        public static string GetCustomTextAnnotateSettings(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T annotate = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_ANNOTATE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>()),
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &annotate.Hdr), "Unable to get camera annotate settings");

            return Encoding.ASCII.GetString(annotate.Text);
        }

        /// <summary>
        /// Gets the <see cref="Color"/> structure that will be used to display information using the annotate feature.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The <see cref="Color"/> structure.</returns>
        public static Color GetTextColourAnnotateSettings(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T annotate = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_ANNOTATE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>()),
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &annotate.Hdr), "Unable to get camera annotate settings");

            return MMALColor.FromYUVBytes(annotate.CustomTextY, annotate.CustomTextU, annotate.CustomTextV);
        }

        /// <summary>
        /// Gets the <see cref="Color"/> structure that will be used as a background when displaying information using the annotate feature.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The <see cref="Color"/> structure.</returns>
        public static Color GetBackgroundColourAnnotateSettings(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T annotate = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_ANNOTATE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>()),
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &annotate.Hdr), "Unable to get camera annotate settings");

            return MMALColor.FromYUVBytes(annotate.CustomBackgroundY, annotate.CustomBackgroundU, annotate.CustomBackgroundV);
        }
        
        internal static void SetAnnotateSettings(this MMALCameraComponent camera)
        {
            if (MMALCameraConfig.Annotate != null)
            {
                MMALLog.Logger.LogDebug("Setting annotate");

                var sb = new StringBuilder();

                var showShutter = 0;
                var showAnalogGain = 0;
                var showLens = 0;
                var showCaf = 0;
                var showMotion = 0;
                var showFrame = 0;
                var enableTextBackground = 0;
                var textSize = (byte)0;
                var customTextColor = 0;
                var customTextY = (byte)0;
                var customTextU = (byte)0;
                var customTextV = (byte)0;
                var customBackgroundColor = 0;
                var customBackgroundY = (byte)0;
                var customBackgroundU = (byte)0;
                var customBackgroundV = (byte)0;
                var justify = MMALCameraConfig.Annotate.Justify;
                var xOffset = MMALCameraConfig.Annotate.XOffset;
                var yOffset = MMALCameraConfig.Annotate.YOffset;

                if (!string.IsNullOrEmpty(MMALCameraConfig.Annotate.CustomText))
                {
                    sb.Append(MMALCameraConfig.Annotate.CustomText + " ");
                }

                if (MMALCameraConfig.Annotate.ShowTimeText)
                {
                    sb.Append(DateTime.Now.ToString(MMALCameraConfig.Annotate.TimeFormat) + " ");
                }

                if (MMALCameraConfig.Annotate.ShowDateText)
                {
                    sb.Append(DateTime.Now.ToString(MMALCameraConfig.Annotate.DateFormat) + " ");
                }

                if (MMALCameraConfig.Annotate.ShowShutterSettings)
                {
                    showShutter = 1;
                }

                if (MMALCameraConfig.Annotate.ShowGainSettings)
                {
                    showAnalogGain = 1;
                }

                if (MMALCameraConfig.Annotate.ShowLensSettings)
                {
                    showLens = 1;
                }

                if (MMALCameraConfig.Annotate.ShowCafSettings)
                {
                    showCaf = 1;
                }

                if (MMALCameraConfig.Annotate.ShowMotionSettings)
                {
                    showMotion = 1;
                }

                if (MMALCameraConfig.Annotate.ShowFrameNumber)
                {
                    showFrame = 1;
                }

                if (MMALCameraConfig.Annotate.AllowCustomBackgroundColour)
                {
                    enableTextBackground = 1;
                }

                textSize = Convert.ToByte(MMALCameraConfig.Annotate.TextSize);

                if (MMALCameraConfig.Annotate.TextColour != Color.Empty)
                {
                    customTextColor = 1;
                    
                    var yuv = MMALColor.RGBToYUVBytes(MMALCameraConfig.Annotate.TextColour);
                    customTextY = yuv.Item1;
                    customTextU = yuv.Item2;
                    customTextV = yuv.Item3;                                        
                }

                if (MMALCameraConfig.Annotate.BgColour != Color.Empty)
                {
                    customBackgroundColor = 1;
                    var yuv = MMALColor.RGBToYUVBytes(MMALCameraConfig.Annotate.BgColour);
                    customBackgroundY = yuv.Item1;
                    customBackgroundU = yuv.Item2;
                    customBackgroundV = yuv.Item3;
                }

                // .NET Core has an issue with marshalling arrays "ByValArray". The array being passed MUST equal the size
                // specified in the "SizeConst" field or you will receive an exception. Mono does not have this restriction
                // and is quite happy to pass an array of a lower size if asked. In order to get around this, I am creating
                // an array equaling "SizeConst" and copying the contents of the annotation text into it, minus the EOL character.
                var text = sb.ToString() + char.MinValue;                                
                var arr = new byte[MMALParametersCamera.MMAL_CAMERA_ANNOTATE_MAX_TEXT_LEN_V3];
                var bytes = Encoding.ASCII.GetBytes(text);

                Array.Copy(bytes, arr, bytes.Length);

                var strV4 = new MMAL_PARAMETER_CAMERA_ANNOTATE_V4_T(
                    new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_ANNOTATE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V4_T>() + (arr.Length - 1)),
                                                                                                    1, showShutter, showAnalogGain, showLens,
                                                                                                    showCaf, showMotion, showFrame, enableTextBackground,
                                                                                                    customBackgroundColor, customBackgroundY, customBackgroundU, customBackgroundV, 0, customTextColor,
                                                                                                    customTextY, customTextU, customTextV, textSize, arr, (int)justify, xOffset, yOffset);

                var ptrV4 = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V4_T>() + (arr.Length - 1));
                Marshal.StructureToPtr(strV4, ptrV4, false);

                try
                {
                    MMALCheck(MMALPort.mmal_port_parameter_set(camera.Control.Ptr, (MMAL_PARAMETER_HEADER_T*)ptrV4),
                        "Unable to set annotate");
                }
                catch
                {
                    Marshal.FreeHGlobal(ptrV4);
                    ptrV4 = IntPtr.Zero;
                    
                    MMALLog.Logger.LogWarning("Unable to set V4 annotation structure. Trying V3. Please update firmware to latest version.");

                    var str = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(
                        new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_ANNOTATE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>() + (arr.Length - 1)),
                        1, showShutter, showAnalogGain, showLens,
                        showCaf, showMotion, showFrame, enableTextBackground,
                        customBackgroundColor, customBackgroundY, customBackgroundU, customBackgroundV, 0, customTextColor,
                        customTextY, customTextU, customTextV, textSize, arr);

                    var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>() + (arr.Length - 1));
                    Marshal.StructureToPtr(str, ptr, false);

                    try
                    {
                        MMALCheck(MMALPort.mmal_port_parameter_set(camera.Control.Ptr, (MMAL_PARAMETER_HEADER_T*)ptr), "Unable to set annotate V3.");
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptr);
                    }
                }
                finally
                {
                    if (ptrV4 != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(ptrV4);
                    }
                }
            }
        }

        internal static void DisableAnnotate(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T annotate = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_ANNOTATE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>()),
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &annotate.Hdr), "Unable to get camera annotate settings");

            MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T disableAnnotate = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_ANNOTATE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>()), 0,
                annotate.ShowShutter, annotate.ShowAnalogGain, annotate.ShowLens, annotate.ShowCaf, annotate.ShowMotion, annotate.ShowFrameNum,
                annotate.EnableTextBackground, annotate.CustomBackgroundColor, annotate.CustomBackgroundY, annotate.CustomBackgroundU, annotate.CustomBackgroundV, 0,
                annotate.CustomTextColor, annotate.CustomTextY, annotate.CustomTextU, annotate.CustomTextV, annotate.TextSize, annotate.Text);

            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>());
            Marshal.StructureToPtr(disableAnnotate, ptr, false);

            try
            {
                MMALCheck(MMALPort.mmal_port_parameter_set(camera.Control.Ptr, (MMAL_PARAMETER_HEADER_T*)ptr), "Unable to set annotate");
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// Gets the sensor mode currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The sensor mode.</returns>
        public static MMALSensorMode GetSensorMode(this MMALCameraComponent camera)
        {
            return (MMALSensorMode)(int)camera.Control.GetParameter(MMAL_PARAMETER_CAMERA_CUSTOM_SENSOR_CONFIG);
        }

        internal static void SetSensorMode(this MMALCameraComponent camera, MMALSensorMode mode)
        {
            var currentMode = (int)camera.Control.GetParameter(MMAL_PARAMETER_CAMERA_CUSTOM_SENSOR_CONFIG);

            // Don't try and set the sensor mode if we aren't changing it.
            if (currentMode != 0 || MMALCameraConfig.SensorMode != 0)
            {
                camera.Control.SetParameter(MMAL_PARAMETER_CAMERA_CUSTOM_SENSOR_CONFIG, MMALCameraConfig.SensorMode);
            }
        }

        /// <summary>
        /// Gets the saturation value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The saturation value.</returns>
        public static int GetSaturation(this MMALCameraComponent camera)
        {
            return (int)camera.Control.GetParameter(MMAL_PARAMETER_SATURATION);
        }

        internal static void SetSaturation(this MMALCameraComponent camera, int saturation)
        {
            MMALLog.Logger.LogDebug($"Setting saturation: {saturation}");

            var value = new MMAL_RATIONAL_T(saturation, 100);

            if (saturation >= -100 && saturation <= 100)
            {
                camera.Control.SetParameter(MMAL_PARAMETER_SATURATION, value);
            }
            else
            {
                throw new Exception("Invalid saturation value");
            }
        }

        /// <summary>
        /// Gets the sharpness value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The sharpness value.</returns>
        public static int GetSharpness(this MMALCameraComponent camera)
        {
            return (int)camera.Control.GetParameter(MMAL_PARAMETER_SHARPNESS);
        }

        internal static void SetSharpness(this MMALCameraComponent camera, int sharpness)
        {
            MMALLog.Logger.LogDebug($"Setting sharpness: {sharpness}");

            var value = new MMAL_RATIONAL_T(sharpness, 100);

            if (sharpness >= -100 && sharpness <= 100)
            {
                camera.Control.SetParameter(MMAL_PARAMETER_SHARPNESS, value);
            }
            else
            {
                throw new Exception("Invalid sharpness value");
            }
        }

        /// <summary>
        /// Gets the contrast value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The contrast value.</returns>
        public static int GetContrast(this MMALCameraComponent camera)
        {
            return (int)camera.Control.GetParameter(MMAL_PARAMETER_CONTRAST);
        }

        internal static void SetContrast(this MMALCameraComponent camera, int contrast)
        {
            MMALLog.Logger.LogDebug($"Setting contrast: {contrast}");

            var value = new MMAL_RATIONAL_T(contrast, 100);

            if (contrast >= -100 && contrast <= 100)
            {
                camera.Control.SetParameter(MMAL_PARAMETER_CONTRAST, value);
            }
            else
            {
                throw new Exception("Invalid contrast value");
            }
        }

        internal static void SetDisableExif(this MMALImageEncoder encoder, bool disable)
        {
            encoder.Outputs[0].SetParameter(MMAL_PARAMETER_EXIF_DISABLE, disable);
        }

        /// <summary>
        /// Gets the brightness value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The brightness value.</returns>
        public static int GetBrightness(this MMALCameraComponent camera)
        {
            return (int)camera.Control.GetParameter(MMAL_PARAMETER_BRIGHTNESS);
        }

        internal static void SetBrightness(this MMALCameraComponent camera, int brightness)
        {
            MMALLog.Logger.LogDebug($"Setting brightness: {brightness}");

            var value = new MMAL_RATIONAL_T(brightness, 100);

            if (brightness >= 0 && brightness <= 100)
            {
                camera.Control.SetParameter(MMAL_PARAMETER_BRIGHTNESS, value);
            }
            else
            {
                throw new Exception("Invalid brightness value");
            }
        }

        /// <summary>
        /// Gets the ISO value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The ISO value.</returns>
        public static int GetISO(this MMALCameraComponent camera)
        {
            return (int)camera.Control.GetParameter(MMAL_PARAMETER_ISO);
        }

        /// <summary>
        /// Sets the ISO to the specified value. Range from 100 to 800.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <param name="iso">The ISO value.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        internal static void SetISO(this MMALCameraComponent camera, int iso)
        {
            MMALLog.Logger.LogDebug($"Setting ISO: {iso}");

            // 0 = auto
            if ((iso < 100 || iso > 800) && iso > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(iso), iso, "Invalid ISO setting. Valid values: 100 - 800");
            }

            camera.Control.SetParameter(MMAL_PARAMETER_ISO, iso);
        }

        /// <summary>
        /// Gets the Video Stabilisation value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The Video Stabilisation value.</returns>
        public static bool GetVideoStabilisation(this MMALCameraComponent camera)
        {
            return camera.Control.GetParameter(MMAL_PARAMETER_VIDEO_STABILISATION);
        }

        internal static void SetVideoStabilisation(this MMALCameraComponent camera, bool vstabilisation)
        {
            MMALLog.Logger.LogDebug($"Setting video stabilisation: {vstabilisation}");

            camera.Control.SetParameter(MMAL_PARAMETER_VIDEO_STABILISATION, vstabilisation);
        }

        /// <summary>
        /// Gets the Exposure Compensation value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The Exposure Compensation value.</returns>
        public static int GetExposureCompensation(this MMALCameraComponent camera)
        {
            return camera.Control.GetParameter(MMAL_PARAMETER_EXPOSURE_COMP);
        }

        /// <summary>
        /// Sets the exposure compensation to the specified value. Range from -10 to 10.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <param name="expCompensation">The exposure compensation value.</param>
        internal static void SetExposureCompensation(this MMALCameraComponent camera, int expCompensation)
        {
            MMALLog.Logger.LogDebug($"Setting exposure compensation: {expCompensation}");

            if (expCompensation < -10 || expCompensation > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(expCompensation), expCompensation, "Invalid exposure compensation value. Valid values (-10 - 10)");
            }

            camera.Control.SetParameter(MMAL_PARAMETER_EXPOSURE_COMP, expCompensation);
        }

        /// <summary>
        /// Gets the Exposure Mode value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The Exposure Mode value.</returns>
        public static MMAL_PARAM_EXPOSUREMODE_T GetExposureMode(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_EXPOSUREMODE_T expMode = new MMAL_PARAMETER_EXPOSUREMODE_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_EXPOSURE_MODE, Marshal.SizeOf<MMAL_PARAMETER_EXPOSUREMODE_T>()),
                default(MMAL_PARAM_EXPOSUREMODE_T));

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &expMode.Hdr), "Unable to get exposure mode");

            return expMode.Value;
        }

        internal static void SetExposureMode(this MMALCameraComponent camera, MMAL_PARAM_EXPOSUREMODE_T mode)
        {
            MMALLog.Logger.LogDebug($"Setting exposure mode: {mode}");

            MMAL_PARAMETER_EXPOSUREMODE_T expMode = new MMAL_PARAMETER_EXPOSUREMODE_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_EXPOSURE_MODE, Marshal.SizeOf<MMAL_PARAMETER_EXPOSUREMODE_T>()),
                mode);

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Control.Ptr, &expMode.Hdr), "Unable to set exposure mode");
        }

        /// <summary>
        /// Gets the Exposure Metering Mode value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The Exposure Metering Mode value.</returns>
        public static MMAL_PARAM_EXPOSUREMETERINGMODE_T GetExposureMeteringMode(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_EXPOSUREMETERINGMODE_T expMode = new MMAL_PARAMETER_EXPOSUREMETERINGMODE_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_EXP_METERING_MODE, Marshal.SizeOf<MMAL_PARAMETER_EXPOSUREMETERINGMODE_T>()),
                default(MMAL_PARAM_EXPOSUREMETERINGMODE_T));

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &expMode.Hdr), "Unable to get exposure metering mode");

            return expMode.Value;
        }

        internal static void SetExposureMeteringMode(this MMALCameraComponent camera, MMAL_PARAM_EXPOSUREMETERINGMODE_T mode)
        {
            MMALLog.Logger.LogDebug($"Setting exposure metering mode: {mode}");

            MMAL_PARAMETER_EXPOSUREMETERINGMODE_T expMode = new MMAL_PARAMETER_EXPOSUREMETERINGMODE_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_EXP_METERING_MODE, Marshal.SizeOf<MMAL_PARAMETER_EXPOSUREMETERINGMODE_T>()),
                                                                                                        mode);

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Control.Ptr, &expMode.Hdr), "Unable to set exposure metering mode");
        }

        /// <summary>
        /// Gets the Automatic White Balance value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The Automatic White Balance value.</returns>
        public static MMAL_PARAM_AWBMODE_T GetAwbMode(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_AWBMODE_T awbMode = new MMAL_PARAMETER_AWBMODE_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_AWB_MODE, Marshal.SizeOf<MMAL_PARAMETER_AWBMODE_T>()),
                                                                                                        default(MMAL_PARAM_AWBMODE_T));

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &awbMode.Hdr), "Unable to get awb mode");

            return awbMode.Value;
        }

        internal static void SetAwbMode(this MMALCameraComponent camera, MMAL_PARAM_AWBMODE_T mode)
        {
            MMALLog.Logger.LogDebug($"Setting AWB mode: {mode}");

            MMAL_PARAMETER_AWBMODE_T awbMode = new MMAL_PARAMETER_AWBMODE_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_AWB_MODE, Marshal.SizeOf<MMAL_PARAMETER_AWBMODE_T>()),
                                                                                                        mode);

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Control.Ptr, &awbMode.Hdr), "Unable to set awb mode");
        }

        /// <summary>
        /// Gets the Exposure Speed value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The Exposure Speed value.</returns>
        public static int GetExposureSpeed(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_CAMERA_SETTINGS_T settings = new MMAL_PARAMETER_CAMERA_SETTINGS_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_CAMERA_SETTINGS, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_SETTINGS_T>()),
                0, new MMAL_RATIONAL_T(0, 0), new MMAL_RATIONAL_T(0, 0),
                new MMAL_RATIONAL_T(0, 0), new MMAL_RATIONAL_T(0, 0), 0);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &settings.Hdr), "Unable to get camera settings");

            return settings.Exposure;
        }

        /// <summary>
        /// Gets the Focus Position value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The Focus Position value.</returns>
        public static int GetFocusPosition(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_CAMERA_SETTINGS_T settings = new MMAL_PARAMETER_CAMERA_SETTINGS_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_CAMERA_SETTINGS, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_SETTINGS_T>()),
                0, new MMAL_RATIONAL_T(0, 0), new MMAL_RATIONAL_T(0, 0),
                new MMAL_RATIONAL_T(0, 0), new MMAL_RATIONAL_T(0, 0), 0);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &settings.Hdr), "Unable to get camera settings");

            return settings.FocusPosition;
        }

        /// <summary>
        /// Gets the AWB Red Gain value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The AWB Red Gain value.</returns>
        public static double GetAwbRedGain(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_CAMERA_SETTINGS_T settings = new MMAL_PARAMETER_CAMERA_SETTINGS_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_CAMERA_SETTINGS, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_SETTINGS_T>()),
                0, new MMAL_RATIONAL_T(0, 0), new MMAL_RATIONAL_T(0, 0),
                new MMAL_RATIONAL_T(0, 0), new MMAL_RATIONAL_T(0, 0), 0);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &settings.Hdr), "Unable to get camera settings");

            return Convert.ToDouble(settings.AwbRedGain.Num / settings.AwbRedGain.Den);
        }

        /// <summary>
        /// Gets the AWB Blue Gain value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The AWB Blue Gain value.</returns>
        public static double GetAwbBlueGain(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_CAMERA_SETTINGS_T settings = new MMAL_PARAMETER_CAMERA_SETTINGS_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_CAMERA_SETTINGS, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_SETTINGS_T>()),
                0, new MMAL_RATIONAL_T(0, 0), new MMAL_RATIONAL_T(0, 0),
                new MMAL_RATIONAL_T(0, 0), new MMAL_RATIONAL_T(0, 0), 0);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &settings.Hdr), "Unable to get camera settings");

            return Convert.ToDouble(settings.AwbBlueGain.Num / settings.AwbBlueGain.Den);
        }

        internal static void SetAwbGains(this MMALCameraComponent camera, double rGain, double bGain)
        {
            MMALLog.Logger.LogDebug($"Setting AWB gains: {rGain}, {bGain}");

            if (MMALCameraConfig.AwbMode != MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_OFF && (rGain > 0 || bGain > 0))
            {
                throw new PiCameraError("AWB Mode must be off when setting AWB gains");
            }

            MMAL_PARAMETER_AWB_GAINS_T awbGains = new MMAL_PARAMETER_AWB_GAINS_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_CUSTOM_AWB_GAINS, Marshal.SizeOf<MMAL_PARAMETER_AWB_GAINS_T>()),
                                                                                                        new MMAL_RATIONAL_T((int)(rGain * 65536), 65536),
                                                                                                        new MMAL_RATIONAL_T((int)(bGain * 65536), 65536));

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Control.Ptr, &awbGains.Hdr), "Unable to set awb gains");
        }

        /// <summary>
        /// Gets the Image Effects value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The Image Effects value.</returns>
        public static MMAL_PARAM_IMAGEFX_T GetImageFx(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_IMAGEFX_T imgFx = new MMAL_PARAMETER_IMAGEFX_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_IMAGE_EFFECT, Marshal.SizeOf<MMAL_PARAMETER_IMAGEFX_T>()),
                                                                                                        default(MMAL_PARAM_IMAGEFX_T));

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &imgFx.Hdr), "Unable to get image fx");

            return imgFx.Value;
        }

        internal static void SetImageFx(this MMALCameraComponent camera, MMAL_PARAM_IMAGEFX_T imageFx)
        {
            MMALLog.Logger.LogDebug($"Setting Image FX: {imageFx}");

            MMAL_PARAMETER_IMAGEFX_T imgFx = new MMAL_PARAMETER_IMAGEFX_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_IMAGE_EFFECT, Marshal.SizeOf<MMAL_PARAMETER_IMAGEFX_T>()),
                imageFx);

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Control.Ptr, &imgFx.Hdr), "Unable to set image fx");
        }

        /// <summary>
        /// Gets the Colour Effects value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The Colour Effects value.</returns>
        public static ColourEffects GetColourFx(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_COLOURFX_T colFx = new MMAL_PARAMETER_COLOURFX_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_COLOUR_EFFECT, Marshal.SizeOf<MMAL_PARAMETER_COLOURFX_T>()),
                                                                                                        0,
                                                                                                        0,
                                                                                                        0);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &colFx.Hdr), "Unable to get colour fx");

            ColourEffects fx = new ColourEffects(colFx.Enable == 1, MMALColor.FromYUVBytes(0, (byte)colFx.U, (byte)colFx.V));
                        
            return fx;
        }

        internal static void SetColourFx(this MMALCameraComponent camera, ColourEffects colourFx)
        {
            MMALLog.Logger.LogDebug("Setting colour effects");

            var uv = MMALColor.RGBToYUVBytes(colourFx.Color);

            MMAL_PARAMETER_COLOURFX_T colFx = new MMAL_PARAMETER_COLOURFX_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_COLOUR_EFFECT, Marshal.SizeOf<MMAL_PARAMETER_COLOURFX_T>()),
                                                                                                        colourFx.Enable ? 1 : 0,
                                                                                                        uv.Item2,
                                                                                                        uv.Item3);

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Control.Ptr, &colFx.Hdr), "Unable to set colour fx");
        }

        /// <summary>
        /// Gets the Rotation value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The Rotation value.</returns>
        public static int GetRotation(this MMALCameraComponent camera)
        {
            return camera.StillPort.GetParameter(MMAL_PARAMETER_ROTATION);
        }

        internal static void SetRotation(this MMALCameraComponent camera, int rotation)
        {
            int rot = ((rotation % 360) / 90) * 90;

            MMALLog.Logger.LogDebug($"Setting rotation: {rot}");

            camera.StillPort.SetParameter(MMAL_PARAMETER_ROTATION, rot);
        }

        /// <summary>
        /// Gets the Flips value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The Flips value.</returns>
        public static MMAL_PARAM_MIRROR_T GetFlips(this MMALCameraComponent camera) => GetStillFlips(camera);

        /// <summary>
        /// Gets the Flips value currently being used by the video port.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The Flips value.</returns>
        public static MMAL_PARAM_MIRROR_T GetVideoFlips(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_MIRROR_T mirror = new MMAL_PARAMETER_MIRROR_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_MIRROR, Marshal.SizeOf<MMAL_PARAMETER_MIRROR_T>()),
                MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_NONE);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.VideoPort.Ptr, &mirror.Hdr), "Unable to get flips");

            return mirror.Value;
        }

        /// <summary>
        /// Gets the Flips value currently being used by the still port.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The Flips value.</returns>
        public static MMAL_PARAM_MIRROR_T GetStillFlips(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_MIRROR_T mirror = new MMAL_PARAMETER_MIRROR_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_MIRROR, Marshal.SizeOf<MMAL_PARAMETER_MIRROR_T>()),
                MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_NONE);

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.StillPort.Ptr, &mirror.Hdr), "Unable to get flips");

            return mirror.Value;
        }

        internal static void SetFlips(this MMALCameraComponent camera, MMAL_PARAM_MIRROR_T flips)
        {
            MMAL_PARAMETER_MIRROR_T mirror = new MMAL_PARAMETER_MIRROR_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_MIRROR, Marshal.SizeOf<MMAL_PARAMETER_MIRROR_T>()),
                flips);

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.StillPort.Ptr, &mirror.Hdr), "Unable to set flips");
            MMALCheck(MMALPort.mmal_port_parameter_set(camera.VideoPort.Ptr, &mirror.Hdr), "Unable to set flips");
        }

        /// <summary>
        /// Gets the Zoom (Region of Interest) value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The Zoom value.</returns>
        public static MMAL_RECT_T GetZoom(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_INPUT_CROP_T crop = new MMAL_PARAMETER_INPUT_CROP_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_INPUT_CROP, Marshal.SizeOf<MMAL_PARAMETER_INPUT_CROP_T>()),
                default(MMAL_RECT_T));

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &crop.Hdr), "Unable to get zoom");

            return crop.Rect;
        }

        /// <summary>
        /// Sets the zoom to the specified value. Each parameter must not be greater than 1.0.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <param name="rect">The region of interest.</param>
        internal static void SetZoom(this MMALCameraComponent camera, Zoom rect)
        {
            if (rect.X > 1.0 || rect.Y > 1.0 || rect.Height > 1.0 || rect.Width > 1.0)
            {
                throw new ArgumentOutOfRangeException(nameof(rect), "Invalid zoom settings. Value mustn't be greater than 1.0");
            }

            MMAL_PARAMETER_INPUT_CROP_T crop = new MMAL_PARAMETER_INPUT_CROP_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_INPUT_CROP, Marshal.SizeOf<MMAL_PARAMETER_INPUT_CROP_T>()),
                new MMAL_RECT_T(Convert.ToInt32(65536 * rect.X), Convert.ToInt32(65536 * rect.Y), Convert.ToInt32(65536 * rect.Width), Convert.ToInt32(65536 * rect.Height)));

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Control.Ptr, &crop.Hdr), "Unable to set zoom");
        }

        /// <summary>
        /// Gets the Shutter Speed value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The Shutter Speed value.</returns>
        public static int GetShutterSpeed(this MMALCameraComponent camera)
        {
            return (int)camera.Control.GetParameter(MMAL_PARAMETER_SHUTTER_SPEED);
        }

        internal static void SetShutterSpeed(this MMALCameraComponent camera, int speed)
        {
            MMALLog.Logger.LogDebug($"Setting shutter speed: {speed}");

            if (speed > 6000000)
            {
                MMALLog.Logger.LogWarning("Shutter speed exceeds upper supported limit of 6000ms. Undefined behaviour may result.");
            }

            camera.Control.SetParameter(MMAL_PARAMETER_SHUTTER_SPEED, speed);
        }

        /// <summary>
        /// Gets the Dynamic Range Compression value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The Dynamic Range Compression value.</returns>
        public static MMAL_PARAMETER_DRC_STRENGTH_T GetDRC(this MMALCameraComponent camera)
        {
            MMAL_PARAMETER_DRC_T drc = new MMAL_PARAMETER_DRC_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_DYNAMIC_RANGE_COMPRESSION, Marshal.SizeOf<MMAL_PARAMETER_DRC_T>()),
                default(MMAL_PARAMETER_DRC_STRENGTH_T));

            MMALCheck(MMALPort.mmal_port_parameter_get(camera.Control.Ptr, &drc.Hdr), "Unable to get DRC");

            return drc.Strength;
        }

        internal static void SetDRC(this MMALCameraComponent camera, MMAL_PARAMETER_DRC_STRENGTH_T strength)
        {
            MMAL_PARAMETER_DRC_T drc = new MMAL_PARAMETER_DRC_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_DYNAMIC_RANGE_COMPRESSION, Marshal.SizeOf<MMAL_PARAMETER_DRC_T>()),
                strength);

            MMALCheck(MMALPort.mmal_port_parameter_set(camera.Control.Ptr, &drc.Hdr), "Unable to set DRC");
        }

        /// <summary>
        /// Gets the Statistics Pass value currently being used by the camera.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The Statistics Pass value.</returns>
        public static bool GetStatsPass(this MMALCameraComponent camera)
        {
            return camera.Control.GetParameter(MMAL_PARAMETER_CAPTURE_STATS_PASS);
        }

        internal static void SetStatsPass(this MMALCameraComponent camera, bool statsPass)
        {
            camera.Control.SetParameter(MMAL_PARAMETER_CAPTURE_STATS_PASS, statsPass);
        }

        /// <summary>
        /// Gets whether the camera's still port has burst mode enabled.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>True if burst mode is enabled.</returns>
        public static bool GetBurstMode(this MMALCameraComponent camera)
        {
            return camera.StillPort.GetParameter(MMAL_PARAMETER_CAMERA_BURST_CAPTURE);
        }

        internal static void SetBurstMode(this MMALCameraComponent camera, bool burstMode)
        {
            camera.StillPort.SetParameter(MMAL_PARAMETER_CAMERA_BURST_CAPTURE, burstMode);
        }

        /// <summary>
        /// Used to retrieve the analog gain value from the sensor.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The analog gain value.</returns>
        public static double GetAnalogGain(this MMALCameraComponent camera)
        {
            return (double)camera.Control.GetParameter(MMAL_PARAMETER_ANALOG_GAIN);
        }

        internal static void SetAnalogGain(this MMALCameraComponent camera, double analogGain)
        {
            if (analogGain > 0 && (analogGain < 1.0 || analogGain > 8.0))
            {
                throw new ArgumentOutOfRangeException(nameof(analogGain), "Invalid analog gain settings. Value must be between 1.0 and 8.0.");
            }

            var num = (int)analogGain * 65536;

            camera.Control.SetParameter(MMAL_PARAMETER_ANALOG_GAIN, new MMAL_RATIONAL_T(num, 65536));
        }

        /// <summary>
        /// Used to retrieve the digital gain value from the ISP.
        /// </summary>
        /// <param name="camera">The camera component.</param>
        /// <returns>The digital gain value.</returns>
        public static double GetDigitalGain(this MMALCameraComponent camera)
        {
            return (double)camera.Control.GetParameter(MMAL_PARAMETER_DIGITAL_GAIN);
        }

        internal static void SetDigitalGain(this MMALCameraComponent camera, double digitalGain)
        {
            if (digitalGain > 0 && (digitalGain < 1.0 || digitalGain > 255.0))
            {
                throw new ArgumentOutOfRangeException(nameof(digitalGain), "Invalid digital gain settings. Value must be between 1.0 and 255.0.");
            }

            var num = (int)digitalGain * 65536;

            camera.Control.SetParameter(MMAL_PARAMETER_DIGITAL_GAIN, new MMAL_RATIONAL_T(num, 65536));
        }
    }
}
#pragma warning restore SA1202 // Public methods before Internal