// <copyright file="BcmHost.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Runtime.InteropServices;

namespace MMALSharp.Native
{
    /// <summary>
    /// Provides interop methods for libbcm_host, the Broadcom hardware interface library.
    /// </summary>
    public static class BcmHost
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE1006 // Naming Styles

        [DllImport("libbcm_host.so", EntryPoint = "bcm_host_init", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bcm_host_init();

        [DllImport("libbcm_host.so", EntryPoint = "bcm_host_deinit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bcm_host_deinit();

        [DllImport("libbcm_host.so", EntryPoint = "graphics_get_display_size", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int graphics_get_display_size(ushort display_number, uint* width, uint* height);
    }
}
