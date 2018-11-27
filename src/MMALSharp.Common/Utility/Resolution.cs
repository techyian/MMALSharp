using System;

namespace MMALSharp.Common.Utility
{
    /// <summary>
    /// Exposes properties for width and height. This class is used to specify a resolution for camera and ports.
    /// </summary>
    public struct Resolution : IComparable<Resolution>
    {
        /// <summary>
        /// The width of the <see cref="Resolution"/> object.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The height of the <see cref="Resolution"/> object.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="Resolution"/> class with the specified width and height.
        /// </summary>
        /// <param name="width">The width to assign this resolution with.</param>
        /// <param name="height">The height to assign this resolution with.</param>
        public Resolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /*
         * 4:3 Aspect ratios 
        */

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 3264 pixels high and 2448 pixels wide.
        /// </summary>
        public static Resolution As8MPixel => new Resolution(3264, 2448);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 3072 pixels high and 2304 pixels wide.
        /// </summary>
        public static Resolution As7MPixel => new Resolution(3072, 2304);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 3032 pixels high and 2008 pixels wide.
        /// </summary>
        public static Resolution As6MPixel => new Resolution(3032, 2008);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 2560 pixels high and 1920 pixels wide.
        /// </summary>
        public static Resolution As5MPixel => new Resolution(2560, 1920);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 2240 pixels high and 1680 pixels wide.
        /// </summary>
        public static Resolution As4MPixel => new Resolution(2240, 1680);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 2048 pixels high and 1536 pixels wide.
        /// </summary>
        public static Resolution As3MPixel => new Resolution(2048, 1536);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 1600 pixels high and 1200 pixels wide.
        /// </summary>
        public static Resolution As2MPixel => new Resolution(1600, 1200);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 1280 pixels high and 960 pixels wide.
        /// </summary>
        public static Resolution As1MPixel => new Resolution(1280, 960);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 640 pixels high and 480 pixels wide.
        /// </summary>
        public static Resolution As03MPixel => new Resolution(640, 480);

        /*
         * 16:9 Aspect ratios 
        */

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 1280 pixels high and 720 pixels wide.
        /// </summary>
        public static Resolution As720p => new Resolution(1280, 720);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 1920 pixels high and 1080 pixels wide.
        /// </summary>
        public static Resolution As1080p => new Resolution(1920, 1080);

        /// <summary>
        /// Creates a new <see cref="Resolution"/> object with 2560 pixels high and 1440 pixels wide.
        /// </summary>
        public static Resolution As1440p => new Resolution(2560, 1440);

        /// <summary>
        /// Compares this Resolution instance against the Resolution passed in. 
        /// </summary>
        /// <param name="res">The resolution we are comparing to.</param>
        /// <returns>0 if width and height are same. 1 if source width is greater than target. -1 if target greater than source.</returns>
        public int CompareTo(Resolution res)
        {
            if (this.Width == res.Width && this.Height == res.Height)
            {
                return 0;
            }

            if (this.Width == res.Width && this.Height > res.Height)
            {
                return 1;
            }

            if (this.Width == res.Width && this.Height < res.Height)
            {
                return -1;
            }

            if (this.Width > res.Width)
                return 1;

            return -1;
        }

        /// <summary>
        /// Pads a <see cref="Resolution"/> object to the desired width/height.
        /// </summary>
        /// <param name="width">The width to be padded to.</param>
        /// <param name="height">The height to be padded to.</param>
        /// <returns>A new <see cref="Resolution"/> struct, padded to the required width/height.</returns>
        public Resolution Pad(int width = 32, int height = 16)
        {
            return new Resolution(VCOS_ALIGN_UP(this.Width, width),
                                  VCOS_ALIGN_UP(this.Height, height));
        }
        
        private int VCOS_ALIGN_UP(int value, int roundTo)
        {
            return (int)(Math.Ceiling(value / Convert.ToDouble(roundTo)) * roundTo);
        }
    }
}