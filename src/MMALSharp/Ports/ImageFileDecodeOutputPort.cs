// <copyright file="ImageFileDecodeOutputPort.cs" company="Techyian">
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
    public unsafe class ImageFileDecodeOutputPort : OutputPort
    {
        public ImageFileDecodeOutputPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid)
            : base(ptr, comp, type, guid)
        {
        }

        public ImageFileDecodeOutputPort(IPort copyFrom)
            : base(copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.PortType, copyFrom.Guid, copyFrom.Handler)
        {
        }

        /// <summary>
        /// The native callback MMAL passes buffer headers to.
        /// </summary>
        /// <param name="port">The port the buffer is sent to.</param>
        /// <param name="buffer">The buffer header.</param>
        internal override void NativeOutputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (OutputLock)
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

                if (port->IsEnabled == 1 && !this.Trigger)
                {
                    this.Trigger = true;
                }
            }
        }
    }
}
