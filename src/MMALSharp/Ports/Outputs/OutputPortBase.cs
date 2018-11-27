// <copyright file="OutputPortBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Callbacks;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports.Inputs;

namespace MMALSharp.Ports.Outputs
{
    /// <summary>
    /// Represents an output port.
    /// </summary>
    public abstract class OutputPortBase : PortBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="OutputPortBase"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        protected unsafe OutputPortBase(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid)
            : base(ptr, comp, type, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="OutputPortBase"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        /// <param name="handler">The capture handler.</param>
        protected unsafe OutputPortBase(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid, ICaptureHandler handler)
            : base(ptr, comp, type, guid, handler)
        {
        }

        /// <summary>
        /// Output callback handler which is called by the native function callback.
        /// </summary>
        internal abstract IOutputCallbackHandler ManagedOutputCallback { get; set; }

        /// <summary>
        /// Connects two components together by their input and output ports.
        /// </summary>
        /// <param name="destinationComponent">The component we want to connect to.</param>
        /// <param name="inputPort">The input port of the component we want to connect to.</param>
        /// <param name="useCallback">Flag to use connection callback (adversely affects performance).</param>
        /// <returns>The input port of the component we're connecting to - allows chain calling of this method.</returns>
        public abstract InputPortBase ConnectTo(MMALDownstreamComponent destinationComponent, int inputPort = 0, bool useCallback = false);

        /// <summary>
        /// Connects two components together by their input and output ports.
        /// </summary>
        /// <param name="destinationComponent">The component we want to connect to.</param>
        /// <param name="inputPort">The input port of the component we want to connect to.</param>
        /// <param name="callback">An operation we would like to carry out after connecting these components together.</param>
        /// <returns>The input port of the component we're connecting to - allows chain calling of this method.</returns>
        public abstract InputPortBase ConnectTo(MMALDownstreamComponent destinationComponent, int inputPort, Func<PortBase> callback);
        
        /// <summary>
        /// Enables processing on an output port.
        /// </summary>
        /// <param name="sendBuffers">Indicates whether we want to send all the buffers in the port pool or simply create the pool.</param>
        internal abstract void EnableOutputPort(bool sendBuffers = true);

        /// <summary>
        /// Release an output port buffer, get a new one from the queue and send it for processing.
        /// </summary>
        /// <param name="bufferImpl">A managed buffer object.</param>
        internal abstract void ReleaseOutputBuffer(MMALBufferImpl bufferImpl);

        /// <summary>
        /// Enable the port specified.
        /// </summary>
        internal abstract void Start();
    }
}