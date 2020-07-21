// <copyright file="DefaultInputPortCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Handlers;
using MMALSharp.Ports.Inputs;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// A default callback handler for ports.
    /// </summary>
    public class DefaultInputPortCallbackHandler : InputPortCallbackHandler<IInputPort, IInputCaptureHandler>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DefaultPortCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IInputPort"/>.</param>
        /// <param name="handler">The input port capture handler.</param>
        public DefaultInputPortCallbackHandler(IInputPort port, IInputCaptureHandler handler)
            : base(port, handler)
        {
        }
    }
}
