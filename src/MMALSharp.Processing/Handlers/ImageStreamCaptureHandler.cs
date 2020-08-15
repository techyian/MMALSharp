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
        /// Creates a new instance of the <see cref="ImageStreamCaptureHandler"/> class with the specified directory and filename extension. Filenames will be in the
        /// format defined by the <see cref="FilenameDateTimeFormat"/> property.
        /// </summary>
        /// <param name="directory">The directory to save captured images.</param>
        /// <param name="extension">The filename extension for saving files.</param>
        /// <param name="continuousCapture">When true, every frame is written to a file.</param>
        public ImageStreamCaptureHandler(string directory, string extension, bool continuousCapture = true)
            : base(directory, extension, continuousCapture) { }

        /// <summary>
        /// Creates a new instance of the <see cref="ImageStreamCaptureHandler"/> class with the specified file pathname. An auto-incrementing number is added to each
        /// new filename.
        /// </summary>
        /// <param name="fullPath">The absolute full path to save captured data to.</param>
        /// <param name="continuousCapture">When true, every frame is written to a file.</param>
        public ImageStreamCaptureHandler(string fullPath, bool continuousCapture = true)
            : base(fullPath, continuousCapture) { }
    }
}
