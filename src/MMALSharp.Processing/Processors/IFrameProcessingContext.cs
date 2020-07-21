// <copyright file="IFrameProcessingContext.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Processors
{
    /// <summary>
    /// A context to apply image processing techniques on image frames.
    /// </summary>
    public interface IFrameProcessingContext
    {
        /// <summary>
        /// Applies a processing function on an image frame.
        /// </summary>
        /// <param name="processor">The image processor.</param>
        /// <returns>The active image context.</returns>
        IFrameProcessingContext Apply(IFrameProcessor processor);
    }
}
