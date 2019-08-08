using System.Collections.Generic;
using MMALSharp.Common;
using MMALSharp.Common.Utility;

namespace MMALSharp.Processors
{
    public abstract class FrameAnalyser : IFrameAnalyser
    {
        protected List<byte> WorkingData { get; set; }
        protected bool FullFrame { get; set; }
        protected IImageContext ImageContext { get; set; }

        protected FrameAnalyser(IImageContext imageContext)
        {
            this.WorkingData = new List<byte>();
            this.ImageContext = imageContext;
        }

        public virtual void Apply(byte[] data, bool eos)
        {
            if (this.FullFrame)
            {
                MMALLog.Logger.Info("Clearing frame");
                this.WorkingData.Clear();
                this.FullFrame = false;
            }

            this.WorkingData.AddRange(data);

            if (eos)
            {
                this.FullFrame = true;
            }
        }
    }
}
