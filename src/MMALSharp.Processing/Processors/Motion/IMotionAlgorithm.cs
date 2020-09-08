// <copyright file="IMotionAlgorithm.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;
using MMALSharp.Handlers;

namespace MMALSharp.Processors.Motion
{
    /// <summary>
    /// Represents a frame-difference-based motion detection algorithm.
    /// </summary>
    public interface IMotionAlgorithm
    {
        /// <summary>
        /// Activates analysis mode.
        /// </summary>
        /// <param name="handler">Optional. If specified, the algorithm analyses each frame, marking cell corners,
        /// outlining cells with motion, and altering the output to grayscale highlights of the calculated diff per pixel.</param>
        void EnableAnalysis(IOutputCaptureHandler handler);

        /// <summary>
        /// Deactivates analysis mode.
        /// </summary>
        void DisableAnalysis();

        /// <summary>
        /// Invoked after the buffer's <see cref="FrameDiffDriver.TestFrame"/> is available
        /// for the first time and frame metrics have been collected. Allows the algorithm
        /// to modify the test frame, prepare matching local buffers, etc.
        /// </summary>
        /// <param name="driver">The <see cref="FrameDiffDriver"/> invoking this method.</param>
        /// <param name="metrics">Motion configuration and properties of the frame data.</param>
        /// <param name="contextTemplate">A sample context object which should be stored to feed to a capture handler if analysis is enabled.</param>
        void FirstFrameCompleted(FrameDiffDriver driver, FrameDiffMetrics metrics, ImageContext contextTemplate);

        /// <summary>
        /// Invoked when <see cref="FrameDiffDriver"/> has a full test frame and a
        /// new full comparison frame available.
        /// </summary>
        /// <param name="driver">The <see cref="FrameDiffDriver"/> invoking this method.</param>
        /// <param name="metrics">Motion configuration and properties of the frame data.</param>
        /// <returns>Indicates whether motion was detected.</returns>
        bool DetectMotion(FrameDiffDriver driver, FrameDiffMetrics metrics);

        /// <summary>
        /// Invoked when <see cref="FrameDiffDriver"/> has been reset. The algorithm should also
        /// reset stateful data, if any.
        /// </summary>
        /// <param name="driver">The <see cref="FrameDiffDriver"/> invoking this method.</param>
        /// <param name="metrics">Motion configuration and properties of the frame data.</param>
        void ResetAnalyser(FrameDiffDriver driver, FrameDiffMetrics metrics);
    }
}
