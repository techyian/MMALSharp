using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Processors
{
    public class FrameProcessingContext : IFrameProcessingContext
    {
        private byte[] _buffer;

        public FrameProcessingContext(byte[] currentState)
        {
            _buffer = currentState;
        }

        public IFrameProcessingContext Apply(IFrameProcessor processor)
        {
            processor.Apply(_buffer);
            return this;
        }
    }
}
