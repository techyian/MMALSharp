using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Utility
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

        /// <summary>
        /// Returns a new <see cref="Color"/> structure based from YIQ floating point values.
        /// See: https://en.wikipedia.org/wiki/YIQ
        /// Math conversion from: https://github.com/python/cpython/blob/2.7/Lib/colorsys.py
        /// </summary>
        /// <param name="y">The luma value.</param>
        /// <param name="i">The chrominance I value.</param>
        /// <param name="q">The chrominance Q value.</param>
        /// <returns>A <see cref="Color"/> structure representing the YIQ parameter values.</returns>
        public static Color FromYIQ(float y, float i, float q)
        {
            var r = (y + 0.948262f * i + 0.624013f * q).Clamp(0, 1);
            var g = (y - 0.276066f * i - 0.639810f * q).Clamp(0, 1);
            var b = (y - 1.105450f * i + 1.729860f * q).Clamp(0, 1);
            
            return Color.FromArgb(255, r.ToByte(), g.ToByte(), b.ToByte());
        }

        /// <summary>
        /// Returns a new <see cref="Color"/> structure based from HLS floating point values.
        /// See: https://en.wikipedia.org/wiki/HSL_and_HSV
        /// Math conversion from: https://github.com/python/cpython/blob/2.7/Lib/colorsys.py
        /// </summary>
        /// <param name="h">The hue value.</param>
        /// <param name="l">The lightness value.</param>
        /// <param name="s">The saturation value.</param>
        /// <returns>A <see cref="Color"/> structure representing the HLS parameter values.</returns>
        public static Color FromHLS(float h, float l, float s)
        {
            h = h.Clamp(0, 1);
            l = l.Clamp(0, 1);
            s = s.Clamp(0, 1);

            float m2;

            if (s == 0.0f)
            {
                return Color.FromArgb(255, l.ToByte(), l.ToByte(), l.ToByte());
            }

            if (l < 0.5f)
            {
                m2 = l * (1.0f + s);
            }
            else
            {
                m2 = l + s - (l * s);
            }

            var m1 = 2.0f * l - m2;

            var r = HLSCoefficient(m1, m2, h + (1.0f / 3.0f));
            var g = HLSCoefficient(m1, m2, h);
            var b = HLSCoefficient(m1, m2, h - (1.0f / 3.0f));
            
            return Color.FromArgb(255, r.ToByte(), g.ToByte(), b.ToByte());
        }

        /// <summary>
        /// Returns a new <see cref="Color"/> structure based from HSV floating point values.
        /// See: https://en.wikipedia.org/wiki/HSL_and_HSV
        /// Math conversion from: https://github.com/python/cpython/blob/2.7/Lib/colorsys.py
        /// </summary>
        /// <param name="h">The hue value.</param>
        /// <param name="s">The saturation value.</param>
        /// <param name="v">The 'value' (lightness) value.</param>
        /// <returns>A <see cref="Color"/> structure representing the HSV parameter values.</returns>
        public static Color FromHSV(float h, float s, float v)
        {
            h = h.Clamp(0, 1);
            s = s.Clamp(0, 1);
            v = v.Clamp(0, 1);

            if (s == 0.0f)
            {
                return Color.FromArgb(255, v.ToByte(), v.ToByte(), v.ToByte());
            }

            var i = (int) h * 6;
            var f = (h * 6.0f) - i;
            var p = v * (1.0f - s);
            var q = v * (1.0f - s * f);
            var t = v * (1.0f - s * (1.0f - f));

            i = i % 6;
            
            if (i == 0)
            {
                return Color.FromArgb(255, v.ToByte(), t.ToByte(), p.ToByte());
            }

            if (i == 1)
            {
                return Color.FromArgb(255, q.ToByte(), v.ToByte(), p.ToByte());
            }

            if (i == 2)
            {
                return Color.FromArgb(255, p.ToByte(), v.ToByte(), t.ToByte());
            }

            if (i == 3)
            {
                return Color.FromArgb(255, p.ToByte(), q.ToByte(), v.ToByte());
            }

            if (i == 4)
            {
                return Color.FromArgb(255, t.ToByte(), p.ToByte(), v.ToByte());
            }

            if (i == 5)
            {
                return Color.FromArgb(255, v.ToByte(), p.ToByte(), q.ToByte());
            }

            throw new Exception("Calculated invalid HSV value.");
        }

        public static Color FromCieXYZ(float x, float y, float z)
        {
            var rVector = new Vector3(3.2404542f * x, -1.5371385f * y, -0.4985314f * z);
            var gVector = new Vector3(-0.9692660f * x, 1.8760108f * y, 0.0415560f * z);
            var bVector = new Vector3(0.0556434f * x, -0.2040259f * y, 1.0572252f * z);

            var rLinear = ToStandardRGB(rVector.X + rVector.Y + rVector.Z);
            var gLinear = ToStandardRGB(gVector.X + gVector.Y + gVector.Z);
            var bLinear = ToStandardRGB(bVector.X + bVector.Y + bVector.Z);

            return Color.FromArgb(255, rLinear.ToByte(), gLinear.ToByte(), bLinear.ToByte());
        }

        public static Color FromCieLab(float l, float a, float b)
        {
            
        }

        public static Color FromCieLUV(float l, float u, float v)
        {

        }

        private static float HLSCoefficient(float m1, float m2, float hue)
        {
            hue = hue % 1f;

            if (hue < (1f / 6f))
            {
                return m1 + (m2 - m1) * hue * 6f;
            }

            if (hue < 0.5f)
            {
                return m2;
            }

            if (hue < (1f / 3f))
            {
                return m1 + (m2 - m1) * ((2f / 3f) - hue) * 6f;
            }

            return m1;
        }

        private static float ToStandardRGB(float c)
        {
            if (c <= 0.0031308f)
            {
                return 12.92f * c;
            }

            return (1 + 0.055f) * (c * (float)Math.Pow(1, 2.4)) - 0.55f;
        }
    }        
}
