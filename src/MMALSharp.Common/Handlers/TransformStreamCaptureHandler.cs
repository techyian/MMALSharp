using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Handlers;

namespace MMALSharp.Common.Handlers
{
    public class TransformStreamCaptureHandler : StreamCaptureHandler
    {
        public Stream InputStream { get; set; }
        public int BufferSize { get; set; }
        public int Offset { get; set; }

        public TransformStreamCaptureHandler(Stream inputStream, string outputDirectory, string outputExtension) : base(outputDirectory, outputExtension)
        {
            this.InputStream = inputStream;
        }

        public override ProcessResult Process()
        {
            var buffer = new byte[this.BufferSize];

            var read = this.InputStream.Read(buffer, 0, this.BufferSize);
            this.Offset += read;
            
            if (read == 0)
            {
                return new ProcessResult { Success = true, BufferFeed = buffer, EOF = true };
            }
            
            return new ProcessResult { Success = true, BufferFeed = buffer };
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
