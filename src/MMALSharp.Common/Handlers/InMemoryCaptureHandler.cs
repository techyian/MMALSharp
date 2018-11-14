using System;
using System.Collections.Generic;

namespace MMALSharp.Handlers
{
    public class InMemoryCaptureHandler : ICaptureHandler
    {
        public List<byte> WorkingData { get; set; }

        public InMemoryCaptureHandler()
        {
            this.WorkingData = new List<byte>();
        }
        
        public void Dispose()
        {
            // Not required.
        }

        public virtual ProcessResult Process(uint allocSize)
        {
            return new ProcessResult();
        }

        public virtual void Process(byte[] data)
        {
            this.WorkingData.AddRange(data);
        }

        public virtual void PostProcess()
        {
        }
    }
}