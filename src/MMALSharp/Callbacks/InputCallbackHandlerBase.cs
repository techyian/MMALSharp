// <copyright file="InputCallbackHandlerBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Callbacks
{
    public abstract class InputCallbackHandlerBase
    {
        /// <summary>
        /// A whitelisted Encoding Type that this callback handler will operate on.
        /// </summary>
        public MMALEncoding EncodingType { get; }

        /// <summary>
        /// The port this callback handler is used with.
        /// </summary>
        public IInputPort WorkingPort { get; internal set; }

        protected InputCallbackHandlerBase(IInputPort port)
        {
            this.WorkingPort = port;
        }

        protected InputCallbackHandlerBase(IInputPort port, MMALEncoding encodingType)
        {
            this.WorkingPort = port;
            this.EncodingType = encodingType;
        }

        /// <summary>
        /// The callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        /// <returns>A <see cref="ProcessResult"/> object based on the result of the callback function.</returns>
        public virtual ProcessResult Callback(MMALBufferImpl buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.Debug("In managed input callback");
            }
            
            if (this.EncodingType != null && this.WorkingPort.EncodingType != this.EncodingType)
            {
                throw new ArgumentException("Port Encoding Type not supported for this handler.");
            }
            
            return this.WorkingPort.Handler?.Process(buffer.AllocSize);
        }
    }
}
