// <copyright file="TransformStreamCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.IO;
using MMALSharp.Common.Utility;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Reads data from a stream, processes this data and writes the result data on another stream.
    /// </summary>
    public class TransformStreamCaptureHandler : FileStreamCaptureHandler
    {
        /// <summary>
        /// Gets or sets the stream to retrieve input data from.
        /// </summary>
        public Stream InputStream { get; }
        
        private int TotalRead { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="TransformStreamCaptureHandler"/> class with the specified input stream, output directory and output filename extension.
        /// </summary>
        /// <param name="inputStream">The stream to retrieve input data from.</param>
        /// <param name="outputDirectory">The directory to save captured data.</param>
        /// <param name="outputExtension">The filename extension for saving files.</param>
        public TransformStreamCaptureHandler(Stream inputStream, string outputDirectory, string outputExtension)
            : base(outputDirectory, outputExtension)
        {
            this.InputStream = inputStream;
        }

        /// <summary>
        /// Reads data from this class' input stream to a <see cref="ProcessResult"/> object.
        /// </summary>
        /// <param name="allocSize">The count of bytes to return at most in the <see cref="ProcessResult"/>.</param>
        /// <returns>A <see cref="ProcessResult"/> object containing read image data.</returns>
        public override ProcessResult Process(uint allocSize)
        {
            var buffer = new byte[allocSize];

            var read = this.InputStream.Read(buffer, 0, (int)allocSize);

            this.TotalRead += read;

            if (read < allocSize)
            {
                return new ProcessResult { Success = true, BufferFeed = buffer, EOF = true, DataLength = read };
            }

            return new ProcessResult { Success = true, BufferFeed = buffer, DataLength = read };
        }

        /// <summary>
        /// Processes the data passed into this method to this class' Stream instance.
        /// </summary>
        /// <param name="data">The image data.</param>
        public override void Process(byte[] data)
        {
            this.Processed += data.Length;

            if (this.CurrentStream.CanWrite)
                this.CurrentStream.Write(data, 0, data.Length);
            else
                throw new IOException("Stream not writable.");
        }

        /// <inheritdoc />
        public override string TotalProcessed()
        {
            return $"{Helpers.ConvertBytesToMegabytes(this.TotalRead)} of {Helpers.ConvertBytesToMegabytes(this.InputStream.Length)}";
        }
    }
}
