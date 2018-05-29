// <copyright file="MMALResizerComponent.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Handlers;
using System;
using static MMALSharp.Native.MMALParameters;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents the resizer component. This component has the ability to change the encoding type &amp; pixel format, as well
    /// as the width/height of resulting frames.
    /// </summary>
    public sealed class MMALResizerComponent : MMALDownstreamHandlerComponent
    {
        private int _width, _height; // Setting both values to 0 results in an EINVAL. Setting both values to 1 crashes the GPU.
        // Using very high values will first lead to an ENOMEM but there might be a point when you get again an EINVAL.

        /// <summary>
        /// Creates a new instance of the <see cref="MMALResizerComponent"/> class that can be used to change the size
        /// and the pixel format of resulting frames. 
        /// </summary>
        /// <param name="width">The width of the output frames. Value must be greater than 1.</param>
        /// <param name="height">The height of the output frames. Value must be greater than 1.</param>
        /// <param name="handler"></param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public MMALResizerComponent(int width, int height, ICaptureHandler handler)
            : base(MMAL_COMPONENT_DEFAULT_RESIZER, handler)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets or sets the width of resulting frames. Value must be greater than 1.
        /// </summary>
        public override int Width
        {
            get => _width;
            set
            {
                if (value < 2)
                    throw new ArgumentOutOfRangeException(nameof(Width), value, "Width must be greater than 1");
                _width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of resulting frames. Value must be greater than 1.
        /// </summary>
        public override int Height
        {
            get => _height;
            set
            {
                if (value < 2)
                    throw new ArgumentOutOfRangeException(nameof(Height), value, "Height must be greater than 1");
                _height = value;
            }
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
