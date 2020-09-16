// <copyright file="IMotionCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
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
        void ConfigureMotionDetection(MotionConfig config, Action onDetect);

        /// <summary>
        /// Enables motion detection. When configured, this will instruct the capture handler to detect motion.
        /// </summary>
        void EnableMotionDetection();

        /// <summary>
        /// Disables motion detection. When configured, this will instruct the capture handler not to detect motion.
        /// </summary>
        /// <param name="disableCallbackOnly">When true, motion detection will continue but the OnDetect callback
        /// will not be invoked. Call <see cref="EnableMotionDetection"/> to re-enable the callback.</param>
        void DisableMotionDetection(bool disableCallbackOnly = false);
    }
}
