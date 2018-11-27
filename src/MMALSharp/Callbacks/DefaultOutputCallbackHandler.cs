// <copyright file="DefaultOutputCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Native;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// A default callback handler for Output ports.
    /// </summary>
    public class DefaultOutputCallbackHandler : OutputCallbackHandlerBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="DefaultOutputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="OutputPortBase"/>.</param>
        public DefaultOutputCallbackHandler(OutputPortBase port)
            : base(port)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DefaultOutputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="OutputPortBase"/>.</param>
        /// <param name="encodingType">The <see cref="MMALEncoding"/> type to restrict on.</param>
        public DefaultOutputCallbackHandler(OutputPortBase port, MMALEncoding encodingType)
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
