// <copyright file="AnnotateImage.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Drawing;

namespace MMALSharp.Config
{
    /// <summary>
    /// Used to indicate how text should be justified when using annotation.
    /// </summary>
    public enum JustifyText
    {
        /// <summary>
        /// Centre aligned.
        /// </summary>
        Centre,

        /// <summary>
        /// Left aligned.
        /// </summary>
        Left,

        /// <summary>
        /// Right aligned.
        /// </summary>
        Right
    }

    /// <summary>
    /// Used to ensure the date/time annotations are updated for longer-running operations such as video recording or streaming.
    /// </summary>
    public enum DateTimeTextRefreshRate
    {
        /// <summary>
        /// Do not automatically refresh the <see cref="AnnotateImage.ShowDateText"/> and <see cref="AnnotateImage.ShowTimeText"/> annotations.
        /// These annotations can be explicitly refreshed by calling <see cref="MMALCamera.EnableAnnotation"/>.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Typically used when the time is not displayed.
        /// Update interval is once per minute.
        /// </summary>
        Daily = 60000,

        /// <summary>
        /// Typically used with the default "HH:mm" <see cref="AnnotateImage.TimeFormat"/>.
        /// Update interval is once per second.
        /// </summary>
        Minutes = 1000,

        /// <summary>
        /// Useful if the <see cref="AnnotateImage.TimeFormat"/> is altered to display seconds, such as "HH:mm:ss".
        /// Update interval is 250ms.
        /// </summary>
        Seconds = 250,

        /// <summary>
        /// Useful if the <see cref="AnnotateImage.TimeFormat"/> is altered to display fractional seconds, such as "HH:mm:ss.ffff".
        /// Update interval is 41ms, which approximately equates to 24FPS.
        /// </summary>
        SubSecond = 41
    }

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
        public int TextSize { get; set; } = 12;

        /// <summary>
        /// The <see cref="Color"/> of the text.
        /// </summary>
        public Color TextColour { get; set; } = Color.White;

        /// <summary>
        /// The <see cref="Color"/> of the background. Note: AllowCustomBackgroundColour should be enabled
        /// for this to work.
        /// </summary>
        public Color BgColour { get; set; }

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
        public bool AllowCustomBackgroundColour { get; set; }

        /// <summary>
        /// Show the current date.
        /// </summary>
        public bool ShowDateText { get; set; } = true;

        /// <summary>
        /// Show the current time.
        /// </summary>
        public bool ShowTimeText { get; set; } = true;

        /// <summary>
        /// The DateTime format string applied when <see cref="ShowDateText"/> is true. The default is "dd/MM/yyyy".
        /// </summary>
        public string DateFormat { get; set; } = "dd/MM/yyyy";

        /// <summary>
        /// The DateTime format string applied when <see cref="ShowTimeText"/> is true. The default is "HH:mm".
        /// </summary>
        public string TimeFormat { get; set; } = "HH:mm";

        /// <summary>
        /// The approximate frequency at which date and time annotations are refreshed. The default is 
        /// <see cref="DateTimeTextRefreshRate.Minutes"/> which matches the resolution of the default <see cref="TimeFormat"/>.
        /// </summary>
        public DateTimeTextRefreshRate RefreshRate { get; set; } = DateTimeTextRefreshRate.Minutes;
        
        /// <summary>
        /// Justify annotation text.
        /// </summary>
        public JustifyText Justify { get; set; }
        
        /// <summary>
        /// X Offset from the justification edge.
        /// </summary>
        public int XOffset { get; set; }
        
        /// <summary>
        /// Y Offset from the justification edge.
        /// </summary>
        public int YOffset { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="AnnotateImage"/>.
        /// </summary>
        public AnnotateImage() { }

        /// <summary>
        /// Creates a new instance of <see cref="AnnotateImage"/>.
        /// </summary>
        /// <param name="customText">The custom text to display.</param>
        /// <param name="textSize">The size of displayed text.</param>
        /// <param name="textColour">The colour of displayed text.</param>
        /// <param name="bgColour">The colour of the background.</param>
        /// <param name="showShutterSettings">Shows shutter settings.</param>
        /// <param name="showGainSettings">Shows gain settings.</param>
        /// <param name="showLensSettings">Show lens settings.</param>
        /// <param name="showCafSettings">Show CAF settings.</param>
        /// <param name="showMotionSettings">Show motion settings.</param>
        /// <param name="showFrameNumber">Show frame number.</param>
        /// <param name="allowCustomBackground">Enable custom background colour.</param>
        /// <param name="showDateText">Show date text.</param>
        /// <param name="showTimeText">Show time text.</param>
        /// <param name="justify">Justify text.</param>
        /// <param name="xOffset">Text X offset value.</param>
        /// <param name="yOffset">Text Y offset value.</param>
        public AnnotateImage(string customText, int textSize, Color textColour, Color bgColour,
            bool showShutterSettings, bool showGainSettings, bool showLensSettings,
            bool showCafSettings, bool showMotionSettings, bool showFrameNumber, bool allowCustomBackground,
            bool showDateText, bool showTimeText, JustifyText justify, int xOffset, int yOffset)
        {
            this.CustomText = customText;
            this.TextSize = textSize;
            this.TextColour = textColour;
            this.BgColour = bgColour;
            this.ShowShutterSettings = showShutterSettings;
            this.ShowGainSettings = showGainSettings;
            this.ShowLensSettings = showLensSettings;
            this.ShowCafSettings = showCafSettings;
            this.ShowMotionSettings = showMotionSettings;
            this.ShowFrameNumber = showFrameNumber;
            this.AllowCustomBackgroundColour = allowCustomBackground;
            this.ShowDateText = showDateText;
            this.ShowTimeText = showTimeText;
            this.Justify = justify;
            this.XOffset = xOffset;
            this.YOffset = yOffset;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AnnotateImage"/> with date and time enabled by default.
        /// </summary>
        /// <param name="customText">The custom text to display.</param>
        /// <param name="textSize">The size of displayed text.</param>
        /// <param name="textColour">The colour of displayed text.</param>
        public AnnotateImage(string customText, int textSize, Color textColour)
        {
            this.CustomText = customText;
            this.TextSize = textSize;
            this.TextColour = textColour;
            
            this.ShowDateText = true;
            this.ShowTimeText = true;
        }
    }
}
