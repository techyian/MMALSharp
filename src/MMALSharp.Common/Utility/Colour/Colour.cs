using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Utility.Colour
{
    public static class MMALColor
    {
        /// <summary>
        /// Returns a new <see cref="Color"/> structure based from YUV floating point values.
        /// See: https://www.fourcc.org/fccyvrgb.php
        /// </summary>
        /// <param name="y">The luma value.</param>
        /// <param name="u">The chrominance U value.</param>
        /// <param name="v">The chrominance V value.</param>
        /// <returns>A <see cref="Color"/> structure representing the YUV parameter values.</returns>
        public static Color FromYUV(float y, float u, float v)
        {
            var r = Math.Ceiling((1.164 * (y - 16) + 1.596 * (v - 128)).Clamp(0, 255));
            var g = Math.Ceiling((1.164 * (y - 16) - 0.813 * (v - 128) - 0.391 * (u - 128)).Clamp(0, 255));
            var b = Math.Ceiling((1.164 * (y - 16) + 2.018 * (u - 128)).Clamp(0, 255));

            return Color.FromArgb(255, (int)r, (int)g, (int)b);
        }

        /// <summary>
        /// Returns a new <see cref="Color"/> structure based from YUV byte values.
        /// See: https://en.wikipedia.org/wiki/YUV#Converting_between_Y%E2%80%B2UV_and_RGB
        /// </summary>
        /// <param name="y">The luma value.</param>
        /// <param name="u">The chrominance U value.</param>
        /// <param name="v">The chrominance V value.</param>
        /// <returns>A <see cref="Color"/> structure representing the YUV parameter values.</returns>
        public static Color FromYUVBytes(byte y, byte u, byte v)
        {
            int c = y - 16;
            int d = u - 128;
            int e = v - 128;

            int r = ((298 * c + 409 * e + 128) >> 8).Clamp(0, 255);
            int g = ((298 * c - 100 * d - 208 * e + 128) >> 8).Clamp(0, 255);
            int b = ((298 * c + 516 * d + 128) >> 8).Clamp(0, 255);

            return Color.FromArgb(255, r, g, b);
        }

        public static Color FromYIQ(float y, float i, float q)
        {
            
        }

        public static Color FromHLS(float h, float l, float s)
        {

        }

        public static Color FromHSV(float h, float s, float v)
        {

        }

        public static Color FromCieXYZ(float x, float y, float z)
        {

        }

        public static Color FromCieLab(float l, float a, float b)
        {

        }

        public static Color FromCieLUV(float l, float u, float v)
        {

        }
    }        
}
