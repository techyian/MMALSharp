// <copyright file="MMALSplitterComponent.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Components
{
    /// <summary>
    /// The Splitter Component is intended on being connected to the camera video output port. In turn, it
    /// provides an additional 4 output ports which can be used to produce multiple image/video outputs
    /// from the single camera video port.
    /// </summary>
    public class MMALSplitterComponent : MMALDownstreamHandlerComponent
    {
        /// <summary>
        /// Creates a new instance of <see cref="MMALSplitterComponent"/>.
        /// </summary>
        /// <param name="handler">The capture handlers to associate with each splitter port.</param>
        public MMALSplitterComponent(params ICaptureHandler[] handler)
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_SPLITTER, handler)
        {
        }

        /// <inheritdoc />
        public override unsafe MMALDownstreamComponent ConfigureInputPort(MMALEncoding encodingType, MMALEncoding pixelFormat, PortBase copyPort, bool zeroCopy = false)
        {
            base.ConfigureInputPort(encodingType, pixelFormat, copyPort, zeroCopy);

            this.Inputs[0].Ptr->BufferNum = Math.Max(this.Inputs[0].Ptr->BufferNumRecommended, 3);
            
            return this;
        }

        /// <inheritdoc />
        public override unsafe MMALDownstreamComponent ConfigureInputPort(MMALPortConfig config)
        {
            base.ConfigureInputPort(config);

            this.Inputs[0].Ptr->BufferNum = Math.Max(this.Inputs[0].Ptr->BufferNumRecommended, 3);

            return this;
        }
    }
}
