// <copyright file="MMALVideoDecoder.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Common.Utility;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a video decoder component.
    /// </summary>
    public class MMALVideoDecoder : MMALEncoderBase
    {
        private int _width;
        private int _height;
        
        /// <summary>
        /// Creates a new instance of <see cref="MMALVideoDecoder"/>.
        /// </summary>
        /// <param name="handler">The capture handler.</param>
        /// <param name="timeout">Optional timeout value.</param>
        public MMALVideoDecoder(ICaptureHandler handler)
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_DECODER, handler)
        {
        }

        /// <inheritdoc />
        public override MMALDownstreamComponent ConfigureOutputPort(int outputPort, MMALPortConfig config)
        {
            base.ConfigureOutputPort(outputPort, config);
            ((VideoPort)this.Outputs[outputPort]).Timeout = config.Timeout;

            return this;
        }
        
        /// <inheritdoc />>
        internal override void InitialiseOutputPort(int outputPort)
        {
            this.Outputs[outputPort] = new VideoPort(this.Outputs[outputPort]);
        }
    }
}
