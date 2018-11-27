// <copyright file="InputPortBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Callbacks;
using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp.Ports.Inputs
{
    /// <summary>
    /// Represents an input port.
    /// </summary>
    public abstract class InputPortBase : PortBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="InputPortBase"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        protected unsafe InputPortBase(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid)
            : base(ptr, comp, type, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="InputPortBase"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        /// <param name="handler">The capture handler.</param>
        protected unsafe InputPortBase(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid, ICaptureHandler handler)
            : base(ptr, comp, type, guid, handler)
        {
        }

        /// <summary>
        /// Managed callback which is called by the native function callback method.
        /// </summary>
        internal abstract IInputCallbackHandler ManagedInputCallback { get; set; }
        
        /// <summary>
        /// Enables processing on an input port.
        /// </summary>
        internal abstract void EnableInputPort();

        /// <summary>
        /// Releases an input port buffer and reads further data from user provided image data if not reached end of file.
        /// </summary>
        /// <param name="bufferImpl">A managed buffer object.</param>
        internal abstract void ReleaseInputBuffer(MMALBufferImpl bufferImpl);

        /// <summary>
        /// Starts the input port.
        /// </summary>
        internal abstract void Start();
    }
}