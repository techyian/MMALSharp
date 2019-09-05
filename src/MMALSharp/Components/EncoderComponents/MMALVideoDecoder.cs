// <copyright file="MMALVideoDecoder.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Components.EncoderComponents;
using MMALSharp.Native;
using MMALSharp.Ports;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a video decoder component.
    /// </summary>
    public class MMALVideoDecoder : MMALEncoderBase, IVideoDecoder
    {
        /// <summary>
        /// Creates a new instance of <see cref="MMALVideoDecoder"/>.
        /// </summary>
        public unsafe MMALVideoDecoder()
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_DECODER)
        {
            this.Inputs.Add(new InputPort((IntPtr)(&(*this.Ptr->Input[0])), this, Guid.NewGuid()));
            this.Outputs.Add(new VideoPort((IntPtr)(&(*this.Ptr->Output[0])), this, Guid.NewGuid()));
        }
    }
}
