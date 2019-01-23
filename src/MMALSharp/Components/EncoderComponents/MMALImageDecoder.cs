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
        /// <summary>
        /// Creates a new instance of <see cref="MMALImageDecoder"/>.
        /// </summary>
        /// <param name="handler">The capture handler.</param>
        public MMALImageDecoder(ICaptureHandler handler)
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_DECODER, handler)
        {
        }
    }
}
