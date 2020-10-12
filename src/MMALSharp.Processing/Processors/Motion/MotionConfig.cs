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
        /// Implements the logic used to detect motion.
        /// </summary>
        public IMotionAlgorithm MotionAlgorithm { get; set; }

        /// <summary>
        /// The frequency at which the test frame is updated. The test frame is the baseline against
        /// which the current frame is compared to detect motion.
        /// </summary>
        public TimeSpan TestFrameInterval { get; set; }

        /// <summary>
        /// The minimum duration with no motion detection events before the test frame will update.
        /// </summary>
        public TimeSpan TestFrameRefreshCooldown { get; set; }

        /// <summary>
        /// The name of a BMP file to apply as a motion-detection mask. The file must match the raw stream's
        /// width, height, and color depth. Black pixels (RGB 0,0,0) are not tested for motion.
        /// </summary>
        public string MotionMaskPathname { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="MotionConfig"/>.
        /// </summary>
        /// <param name="algorithm">An instance of the motion detection algorithm implementation.</param>
        /// <param name="testFrameInterval">Frequency at which the test frame is updated. The default is 10 seconds.</param>
        /// <param name="testFrameCooldown">The minimum duration with no motion detection before the test frame updates. The default is 3 seconds.</param>
        /// <param name="motionMaskPathname">Pathname to an optional motion-detection mask bitmap.</param>
        public MotionConfig(
            IMotionAlgorithm algorithm,
            TimeSpan testFrameInterval = default, 
            TimeSpan testFrameCooldown = default, 
            string motionMaskPathname = null)
        {
            this.MotionAlgorithm = algorithm;
            this.TestFrameInterval = testFrameInterval.Equals(TimeSpan.Zero) ? TimeSpan.FromSeconds(10) : testFrameInterval;
            this.TestFrameRefreshCooldown = testFrameCooldown.Equals(TimeSpan.Zero) ? TimeSpan.FromSeconds(3) : testFrameCooldown;
            this.MotionMaskPathname = motionMaskPathname;
        }
    }
}
