using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Native
{   
    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_CORE_STATISTICS_T
    {
        public uint bufferCount, firstBufferTime, lastBufferTime, maxDelay;

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
        public MMAL_CORE_STATISTICS_T rx, tx;

        public MMAL_CORE_PORT_STATISTICS_T(MMAL_CORE_STATISTICS_T rx, MMAL_CORE_STATISTICS_T tx)
        {
            this.rx = rx;
            this.tx = tx;
        }
    }

}
