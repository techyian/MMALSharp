// <copyright file="PortExtensions.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Config;
using MMALSharp.Native;
using MMALSharp.Ports;
using static MMALSharp.MMALNativeExceptionHelper;
using static MMALSharp.Native.MMALParametersCamera;

namespace MMALSharp
{
#pragma warning disable SA1202 // Public methods before Internal
    /// <summary>
    /// Provides extension methods to obtain and change the configuration of a port.
    /// </summary>
    public static class PortExtensions
    {
        /// <summary>
        /// Provides a facility to get data from the port using the native helper functions.
        /// </summary>
        /// <param name="port">The port to get the parameter from.</param>
        /// <param name="key">The unique key for the parameter.</param>
        /// <returns>Dynamic parameter based on key parameter.</returns>
        public static unsafe dynamic GetParameter(this IPort port, int key)
        {
            var t = MMALParameterHelpers.ParameterHelper.Where(c => c.ParamValue == key).FirstOrDefault();

            if (t == null)
            {
                throw new PiCameraError($"Could not find parameter {key}");
            }

            MMALLog.Logger.LogDebug($"Getting parameter {t.ParamName}");

            try
            {
                switch (t.ParamType.Name)
                {
                    case "MMAL_PARAMETER_BOOLEAN_T":
                        int boolVal = 0;
                        MMALCheck(MMALUtil.mmal_port_parameter_get_boolean(port.Ptr, (uint)key, ref boolVal), "Unable to get boolean value");
                        return boolVal == 1;
                    case "MMAL_PARAMETER_UINT64_T":
                        ulong ulongVal = 0UL;
                        MMALCheck(MMALUtil.mmal_port_parameter_get_uint64(port.Ptr, (uint)key, ref ulongVal), "Unable to get ulong value");
                        return ulongVal;
                    case "MMAL_PARAMETER_INT64_T":
                        long longVal = 0U;
                        MMALCheck(MMALUtil.mmal_port_parameter_get_int64(port.Ptr, (uint)key, ref longVal), "Unable to get long value");
                        return longVal;
                    case "MMAL_PARAMETER_UINT32_T":
                        uint uintVal = 0U;
                        MMALCheck(MMALUtil.mmal_port_parameter_get_uint32(port.Ptr, (uint)key, ref uintVal), "Unable to get uint value");
                        return uintVal;
                    case "MMAL_PARAMETER_INT32_T":
                        int intVal = 0;
                        MMALCheck(MMALUtil.mmal_port_parameter_get_int32(port.Ptr, (uint)key, ref intVal), "Unable to get int value");
                        return intVal;
                    case "MMAL_PARAMETER_RATIONAL_T":
                        MMAL_RATIONAL_T ratVal = default(MMAL_RATIONAL_T);
                        MMALCheck(MMALUtil.mmal_port_parameter_get_rational(port.Ptr, (uint)key, ref ratVal), "Unable to get rational value");
                        return (double)ratVal.Num / ratVal.Den;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch
            {
                MMALLog.Logger.LogWarning($"Unable to get port parameter {t.ParamName}");
                throw;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether to include raw Bayer image data on this port.
        /// </summary>
        /// <param name="port">The port you are querying.</param>
        /// <returns>True if raw Bayer image data will be returned.</returns>
        public static bool GetRawCapture(this IPort port)
        {
            return port.GetParameter(MMAL_PARAMETER_ENABLE_RAW_CAPTURE);
        }

        /// <summary>
        /// Retrieves an array of FourCC integer codes that this port is compatible with.
        /// </summary>
        /// <param name="port">The port we are getting supported encodings for.</param>
        /// <returns>An array of FourCC integers.</returns>
        public static unsafe int[] GetSupportedEncodings(this IPort port)
        {
            IntPtr ptr1 = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_ENCODING_T>() + 20);
            var str1 = (MMAL_PARAMETER_HEADER_T*)ptr1;

            str1->Id = MMALParametersCommon.MMAL_PARAMETER_SUPPORTED_ENCODINGS;

            // Deliberately undersize to check if running on older firmware.
            str1->Size = Marshal.SizeOf<MMAL_PARAMETER_ENCODING_T>() + 20;

            MMAL_PARAMETER_ENCODING_T encodings;

            try
            {
                MMALCheck(MMALPort.mmal_port_parameter_get(port.Ptr, str1), "Unable to get supported encodings");
                encodings = (MMAL_PARAMETER_ENCODING_T)Marshal.PtrToStructure(ptr1, typeof(MMAL_PARAMETER_ENCODING_T));
                return encodings.Value;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr1);
            }
        }

        /// <summary>
        /// Returns the type of port in string format.
        /// </summary>
        /// <param name="type">The port type.</param>
        /// <returns>A string representation of the port type.</returns>
        public static string GetPortType(this PortType type)
        {
            switch (type)
            {
                case PortType.Input:
                    return "Input";
                case PortType.Output:
                    return "Output";
                case PortType.Clock:
                    return "Clock";
                case PortType.Control:
                    return "Control";
            }

            return string.Empty;
        }
        
        /// <summary>
        /// Provides a facility to set data on the port using the native helper functions.
        /// </summary>
        /// <param name="port">The port we want to set the parameter on.</param>
        /// <param name="key">The unique key of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        internal static unsafe void SetParameter(this IPort port, int key, dynamic value)
        {
            var t = MMALParameterHelpers.ParameterHelper.FirstOrDefault(c => c.ParamValue == key);

            if (t == null)
            {
                throw new PiCameraError($"Could not find parameter {key}");
            }

            MMALLog.Logger.LogDebug($"Setting parameter {t.ParamName}");

            try
            {
                switch (t.ParamType.Name)
                {
                    case "MMAL_PARAMETER_BOOLEAN_T":
                        int i = (bool)value ? 1 : 0;
                        MMALCheck(MMALUtil.mmal_port_parameter_set_boolean(port.Ptr, (uint)key, i), "Unable to set boolean value");
                        break;
                    case "MMAL_PARAMETER_UINT64_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_uint64(port.Ptr, (uint)key, (ulong)value), "Unable to set ulong value");
                        break;
                    case "MMAL_PARAMETER_INT64_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_int64(port.Ptr, (uint)key, (long)value), "Unable to set long value");
                        break;
                    case "MMAL_PARAMETER_UINT32_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_uint32(port.Ptr, (uint)key, (uint)value), "Unable to set uint value");
                        break;
                    case "MMAL_PARAMETER_INT32_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_int32(port.Ptr, (uint)key, (int)value), "Unable to set int value");
                        break;
                    case "MMAL_PARAMETER_RATIONAL_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_rational(port.Ptr, (uint)key, (MMAL_RATIONAL_T)value), "Unable to set rational value");
                        break;
                    case "MMAL_PARAMETER_STRING_T":
                        MMALCheck(MMALUtil.mmal_port_parameter_set_string(port.Ptr, (uint)key, (string)value), "Unable to set rational value");
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch
            {
                MMALLog.Logger.LogWarning($"Unable to set port parameter {t.ParamName}");
                throw;
            }
        }

        /// <summary>
        /// Starts or stops image processing on a port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="enable">Set true to start image capture.</param>
        internal static void SetImageCapture(this IPort port, bool enable)
        {
            port.SetParameter(MMAL_PARAMETER_CAPTURE, enable);
        }

        /// <summary>
        /// Enables or disables inclusion of raw Bayer metadata on this port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="raw">Set true to include Bayer metadata.</param>
        internal static void SetRawCapture(this IPort port, bool raw)
        {
            port.SetParameter(MMAL_PARAMETER_ENABLE_RAW_CAPTURE, raw);
        }

        internal static unsafe void SetStereoMode(this IPort port, StereoMode mode)
        {
            MMAL_PARAMETER_STEREOSCOPIC_MODE_T stereo = new MMAL_PARAMETER_STEREOSCOPIC_MODE_T(
                new MMAL_PARAMETER_HEADER_T(MMAL_PARAMETER_STEREOSCOPIC_MODE, Marshal.SizeOf<MMAL_PARAMETER_STEREOSCOPIC_MODE_T>()),
                mode.Mode,
                mode.Decimate,
                mode.SwapEyes);

            MMALCheck(MMALPort.mmal_port_parameter_set(port.Ptr, &stereo.Hdr), "Unable to set Stereo mode");
        }

        internal static void CheckSupportedEncoding(this IPort port, MMALEncoding encoding)
        {
            var encodings = port.GetSupportedEncodings();

            if (!encodings.Any(c => c == encoding.EncodingVal))
            {
                throw new PiCameraError("Unsupported encoding type for this port");
            }
        }

        internal static bool RgbOrderFixed(this IPort port)
        {
            int newFirmware = 0;
            var encodings = port.GetSupportedEncodings();

            foreach (int enc in encodings)
            {
                if (enc == Helpers.FourCCFromString("BGR3"))
                {
                    break;
                }

                if (enc == Helpers.FourCCFromString("RGB3"))
                {
                    newFirmware = 1;
                }
            }

            return newFirmware == 1;
        }

        /// <summary>
        /// Retrieves the currently configured framerate range for a given port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns>A <see cref="MMAL_PARAMETER_FPS_RANGE_T"/> structure containing the configured framerate range.</returns>
        public static unsafe MMAL_PARAMETER_FPS_RANGE_T GetFramerateRange(this IPort port)
        {
            var str = new MMAL_PARAMETER_FPS_RANGE_T(
                    new MMAL_PARAMETER_HEADER_T(
                        MMALParametersCamera.MMAL_PARAMETER_FPS_RANGE,
                        Marshal.SizeOf<MMAL_PARAMETER_FPS_RANGE_T>()), default(MMAL_RATIONAL_T), default(MMAL_RATIONAL_T));

            MMALCheck(MMALPort.mmal_port_parameter_get(port.Ptr, &str.Hdr), "Unable to get framerate range for port.");

            return str;
        }

        internal static unsafe void SetFramerateRange(this IPort port, MMAL_RATIONAL_T fpsLow, MMAL_RATIONAL_T fpsHigh)
        {
            var str = new MMAL_PARAMETER_FPS_RANGE_T(
                    new MMAL_PARAMETER_HEADER_T(
                        MMALParametersCamera.MMAL_PARAMETER_FPS_RANGE,
                        Marshal.SizeOf<MMAL_PARAMETER_FPS_RANGE_T>()), fpsLow, fpsHigh);

            MMALCheck(MMALPort.mmal_port_parameter_set(port.Ptr, &str.Hdr), "Unable to set framerate range for port.");
        }
    }
}
#pragma warning restore SA1202 // Public methods before Internal