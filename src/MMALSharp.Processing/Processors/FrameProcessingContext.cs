// <copyright file="FrameProcessingContext.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;

namespace MMALSharp.Processors
{
    /// <summary>
    /// A context providing a means to apply image processing. 
    /// </summary>
    public class FrameProcessingContext : IFrameProcessingContext
    {
        private IImageContext _context;

        /// <summary>
        /// Creates a new instance of <see cref="FrameProcessingContext"/>.
        /// </summary>
        /// <param name="context">Metadata for the image frame.</param>
        public FrameProcessingContext(IImageContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public IFrameProcessingContext Apply(IFrameProcessor processor)
        {
            processor.Apply(_context);
            
            return this;
        }
    }
}
