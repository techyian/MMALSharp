using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Common;
using MMALSharp.Common.PixelFormats;

namespace MMALSharp.Processors
{
    public class FrameProcessingContext : IFrameProcessingContext
    {
        private byte[] _buffer;
        private IImageContext _context;

        public FrameProcessingContext(byte[] currentState, IImageContext context)
        {
            _buffer = currentState;
            _context = context;
        }

        public IFrameProcessingContext Apply(IFrameProcessor processor)
        {
            processor.Apply(_buffer, _context);
            return this;
        }
    }
}
