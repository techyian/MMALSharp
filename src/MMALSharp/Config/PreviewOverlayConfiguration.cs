// <copyright file="PreviewOverlayConfiguration.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Components;

namespace MMALSharp.Config
{
    /// <summary>
    /// Defines the settings for a <see cref="MMALOverlayRenderer"/> component.
    /// </summary>
    public class PreviewOverlayConfiguration : PreviewConfiguration
    {
        /// <summary>
        /// Specifies the resolution of the static resource to be used with this Preview Overlay. If this is null then the parent renderer's resolution will be used instead.
        /// </summary>
        public Resolution Resolution { get; set; }

        /// <summary>
        /// The encoding of the static resource. Can be one of the following: YUV, RGB, RGBA, BGR, BGRA.
        /// If left null, we will try to work out the encoding based on the size of the image (3 bytes for RGB, 4 bytes for RGBA).
        /// </summary>
        public MMALEncoding Encoding { get; set; }
    }
}
