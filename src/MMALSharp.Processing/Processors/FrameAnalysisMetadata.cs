// <copyright file="FrameAnalysisMetadata.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Processors
{
    /// <summary>
    /// A structure for storing frame metadata used for parallel processing by image analysis
    /// and effects APIs. A struct is passed by-value which makes it a threadsafe local copy.
    /// Pass this structure to parallel processing algorithms as a method argument to prevent
    /// multiple threads from accessing the same copy.
    /// </summary>
    public struct FrameAnalysisMetadata
    {
        // Members are fields rather than properties for parallel processing performance reasons.
        // These must be value-type fields for thread safety. Object references would not be thread safe.

        /// <summary>
        /// Frame width in pixels.
        /// </summary>
        internal int Width;

        /// <summary>
        /// Frame height in pixels.
        /// </summary>
        internal int Height;

        /// <summary>
        /// Frame stride (bytes per row).
        /// </summary>
        internal int Stride;

        /// <summary>
        /// Frame bytes per pixel.
        /// </summary>
        internal int Bpp;

        /// <summary>
        /// Width of a parallel processing cell in pixels.
        /// </summary>
        internal int CellWidth;

        /// <summary>
        /// Height of a parallel processing cell in pixels.
        /// </summary>
        internal int CellHeight;
    }
}
