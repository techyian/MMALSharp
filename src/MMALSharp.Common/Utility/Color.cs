// <copyright file="Color.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using System.Numerics;

namespace MMALSharp.Common.Utility
{
    /// <summary>
    /// Provides useful methods to convert from various colour spaces to RGB.
    /// </summary>
    public static class MMALColor
    {
        /// <summary>
        /// Returns a new <see cref="Color"/> structure based from CIE 1960 floating point values.
        /// See: https://en.wikipedia.org/wiki/CIE_1960_color_space
        /// </summary>
        /// <param name="u">The chrominance U value.</param>
        /// <param name="v">The chrominance V value.</param>
        /// <param name="y">The CIE XYZ Y tristimulus value.</param>
        /// <returns>A <see cref="Color"/> structure representing the CIE 1960 parameter values.</returns>
        public static Color FromCIE1960(float u, float v, float y)
        {
            // x and y chromaticity values
            var xc = (3f * u) / ((2f * u) - (8f * v) + 4);
            var yc = (2f * v) / ((2f * u) - (8f * v) + 4);

            var x = (y / yc) * xc;
            var z = (y / yc) * (1 - xc - yc);

            return FromCieXYZ(x, y, z);
        }

        /// <summary>
        /// Converts a RGB <see cref="Color"/> structure to the CIE 1960 uniform colour space.
        /// See: https://en.wikipedia.org/wiki/CIE_1960_color_space        
        /// </summary>
        /// <param name="c">The <see cref="Color"/> structure.</param>        
        /// <returns>A 2 pair <see cref="Tuple"/> of floating point values representing the RGB conversion to CIE 1960.</returns>
        public static Tuple<float, float, float> RGBToCIE1960(Color c)
        {
            var xyz = RGBToCIEXYZ(c);

            var u = (2f / 3f) * xyz.Item1;
            var v = xyz.Item2;
            var w = (1f / 2f) * (-xyz.Item1 + (3 * xyz.Item2) + xyz.Item3);

            // calculate chromaticity variables
            var cu = u / (u + v + w);
            var cv = v / (u + v + w);

            return new Tuple<float, float, float>(cu, cv, xyz.Item2);
        }

        /// <summary>
        /// Converts a RGB <see cref="Color"/> structure to the CIE XYZ colour space.
        /// See: https://en.wikipedia.org/wiki/SRGB#The_forward_transformation_(CIE_XYZ_to_sRGB)
        /// </summary>
        /// <param name="c">The <see cref="Color"/> structure.</param>        
        /// <returns>A 3 pair <see cref="Tuple"/> of floating point values representing the RGB conversion to CIE XYZ.</returns>
        public static Tuple<float, float, float> RGBToCIEXYZ(Color c)
        {
            var r = c.R.ToFloat();
            var g = c.G.ToFloat();
            var b = c.B.ToFloat();

            var rl = ToXYZLinear(r);
            var gl = ToXYZLinear(g);
            var bl = ToXYZLinear(b);

            var rVector = new Vector3(0.4124f * rl, 0.3576f * gl, 0.1805f * bl);
            var gVector = new Vector3(0.2126f * rl, 0.7152f * gl, 0.0722f * bl);
            var bVector = new Vector3(0.0193f * rl, 0.1192f * gl, 0.9505f * bl);

            var x = rVector.X + rVector.Y + rVector.Z;
            var y = gVector.X + gVector.Y + gVector.Z;
            var z = bVector.X + bVector.Y + bVector.Z;

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
            var r = c.R.ToFloat();
            var g = c.G.ToFloat();
            var b = c.B.ToFloat();

            var y = (float)((0.30 * r) + (0.59 * g) + (0.11 * b));
            var i = (float)((0.60 * r) - (0.28 * g) - (0.32 * b));
            var q = (float)((0.21 * r) - (0.52 * g) + (0.31 * b));

            return new Tuple<float, float, float>(y.Clamp(0, 1), i.Clamp(-1, 1), q.Clamp(-1, 1));
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

            var r = c.R.ToFloat();
            var g = c.G.ToFloat();
            var b = c.B.ToFloat();

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

            var r = c.R.ToFloat();
            var g = c.G.ToFloat();
            var b = c.B.ToFloat();

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
            var r = c.R.ToFloat();
            var g = c.G.ToFloat();
            var b = c.B.ToFloat();

            var y = (0.299f * r) + (0.587f * g) + (0.114f * b);
            var u = (-0.147f * r) - (0.289f * g) + (0.436f * b);
            var v = (0.615f * r) - (0.515f * g) - (0.100f * b);

            return new Tuple<float, float, float>(y, u, v);
        }

