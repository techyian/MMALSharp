// <copyright file="GenericExtensions.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;

namespace MMALSharp.Common
{
    /// <summary>
    /// Provides extension methods for mathematical operations.
    /// </summary>
    public static class GenericExtensions
    {
        /// <summary>
        /// Returns a representation of this object that is in the specified range. Too large values will be dreceased to max; too small values will be increased to min.
        /// </summary>
        /// <typeparam name="T">The type of the value to be clamped.</typeparam>
        /// <param name="val">The value to be clamped.</param>
        /// <param name="min">The mininum inclusive value.</param>
        /// <param name="max">The maximum inclusive value.</param>
        /// <returns>A clamped representation of the initial value.</returns>
        public static T Clamp<T>(this T val, T min, T max) 
            where T : IComparable<T>
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
        /// <param name="val">The byte value to convert.</param>
        /// <returns>The converted float value.</returns>
        public static float ToFloat(this byte val)
        {
            return val / 255.0f;
        }

        /// <summary>
        /// Converts a <see cref="float"/> value (0.0 to 1.0) to a <see cref="byte"/> value from 0 to 255.
        /// </summary>
        /// <param name="val">The float value to convert.</param>
        /// <remarks>https://stackoverflow.com/questions/1914115/converting-color-value-from-float-0-1-to-byte-0-255</remarks>
        /// <returns>The converted byte value.</returns>
        public static byte ToByte(this float val)
        {
            return (byte)(val * 255.999f);
        }
    }
}
