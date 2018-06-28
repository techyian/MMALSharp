// <copyright file="DefaultOutputCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Native;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// A default callback handler for Output and Control ports.
    /// </summary>
    public class DefaultOutputCallbackHandler : OutputCallbackHandlerBase
    {
        public DefaultOutputCallbackHandler(MMALPortBase port)
            : base(port)
        {
        }

        public DefaultOutputCallbackHandler(MMALPortBase port, MMALEncoding encodingType)
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
            this.WorkingPort.ComponentReference.Handler?.Process(data);
        }
    }
}
