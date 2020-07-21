// <copyright file="JpegThumbnail.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Config
{
    /// <summary>
    /// Provides config settings for JPEG thumbnail embedding.
    /// </summary>
    public class JpegThumbnail
    {
        /// <summary>
        /// Enable JPEG thumbnail.
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// The width of the thumbnail.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the thumbnail.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The quality of the thumbnail.
        /// </summary>
        public int Quality { get; set; }

        /// <summary>
        /// Create a new instance of <see cref="JpegThumbnail"/>.
        /// </summary>
        /// <param name="enable">Enable JPEG thumbnail.</param>
        /// <param name="width">The width of the thumbnail.</param>
        /// <param name="height">The height of the thumbnail.</param>
        /// <param name="quality">The quality of the thumbnail.</param>
        public JpegThumbnail(bool enable, int width, int height, int quality)
        {
            this.Enable = enable;
            this.Width = width;
            this.Height = height;
            this.Quality = quality;
        }
    }
}
