using System;
using System.IO;

namespace MMALSharp.Handlers
{
    public class MemoryStreamCaptureHandler : StreamCaptureHandler<MemoryStream>
    {
        /// <summary>
        /// The working storage of this capture handler. Stores all data processed by a component.
        /// </summary>
        public MemoryStream Stream { get; protected set; }
        
        public MemoryStreamCaptureHandler()
        {
            this.Stream = new MemoryStream();
        }

        public MemoryStreamCaptureHandler(int size)
        {
            this.Stream = new MemoryStream(size);
        }
    }
}
