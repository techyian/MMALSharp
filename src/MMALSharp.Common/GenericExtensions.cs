// <copyright file="GenericExtensions.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;

namespace MMALSharp
{
    /// <summary>
    /// Provides extension methods for mathematical operations.
    /// </summary>
    public static class GenericExtensions
    {
        /// <summary>
        /// Returns a representation of this object that is in the specified range. Too large values will be dreceased to max; too small values will be increased to min.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="min">The mininum inclusive value.</param>
        /// <param name="max">The maximum inclusive value.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Converts a <see cref="byte"/> value to a <see cref="float"/> value from 0.0 to 1.0.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float ToFloat(this byte val)
        {
            return val >= 255 ? 1.0f : val / 255.0f;
        }

        /// <summary>
        /// Converts a <see cref="float"/> value (0.0 to 1.0) to a <see cref="byte"/> value from 0 to 255.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static byte ToByte(this float val)
        {
            return (byte)(val >= 1.0f ? 255 : val * 255.0f);
        }
    }
}
