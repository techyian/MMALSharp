// <copyright file="MotionConfig.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
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
        /// How long should we record for when motion is detected.
        /// </summary>
        public TimeSpan RecordDuration { get; set; }

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
        /// <param name="recordDuration">Recording duration. Use TimeSpan.Zero to record indefinitely (your code must tell the handler when to stop). Defaults to TimeSpan.Zero.</param>
        /// <param name="threshold">Motion sensitivity threshold. The default is 130 (suitable for many indoor scenes).</param>
        /// <param name="testFrameInterval">Frequency at which the test frame is updated. The default is 10 seconds.</param>
        public MotionConfig(TimeSpan recordDuration = default, int threshold = 130, TimeSpan testFrameInterval = default)
        {
            this.RecordDuration = recordDuration;
            this.Threshold = threshold;
            this.TestFrameInterval = testFrameInterval.Equals(TimeSpan.Zero) ? TimeSpan.FromSeconds(10) : testFrameInterval;
        }
    }
}