        /// <summary>
        /// Converts a RGB <see cref="Color"/> structure to the YUV colour space as byte values.
        /// See: https://en.wikipedia.org/wiki/YUV#Converting_between_Y%E2%80%B2UV_and_RGB
        /// </summary>
        /// <param name="c">The <see cref="Color"/> structure.</param>
        /// <returns>A 3 pair <see cref="Tuple"/> of byte values representing the RGB conversion to YUV.</returns>
        public static Tuple<byte, byte, byte> RGBToYUVBytes(Color c)
        {
            var y = (((66 * c.R) + (129 * c.G) + (25 * c.B) + 128) >> 8) + 16;
            var u = (((-38 * c.R) - (74 * c.G) + (112 * c.B) + 128) >> 8) + 128;
            var v = (((112 * c.R) - (94 * c.G) - (18 * c.B) + 128) >> 8) + 128;

            return new Tuple<byte, byte, byte>((byte)y, (byte)u, (byte)v);
        }

        /// <summary>
        /// Returns a new <see cref="Color"/> structure based from YUV floating point values.
        /// See: https://en.wikipedia.org/wiki/YUV#Conversion_to/from_RGB
        /// </summary>
        /// <param name="y">The luma value.</param>
        /// <param name="u">The chrominance U value.</param>
        /// <param name="v">The chrominance V value.</param>
        /// <returns>A <see cref="Color"/> structure representing the YUV parameter values.</returns>
        public static Color FromYUV(float y, float u, float v)
        {
            y = y.Clamp(0, 1);
            u = u.Clamp(-0.436f, 0.436f);
            v = v.Clamp(-0.615f, 0.615f);

            var r = y + (1.140f * v);
            var g = y - (0.395f * u) - (0.581f * v);
            var b = y + (2.032f * u);

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

            int r = (((298 * c) + (409 * e) + 128) >> 8).Clamp(0, 255);
            int g = (((298 * c) - (100 * d) - (208 * e) + 128) >> 8).Clamp(0, 255);
            int b = (((298 * c) + (516 * d) + 128) >> 8).Clamp(0, 255);

            return Color.FromArgb(255, r, g, b);
        }

