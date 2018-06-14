// <copyright file="MMALPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace MMALSharp.Native
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class MMALPort
    {
        public enum MMAL_PORT_TYPE_T
        {
            MMAL_PORT_TYPE_UNKNOWN,
            MMAL_PORT_TYPE_CONTROL,
            MMAL_PORT_TYPE_INPUT,
            MMAL_PORT_TYPE_OUTPUT,
            MMAL_PORT_TYPE_CLOCK
        }

        public const uint MMAL_PORT_TYPE_INVALID = 0xffffffff;
        public const int MMAL_PORT_CAPABILITY_PASSTHROUGH = 0x01;
        public const int MMAL_PORT_CAPABILITY_ALLOCATION = 0x02;
        public const int MMAL_PORT_CAPABILITY_SUPPORTS_EVENT_FORMAT_CHANGE = 0x04;

#pragma warning disable IDE1006 // Naming Styles

        // MMAL_PORT_T* port    
        [DllImport("libmmal.so", EntryPoint = "mmal_port_format_commit", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_port_format_commit(MMAL_PORT_T* port);

        // typedef - Pointer to MMAL_PORT_T * Pointer to MMAL_BUFFER_HEADER_T -> Returns void
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void MMAL_PORT_BH_CB_T(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer);

        // MMAL_PORT_T* port * MMAL_PORT_BH_CB_T cb -> Returns MMAL_STATUS_T
        [DllImport("libmmal.so", EntryPoint = "mmal_port_enable", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_port_enable(MMAL_PORT_T* port, IntPtr cb);

        [DllImport("libmmal.so", EntryPoint = "mmal_wrapper_port_enable", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_wrapper_port_enable(MMAL_PORT_T* port, uint flags);
        
        // MMAL_PORT_T* port -> Returns MMAL_STATUS_T
        [DllImport("libmmal.so", EntryPoint = "mmal_port_disable", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_port_disable(MMAL_PORT_T* port);

        [DllImport("libmmal.so", EntryPoint = "mmal_wrapper_port_disable", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_wrapper_port_disable(MMAL_PORT_T* port);

        // MMAL_PORT_T* port -> Returns MMAL_STATUS_T
        [DllImport("libmmal.so", EntryPoint = "mmal_port_flush", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_port_flush(MMAL_PORT_T* port);

        // MMAL_PORT_T* port * MMAL_PARAMETER_HEADER_T* header
        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_set", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_port_parameter_set(MMAL_PORT_T* port, MMAL_PARAMETER_HEADER_T* header);

        // MMAL_PORT_T* port * MMAL_PARAMETER_HEADER_T* header
        [DllImport("libmmal.so", EntryPoint = "mmal_port_parameter_get", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_port_parameter_get(MMAL_PORT_T* port, MMAL_PARAMETER_HEADER_T* header);

        // MMAL_PORT_T* port * MMAL_BUFFER_HEADER_T* header
        [DllImport("libmmal.so", EntryPoint = "mmal_port_send_buffer", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_port_send_buffer(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* header);

        // MMAL_PORT_T* port * MMAL_PORT_T* port2
        [DllImport("libmmal.so", EntryPoint = "mmal_port_connect", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_port_connect(MMAL_PORT_T* port, MMAL_PORT_T* port2);

        // MMAL_PORT_T* port
        [DllImport("libmmal.so", EntryPoint = "mmal_port_disconnect", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_port_disconnect(MMAL_PORT_T* port);

        // MMAL_PORT_T* port * UInt32 payload_size 
        [DllImport("libmmal.so", EntryPoint = "mmal_port_payload_alloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* mmal_port_payload_alloc(MMAL_PORT_T* port, [In] uint payload_size);

        // MMAL_PORT_T* port * uint8* payload_size
        [DllImport("libmmal.so", EntryPoint = "mmal_port_payload_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void mmal_port_payload_free(MMAL_PORT_T* port, [In] ref byte payload_size);

        // MMAL_PORT_T* port * MMAL_BUFFER_HEADER_T** buffer
        [DllImport("libmmal.so", EntryPoint = "mmal_port_event_get", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_port_event_get(MMAL_PORT_T* port, IntPtr* buffer);
#pragma warning restore IDE1006 // Naming Styles
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_PORT_PRIVATE_T
    {
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_PORT_T
    {
        public IntPtr Priv { get; }

        public char* Name { get; }

        public MMALPort.MMAL_PORT_TYPE_T Type { get; }

        public ushort Index { get; }

        public ushort IndexAll { get; }

        public int IsEnabled { get; }

        public MMAL_ES_FORMAT_T* Format { get; }

        public int BufferNumMin { get; }

        public int BufferSizeMin { get; }

        public int BufferAlignmentMin { get; }

        public int BufferNumRecommended { get; }

        public int BufferSizeRecommended { get; }

        public int BufferNum { get; set; }

        public int BufferSize { get; set; }

        public MMAL_COMPONENT_T* Component { get; }

        public IntPtr UserData { get; }

        public uint Capabilities { get; }

        public MMAL_PORT_T(IntPtr priv, char* name, MMALPort.MMAL_PORT_TYPE_T type, ushort index, ushort indexAll,
                           int isEnabled, MMAL_ES_FORMAT_T* format, int bufferNumMin, int bufferSizeMin, int bufferAlignmentMin, 
                           int bufferNumRecommended, int bufferSizeRecommended, int bufferNum, int bufferSize, MMAL_COMPONENT_T* component, 
                           IntPtr userData, uint capabilities)
        {
            this.Priv = priv;
            this.Name = name;
            this.Type = type;
            this.Index = index;
            this.IndexAll = indexAll;
            this.IsEnabled = isEnabled;
            this.Format = format;
            this.BufferNumMin = bufferNumMin;
            this.BufferSizeMin = bufferSizeMin;
            this.BufferAlignmentMin = bufferAlignmentMin;
            this.BufferNumRecommended = bufferNumRecommended;
            this.BufferSizeRecommended = bufferSizeRecommended;
            this.BufferNum = bufferNum;
            this.BufferSize = bufferSize;
            this.Component = component;
            this.UserData = userData;
            this.Capabilities = capabilities;
        }
    }
}
