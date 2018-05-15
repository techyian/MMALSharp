// <copyright file="GenericExtensions.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;

namespace MMALSharp
{
    public static class GenericExtensions
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0)
            {
                return min;
            }

            if (val.CompareTo(max) > 0)
            {
                return max;
            }

            return val;
        }

        public static float ToFloat(this byte val)
        {
            return val >= 255 ? 1f : val / 255f;
        }

        public static byte ToByte(this float val)
        {
            return (byte)(val >= 1.0 ? 255 : val * 256);
        }
    }
}
