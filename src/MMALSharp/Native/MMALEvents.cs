// <copyright file="MMALEvents.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Runtime.InteropServices;

namespace MMALSharp.Native
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class MMALEvents
    {
        public static int MMAL_EVENT_ERROR = MMALUtil.MMAL_FOURCC("ERRO");
        public static int MMAL_EVENT_EOS = MMALUtil.MMAL_FOURCC("EEOS");
        public static int MMAL_EVENT_FORMAT_CHANGED = MMALUtil.MMAL_FOURCC("EFCH");
        public static int MMAL_EVENT_PARAMETER_CHANGED = MMALUtil.MMAL_FOURCC("EPCH");

#pragma warning disable IDE1006 // Naming Styles
        [DllImport("libmmal.so", EntryPoint = "mmal_event_format_changed_get", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMAL_EVENT_FORMAT_CHANGED_T* mmal_event_format_changed_get(MMAL_BUFFER_HEADER_T* buffer);
#pragma warning restore IDE1006 // Naming Styles
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_EVENT_END_OF_STREAM_T
    {
        public MMALPort.MMAL_PORT_TYPE_T PortType { get; }

        public uint PortIndex { get; }

        public MMAL_EVENT_END_OF_STREAM_T(MMALPort.MMAL_PORT_TYPE_T portType, uint portIndex)
        {
            this.PortType = portType;
            this.PortIndex = portIndex;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_EVENT_FORMAT_CHANGED_T
    {
        public uint BufferSizeMin { get; }

        public uint BufferNumMin { get; }

        public uint BufferSizeRecommended { get; }

        public uint BufferNumRecommended { get; }

        public MMAL_ES_FORMAT_T* Format { get; }

        public MMAL_EVENT_FORMAT_CHANGED_T(uint bufferSizeMin, uint bufferNumMin, uint bufferSizeRecommended, uint bufferNumRecommended,
                                           MMAL_ES_FORMAT_T* format)
        {
            this.BufferSizeMin = bufferSizeMin;
            this.BufferNumMin = bufferNumMin;
            this.BufferSizeRecommended = bufferSizeRecommended;
            this.BufferNumRecommended = bufferNumRecommended;
            this.Format = format;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_EVENT_PARAMETER_CHANGED_T
    {
        public MMAL_PARAMETER_HEADER_T Hdr;
    
        public MMAL_EVENT_PARAMETER_CHANGED_T(MMAL_PARAMETER_HEADER_T hdr)
        {
            this.Hdr = hdr;
        }
    }
}
