// <copyright file="IOutputPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Callbacks;
using MMALSharp.Components;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Represents an output port.
    /// </summary>
    public interface IOutputPort : IPort
    {
        /// <summary>
        /// Output callback handler which is called by the native function callback.
        /// </summary>
        IOutputCallbackHandler ManagedOutputCallback { get; set; }
        
        /// <summary>
        /// Connects two components together by their input and output ports.
        /// </summary>
        /// <param name="destinationComponent">The component we want to connect to.</param>
        /// <param name="inputPort">The input port of the component we want to connect to.</param>
        /// <param name="useCallback">Flag to use connection callback (adversely affects performance).</param>
        /// <returns>The input port of the component we're connecting to - allows chain calling of this method.</returns>
        IInputPort ConnectTo(MMALDownstreamComponent destinationComponent, int inputPort = 0, bool useCallback = false);
        
        /// <summary>
        /// Connects two components together by their input and output ports.
        /// </summary>
        /// <param name="destinationComponent">The component we want to connect to.</param>
        /// <param name="inputPort">The input port of the component we want to connect to.</param>
        /// <param name="callback">An operation we would like to carry out after connecting these components together.</param>
        /// <returns>The input port of the component we're connecting to - allows chain calling of this method.</returns>
        IInputPort ConnectTo(MMALDownstreamComponent destinationComponent, int inputPort, Func<IPort> callback);
        
        /// <summary>
        /// Enables processing on an output port.
        /// </summary>
        /// <param name="sendBuffers">Indicates whether we want to send all the buffers in the port pool or simply create the pool.</param>
        void EnableOutputPort(bool sendBuffers = true);
        
        /// <summary>
        /// Release an output port buffer, get a new one from the queue and send it for processing.
        /// </summary>
        /// <param name="bufferImpl">A managed buffer object.</param>
        void ReleaseOutputBuffer(MMALBufferImpl bufferImpl);
        
        /// <summary>
        /// Enable the port specified.
        /// </summary>
        void Start();
    }
}