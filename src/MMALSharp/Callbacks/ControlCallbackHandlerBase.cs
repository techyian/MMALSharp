// <copyright file="OutputCallbackHandlerBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Ports.Controls;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// The base class for Control port callback handlers.
    /// </summary>
    public abstract class ControlCallbackHandlerBase : IControlCallbackHandler
    {
        /// <inheritdoc />
        public ControlPortBase WorkingPort { get; }
        
        /// <summary>
        /// Creates a new instance of <see cref="ControlCallbackHandlerBase"/>.
        /// </summary>
        /// <param name="port">The <see cref="ControlPortBase"/>.</param>
        protected ControlCallbackHandlerBase(ControlPortBase port)
        {
            this.WorkingPort = port;
        }
        
        /// <inheritdoc />
        public virtual void Callback(MMALBufferImpl buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.Debug($"In managed {this.WorkingPort.PortType.GetPortType()} callback");
            }
        }
    }
}
