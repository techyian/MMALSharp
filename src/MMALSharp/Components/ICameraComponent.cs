// <copyright file="ICameraComponent.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Handlers;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a camera component.
    /// </summary>
    public interface ICameraComponent : IComponent
    {
        /// <summary>
        /// The camera's preview port.
        /// </summary>
        IOutputPort PreviewPort { get; }

        /// <summary>
        /// The camera's video port.
        /// </summary>
        IOutputPort VideoPort { get; }

        /// <summary>
        /// The camera's still port.
        /// </summary>
        IOutputPort StillPort { get; }

        /// <summary>
        /// The managed camera info component object.
        /// </summary>
        ICameraInfoComponent CameraInfo { get; }

        /// <summary>
        /// Call to initialise the camera component.
        /// </summary>
        /// <param name="stillCaptureHandler">A capture handler when capturing raw image frames from the camera's still port (no encoder attached).</param>
        /// <param name="videoCaptureHandler">A capture handler when capturing raw video from the camera's video port (no encoder attached).</param>
        void Initialise(IOutputCaptureHandler stillCaptureHandler = null, IOutputCaptureHandler videoCaptureHandler = null);
    }
}
