// <copyright file="PortCallbackHandlerBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Common.Utility;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// The base class for Output port callback handlers.
    /// </summary>
    public abstract class PortCallbackHandlerBase : ICallbackHandler
    {      
        /// <inheritdoc />
        public MMALEncoding EncodingType { get; }

        /// <inheritdoc />
        public IPort WorkingPort { get; }

        /// <summary>
        /// Creates a new instance of <see cref="PortCallbackHandlerBase"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IPort"/>.</param>
        protected PortCallbackHandlerBase(IPort port)
        {
            this.WorkingPort = port;        
        }

        /// <summary>
        /// Creates a new instance of <see cref="PortCallbackHandlerBase"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IPort"/>.</param>
        /// <param name="encodingType">The <see cref="MMALEncoding"/> type to restrict on.</param>
        protected PortCallbackHandlerBase(IPort port, MMALEncoding encodingType)
        {
            this.WorkingPort = port;
            this.EncodingType = encodingType;       
        }
        
        /// <inheritdoc />
        public virtual void Callback(IBuffer buffer)
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

        /// <inheritdoc />
        public virtual ProcessResult CallbackWithResult(IBuffer buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.Debug($"In managed {this.WorkingPort.PortType.GetPortType()} callback");
            }

            if (this.EncodingType != null && this.WorkingPort.EncodingType != this.EncodingType)
            {
                throw new ArgumentException("Port Encoding Type not supported for this handler.");
            }

            MMALLog.Logger.Info($"Processing {this.WorkingPort.Handler?.TotalProcessed()}");

            return this.WorkingPort.Handler?.Process(buffer.AllocSize);
        }               
    }
}
