// <copyright file="MMALImageFileEncoder.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Native;
using MMALSharp.Components.EncoderComponents;
using MMALSharp.Handlers;
using MMALSharp.Ports;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    /// <summary>
    /// This component is used to encode image data stored in a stream.
    /// </summary>
    public class MMALImageFileEncoder : MMALFileEncoderBase, IImageFileEncoder
    {
        /// <summary>
        /// Creates a new instance of <see cref="MMALImageFileEncoder"/>.
        /// </summary>
        /// <param name="handler">The capture handler.</param>
        public unsafe MMALImageFileEncoder()
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_ENCODER)
        {
            this.Inputs.Add(new ImageFileEncodeInputPort((IntPtr)(&(*this.Ptr->Input[0])), this, PortType.Input, Guid.NewGuid()));
            this.Outputs.Add(new ImageFileEncodeOutputPort((IntPtr)(&(*this.Ptr->Output[0])), this, PortType.Output, Guid.NewGuid()));
        }
    }
}
