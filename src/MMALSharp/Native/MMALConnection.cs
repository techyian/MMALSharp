// <copyright file="MMALConnection.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace MMALSharp.Native
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class MMALConnection
    {
        public const uint MMAL_CONNECTION_FLAG_TUNNELLING = 0x1u;
        public const uint MMAL_CONNECTION_FLAG_ALLOCATION_ON_INPUT = 0x2u;
        public const uint MMAL_CONNECTION_FLAG_ALLOCATION_ON_OUTPUT = 0x4u;
        public const uint MMAL_CONNECTION_FLAG_KEEP_BUFFER_REQUIREMENTS = 0x8u;
        public const uint MMAL_CONNECTION_FLAG_DIRECT = 0x10u;

        // typedef - Pointer to MMAL_CONNECTION_T -> Returns MMAL_BOOL_T
        public unsafe delegate int MMAL_CONNECTION_CALLBACK_T(MMAL_CONNECTION_T* conn);

#pragma warning disable IDE1006 // Naming Styles
        [DllImport("libmmal.so", EntryPoint = "mmal_connection_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_connection_create(IntPtr* connection, MMAL_PORT_T* output, MMAL_PORT_T* input, uint flags);

        [DllImport("libmmal.so", EntryPoint = "mmal_connection_acquire", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void mmal_connection_acquire(MMAL_CONNECTION_T* connection);

        [DllImport("libmmal.so", EntryPoint = "mmal_connection_release", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_connection_release(MMAL_CONNECTION_T* connection);

        [DllImport("libmmal.so", EntryPoint = "mmal_connection_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_connection_destroy(MMAL_CONNECTION_T* connection);

        [DllImport("libmmal.so", EntryPoint = "mmal_connection_enable", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_connection_enable(MMAL_CONNECTION_T* connection);

        [DllImport("libmmal.so", EntryPoint = "mmal_connection_disable", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_connection_disable(MMAL_CONNECTION_T* connection);

        [DllImport("libmmal.so", EntryPoint = "mmal_connection_event_format_changed", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_connection_event_format_changed(MMAL_CONNECTION_T* connection, MMAL_BUFFER_HEADER_T* buffer);
#pragma warning restore IDE1006 // Naming Styles
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_CONNECTION_T
    {
        public IntPtr UserData { get; }

        public IntPtr Callback { get; set; }

        public uint IsEnabled { get; }

        public uint Flags { get; }

        public MMAL_PORT_T* Input { get; }

        public MMAL_PORT_T* Output { get; }

        public MMAL_POOL_T* Pool { get; }

        public MMAL_QUEUE_T* Queue { get; }

        public char* Name { get; }

        public long TimeSetup { get; }

        public long TimeEnable { get; }

        public long TimeDisable { get; }

        public MMAL_CONNECTION_T(IntPtr userData, IntPtr callback, uint isEnabled, uint flags, MMAL_PORT_T* input, MMAL_PORT_T* output,
                                 MMAL_POOL_T* pool, MMAL_QUEUE_T* queue, char* name, long timeSetup, long timeEnable, long timeDisable)
        {
            this.UserData = userData;
            this.Callback = callback;
            this.IsEnabled = isEnabled;
            this.Flags = flags;
            this.Input = input;
            this.Output = output;
            this.Pool = pool;
            this.Queue = queue;
            this.Name = name;
            this.TimeSetup = timeSetup;
            this.TimeEnable = timeEnable;
            this.TimeDisable = timeDisable;
        }
    }
}
