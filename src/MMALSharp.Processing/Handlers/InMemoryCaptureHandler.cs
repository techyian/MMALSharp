using System;
using System.Collections.Generic;
using MMALSharp.Common;
using MMALSharp.Processors;

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

        public void Manipulate(Action<IFrameProcessingContext> context, IImageContext imageContext)
        {
            var tempData = this.WorkingData.ToArray();
            context(new FrameProcessingContext(tempData, imageContext));
            this.WorkingData = new List<byte>(tempData);
        }
    }
}