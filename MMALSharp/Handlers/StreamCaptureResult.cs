using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    public class StreamCaptureResult : ICaptureHandler<Stream>
    {
        private Stream _stream;

        public StreamCaptureResult(Stream stream)
        {
            this._stream = stream;
        }

        public Stream Process(byte[] data)
        {
            using (var writer = new BinaryWriter(this._stream))
            {
                writer.Write(data);
            }

            return this._stream;
        }
    }
}
