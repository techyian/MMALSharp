// <copyright file="MMALResizerComponent.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Handlers;
using System;
using MMALSharp.Ports;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;
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
        public unsafe MMALResizerComponent(ICaptureHandler handler)
            : base(MMAL_COMPONENT_DEFAULT_RESIZER, handler)
        {
            // Default to use still image port behaviour.
            this.Inputs.Add(new InputPort(&(*this.Ptr->Input[0]), this, PortType.Input, Guid.NewGuid()));
            this.Outputs.Add(new StillPort(&(*this.Ptr->Output[0]), this, PortType.Output, Guid.NewGuid()));
        }

        public unsafe MMALResizerComponent(ICaptureHandler handler, Type outputPort)
            : base(MMAL_COMPONENT_DEFAULT_RESIZER, handler)
        {
            this.Inputs.Add(new InputPort(&(*this.Ptr->Input[0]), this, PortType.Input, Guid.NewGuid()));
            Activator.CreateInstance(outputPort, IntPtr.Zero, this, PortType.Output, Guid.NewGuid());

            this.Outputs.Add(new StillPort(&(*this.Ptr->Output[0]), this, PortType.Output, Guid.NewGuid()));
        }
    }
}
