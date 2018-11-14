// <copyright file="StillPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Represents a still image encoder/decoder port.
    /// </summary>
    public unsafe class StillPort : OutputPort
    {
        /// <summary>
        /// Creates a new instance of <see cref="StillPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public StillPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid)
            : base(ptr, comp, type, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="StillPort"/>.
        /// </summary>
        /// <param name="copyFrom">The port to copy data from.</param>
        public StillPort(IPort copyFrom)
            : base(copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.PortType, copyFrom.Guid, copyFrom.Handler)
        {
        }
    }    
}
