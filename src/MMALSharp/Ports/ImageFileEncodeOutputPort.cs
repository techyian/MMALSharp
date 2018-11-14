// <copyright file="ImageFileEncodeOutputPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    /// <summary>
    /// A custom port definition used specifically when using encoder conversion functionality.
    /// </summary>
    public unsafe class ImageFileEncodeOutputPort : OutputPort
    {
        /// <summary>
        /// Creates a new instance of <see cref="ImageFileEncodeOutputPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public ImageFileEncodeOutputPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid)
            : base(ptr, comp, type, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="ImageFileEncodeOutputPort"/>.
        /// </summary>
        /// <param name="copyFrom">The port to copy data from.</param>
        public ImageFileEncodeOutputPort(IPort copyFrom)
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
                if (MMALCameraConfig.Debug)
                {
                    MMALLog.Logger.Debug($"Putting output port buffer back into queue {((IntPtr)MMALImageFileEncoder.WorkingQueue.Ptr).ToString()}");
                }

                var bufferImpl = new MMALBufferImpl(buffer);
                MMALImageFileEncoder.WorkingQueue.Put(bufferImpl);

                if (port->IsEnabled == 1 && !this.Trigger)
                {
                    this.Trigger = true;
                }
            }
        }
    }
}
