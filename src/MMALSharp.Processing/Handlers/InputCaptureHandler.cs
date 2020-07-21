// <copyright file="InputCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.IO;
using MMALSharp.Common.Utility;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Represents an InputCaptureHandler which is responsible for feeding data from a stream.
    /// </summary>
    public class InputCaptureHandler : IInputCaptureHandler
    {
        /// <summary>
        /// Gets or sets the stream to retrieve input data from.
        /// </summary>
        public Stream CurrentStream { get; }

        /// <summary>
        /// The total amount of data processed by this <see cref="InputCaptureHandler"/>.
        /// </summary>
        public int Processed { get; protected set; }

        /// <summary>
        /// Creates a new instance of the <see cref="InputCaptureHandler"/> class with the specified input stream, output directory and output filename extension.
        /// </summary>
        /// <param name="inputStream">The stream to retrieve input data from.</param>
        public InputCaptureHandler(Stream inputStream)
        {
            this.CurrentStream = inputStream;
        }

        /// <summary>
        /// When overridden in a derived class, returns user provided image data.
        /// </summary>
        /// <param name="allocSize">The count of bytes to return at most in the <see cref="ProcessResult"/>.</param>
        /// <returns>A <see cref="ProcessResult"/> object containing the user provided image data.</returns>
        public virtual ProcessResult Process(uint allocSize)
        {
            var buffer = new byte[allocSize];

            var read = this.CurrentStream.Read(buffer, 0, (int)allocSize);

            this.Processed += read;

            if (read == 0)
            {
                return new ProcessResult { Success = true, BufferFeed = buffer, EOF = true, DataLength = read };
            }

            return new ProcessResult { Success = true, BufferFeed = buffer, DataLength = read };
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.CurrentStream?.Dispose();
        }

        /// <inheritdoc />
        public string TotalProcessed()
        {
            return $"{Helpers.ConvertBytesToMegabytes(this.Processed)}";
        }
    }
}
