// <copyright file="ImageStreamCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Processes the image data to a stream.
    /// </summary>
    public class ImageStreamCaptureHandler : StreamCaptureHandler
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ImageStreamCaptureHandler"/> class with the specified directory and filename extension.
        /// </summary>
        /// <param name="directory">The directory to save captured images.</param>
        /// <param name="extension">The filename extension for saving files.</param>
        public ImageStreamCaptureHandler(string directory, string extension)
            : base(directory, extension) { }
    }
}
