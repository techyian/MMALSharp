// <copyright file="InMemoryCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Collections.Generic;
using MMALSharp.Processors;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// A capture handler which stores its data to memory.
    /// </summary>
    public class InMemoryCaptureHandler : CaptureHandlerProcessorBase
    {
        public List<byte> WorkingData { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="InMemoryCaptureHandler"/>.
        /// </summary>
        public InMemoryCaptureHandler()
        {
            this.WorkingData = new List<byte>();
        }
        
        /// <inheritdoc />
        public override void Dispose()
        {
            // Not required.
        }
        
        /// <inheritdoc />
        public override void Process(byte[] data)
        {
            this.WorkingData.AddRange(data);
        }

        /// <inheritdoc />
        public override void PostProcess()
        {
            if (_manipulate != null && _imageContext != null)
            {
                _imageContext.Data = this.WorkingData.ToArray();
                _manipulate(new FrameProcessingContext(_imageContext));
                this.WorkingData = new List<byte>(_imageContext.Data);     
            }
        }

        /// <inheritdoc />
        public override string TotalProcessed()
        {
            return $"{this.WorkingData.Count}";
        }
    }
}