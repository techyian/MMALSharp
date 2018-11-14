// <copyright file="MMALSplitterComponent.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common.Utility;
using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp.Components
{
    /// <summary>
    /// The Splitter Component is intended on being connected to the camera video output port. In turn, it
    /// provides an additional 4 output ports which can be used to produce multiple image/video outputs
    /// from the single camera video port.
    /// </summary>
    public class MMALSplitterComponent : MMALDownstreamHandlerComponent
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
        /// Creates a new instance of <see cref="MMALSplitterComponent"/>.
        /// </summary>
        /// <param name="handler">The capture handlers to associate with each splitter port.</param>
        public MMALSplitterComponent(params ICaptureHandler[] handler)
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_SPLITTER, handler)
        {
        }

        /// <summary>
        /// Prints a summary of the ports and the resolution associated with this component to the console.
        /// </summary>
        public override void PrintComponent()
        {
            base.PrintComponent();
            MMALLog.Logger.Info($"    Width: {this.Width}. Height: {this.Height}");
        }
    }
}
