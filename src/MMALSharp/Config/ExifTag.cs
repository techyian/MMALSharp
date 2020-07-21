// <copyright file="ExifTag.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Config
{
    /// <summary>
    /// Represents an Exif tag for use with JPEG still captures.
    /// </summary>
    public class ExifTag
    {
        /// <summary>
        /// The Exif key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The Exif value.
        /// </summary>
        public string Value { get; set; }
    }
}
