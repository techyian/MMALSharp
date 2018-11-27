// <copyright file="VideoStreamCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Processes the video data to a stream.
    /// </summary>
    public class VideoStreamCaptureHandler : FileStreamCaptureHandler
    {
        /// <summary>
        /// Creates a new instance of the <see cref="VideoStreamCaptureHandler"/> class with the specified directory and filename extension.
        /// </summary>
        /// <param name="directory">The directory to save captured videos.</param>
        /// <param name="extension">The filename extension for saving files.</param>
        public VideoStreamCaptureHandler(string directory, string extension)
            : base(directory, extension) { }
        
        /// <summary>
        /// Splits the current file by closing the current stream and opening a new one.
        /// </summary>
        public void Split() => this.NewFile();
    }
}
