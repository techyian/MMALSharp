using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Processes the image data to a stream.
    /// </summary>
    public class StreamCaptureResult : ICaptureHandler<Stream>
    {
        private Stream _stream;

        public StreamCaptureResult(Stream stream)
        {
            this._stream = stream;
        }
                
        public void Process(byte[] data)
        {                
            using (var writer = new BinaryWriter(this._stream))
            {
                writer.Write(data);
            }                        
        }        
    }
}
