// <copyright file="MMALStillEncodeConvertPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Components;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    /// <summary>
    /// A custom port definition used specifically when using encoder conversion functionality.
    /// </summary>
    public unsafe class MMALStillEncodeConvertPort : MMALStillPort
    {
        public MMALStillEncodeConvertPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type)
            : base(ptr, comp, type)
        {
        }

        internal override unsafe void NativeInputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (MMALPortBase.InputLock)
            {
                if (MMALCameraConfig.Debug)
                {
                    MMALLog.Logger.Debug("Releasing input port buffer");
                }

                var bufferImpl = new MMALBufferImpl(buffer);
                bufferImpl.Release();

                if (this.Trigger != null && this.Trigger.CurrentCount > 0)
                {
                    this.Trigger.Signal();
                }
            }
        }

        /// <summary>
        /// The native callback MMAL passes buffer headers to.
        /// </summary>
        /// <param name="port">The port the buffer is sent to.</param>
        /// <param name="buffer">The buffer header.</param>
        internal override void NativeOutputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (MMALPortBase.OutputLock)
            {
                if (MMALCameraConfig.Debug)
                {
                    MMALLog.Logger.Debug($"Putting output port buffer back into queue {((IntPtr)MMALImageFileEncoder.WorkingQueue.Ptr).ToString()}");
                }

                var bufferImpl = new MMALBufferImpl(buffer);
                MMALImageFileEncoder.WorkingQueue.Put(bufferImpl);

                if (this.Trigger != null && this.Trigger.CurrentCount > 0)
                {
                    this.Trigger.Signal();
                }
            }
        }
    }
}
