// <copyright file="InMemoryCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Collections.Generic;
using MMALSharp.Common.Utility;
using MMALSharp.Processors;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// A capture handler which stores its data to memory.
    /// </summary>
    public class InMemoryCaptureHandler : OutputCaptureHandler
    {
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
            MMALLog.Logger.Info($"Successfully processed {Helpers.ConvertBytesToMegabytes(this.WorkingData.Count)}.");
        }
        
        /// <inheritdoc />
        public override void Process(byte[] data, bool eos)
        {
            this.WorkingData.AddRange(data);
        }

        /// <inheritdoc />
        public override void PostProcess()
        {
            if (this.OnManipulate != null && this.ImageContext != null)
            {
                this.ImageContext.Data = this.WorkingData.ToArray();
                this.OnManipulate(new FrameProcessingContext(this.ImageContext));
                this.WorkingData = new List<byte>(this.ImageContext.Data);     
            }
        }

        /// <inheritdoc />
        public override string TotalProcessed()
        {
            return $"{this.WorkingData.Count}";
        }
    }
}