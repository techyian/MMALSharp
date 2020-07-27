// <copyright file="MotionConfig.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;

namespace MMALSharp.Processors.Motion
{
    /// <summary>
    /// This class is used to store user preferences when detecting motion between two image frames.
    /// </summary>
    public class MotionConfig
    {
        /// <summary>
        /// The amount of change which will trigger a motion event.
        /// </summary>
        public int Threshold { get; set; }

        /// <summary>
        /// The frequency at which the test frame is updated. The test frame is the baseline against
        /// which the current frame is compared to detect motion.
        /// </summary>
        public TimeSpan TestFrameInterval { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="MotionConfig"/>.
        /// </summary>
        /// <param name="threshold">Motion sensitivity threshold. The default is 130 (suitable for many indoor scenes).</param>
        /// <param name="testFrameInterval">Frequency at which the test frame is updated. The default is 10 seconds.</param>
        public MotionConfig(int threshold = 130, TimeSpan testFrameInterval = default)
        {
            this.Threshold = threshold;
            this.TestFrameInterval = testFrameInterval.Equals(TimeSpan.Zero) ? TimeSpan.FromSeconds(10) : testFrameInterval;
        }
    }
}
