// <copyright file="IOutputCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Represents a capture handler attached to an Output port.
    /// </summary>
    public interface IOutputCaptureHandler : ICaptureHandler
    {
        /// <summary>
        /// Used to process the image data from an output port.
        /// </summary>
        /// <param name="context">Contains the data and metadata for an image frame.</param>
        void Process(ImageContext context);

        /// <summary>
        /// Used for any further processing once we have completed capture.
        /// </summary>
        void PostProcess();
    }
}
