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
        /// <summary>
        /// The working data.
        /// </summary>
        public byte[] Data { get; set; }
        
        /// <summary>
        /// Flag to indicate whether image frame is raw.
        /// </summary>
        public bool Raw { get; set; }
        
        /// <summary>
        /// The resolution of the frame we're processing.
        /// </summary>
        public Resolution Resolution { get; set; }

        /// <summary>
        /// The encoding format of the frame we're processing.
        /// </summary>
        public MMALEncoding Encoding { get; set; }

        /// <summary>
        /// The pixel format of the frame we're processing.
        /// </summary>
        public MMALEncoding PixelFormat { get; set; }
        
        /// <summary>
        /// The image format to store the processed data in.
        /// </summary>
        public ImageFormat StoreFormat { get; set; }

        /// <summary>
        /// Indicates if this frame represents the end of the stream.
        /// </summary>
        public bool Eos { get; set; }

        /// <summary>
        /// Indicates if this frame contains IFrame data.
        /// </summary>
        public bool IFrame { get; set; }

        /// <summary>
        /// The timestamp value.
        /// </summary>
        public long? Pts { get; set; }
        
        /// <summary>
        /// The pixel format stride.
        /// </summary>
        public int Stride { get; set; }
    }
}