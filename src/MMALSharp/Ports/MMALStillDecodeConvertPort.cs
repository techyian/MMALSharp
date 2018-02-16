// <copyright file="MMALStillDecodeConvertPort.cs" company="Techyian">
// Copyright (c) Techyian. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Components;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    /// <summary>
    /// A custom port definition used specifically when using encoder conversion functionality
    /// </summary>
    public unsafe class MMALStillDecodeConvertPort : MMALStillPort
    {
        public MMALStillDecodeConvertPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type)
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
        /// The native callback MMAL passes buffer headers to
        /// </summary>
        /// <param name="port">The port the buffer is sent to</param>
        /// <param name="buffer">The buffer header</param>
        internal override void NativeOutputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (MMALPortBase.OutputLock)
            {
                var bufferImpl = new MMALBufferImpl(buffer);

                if (bufferImpl.CheckState())
                {
                    if (MMALCameraConfig.Debug)
                    {
                        MMALLog.Logger.Debug($"Putting output port buffer back into queue {((IntPtr)MMALImageFileDecoder.WorkingQueue.Ptr).ToString()}");
                    }

                    bufferImpl.PrintProperties();

                    MMALImageFileDecoder.WorkingQueue.Put(bufferImpl);
                }
                else
                {
                    MMALLog.Logger.Debug($"Invalid output buffer received");
                }

                if (this.Trigger != null && this.Trigger.CurrentCount > 0)
                {
                    this.Trigger.Signal();
                }
            }
        }
    }
}
