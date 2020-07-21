// <copyright file="OverlayPort.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Components;
using MMALSharp.Native;

namespace MMALSharp.Ports.Inputs
{
    /// <summary>
    /// Represents port behaviour especially for the static overlay renderer functionality. This object overrides <see cref="NativeInputPortCallback"/>
    /// forcing it to do nothing when it receives a callback from the component.
    /// </summary>
    public unsafe class OverlayPort : InputPort
    {
        /// <summary>
        /// Creates a new instance of <see cref="OverlayPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public OverlayPort(IntPtr ptr, IComponent comp, Guid guid) 
            : base(ptr, comp, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="OverlayPort"/>.
        /// </summary>
        /// <param name="copyFrom">The port to copy data from.</param>
        public OverlayPort(IPort copyFrom)
            : base((IntPtr)copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.Guid)
        {
        }

        internal override void NativeInputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer) { }
    }
}
