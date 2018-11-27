// <copyright file="DefaultInputCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Native;
using MMALSharp.Ports.Inputs;

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
        /// <param name="port">The working <see cref="InputPortBase"/>.</param>
        public DefaultInputCallbackHandler(InputPortBase port) 
            : base(port)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DefaultInputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="InputPortBase"/>.</param>
        /// <param name="encodingType">The <see cref="MMALEncoding"/> type to restrict on.</param>
        public DefaultInputCallbackHandler(InputPortBase port, MMALEncoding encodingType)
            : base(port, encodingType)
        {
        }
    }
}
