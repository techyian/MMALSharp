// <copyright file="MemoryStreamCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.IO;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Processes frame data to a <see cref="MemoryStream"/>.
    /// </summary>
    public class MemoryStreamCaptureHandler : StreamCaptureHandler<MemoryStream>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MemoryStreamCaptureHandler"/>.
        /// </summary>
        public MemoryStreamCaptureHandler()
        {
            this.CurrentStream = new MemoryStream();
        }

        /// <summary>
        /// Creates a new instance of <see cref="MemoryStreamCaptureHandler"/> with a specified capacity.
        /// </summary>
        /// <param name="size">The capacity of the <see cref="MemoryStream"/>.</param>
        public MemoryStreamCaptureHandler(int size)
        {
            this.CurrentStream = new MemoryStream(size);
        }
    }
}
