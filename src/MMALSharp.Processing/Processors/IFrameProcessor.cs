// <copyright file="IFrameProcessor.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;

namespace MMALSharp.Processors
{
    public interface IFrameProcessor
    {
        /// <summary>
        /// Apply the convolution.
        /// </summary>
        /// <param name="store">The image data.</param>
        /// <param name="context">The image's metadata.</param>
        void Apply(byte[] store, IImageContext context);
    }
}
