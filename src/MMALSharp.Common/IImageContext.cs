// <copyright file="IImageContext.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Drawing.Imaging;
using MMALSharp.Common.Utility;

namespace MMALSharp.Common
{
    /// <summary>
    /// Represents a context to hold metadata for image frames.
    /// </summary>
    public interface IImageContext
    {
        /// <summary>
        /// The frame data.
        /// </summary>
        byte[] Data { get; set; }

        /// <summary>
        /// Indicator if working with raw image data.
        /// </summary>
        bool Raw { get; }

        /// <summary>
        /// The frame resolution.
        /// </summary>
        Resolution Resolution { get; }

        /// <summary>
        /// The frame pixel format.
        /// </summary>
        PixelFormat PixelFormat { get; }

        /// <summary>
        /// The frame encoding format.
        /// </summary>
        ImageFormat StoreFormat { get; }
    }
}