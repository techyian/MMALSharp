// <copyright file="DefaultOutputCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Native;
using MMALSharp.Ports;

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
        /// <param name="port">The working <see cref="IOutputPort"/>.</param>
        public DefaultOutputCallbackHandler(IOutputPort port)
            : base(port)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DefaultOutputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IOutputPort"/>.</param>
        /// <param name="encodingType">The <see cref="MMALEncoding"/> type to restrict on.</param>
        public DefaultOutputCallbackHandler(IOutputPort port, MMALEncoding encodingType)
            : base(port, encodingType)
        {
        }

        /// <summary>
        /// The callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        public override void Callback(MMALBufferImpl buffer)
        {
            base.Callback(buffer);
            
            var data = buffer.GetBufferData();

            this.WorkingPort.Handler?.Process(data);
        }
    }
}
