// <copyright file="MMALImageDecoder.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common.Utility;
using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents an image decoder component.
    /// </summary>
    public class MMALImageDecoder : MMALEncoderBase
    {
        private int _width;
        private int _height;

        public MMALImageDecoder(ICaptureHandler handler)
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_DECODER, handler)
        {
        }

        public override int Width
        {
            get
            {
                if (this._width == 0)
                {
                    return MMALCameraConfig.StillResolution.Width;
                }

                return this._width;
            }
            set => this._width = value;
        }

        public override int Height
        {
            get
            {
                if (this._height == 0)
                {
                    return MMALCameraConfig.StillResolution.Height;
                }

                return this._height;
            }
            set => this._height = value;
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
