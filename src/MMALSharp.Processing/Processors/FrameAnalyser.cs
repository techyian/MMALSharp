// <copyright file="FrameAnalyser.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Collections.Generic;
using Microsoft.Extensions.Logging;
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
        /// Creates a new instance of <see cref="FrameAnalyser"/>.
        /// </summary>
        protected FrameAnalyser()
        {
            this.WorkingData = new List<byte>();
        }

        /// <summary>
        /// Applies an operation.
        /// </summary>
        /// <param name="context">Contains the data and metadata for an image frame.</param>
        public virtual void Apply(ImageContext context)
        {
            if (this.FullFrame)
            {
                MMALLog.Logger.LogDebug("Clearing frame");
                this.WorkingData.Clear();
                this.FullFrame = false;
            }

            this.WorkingData.AddRange(context.Data);

            if (context.Eos)
            {
                this.FullFrame = true;
            }
        }
    }
}
