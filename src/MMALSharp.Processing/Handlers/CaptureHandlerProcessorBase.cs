using System;
using MMALSharp.Common;
using MMALSharp.Processors;

namespace MMALSharp.Handlers
{
    public abstract class CaptureHandlerProcessorBase : ICaptureHandler
    {
        protected Action<IFrameProcessingContext> _manipulate;
        protected IImageContext _imageContext;
        
        public abstract void Dispose();

        public abstract string TotalProcessed();
        
        /// <summary>
        /// When overridden in a derived class, returns user provided image data.
        /// </summary>
        /// <param name="allocSize">The count of bytes to return at most in the <see cref="ProcessResult"/>.</param>
        /// <returns>A <see cref="ProcessResult"/> object containing the user provided image data.</returns>
        public virtual ProcessResult Process(uint allocSize)
        {
            return new ProcessResult();
        }

        /// <summary>
        /// Processes the data passed into this method to this class' Stream instance.
        /// </summary>
        /// <param name="data">The image data.</param>
        public virtual void Process(byte[] data)
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
            _manipulate = context;
            _imageContext = imageContext;
        }
    }
}