// <copyright file="IOutputPort.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Callbacks;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Ports.Inputs;

namespace MMALSharp.Ports.Outputs
{
    /// <summary>
    /// Represents an output port.
    /// </summary>
    public interface IOutputPort : IPort
    {
        /// <summary>
        /// Call to configure an output port.
        /// </summary>
        /// <param name="config">The port configuration object.</param>
        /// <param name="copyFrom">The port to copy from.</param>
        /// <param name="handler">The capture handler to assign to this port.</param>
        void Configure(IMMALPortConfig config, IInputPort copyFrom, IOutputCaptureHandler handler);

        /// <summary>
        /// Connects two components together by their input and output ports.
        /// </summary>
        /// <param name="component">The component we want to connect to.</param>
        /// <param name="inputPort">The input port of the component we want to connect to.</param>
        /// <param name="useCallback">Flag to use connection callback (adversely affects performance).</param>
        /// <returns>The connection instance between the source output and destination input ports.</returns>
        IConnection ConnectTo(IDownstreamComponent component, int inputPort = 0, bool useCallback = false);

        /// <summary>
        /// Enable the port specified.
        /// </summary>
        void Start();

        /// <summary>
        /// Release an output port buffer, get a new one from the queue and send it for processing.
        /// </summary>
        /// <param name="bufferImpl">A managed buffer object.</param>
        /// <param name="eos">Flag that this buffer is the end of stream.</param>
        void ReleaseBuffer(IBuffer bufferImpl, bool eos);

        /// <summary>
        /// Call to register a new callback handler with this port.
        /// </summary>
        /// <param name="callbackHandler">The output callback handler.</param>
        void RegisterCallbackHandler(IOutputCallbackHandler callbackHandler);
    }
}
