using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Common;

namespace MMALSharp.Processors.Motion
{
    public class MotionVectorAnalyser : FrameAnalyser
    {
        internal Action OnDetect { get; set; }

        public MotionVectorAnalyser(IImageContext imageContext) 
            : base(imageContext)
        {
        }

        public override void Apply(byte[] data, bool eos)
        {

        }
    }
}
