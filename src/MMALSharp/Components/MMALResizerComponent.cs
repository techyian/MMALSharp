// <copyright file="MMALResizerComponent.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Handlers;
using static MMALSharp.Native.MMALParameters;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents the resizer component. This component has the ability to change the encoding type &amp; pixel format, as well
    /// as the width/height of resulting frames.
    /// </summary>
    public sealed class MMALResizerComponent : MMALDownstreamHandlerComponent
    {
        public override int Width { get; set; }

        public override int Height { get; set; }

        public MMALResizerComponent(int width, int height, ICaptureHandler handler)
            : base(MMAL_COMPONENT_DEFAULT_RESIZER, handler)
        {
            this.Width = width;
            this.Height = height;
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
