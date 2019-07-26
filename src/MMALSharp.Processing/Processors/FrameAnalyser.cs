using System.Collections.Generic;
using MMALSharp.Common;

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
            if (!eos && this.FullFrame)
            {
                // Start fresh frame.
                this.WorkingData.Clear();
            }

            if (!this.FullFrame)
            {
                this.WorkingData.AddRange(data);
            }

            if (eos)
            {
                this.FullFrame = true;
            }
        }
    }
}
