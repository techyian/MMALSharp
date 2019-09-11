// <copyright file="MotionConfig.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Processors.Effects;

namespace MMALSharp.Processors.Motion
{
    /// <summary>
    /// This class is used to store user preferences when detecting motion between two image frames.
    /// </summary>
    public class MotionConfig
    {
        /// <summary>
        /// The edge detection sensitivity.
        /// </summary>
        public EDStrength Sensitivity { get; set; }

        /// <summary>
        /// How long should we record for when detected.
        /// </summary>
        public DateTime RecordDuration { get; set; }

        /// <summary>
        /// The threshold to use.
        /// </summary>
        public int Threshold { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="MotionConfig"/>.
        /// </summary>
        /// <param name="sensitivity">The edge detection sensitivity.</param>
        /// <param name="recordDuration">The record duration.</param>
        /// <param name="threshold">The threshold.</param>
        public MotionConfig(EDStrength sensitivity, DateTime recordDuration, int threshold)
        {
            this.Sensitivity = sensitivity;
            this.RecordDuration = recordDuration;
            this.Threshold = threshold;
        }
    }
}
