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
        protected MMALEncoderBase(string encoderName, ICaptureHandler handler)
            : base(encoderName, handler)
        {
        }
    }
}
