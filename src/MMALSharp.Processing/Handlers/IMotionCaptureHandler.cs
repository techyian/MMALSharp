// <copyright file="IMotionCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Processors.Motion;
using System;
using MMALSharp.Common;

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
        /// Call to enable motion detection.
        /// </summary>
        /// <param name="config">The motion configuration.</param>
        /// <param name="onDetect">A callback for when motion is detected.</param>
        void DetectMotion(MotionConfig config, Action onDetect);
    }
}
