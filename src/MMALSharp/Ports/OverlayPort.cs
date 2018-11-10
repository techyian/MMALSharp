// <copyright file="OverlayPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Represents port behaviour especially for the static overlay renderer functionality. This object overrides <see cref="NativeInputPortCallback"/>
    /// forcing it to do nothing when it receives a callback from the component.
    /// </summary>
    public unsafe class OverlayPort : InputPort
    {
        public OverlayPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid) : base(ptr, comp, type, guid)
        {
        }

        public OverlayPort(IPort copyFrom)
            : base(copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.PortType, copyFrom.Guid)
        {
        }

        internal override void NativeInputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer) { }
    }
}