        /// <summary>
        /// Returns a new <see cref="Color"/> structure based from YIQ floating point values.
        /// See: https://en.wikipedia.org/wiki/YIQ
        /// Math conversion from: https://github.com/python/cpython/blob/2.7/Lib/colorsys.py
        /// </summary>
        /// <param name="y">The luma value (between 0 - 1).</param>
        /// <param name="i">The chrominance I value (between -1 - 1).</param>
        /// <param name="q">The chrominance Q value (between -1 - 1).</param>
        /// <returns>A <see cref="Color"/> structure representing the YIQ parameter values.</returns>
        public static Color FromYIQ(float y, float i, float q)
        {
            y = y.Clamp(0, 1);
            i = i.Clamp(-1, 1);
            q = q.Clamp(-1, 1);

            var r = (y + (0.948262f * i) + (0.624013f * q)).Clamp(0, 1);
            var g = (y - (0.276066f * i) - (0.639810f * q)).Clamp(0, 1);
            var b = (y - (1.105450f * i) + (1.729860f * q)).Clamp(0, 1);

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

            var m1 = (2.0f * l) - m2;

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

            var i = (int)(h * 6);
            var f = (h * 6.0f) - i;
            var p = v * (1.0f - s);
            var q = v * (1.0f - (s * f));
            var t = v * (1.0f - (s * (1.0f - f)));

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
        /// Returns a new <see cref="Color"/> structure based from CIEXYZ floating point values. Assumes D65 illuminant.
        /// See: https://en.wikipedia.org/wiki/SRGB#The_forward_transformation_(CIE_XYZ_to_sRGB) 
        /// </summary>
        /// <param name="x">The chrominance X value (0 &lt;= x &lt;= 0.9505).</param>
        /// <param name="y">The luminance Y value (0 &lt;= y &lt;= 1.0000).</param>
        /// <param name="z">The chrominance Z value (0 &lt;= z &lt;= 1.0890).</param>
        /// <returns>A <see cref="Color"/> structure representing the CIEXYZ parameter values.</returns>
        public static Color FromCieXYZ(float x, float y, float z)
        {
            x = x.Clamp(0, 0.9505f);
            y = y.Clamp(0, 1.0000f);
            z = z.Clamp(0, 1.0890f);

            var rLinear = new Vector3(3.2404542f * x, -1.5371385f * y, -0.4985314f * z);
            var gLinear = new Vector3(-0.9692660f * x, 1.8760108f * y, 0.0415560f * z);
            var bLinear = new Vector3(0.0556434f * x, -0.2040259f * y, 1.0572252f * z);

            var sr = StandardRGBLinearTransform(rLinear.X + rLinear.Y + rLinear.Z).Clamp(0, 1);
            var sg = StandardRGBLinearTransform(gLinear.X + gLinear.Y + gLinear.Z).Clamp(0, 1);
            var sb = StandardRGBLinearTransform(bLinear.X + bLinear.Y + bLinear.Z).Clamp(0, 1);

            return Color.FromArgb(255, sr.ToByte(), sg.ToByte(), sb.ToByte());
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
            var xn = 0.95047f;
            var yn = 1.0f;
            var zn = 1.08883f;

            var f1 = (l + 16f) / 116f;
            var f2 = f1 + (a / 500f);
            var f3 = f1 - (b / 200f);

            var y = yn * CieLABConstant(f1);
            var x = xn * CieLABConstant(f2);
            var z = zn * CieLABConstant(f3);

            return FromCieXYZ(x, y, z);
        }

        /// <summary>
        /// Returns a new <see cref="Color"/> structure based from CIELUV floating point values.
        /// See: https://en.wikipedia.org/wiki/CIELUV
        /// </summary>
        /// <param name="l">The lightness L value.</param>
        /// <param name="u">The chrominance U value.</param>
        /// <param name="v">The chrominance V value.</param>
        /// <returns>A <see cref="Color"/> structure representing the CIELUV parameter values.</returns>
        public static Color FromCieLUV(float l, float u, float v)
        {
            // D65 Illuminant values
            var xn = 0.95047f;
            var yn = 1.0f;
            var zn = 1.08883f;

            var uc = 4f * xn / (xn + (15f * yn) + (3f * zn));
            var vc = 9f * yn / (xn + (15f * yn) + (3f * zn));

            var upt = (u / (13f * l)) + uc;
            var vpt = (v / (13f * 1)) + vc;

            float y;

            if (l <= 8)
            {
                y = yn * (float)(l * Math.Pow(3f / 29f, 3f));
            }
            else
            {
                y = yn * (float)Math.Pow((l + 16f) / 116f, 3f);
            }

            var x = y * (9f * upt) / (4f * vpt);
            var z = y * (12f - (3f * upt) - (20f * vpt)) / (4f * vpt);

            return FromCieXYZ(x, y, z);
        }

        private static float CieLABConstant(float t)
        {
            float theta = 6f / 29f;

            if (t > theta)
            {
                return (float)Math.Pow(t, 3);
            }

            return (float)((3 * Math.Pow(theta, 2)) * (t - (4 / 29)));
        }

        private static float HLSConstant(float m1, float m2, float hue)
        {
            hue = hue % 1f;

            if (hue < (1f / 6f))
            {
                return m1 + ((m2 - m1) * hue * 6f);
            }

            if (hue < 0.5f)
            {
                return m2;
            }

            if (hue < (1f / 3f))
            {
                return m1 + ((m2 - m1) * ((2f / 3f) - hue) * 6f);
            }

            return m1;
        }

        private static float StandardRGBLinearTransform(float c)
        {
            if (c <= 0.0031308f)
            {
                return 12.92f * c;
            }

            return 1.055f * (((float)Math.Pow(c, 1 / 2.4)) - 0.055f);
        }

        private static float ToXYZLinear(float c)
        {
            if (c <= 0.04045f)
            {
                return c / 12.92f;
            }

            return (float)Math.Pow((c + 0.055) / 1.055, 2.4);
        }

        private static float GetMaxComponent(float r, float g, float b) => Math.Max(Math.Max(r, g), b);

        private static float GetMinComponent(float r, float g, float b) => Math.Min(Math.Min(r, g), b);
    }
}
