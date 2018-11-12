// <copyright file="DefaultInputCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// A default callback handler for Input ports.
    /// </summary>
    public class DefaultInputCallbackHandler : InputCallbackHandlerBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="DefaultInputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IInputPort"/>.</param>
        public DefaultInputCallbackHandler(IInputPort port) 
            : base(port)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DefaultInputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IInputPort"/>.</param>
        /// <param name="encodingType">The <see cref="MMALEncoding"/> type to restrict on.</param>
        public DefaultInputCallbackHandler(IInputPort port, MMALEncoding encodingType)
            : base(port, encodingType)
        {
        }
    }
}
