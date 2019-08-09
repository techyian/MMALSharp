// <copyright file="MMALVideoFileEncoder.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Components.EncoderComponents;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    /// <summary>
    /// This component is used to encode video data stored in a stream.
    /// </summary>
    public class MMALVideoFileEncoder : MMALFileEncoderBase, IVideoFileEncoder
    {
        /// <summary>
        /// Creates a new instance of <see cref="MMALImageFileEncoder"/>.
        /// </summary>
        /// <param name="handler">The capture handler.</param>
        public unsafe MMALVideoFileEncoder(ICaptureHandler handler)
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_ENCODER)
        {
            this.Inputs.Add(new VideoFileEncodeInputPort((IntPtr)(&(*this.Ptr->Input[0])), this, PortType.Input, Guid.NewGuid(), handler));
            this.Outputs.Add(new VideoFileEncodeOutputPort((IntPtr)(&(*this.Ptr->Output[0])), this, PortType.Output, Guid.NewGuid(), handler));
        }
    }
}
