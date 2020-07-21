// <copyright file="IInputPort.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Callbacks;
using MMALSharp.Handlers;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Ports.Inputs
{
    /// <summary>
    /// Represents an input port.
    /// </summary>
    public interface IInputPort : IPort
    {
        /// <summary>
        /// Call to connect this input port to an output port. This method
        /// simply assigns the <see cref="IConnection"/> to the ConnectedReference property. 
        /// </summary>
        /// <param name="outputPort">The connected output port.</param>
        /// <param name="connection">The connection object.</param>
        void ConnectTo(IOutputPort outputPort, IConnection connection);

        /// <summary>
        /// Call to configure an input port.
        /// </summary>
        /// <param name="config">The port configuration object.</param>
        /// <param name="copyPort">The port to copy from.</param>
        /// <param name="handler">The capture handler to assign to this port.</param>
        void Configure(IMMALPortConfig config, IPort copyPort, IInputCaptureHandler handler);

        /// <summary>
        /// Enables processing on an input port.
        /// </summary>
        void Enable();

        /// <summary>
        /// Releases an input port buffer and reads further data from user provided image data if not reached end of file.
        /// </summary>
        /// <param name="bufferImpl">A managed buffer object.</param>
        void ReleaseBuffer(IBuffer bufferImpl);

        /// <summary>
        /// Starts the input port.
        /// </summary>
        void Start();

        /// <summary>
        /// Registers a new input callback handler with this port.
        /// </summary>
        /// <param name="callbackHandler">The callback handler.</param>
        void RegisterCallbackHandler(IInputCallbackHandler callbackHandler);
    }
}
