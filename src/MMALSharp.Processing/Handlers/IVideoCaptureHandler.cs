// <copyright file="IVideoCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Processors.Motion;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Represents a VideoCaptureHandler for use when recording video frames.
    /// </summary>
    public interface IVideoCaptureHandler : IOutputCaptureHandler
    {
        /// <summary>
        /// The motion type associated with this VideoCaptureHandler.
        /// </summary>
        MotionType MotionType { get; set; }
        
        /// <summary>
        /// Signals that we should begin writing to a new video file.
        /// </summary>
        void Split();
    }
}
