// <copyright file="ProcessedFileResult.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Represents a file that contains the image data written by a capture handler.
    /// </summary>
    public class ProcessedFileResult
    {
        /// <summary>
        /// Gets or sets the directory path of the file.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// Gets or sets the filename without extension.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the file extension without the leading dot.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="ProcessedFileResult"/> class with the specified directory, filename and extension.
        /// </summary>
        /// <param name="directory">The directory path of the file.</param>
        /// <param name="filename">The filename without extension.</param>
        /// <param name="extension">The file extension without the leading dot.</param>
        public ProcessedFileResult(string directory, string filename, string extension)
        {
            this.Directory = directory;
            this.Filename = filename;
            this.Extension = extension;
        }
    }
}
