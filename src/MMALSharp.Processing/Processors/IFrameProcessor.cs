// <copyright file="IFrameProcessor.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;

namespace MMALSharp.Processors
{
    /// <summary>
    /// A processor to apply image processing techniques on image frame data.
    /// </summary>
    public interface IFrameProcessor
    {
        /// <summary>
        /// Apply the convolution.
        /// </summary>
        /// <param name="context">The image's metadata.</param>
        void Apply(ImageContext context);
    }
}
