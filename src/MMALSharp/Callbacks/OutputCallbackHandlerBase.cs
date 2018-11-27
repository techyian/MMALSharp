// <copyright file="OutputCallbackHandlerBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Common.Utility;
using MMALSharp.Native;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// The base class for Output port callback handlers.
    /// </summary>
    public abstract class OutputCallbackHandlerBase : IOutputCallbackHandler
    {
        /// <inheritdoc />
        public MMALEncoding EncodingType { get; }

        /// <inheritdoc />
        public OutputPortBase WorkingPort { get; }
        
        /// <summary>
        /// Creates a new instance of <see cref="OutputCallbackHandlerBase"/>.
        /// </summary>
        /// <param name="port">The working <see cref="OutputPortBase"/>.</param>
        protected OutputCallbackHandlerBase(OutputPortBase port)
        {
            this.WorkingPort = port;
        }

        /// <summary>
        /// Creates a new instance of <see cref="OutputCallbackHandlerBase"/>.
        /// </summary>
        /// <param name="port">The working <see cref="OutputPortBase"/>.</param>
        /// <param name="encodingType">The <see cref="MMALEncoding"/> type to restrict on.</param>
        protected OutputCallbackHandlerBase(OutputPortBase port, MMALEncoding encodingType)
        {
            this.WorkingPort = port;
            this.EncodingType = encodingType;
        }
        
        /// <inheritdoc />
        public virtual void Callback(MMALBufferImpl buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.Debug($"In managed {this.WorkingPort.PortType.GetPortType()} callback");
            }
            
            if (this.EncodingType != null && this.WorkingPort.EncodingType != this.EncodingType)
            {
                throw new ArgumentException("Port Encoding Type not supported for this handler.");
            }
        }
    }
}
