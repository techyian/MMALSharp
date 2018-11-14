// <copyright file="MMALEncoderBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Handlers;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a base class for all encoder components.
    /// </summary>
    public abstract class MMALEncoderBase : MMALDownstreamHandlerComponent
    {
        /// <summary>
        /// Creates a new instance of <see cref="MMALEncoderBase"/>.
        /// </summary>
        /// <param name="encoderName">The name of the encoder component.</param>
        /// <param name="handler">The capture handler used.</param>
        protected MMALEncoderBase(string encoderName, ICaptureHandler handler)
            : base(encoderName, handler)
        {
        }
    }
}
