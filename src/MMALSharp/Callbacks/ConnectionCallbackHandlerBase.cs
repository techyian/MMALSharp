// <copyright file="ConnectionCallbackHandlerBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common.Utility;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// Base class for connection callback handlers.
    /// </summary>
    public abstract class ConnectionCallbackHandlerBase : IConnectionCallbackHandler
    {
        /// <inheritdoc />
        public IConnection WorkingConnection { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ConnectionCallbackHandlerBase"/>.
        /// </summary>
        /// <param name="connection">The connection.</param>
        protected ConnectionCallbackHandlerBase(IConnection connection)
        {
            this.WorkingConnection = connection;
        }

        /// <summary>
        /// The input port callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        public virtual void InputCallback(IBuffer buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.Debug("Inside Managed input port connection callback");
            }
        }

        /// <inheritdoc />
        public virtual void OutputCallback(IBuffer buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.Debug("Inside Managed output port connection callback");
            }
        }
    }
}
