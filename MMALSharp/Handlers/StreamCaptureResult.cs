using MMALSharp.Utility;
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
        private int _processed;

        public StreamCaptureResult(Stream stream)
        {
            this._stream = stream;
        }
                
        public void Process(byte[] data)
        {
            this._processed += data.Length;
            this._stream.Write(data, 0, data.Length);                               
        }

        public void PostProcess()
        {
            Console.WriteLine(string.Format("Successfully processed {0}", Helpers.ConvertBytesToMegabytes(this._processed)));
        }

    }
}
