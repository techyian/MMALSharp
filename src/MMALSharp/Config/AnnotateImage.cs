// <copyright file="AnnotateImage.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Drawing;

namespace MMALSharp.Config
{
    /// <summary>
    /// The <see cref="AnnotateImage"/> type is for use with the image annotation functionality.
    /// This will produce a textual overlay on image stills depending on the options enabled.
    /// </summary>
    public class AnnotateImage
    {
        /// <summary>
        /// Custom text to overlay on the stills capture.
        /// </summary>
        public string CustomText { get; set; }

        /// <summary>
        /// The text size to use.
        /// </summary>
        public int TextSize { get; set; }

        /// <summary>
        /// The <see cref="Color"/> of the text.
        /// </summary>
        public Color TextColour { get; set; } = Color.Empty;

        /// <summary>
        /// The <see cref="Color"/> of the background. Note: ShowBlackBackground should be enabled
        /// for this to work.
        /// </summary>
        public Color BgColour { get; set; } = Color.Empty;

        /// <summary>
        /// Show shutter settings.
        /// </summary>
        public bool ShowShutterSettings { get; set; }

        /// <summary>
        /// Show gain settings.
        /// </summary>
        public bool ShowGainSettings { get; set; }

        /// <summary>
        /// Show lens settings.
        /// </summary>
        public bool ShowLensSettings { get; set; }

        /// <summary>
        /// Show Continuous Auto Focus settings.
        /// </summary>
        public bool ShowCafSettings { get; set; }

        /// <summary>
        /// Show motion settings.
        /// </summary>
        public bool ShowMotionSettings { get; set; }

        /// <summary>
        /// Show the frame number.
        /// </summary>
        public bool ShowFrameNumber { get; set; }

        /// <summary>
        /// Allows custom background colour to be used.
        /// </summary>
        public bool ShowBlackBackground { get; set; }

        /// <summary>
        /// Show the current date.
        /// </summary>
        public bool ShowDateText { get; set; }

        /// <summary>
        /// Show the current time.
        /// </summary>
        public bool ShowTimeText { get; set; }
        
        /// <summary>
        /// Justify annotation text. 0 = centre, 1 = left, 2 = right.
        /// </summary>
        public int Justify { get; set; }
        
        /// <summary>
        /// X Offset from the justification edge.
        /// </summary>
        public int XOffset { get; set; }
        
        /// <summary>
        /// Y Offset from the justification edge.
        /// </summary>
        public int YOffset { get; set; }
    }
}
