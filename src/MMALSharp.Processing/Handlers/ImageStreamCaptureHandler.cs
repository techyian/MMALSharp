// <copyright file="ImageStreamCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.IO;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Processes Image data to a <see cref="FileStream"/>.
    /// </summary>
    public class ImageStreamCaptureHandler : FileStreamCaptureHandler
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ImageStreamCaptureHandler"/> class with the specified directory and filename extension.
        /// </summary>
        /// <param name="directory">The directory to save captured images.</param>
        /// <param name="extension">The filename extension for saving files.</param>
        public ImageStreamCaptureHandler(string directory, string extension)
            : base(directory, extension) { }

        /// <summary>
        /// Creates a new instance of the <see cref="ImageStreamCaptureHandler"/> class with the specified file path.
        /// </summary>
        /// <param name="fullPath">The absolute full path to save captured data to.</param>
        public ImageStreamCaptureHandler(string fullPath)
            : base(fullPath) { }
    }
}
