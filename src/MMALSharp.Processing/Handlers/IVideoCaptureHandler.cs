// <copyright file="IVideoCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Represents a VideoCaptureHandler for use when recording video frames.
    /// </summary>
    public interface IVideoCaptureHandler : IOutputCaptureHandler
    {        
        /// <summary>
        /// Signals that we should begin writing to a new video file.
        /// </summary>
        void Split();
    }
}
