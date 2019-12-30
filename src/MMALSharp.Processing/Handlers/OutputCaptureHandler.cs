// <copyright file="OutputCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Common;
using MMALSharp.Processors;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Represents an OutputCaptureHandler which is responsible for storing frames processed by a component.
    /// </summary>
    public abstract class OutputCaptureHandler : IOutputCaptureHandler
    {
        /// <summary>
        /// A callback for use with the Image Processing API.
        /// </summary>
        protected Action<IFrameProcessingContext> OnManipulate { get; set; }

        /// <summary>
        /// A callback for use with the Image Analysis API.
        /// </summary>
        protected Func<IFrameProcessingContext, IFrameAnalyser> OnAnalyse { get; set; }

        /// <summary>
        /// An ImageContext providing metadata for image data.
        /// </summary>
        protected IImageContext ImageContext { get; set; }

        /// <inheritdoc />
        public abstract void Dispose();

        /// <inheritdoc />
        public abstract string TotalProcessed();

        /// <summary>
        /// The total amount of data that has been processed by this capture handler.
        /// </summary>
        protected int Processed { get; set; }

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