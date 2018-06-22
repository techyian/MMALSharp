using System;
using System.IO;

namespace MMALSharp.Handlers
{
    public class MemoryStreamCaptureHandler : StreamCaptureHandler<MemoryStream>
    {
        public MemoryStreamCaptureHandler()
        {
            this.CurrentStream = new MemoryStream();
        }

        public MemoryStreamCaptureHandler(int size)
        {
            this.CurrentStream = new MemoryStream(size);
        }
    }
}
