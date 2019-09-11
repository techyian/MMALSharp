using System.Collections.Generic;
using MMALSharp.Common;
using MMALSharp.Common.Utility;

namespace MMALSharp.Processors
{
    /// <summary>
    /// The FrameAnalyser class is used with the Image Analysis API.
    /// </summary>
    public abstract class FrameAnalyser : IFrameAnalyser
    {
        /// <summary>
        /// The frame we are working with.
        /// </summary>
        protected List<byte> WorkingData { get; set; }

        /// <summary>
        /// True if the working data store contains a full frame.
        /// </summary>
        protected bool FullFrame { get; set; }

        /// <summary>
        /// The image metadata.
        /// </summary>
        protected IImageContext ImageContext { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="FrameAnalyser"/>.
        /// </summary>
        /// <param name="imageContext">The image metadata.</param>
        protected FrameAnalyser(IImageContext imageContext)
        {
            this.WorkingData = new List<byte>();
            this.ImageContext = imageContext;
        }

        /// <summary>
        /// Applies an operation.
        /// </summary>
        /// <param name="data">The new image frame data.</param>
        /// <param name="eos">Marks end of stream.</param>
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
