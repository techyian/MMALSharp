// <copyright file="MMALSplitterComponent.cs" company="Techyian">
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
    /// The Splitter Component is intended on being connected to the camera video output port. In turn, it
    /// provides an additional 4 output ports which can be used to produce multiple image/video outputs
    /// from the single camera video port.
    /// </summary>
    public class MMALSplitterComponent : MMALDownstreamHandlerComponent
    {
        /// <summary>
        /// Creates a new instance of <see cref="MMALSplitterComponent"/>.
        /// </summary>
        public unsafe MMALSplitterComponent()
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_SPLITTER)
        {
            this.Inputs.Add(new InputPort((IntPtr)(&(*this.Ptr->Input[0])), this, Guid.NewGuid()));

            for (var i = 0; i < 4; i++)
            {
                this.Outputs.Add(new SplitterVideoPort((IntPtr)(&(*this.Ptr->Output[i])), this, Guid.NewGuid()));
            }
        }

        /// <inheritdoc />
        public override IDownstreamComponent ConfigureInputPort(IMMALPortConfig config, IPort copyPort, IInputCaptureHandler handler)
        {
            var bufferNum = Math.Max(Math.Max(this.Inputs[0].BufferNumRecommended, 3), config.BufferNum);
            
            config = new MMALPortConfig(
                config.EncodingType,
                config.PixelFormat,
                config.Quality,
                config.Bitrate,
                config.Timeout,
                config.Split,
                config.StoreMotionVectors,
                config.Width,
                config.Height,
                config.Framerate,
                config.ZeroCopy,
                bufferNum,
                config.BufferSize,
                config.Crop);

            base.ConfigureInputPort(config, copyPort, handler);

            return this;
        }
    }
}
