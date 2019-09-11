// <copyright file="ImageContext.cs" company="Techyian">
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
    public class ImageContext : IImageContext
    {
        /// <summary>
        /// The working data.
        /// </summary>
        public byte[] Data { get; set; }
        
        /// <summary>
        /// Flag to indicate whether image frame is raw.
        /// </summary>
        public bool Raw { get; }
        
        /// <summary>
        /// The resolution of the frame we're processing.
        /// </summary>
        public Resolution Resolution { get; }
        
        /// <summary>
        /// The pixel format of the frame we're processing.
        /// </summary>
        public PixelFormat PixelFormat { get; }
        
        /// <summary>
        /// The image format to store the processed data in.
        /// </summary>
        public ImageFormat StoreFormat { get; }

        /// <summary>
        /// Create a new instance of <see cref="ImageContext"/> with the provided resolution. Assumes RGB24 and stores result to JPEG format when using image convolution techniques.
        /// </summary>
        /// <param name="res">The resolution of the image to process.</param>
        public ImageContext(Resolution res)
        {
            this.Resolution = res;
        }

        /// <summary>
        /// Create a new instance of <see cref="ImageContext"/> with the provided resolution. Stores result to JPEG format when using image convolution techniques.
        /// </summary>
        /// <param name="res">The resolution of the image to process.</param>
        /// <param name="format">The pixel format of the data to process.</param>
        /// <param name="raw">Image frame is raw data.</param>
        public ImageContext(Resolution res, PixelFormat format, bool raw)
        {
            this.Resolution = res;
            this.PixelFormat = format;
            this.Raw = raw;
        }

        /// <summary>
        /// Create a new instance of <see cref="ImageContext"/> with the provided resolution.
        /// </summary>
        /// <param name="res">The resolution of the image to process.</param>
        /// <param name="format">The pixel format of the data to process.</param>
        /// <param name="raw">Image frame is raw data.</param>
        /// <param name="storeFormat">Image format to store processed data in.</param>
        public ImageContext(Resolution res, PixelFormat format, bool raw, ImageFormat storeFormat)
        {
            this.Resolution = res;
            this.PixelFormat = format;
            this.Raw = raw;
            this.StoreFormat = storeFormat;
        }
    }
}