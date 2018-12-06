using System.Collections.Generic;
using MMALSharp.Processors;

namespace MMALSharp.Handlers
{
    public class InMemoryCaptureHandler : CaptureHandlerProcessorBase
    {
        public List<byte> WorkingData { get; set; }

        public InMemoryCaptureHandler()
        {
            this.WorkingData = new List<byte>();
        }
        
        public override void Dispose()
        {
            // Not required.
        }
        
        /// <inheritdoc />
        public override void Process(byte[] data)
        {
            this.WorkingData.AddRange(data);
        }

        /// <inheritdoc />
        public override void PostProcess()
        {
            if (_manipulate != null && _imageContext != null)
            {
                _imageContext.Data = this.WorkingData.ToArray();
                _manipulate(new FrameProcessingContext(_imageContext));
                this.WorkingData = new List<byte>(_imageContext.Data);     
            }
        }
        
        public override string TotalProcessed()
        {
            return $"{this.WorkingData.Count}";
        }
    }
}