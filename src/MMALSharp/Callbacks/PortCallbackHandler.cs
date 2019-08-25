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
    public abstract class PortCallbackHandler<TPort, TCaptureHandler> : IOutputCallbackHandler
        where TPort : IPort
        where TCaptureHandler : IOutputCaptureHandler
    {
        public MMALEncoding EncodingType { get; }
        
        public TPort WorkingPort { get; }

        public TCaptureHandler CaptureHandler { get; }

        protected bool FoundEos { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="PortCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IPort"/>.</param>
        protected PortCallbackHandler(TPort port, TCaptureHandler captureHandler)
        {
            this.WorkingPort = port;
            this.CaptureHandler = captureHandler;
        }

        /// <summary>
        /// Creates a new instance of <see cref="PortCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IPort"/>.</param>
        /// <param name="encodingType">The <see cref="MMALEncoding"/> type to restrict on.</param>
        protected PortCallbackHandler(TPort port, TCaptureHandler captureHandler, MMALEncoding encodingType)
            : this(port, captureHandler)
        {
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

            var data = buffer.GetBufferData();
            var eos = buffer.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END) ||
                      buffer.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS);

            this.CaptureHandler?.Process(data, eos);
        }
    }
}
