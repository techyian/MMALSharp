// <copyright file="ImageContext.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Drawing.Imaging;
using MMALSharp.Common.Utility;

namespace MMALSharp.Common
{
    /// <summary>
    /// Represents a context to hold metadata for image frames.
    /// </summary>
    public class ImageContext
    {
        // Fields are used rather than properties for hot-path performance reasons.

        /// <summary>
        /// The working data.
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// Flag to indicate whether image frame is raw.
        /// </summary>
        public bool Raw;
        
        /// <summary>
        /// The resolution of the frame we're processing.
        /// </summary>
        public Resolution Resolution;

        /// <summary>
        /// The encoding format of the frame we're processing.
        /// </summary>
        public MMALEncoding Encoding;

        /// <summary>
        /// The pixel format of the frame we're processing.
        /// </summary>
        public MMALEncoding PixelFormat;

        /// <summary>
        /// The image format to store the processed data in.
        /// </summary>
        public ImageFormat StoreFormat;

        /// <summary>
        /// Indicates if this frame represents the end of the stream.
        /// </summary>
        public bool Eos;

        /// <summary>
        /// Indicates if this frame contains IFrame data.
        /// </summary>
        public bool IFrame;

        /// <summary>
        /// The timestamp value.
        /// </summary>
        public long? Pts;

        /// <summary>
        /// The pixel format stride.
        /// </summary>
        public int Stride;
    }
}