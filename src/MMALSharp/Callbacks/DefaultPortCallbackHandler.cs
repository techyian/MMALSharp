// <copyright file="DefaultPortCallbackHandler.cs" company="Techyian">
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
        /// <param name="port">The working <see cref="IPort"/>.</param>
        public DefaultPortCallbackHandler(IPort port)
            : base(port)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DefaultPortCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IPort"/>.</param>
        /// <param name="encodingType">The <see cref="MMALEncoding"/> type to restrict on.</param>
        public DefaultPortCallbackHandler(IPort port, MMALEncoding encodingType)
            : base(port, encodingType)
        {
        }

        /// <inheritdoc />
        public override void Callback(IBuffer buffer)
        {
            base.Callback(buffer);
            
            var data = buffer.GetBufferData();
            var eos = buffer.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END) ||
                      buffer.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS);

            this.WorkingPort.Handler?.Process(data, eos);
        }
    }
}
