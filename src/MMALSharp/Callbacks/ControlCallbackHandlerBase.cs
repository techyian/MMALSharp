// <copyright file="OutputCallbackHandlerBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// The base class for Control port callback handlers.
    /// </summary>
    public abstract class ControlCallbackHandlerBase : IControlCallbackHandler
    {
        /// <summary>
        /// A whitelisted Encoding Type that this callback handler will operate on.
        /// </summary>
        public MMALEncoding EncodingType { get; }

        /// <summary>
        /// The port this callback handler is used with.
        /// </summary>
        public IControlPort WorkingPort { get; }
        
        /// <summary>
        /// Creates a new instance of <see cref="ControlCallbackHandlerBase"/>.
        /// </summary>
        /// <param name="port">The <see cref="IControlPort"/>.</param>
        protected ControlCallbackHandlerBase(IControlPort port)
        {
            this.WorkingPort = port;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ControlCallbackHandlerBase"/>.
        /// </summary>
        /// <param name="port">The <see cref="IControlPort"/>.</param>
        /// <param name="encodingType">The <see cref="MMALEncoding"/> type to restrict on.</param>
        protected ControlCallbackHandlerBase(IControlPort port, MMALEncoding encodingType)
        {
            this.WorkingPort = port;
            this.EncodingType = encodingType;
        }
        
        /// <summary>
        /// The callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
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
