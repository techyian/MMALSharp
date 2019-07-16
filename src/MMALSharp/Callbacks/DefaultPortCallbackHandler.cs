// <copyright file="DefaultCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// A default callback handler for ports.
    /// </summary>
    public class DefaultPortCallbackHandler : PortCallbackHandlerBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="DefaultPortCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="PortBase"/>.</param>
        public DefaultPortCallbackHandler(PortBase port)
            : base(port)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DefaultPortCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="PortBase"/>.</param>
        /// <param name="encodingType">The <see cref="MMALEncoding"/> type to restrict on.</param>
        public DefaultPortCallbackHandler(PortBase port, MMALEncoding encodingType)
            : base(port, encodingType)
        {
        }

        /// <inheritdoc />
        public override void Callback(MMALBufferImpl buffer)
        {
            base.Callback(buffer);
            
            var data = buffer.GetBufferData();

            this.WorkingPort.Handler?.Process(data);
        }
    }
}
