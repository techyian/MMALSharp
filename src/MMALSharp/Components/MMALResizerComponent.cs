// <copyright file="MMALResizerComponent.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Handlers;
using System;
using static MMALSharp.Native.MMALParameters;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents the resizer component. This component has the ability to change the encoding type &amp; pixel format, as well
    /// as the width/height of resulting frames.
    /// </summary>
    public sealed class MMALResizerComponent : MMALDownstreamHandlerComponent
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MMALResizerComponent"/> class that can be used to change the size
        /// and the pixel format of resulting frames. 
        /// </summary>
        /// <param name="width">The width of the output frames. Value must be greater than 1.</param>
        /// <param name="height">The height of the output frames. Value must be greater than 1.</param>
        /// <param name="handler">The capture handler associated with this component.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public MMALResizerComponent(ICaptureHandler handler)
            : base(MMAL_COMPONENT_DEFAULT_RESIZER, handler)
        {
        }
    }
}
