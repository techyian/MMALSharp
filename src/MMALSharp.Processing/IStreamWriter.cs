// <copyright file="ICaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Provides the functionality to write a stream to storage.
    /// </summary>
    public interface IStreamWriter
    {
        /// <summary>
        /// When true, the object is preparing to write the stream (it may require additional work such as waiting for the end of a frame).
        /// </summary>
        bool WriteRequested { get; set; }

        /// <summary>
        /// Immediately output the stream to a file. Not a request, typically in response to end-of-frame when <see cref="WriteRequested"/> is true.
        /// </summary>
        void WriteStreamToFile();

        /// <summary>
        /// Discard the current data buffered in the stream.
        /// </summary>
        void ResetStream();
    }
}
