// <copyright file="ConnectionCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using Microsoft.Extensions.Logging;
using MMALSharp.Common.Utility;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// Base class for connection callback handlers.
    /// </summary>
    public abstract class ConnectionCallbackHandler : IConnectionCallbackHandler
    {
        /// <inheritdoc />
        public IConnection WorkingConnection { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ConnectionCallbackHandler"/>.
        /// </summary>
        /// <param name="connection">The connection.</param>
        protected ConnectionCallbackHandler(IConnection connection)
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
                MMALLog.Logger.LogDebug("Inside Managed input port connection callback");
            }
        }

        /// <inheritdoc />
        public virtual void OutputCallback(IBuffer buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.LogDebug("Inside Managed output port connection callback");
            }
        }
    }
}
