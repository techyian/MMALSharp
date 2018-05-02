using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Utility
{
    /// <summary>
    /// Provides useful methods to convert from various colour spaces to RGB.
    /// </summary>
    public static class MMALColor
    {
        /// <summary>
        /// Converts a RGB <see cref="Color"/> structure to the CIE 1960 uniform colour space.
        /// See: https://en.wikipedia.org/wiki/CIE_1960_color_space        
        /// </summary>
        /// <param name="c">The <see cref="Color"/> structure.</param>        
        /// <returns>A 2 pair <see cref="Tuple"/> of floating point values representing the RGB conversion to CIE 1960.</returns>
        public static Tuple<float, float> RGBToCIE1960(Color c)
        {
            var xyz = RGBToXYZ(c);

            var u = (2 / 3) * xyz.Item1;
            var v = xyz.Item2;

            return new Tuple<float, float>(u, v);
        }

        /// <summary>
        /// Converts a RGB <see cref="Color"/> structure to the CIE XYZ colour space.
        /// See: https://en.wikipedia.org/wiki/SRGB#The_forward_transformation_(CIE_XYZ_to_sRGB)
        /// </summary>
        /// <param name="c">The <see cref="Color"/> structure.</param>        
        /// <returns>A 3 pair <see cref="Tuple"/> of floating point values representing the RGB conversion to CIE XYZ.</returns>
        public static Tuple<float, float, float> RGBToXYZ(Color c)
        {
            var r = c.R.FromByte();
            var g = c.G.FromByte();
            var b = c.B.FromByte();

            var rVector = new Vector3(0.4124f * r, 0.3576f * g, 0.1805f * b);
            var gVector = new Vector3(0.2126f * r, 0.7152f * g, 0.0722f * b);
            var bVector = new Vector3(0.0193f * r, 0.1192f * g, 0.9505f * b);

            var x = ToXYZLinear(rVector.X + rVector.Y + rVector.Z);
            var y = ToXYZLinear(gVector.X + gVector.Y + gVector.Z);
            var z = ToXYZLinear(bVector.X + bVector.Y + bVector.Z);

            return new Tuple<float, float, float>(x, y, z);
        }

        /// <summary>
        /// Converts a RGB <see cref="Color"/> structure to the YIQ colour space.
        /// See: https://en.wikipedia.org/wiki/YIQ
        /// Math conversion from: https://github.com/python/cpython/blob/2.7/Lib/colorsys.py
        /// </summary>
        /// <param name="c">The <see cref="Color"/> structure.</param>        
        /// <returns>A 3 pair <see cref="Tuple"/> of floating point values representing the RGB conversion to YIQ.</returns>
        public static Tuple<float, float, float> RGBToYIQ(Color c)
        {
            var r = c.R.FromByte();
            var g = c.G.FromByte();
            var b = c.B.FromByte();

            var y = (float)(0.30 * r + 0.59 * g + 0.11 * b);
            var i = (float)(0.60 * r - 0.28 * g - 0.32 * b);
            var q = (float)(0.21 * r - 0.52 * g + 0.31 * b);

            return new Tuple<float, float, float>(y.Clamp(0, 1), i.Clamp(0, 1), q.Clamp(0, 1));
        }

        /// <summary>
        /// Converts a RGB <see cref="Color"/> structure to the HLS colour space.
        /// See: https://en.wikipedia.org/wiki/HSL_and_HSV
        /// Math conversion from: https://github.com/python/cpython/blob/2.7/Lib/colorsys.py
        /// </summary>
        /// <param name="c">The <see cref="Color"/> structure.</param>        
        /// <returns>A 3 pair <see cref="Tuple"/> of floating point values representing the RGB conversion to HLS.</returns>
        public static Tuple<float, float, float> RGBToHLS(Color c)
        {
            float h, l, s;

            var r = c.R.FromByte();
            var g = c.G.FromByte();
            var b = c.B.FromByte();

            var maxc = GetMaxComponent(r, g, b);
            var minc = GetMinComponent(r, g, b);

            l = (minc + maxc) / 2.0f;

            if (minc == maxc)
            {
                return new Tuple<float, float, float>(0.0f, l, 0.0f);
            }

            if (l <= 0.5f)
            {
                s = (maxc - minc) / (maxc + minc);
            }
            else
            {
                s = (maxc - minc) / (2.0f - maxc - minc);
            }

            var rc = (maxc - r) / (maxc - minc);
            var gc = (maxc - g) / (maxc - minc);
            var bc = (maxc - b) / (maxc - minc);

            if (r == maxc)
            {
                h = bc - gc;
            }
            else if (g == maxc)
            {
                h = 2.0f + rc - bc;
            }
            else
            {
                h = 4.0f + gc - rc;
            }

            h = (h / 6.0f) % 1.0f;

            return new Tuple<float, float, float>(h.Clamp(0, 1), l.Clamp(0, 1), s.Clamp(0, 1));
        }

        /// <summary>
        /// Converts a RGB <see cref="Color"/> structure to the HSV colour space.
        /// See: https://en.wikipedia.org/wiki/HSL_and_HSV
        /// Math conversion from: https://github.com/python/cpython/blob/2.7/Lib/colorsys.py
        /// </summary>
        /// <param name="c">The <see cref="Color"/> structure.</param>        
        /// <returns>A 3 pair <see cref="Tuple"/> of floating point values representing the RGB conversion to HSV.</returns>
        public static Tuple<float, float, float> RGBToHSV(Color c)
        {
            float h, s, v;

            var r = c.R.FromByte();
            var g = c.G.FromByte();
            var b = c.B.FromByte();

            var maxc = GetMaxComponent(r, g, b);
            var minc = GetMinComponent(r, g, b);

            v = maxc;

            if (minc == maxc)
            {
                return new Tuple<float, float, float>(0.0f, 0.0f, v);
            }

            s = (maxc - minc) / maxc;

            var rc = (maxc - r) / (maxc - minc);
            var gc = (maxc - g) / (maxc - minc);
            var bc = (maxc - b) / (maxc - minc);

            if (r == maxc)
            {
                h = bc - gc;
            }
            else if (g == maxc)
            {
                h = 2.0f + rc - bc;
            }
            else
            {
                h = 4.0f + gc - rc;
            }

            h = (h / 6.0f) % 1.0f;

            return new Tuple<float, float, float>(h.Clamp(0, 1), s.Clamp(0, 1), v.Clamp(0, 1));
        }

        /// <summary>
        /// Converts a RGB <see cref="Color"/> structure to the YUV colour space.
        /// See: https://en.wikipedia.org/wiki/YUV#Converting_between_Y%E2%80%B2UV_and_RGB
        /// </summary>
        /// <param name="c">The <see cref="Color"/> structure.</param>        
        /// <returns>A 3 pair <see cref="Tuple"/> of floating point values representing the RGB conversion to YUV.</returns>
        public static Tuple<float, float, float> RGBToYUV(Color c)
        {
            var r = c.R.FromByte();
            var g = c.G.FromByte();
            var b = c.B.FromByte();

            var y = (float)(0.299 * r + 0.587 * g + 0.114 * b);
            var u = (float)(-0.147 * r - 0.289 * g + 0.436 * b);
            var v = (float)(0.615 * r - 0.515 * g - 0.100 * b);

            return new Tuple<float, float, float>(y, u, v);
        }

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
            var r = (float)(1.164 * (y - 16) + 1.596 * (v - 128)).Clamp(0, 1);
            var g = (float)(1.164 * (y - 16) - 0.813 * (v - 128) - 0.391 * (u - 128)).Clamp(0, 1);
            var b = (float)(1.164 * (y - 16) + 2.018 * (u - 128)).Clamp(0, 1);

            return Color.FromArgb(255, r.ToByte(), g.ToByte(), b.ToByte());
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
            y = y.Clamp(0, 1);
            i = i.Clamp(0, 1);
            q = q.Clamp(0, 1);

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

            var r = HLSConstant(m1, m2, h + (1.0f / 3.0f));
            var g = HLSConstant(m1, m2, h);
            var b = HLSConstant(m1, m2, h - (1.0f / 3.0f));
            
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

        /// <summary>
        /// Returns a new <see cref="Color"/> structure based from CIEXYZ floating point values.
        /// See: https://en.wikipedia.org/wiki/SRGB#The_forward_transformation_(CIE_XYZ_to_sRGB) 
        /// </summary>
        /// <param name="x">The chrominance X value.</param>
        /// <param name="y">The luminance Y value.</param>
        /// <param name="z">The chrominance Z value.</param>
        /// <returns>A <see cref="Color"/> structure representing the CIEXYZ parameter values.</returns>
        public static Color FromCieXYZ(float x, float y, float z)
        {
            var rVector = new Vector3(3.2404542f * x, -1.5371385f * y, -0.4985314f * z);
            var gVector = new Vector3(-0.9692660f * x, 1.8760108f * y, 0.0415560f * z);
            var bVector = new Vector3(0.0556434f * x, -0.2040259f * y, 1.0572252f * z);

            var rLinear = ToStandardRGBLinear(rVector.X + rVector.Y + rVector.Z);
            var gLinear = ToStandardRGBLinear(gVector.X + gVector.Y + gVector.Z);
            var bLinear = ToStandardRGBLinear(bVector.X + bVector.Y + bVector.Z);

            return Color.FromArgb(255, rLinear.ToByte(), gLinear.ToByte(), bLinear.ToByte());
        }

        /// <summary>
        /// Returns a new <see cref="Color"/> structure based from CIELab floating point values.
        /// See: https://en.wikipedia.org/wiki/Lab_color_space#Forward_transformation
        /// </summary>
        /// <param name="l">The lightness L value.</param>
        /// <param name="a">The chrominance A value.</param>
        /// <param name="b">The chrominance B value.</param>
        /// <returns>A <see cref="Color"/> structure representing the CIELab parameter values.</returns>
        public static Color FromCieLab(float l, float a, float b)
        {
            // D65 Illuminant values
            var xn = 95.047f;
            var yn = 100f;
            var zn = 108.883f;

            var f1 = (l + 16) / 116;
            var f2 = f1 + a / 500;
            var f3 = f1 - b / 200;

            var y = yn * CieLABConstant(f1);
            var x = xn * CieLABConstant(f2);            
            var z = zn * CieLABConstant(f3);

            return FromCieXYZ(x, y, z);            
        }

        /// <summary>
        /// Returns a new <see cref="Color"/> structure based from CIELUV floating point values.
        /// See: https://en.wikipedia.org/wiki/Lab_color_space#Forward_transformation
        /// </summary>
        /// <param name="l">The lightness L value.</param>
        /// <param name="u">The chrominance U value.</param>
        /// <param name="v">The chrominance V value.</param>
        /// <returns>A <see cref="Color"/> structure representing the CIELUV parameter values.</returns>
        public static Color FromCieLUV(float l, float u, float v)
        {
            // D65 Illuminant
            var un = 0.2009f;
            var vn = 0.4610f;

            var upt = u / (13 / l) + un;
            var vpt = v / (13 / 1) + vn;

            float y;

            if (l <= 8)
            {
                y = (float)(1.0f * (l * Math.Pow(3 / 29, 3)));
            }
            else
            {
                y = (float)(1.0f * Math.Pow((l + 16) / 116, 3));
            }

            var x = y * (9 * u) / (4 * v);
            var z = y * ((12 - (3 * u) - (20 * v)) / (4 * v));

            return FromCieXYZ(x, y, z);
        }

        private static float CieLABConstant(float t)
        {
            float theta = 6 / 29;

            if (t > theta)
            {
                return (float)Math.Pow(t, 3);
            }

            return (float)(3 * Math.Pow(theta, 2) * (t - (4 / 29)));
        }

        private static float HLSConstant(float m1, float m2, float hue)
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

        private static float ToStandardRGBLinear(float c)
        {
            if (c <= 0.0031308f)
            {
                return 12.92f * c;
            }

            return (1 + 0.055f) * (c * (float)Math.Pow(1, 2.4)) - 0.55f;
        }

        private static float ToXYZLinear(float c)
        {
            if (c <= 0.04045f)
            {
                return c / 12.92f;
            }

            return (float)Math.Pow((c + 0.055) / (1 + 0.055), 2.4);
        }

        private static int GetMaxComponent(int r, int g, int b) => Math.Max(Math.Max(r, g), b);

        private static int GetMinComponent(int r, int g, int b) => Math.Min(Math.Min(r, g), b);
    }        
}
