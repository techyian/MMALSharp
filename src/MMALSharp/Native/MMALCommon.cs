// <copyright file="MMALCommon.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Runtime.InteropServices;

namespace MMALSharp.Native
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1132 // Each field should be declared on its own line

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_CORE_STATISTICS_T
    {
        private uint bufferCount, firstBufferTime, lastBufferTime, maxDelay;

        public uint BufferCount => bufferCount;
        public uint FirstBufferTime => firstBufferTime;
        public uint LastBufferTime => lastBufferTime;
        public uint MaxDelay => maxDelay;

        public MMAL_CORE_STATISTICS_T(uint bufferCount, uint firstBufferTime, uint lastBufferTime, uint maxDelay)
        {
            this.bufferCount = bufferCount;
            this.firstBufferTime = firstBufferTime;
            this.lastBufferTime = lastBufferTime;
            this.maxDelay = maxDelay;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_CORE_PORT_STATISTICS_T
    {
        private MMAL_CORE_STATISTICS_T rx, tx;

        public MMAL_CORE_STATISTICS_T Rx => rx;
        public MMAL_CORE_STATISTICS_T Tx => tx;

        public MMAL_CORE_PORT_STATISTICS_T(MMAL_CORE_STATISTICS_T rx, MMAL_CORE_STATISTICS_T tx)
        {
            this.rx = rx;
            this.tx = tx;
        }
    }
}
