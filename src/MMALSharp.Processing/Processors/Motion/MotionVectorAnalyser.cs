// <copyright file="MotionVectorAnalyser.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Common;

namespace MMALSharp.Processors.Motion
{
    /// <summary>
    /// A frame analyser for use with motion vector detection.
    /// </summary>
    public class MotionVectorAnalyser : FrameAnalyser
    {
        internal Action OnDetect { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="MotionVectorAnalyser"/>.
        /// </summary>
        /// <param name="imageContext">The image metadata.</param>
        public MotionVectorAnalyser(IImageContext imageContext) 
            : base(imageContext)
        {
        }

        /// <inheritdoc />
        public override void Apply(byte[] data, bool eos)
        {
        }
    }
}
