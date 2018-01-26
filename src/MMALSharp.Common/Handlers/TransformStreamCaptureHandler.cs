// <copyright file="TransformStreamCaptureHandler.cs" company="Techyian">
// Copyright (c) Techyian. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.IO;
using MMALSharp.Handlers;

namespace MMALSharp.Common.Handlers
{
    public class TransformStreamCaptureHandler : StreamCaptureHandler
    {
        public Stream InputStream { get; set; }
        public int BufferSize { get; set; }
       
        public TransformStreamCaptureHandler(Stream inputStream, string outputDirectory, string outputExtension) : base(outputDirectory, outputExtension)
        {
            this.InputStream = inputStream;
        }

        public override ProcessResult Process()
        {
            var buffer = new byte[this.BufferSize];

            var read = this.InputStream.Read(buffer, 0, this.BufferSize);
       
            if (read < this.BufferSize)
            {       
                return new ProcessResult { Success = true, BufferFeed = buffer, EOF = true, DataLength = read };
            }
            
            return new ProcessResult { Success = true, BufferFeed = buffer, DataLength = read };
        }

        public override void Process(byte[] data)
        {
            this.Processed += data.Length;

            if (this.CurrentStream.CanWrite)
                this.CurrentStream.Write(data, 0, data.Length);
            else
                throw new IOException("Stream not writable.");
        }

        public void ConfigureBufferSize(int bufferSize)
        {
            this.BufferSize = bufferSize;
        }
    }
}
