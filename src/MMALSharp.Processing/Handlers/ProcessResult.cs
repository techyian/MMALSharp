// <copyright file="ProcessResult.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Contains the user provided image data with for a process operation.
    /// </summary>
    public class ProcessResult
    {
        /// <summary>
        /// Gets or sets a value indicated whether the associated process operation succeeded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets a message associated with this <see cref="ProcessResult"/>.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the end of file has been reached.
        /// </summary>
        public bool EOF { get; set; }

        /// <summary>
        /// Gets or sets the buffer feed containing the image data.
        /// </summary>
        public byte[] BufferFeed { get; set; }

        /// <summary>
        /// Gets or sets the count of bytes in the buffer used for image data.
        /// </summary>
        public int DataLength { get; set; }

        /// <summary>
        /// Gets or sets the count of bytes allocated for the buffer.
        /// </summary>
        public int AllocSize { get; set; }
    }
}
