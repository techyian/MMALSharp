// <copyright file="MMALImageDecoder.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents an image decoder component.
    /// </summary>
    public class MMALImageDecoder : MMALEncoderBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="MMALImageDecoder"/>.
        /// </summary>
        /// <param name="handler">The capture handler.</param>
        public unsafe MMALImageDecoder(ICaptureHandler handler)
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_DECODER)
        {
            this.Inputs.Add(new InputPort((IntPtr)(&(*this.Ptr->Input[0])), this, PortType.Input, Guid.NewGuid()));
            this.Outputs.Add(new StillPort((IntPtr)(&(*this.Ptr->Output[0])), this, PortType.Output, Guid.NewGuid(), handler));
        }
    }
}
