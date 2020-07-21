// <copyright file="MMALEncoderBase.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Components.EncoderComponents;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a base class for all encoder components.
    /// </summary>
    public abstract class MMALEncoderBase : MMALDownstreamHandlerComponent, IEncoder
    {
        /// <summary>
        /// Creates a new instance of <see cref="MMALEncoderBase"/>.
        /// </summary>
        /// <param name="encoderName">The name of the encoder component.</param>
        protected MMALEncoderBase(string encoderName)
            : base(encoderName)
        {
        }
    }
}
