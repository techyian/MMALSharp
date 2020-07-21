// <copyright file="ICaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Provides the functionality to process user provided or captured image data.
    /// </summary>
    public interface ICaptureHandler : IDisposable
    {
        /// <summary>
        /// Returns a string of how much data has been processed by this capture handler.
        /// </summary>
        /// <returns>How much data has been processed by this capture handler.</returns>
        string TotalProcessed();
    }
}
