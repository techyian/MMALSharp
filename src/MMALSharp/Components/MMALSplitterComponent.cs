// <copyright file="MMALSplitterComponent.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
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
    /// The Splitter Component can be connected to either the camera's still or video port and in turn, it
    /// provides an additional 4 output ports which can be used to produce multiple image/video outputs.
    /// </summary>
    public class MMALSplitterComponent : MMALDownstreamHandlerComponent
    {
        /// <summary>
        /// Creates a new instance of <see cref="MMALSplitterComponent"/>. Defaults to video port behaviour; configure the output port
        /// using <see cref="SplitterStillPort"/> as a generic constraint if this splitter is intended to be connected to the camera's still port.
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
