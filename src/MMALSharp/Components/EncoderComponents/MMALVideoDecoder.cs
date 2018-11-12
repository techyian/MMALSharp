// <copyright file="MMALVideoDecoder.cs" company="Techyian">
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
    /// Represents a video decoder component.
    /// </summary>
    public class MMALVideoDecoder : MMALEncoderBase
    {
        private int _width;
        private int _height;

        /// <inheritdoc />
        public override int Width
        {
            get
            {
                if (_width == 0)
                {
                    return MMALCameraConfig.VideoResolution.Width;
                }

                return _width;
            }
            set { _width = value; }
        }

        /// <inheritdoc />
        public override int Height
        {
            get
            {
                if (_height == 0)
                {
                    return MMALCameraConfig.VideoResolution.Height;
                }

                return _height;
            }
            set { _height = value; }
        }

        /// <summary>
        /// A <see cref="DateTime"/> for when you want processing to terminate on this component.
        /// </summary>
        public DateTime? Timeout { get; set; }
        
        public MMALVideoDecoder(ICaptureHandler handler, DateTime? timeout = null)
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_DECODER, handler)
        {
            this.Timeout = timeout;
        }

        /// <inheritdoc />>
        public override MMALDownstreamComponent ConfigureOutputPort(int outputPort, MMALEncoding encodingType, MMALEncoding pixelFormat, int quality, int bitrate = 0, bool zeroCopy = false)
        {
            base.ConfigureOutputPort(outputPort, encodingType, pixelFormat, quality, bitrate, zeroCopy);
            ((VideoPort)this.Outputs[outputPort]).Timeout = this.Timeout;

            return this;
        }

        /// <summary>
        /// Prints a summary of the ports and the resolution associated with this component to the console.
        /// </summary>
        public override void PrintComponent()
        {
            base.PrintComponent();
            MMALLog.Logger.Info($"    Width: {this.Width}. Height: {this.Height}");
        }

        /// <inheritdoc />>
        internal override void InitialiseOutputPort(int outputPort)
        {
            this.Outputs[outputPort] = new VideoPort(this.Outputs[outputPort]);
        }
    }
}
