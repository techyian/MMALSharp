// <copyright file="MMALCommon.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Runtime.InteropServices;

namespace MMALSharp.Native
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_CORE_STATISTICS_T
    {
        public uint BufferCount { get; }

        public uint FirstBufferTime { get; }

        public uint LastBufferTime { get; }

        public uint MaxDelay { get; }

        public MMAL_CORE_STATISTICS_T(uint bufferCount, uint firstBufferTime, uint lastBufferTime, uint maxDelay)
        {
            this.BufferCount = bufferCount;
            this.FirstBufferTime = firstBufferTime;
            this.LastBufferTime = lastBufferTime;
            this.MaxDelay = maxDelay;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_CORE_PORT_STATISTICS_T
    {
        public MMAL_CORE_STATISTICS_T Rx { get; }

        public MMAL_CORE_STATISTICS_T Tx { get; }

        public MMAL_CORE_PORT_STATISTICS_T(MMAL_CORE_STATISTICS_T rx, MMAL_CORE_STATISTICS_T tx)
        {
            this.Rx = rx;
            this.Tx = tx;
        }
    }
}
