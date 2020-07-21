// <copyright file="DefaultOutputPortCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Handlers;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// A default callback handler for ports.
    /// </summary>
    public class DefaultOutputPortCallbackHandler : PortCallbackHandler<IOutputPort, IOutputCaptureHandler>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DefaultOutputPortCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IOutputPort"/>.</param>
        /// <param name="handler">The output port capture handler.</param>
        public DefaultOutputPortCallbackHandler(IOutputPort port, IOutputCaptureHandler handler)
            : base(port, handler)
        {
        }
    }
}
