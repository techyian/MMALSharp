// <copyright file="OutputCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing.Imaging;
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
        protected ImageContext ImageContext { get; set; }
        
        /// <summary>
        /// The image format to save manipulated files in.
        /// </summary>
        protected ImageFormat StoreFormat { get; set; }

        /// <inheritdoc />
        public abstract void Dispose();

        /// <inheritdoc />
        public abstract string TotalProcessed();

        /// <summary>
        /// The total amount of data that has been processed by this capture handler.
        /// </summary>
        protected int Processed { get; set; }

        /// <summary>
        /// Used to process the image data from an output port. Users who extend this class should call the base class
        /// to ensure the ImageContext property is assigned with the current frame's context.
        /// </summary>
        /// <param name="context">Contains the data and metadata for an image frame.</param>
        public virtual void Process(ImageContext context)
        {
            this.ImageContext = context;
        }

        /// <summary>
        /// Allows us to do any further processing once the capture method has completed.
        /// </summary>
        public virtual void PostProcess()
        {
        }

        /// <summary>
        /// Allows manipulation of the image frame.
        /// </summary>
        /// <param name="context">A delegate to the manipulation you wish to carry out.</param>
        /// <param name="storeFormat">The image format to save manipulated files in, or null to return raw data.</param>
        public void Manipulate(Action<IFrameProcessingContext> context, ImageFormat storeFormat)
        {
            this.OnManipulate = context;
            this.StoreFormat = storeFormat;
        }
    }
}