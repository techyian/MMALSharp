// <copyright file="PreviewConfiguration.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Drawing;
using MMALSharp.Components;
using MMALSharp.Native;

namespace MMALSharp.Config
{
    /// <summary>
    /// Defines the settings for a <see cref="MMALVideoRenderer"/> component.
    /// </summary>
    public class PreviewConfiguration
    {
        /// <summary>
        /// Indicates whether to use full screen or windowed mode.
        /// </summary>
        public bool FullScreen { get; set; } = true;

        /// <summary>
        /// If set to true, indicates that any display scaling should disregard the aspect ratio of the frame region being displayed.
        /// </summary>
        public bool NoAspect { get; set; }

        /// <summary>
        /// Enable copy protection. 
        /// Note: Doesn't appear to be supported by the firmware.
        /// </summary>
        public bool CopyProtect { get; set; }

        /// <summary>
        /// Specifies where the preview overlay should be drawn on the screen.
        /// </summary>
        public Rectangle PreviewWindow { get; set; } = new Rectangle(0, 0, 1024, 768);

        /// <summary>
        /// Opacity of the preview windows. Value between 1 (fully invisible) - 255 (fully opaque).
        /// Note: If RGBA encoding is used with the preview component then the alpha channel will be ignored.
        /// </summary>
        public int Opacity { get; set; } = 255;

        /// <summary>
        /// Sets the relative depth of the images, with greater values being in front of smaller values.
        /// </summary>
        public int Layer { get; set; } = 2;

        /// <summary>
        /// Indicates whether any flipping or rotation should be used on the overlay.
        /// </summary>
        public MMALParametersVideo.MMAL_DISPLAYTRANSFORM_T DisplayTransform { get; set; }

        /// <summary>
        /// Indicates how the image should be scaled to fit the display.
        /// </summary>
        public MMALParametersVideo.MMAL_DISPLAYMODE_T DisplayMode { get; set; }
    }
}
