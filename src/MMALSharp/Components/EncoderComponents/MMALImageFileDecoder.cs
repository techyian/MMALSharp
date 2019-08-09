// <copyright file="MMALImageFileDecoder.cs" company="Techyian">
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
    /// This component is used to decode image data stored in a stream.
    /// </summary>
    public class MMALImageFileDecoder : MMALFileEncoderBase, IImageFileDecoder
    {
        /// <summary>
        /// Creates a new instance of <see cref="MMALImageFileDecoder"/>.
        /// </summary>
        /// <param name="handler">The capture handle to use.</param>
        public unsafe MMALImageFileDecoder(ICaptureHandler handler)
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_DECODER)
        {
            this.Inputs.Add(new ImageFileDecodeInputPort((IntPtr)(&(*this.Ptr->Input[0])), this, PortType.Input, Guid.NewGuid(), handler));
            this.Outputs.Add(new ImageFileDecodeOutputPort((IntPtr)(&(*this.Ptr->Output[0])), this, PortType.Output, Guid.NewGuid(), handler));
        }
    }
}
