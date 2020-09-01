// <copyright file="FrameDiffMetrics.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Processors.Motion
{
    /// <summary>
    /// A structure for storing frame metrics used in the parallel analysis algorithms.
    /// Structures are passed by value which makes them a threadsafe local copy.
    /// </summary>
    public struct FrameDiffMetrics
    {
        // Prefer fields over properties for parallel processing performance reasons.
        // These must be value-type fields for thread safety. Object references would
        // not be thread safe.

        /// <summary>
        /// Frame metrics collected when the first frame is completed.
        /// </summary>
        internal int FrameWidth;

        /// <summary>
        /// Frame metrics collected when the first frame is completed.
        /// </summary>
        internal int FrameHeight;

        /// <summary>
        /// Frame metrics collected when the first frame is completed.
        /// </summary>
        internal int FrameStride;

        /// <summary>
        /// Frame metrics collected when the first frame is completed.
        /// </summary>
        internal int FrameBpp;
    }
}
