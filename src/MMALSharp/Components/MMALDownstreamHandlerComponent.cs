// <copyright file="MMALDownstreamHandlerComponent.cs" company="Techyian">
// Copyright (c) Techyian. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Handlers;

namespace MMALSharp.Components
{
    public abstract class MMALDownstreamHandlerComponent : MMALDownstreamComponent
    {
        protected MMALDownstreamHandlerComponent(string name, ICaptureHandler handler)
            : base(name)
        {
            this.Handler = handler;
        }
    }
}
