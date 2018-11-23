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
        private byte[] _buffer;
        private IImageContext _context;

        /// <summary>
        /// Creates a new instance of <see cref="FrameProcessingContext"/>.
        /// </summary>
        /// <param name="currentState">The image frame.</param>
        /// <param name="context">Metadata for the image frame.</param>
        public FrameProcessingContext(byte[] currentState, IImageContext context)
        {
            _buffer = currentState;
            _context = context;
        }

        /// <inheritdoc />
        public IFrameProcessingContext Apply(IFrameProcessor processor)
        {
            processor.Apply(_buffer, _context);
            return this;
        }
    }
}
