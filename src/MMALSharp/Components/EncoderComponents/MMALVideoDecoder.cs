// <copyright file="MMALVideoDecoder.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

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
        /// <summary>
        /// Creates a new instance of <see cref="MMALVideoDecoder"/>.
        /// </summary>
        /// <param name="handler">The capture handler.</param>
        public MMALVideoDecoder(ICaptureHandler handler)
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_DECODER, handler)
        {
        }

        /// <inheritdoc />
        public override MMALDownstreamComponent ConfigureOutputPort(int outputPort, MMALPortConfig config)
        {
            base.ConfigureOutputPort(outputPort, config);

            if (this.Outputs[outputPort].GetType() == typeof(VideoPort) || this.Outputs[outputPort].GetType().IsSubclassOf(typeof(VideoPort)))
            {
                ((VideoPort)this.Outputs[outputPort]).Timeout = config.Timeout;
            }
            
            return this;
        }
        
        /// <inheritdoc />
        internal override void InitialiseOutputPort(int outputPort)
        {
            this.Outputs[outputPort] = new VideoPort(this.Outputs[outputPort]);
        }
    }
}
