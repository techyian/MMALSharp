using System;
using MMALSharp.Common;
using MMALSharp.Processors;

namespace MMALSharp.Handlers
{
    public abstract class OutputCaptureHandlerProcessor : IOutputCaptureHandler
    {
        protected Action<IFrameProcessingContext> OnManipulate { get; set; }
        protected Func<IFrameProcessingContext, IFrameAnalyser> OnAnalyse { get; set; }
        protected IImageContext ImageContext { get; set; }
        
        public abstract void Dispose();

        /// <inheritdoc />
        public abstract string TotalProcessed();
        
        /// <summary>
        /// Processes the data passed into this method to this class' Stream instance.
        /// </summary>
        /// <param name="data">The image data.</param>
        /// <param name="eos">Is end of stream.</param>
        public virtual void Process(byte[] data, bool eos)
        {
        }

        /// <summary>
        /// Allows us to do any further processing once the capture method has completed.
        /// </summary>
        public virtual void PostProcess()
        {
        }

        /// <summary>
        /// Allows manipulating of the image frame.
        /// </summary>
        /// <param name="context">A delegate to the manipulation you wish to carry out.</param>
        /// <param name="imageContext">Metadata for the image frame.</param>
        public void Manipulate(Action<IFrameProcessingContext> context, IImageContext imageContext)
        {
            this.OnManipulate = context;
            this.ImageContext = imageContext;
        }
    }
}