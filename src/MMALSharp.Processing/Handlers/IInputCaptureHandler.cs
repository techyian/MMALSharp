// <copyright file="IInputCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Represents a capture handler attached to an Input port.
    /// </summary>
    public interface IInputCaptureHandler : ICaptureHandler
    {
        /// <summary>
        /// Processes image data to an input port.
        /// </summary>
        /// <param name="allocSize">The feed chunk size.</param>
        /// <returns>A <see cref="ProcessResult"/> with the fed image data.</returns>
        ProcessResult Process(uint allocSize);
    }
}
