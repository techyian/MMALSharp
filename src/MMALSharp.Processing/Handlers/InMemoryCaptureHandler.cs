// <copyright file="InMemoryCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Processors;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// A capture handler which stores its data to memory.
    /// </summary>
    public class InMemoryCaptureHandler : OutputCaptureHandler
    {
        private int _totalProcessed;

        /// <summary>
        /// The working data store.
        /// </summary>
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
            MMALLog.Logger.LogInformation($"Successfully processed {Helpers.ConvertBytesToMegabytes(_totalProcessed)}.");
        }

        /// <inheritdoc />
        public override void Process(ImageContext context)
        {
            this.WorkingData.AddRange(context.Data);
            _totalProcessed += context.Data.Length;
            base.Process(context);
        }

        /// <summary>
        /// Allows us to do any further processing once the capture method has completed. Note: It is the user's responsibility to 
        /// clear the WorkingData list after processing is complete.
        /// </summary>
        public override void PostProcess()
        {
            // When the context data length is zero, the data in the stream is a partial frame due to a race condition
            // where the hardware has started the next frame before the library has begun the shutdown process. The buffer
            // which triggered the call to PostProcess (from PortCallbackHandler) has a zero length which is what we're
            // checking for here.
            if (this.OnManipulate != null && this.ImageContext != null && this.ImageContext.Data.Length > 0)
            {
                this.ImageContext.Data = this.WorkingData.ToArray();
                this.OnManipulate(new FrameProcessingContext(this.ImageContext));
                this.WorkingData = new List<byte>(this.ImageContext.Data);     
            }
        }

        /// <inheritdoc />
        public override string TotalProcessed()
        {
            return $"{_totalProcessed}";
        }
    }
}