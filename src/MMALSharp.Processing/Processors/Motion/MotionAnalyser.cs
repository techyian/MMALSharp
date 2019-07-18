using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Processors.Motion
{
    public class MotionAnalyser : IFrameAnalyser
    {
        internal Action OnDetect { get; set; }

        internal MotionConfig MotionConfig { get; set; }

        public MotionAnalyser(MotionConfig config, Action onDetect)
        {
            this.MotionConfig = config;
            this.OnDetect = onDetect;
        }

        public void Apply()
        {
            throw new NotImplementedException();
        }
    }
}
