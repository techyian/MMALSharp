// <copyright file="ImageFileDecodeInputPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Common.Utility;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    /// <summary>
    /// A custom port definition used specifically when using encoder conversion functionality.
    /// </summary>
    public unsafe class ImageFileDecodeInputPort : InputPort
    {
        /// <summary>
        /// Creates a new instance of <see cref="ImageFileDecodeInputPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public ImageFileDecodeInputPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid)
            : base(ptr, comp, type, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="ImageFileDecodeInputPort"/>.
        /// </summary>
        /// <param name="copyFrom">The port to copy data from.</param>
        public ImageFileDecodeInputPort(IPort copyFrom)
            : base(copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.PortType, copyFrom.Guid)
        {
        }

        internal override void NativeInputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (InputLock)
            {
                if (MMALCameraConfig.Debug)
                {
                    MMALLog.Logger.Debug("Releasing input port buffer");
                }

                var bufferImpl = new MMALBufferImpl(buffer);
                bufferImpl.Release();

                if (!this.Trigger)
                {
                    this.Trigger = true;
                }
            }
        }
    }
}
