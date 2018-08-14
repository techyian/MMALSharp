// <copyright file="ConnectionCallbackHandlerBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common.Utility;

namespace MMALSharp.Callbacks
{
    public abstract class ConnectionCallbackHandlerBase
    {
        /// <summary>
        /// The port this callback handler is used with.
        /// </summary>
        public MMALConnectionImpl WorkingConnection { get; internal set; }

        protected ConnectionCallbackHandlerBase(MMALConnectionImpl connection)
        {
            this.WorkingConnection = connection;
        }

        /// <summary>
        /// The input port callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        public virtual void InputCallback(MMALBufferImpl buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.Debug("Inside Managed input port connection callback");
            }
        }

        /// <summary>
        /// The output port callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        public virtual void OutputCallback(MMALBufferImpl buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.Debug("Inside Managed output port connection callback");
            }
        }
    }
}
