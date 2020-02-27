// <copyright file="IMotionCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Processors.Motion;
using System;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Represents a capture handler which can detect motion.
    /// </summary>
    public interface IMotionCaptureHandler
    {
        /// <summary>
        /// The motion type associated with this MotionCaptureHandler.
        /// </summary>
        MotionType MotionType { get; set; }

        /// <summary>
        /// Call to configure motion detection.
        /// </summary>
        /// <param name="config">The motion configuration.</param>
        /// <param name="onDetect">A callback for when motion is detected.</param>
        /// <param name="onStopDetect">An optional callback for when the record duration has passed.</param>
        void ConfigureMotionDetection(MotionConfig config, Action onDetect, Action onStopDetect = null);

        /// <summary>
        /// Enables motion detection. When configured, this will instruct the capture handler to detect motion.
        /// </summary>
        void EnableMotionDetection();

        /// <summary>
        /// Disables motion detection. When configured, this will instruct the capture handler not to detect motion.
        /// </summary>
        void DisableMotionDetection();
    }
}
