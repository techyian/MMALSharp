// <copyright file="Helpers.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Common.Utility
{
    /// <summary>
    /// Provides general helper methods.
    /// </summary>
    public class Helpers
    {
        /// <summary>
        /// Converts the count of bytes to megabytes in the format "0.00mb".
        /// </summary>
        /// <param name="bytes">The number of bytes.</param>
        /// <returns>A string representing the byte-megabyte conversion.</returns>
        public static string ConvertBytesToMegabytes(long bytes)
        {
            return ((bytes / 1024f) / 1024f).ToString("0.00mb");
        }

        /// <summary>
        /// Returns an integer FourCC code from a string.
        /// </summary>
        /// <param name="s">The FourCC code.</param>
        /// <returns>The FourCC code as an integer.</returns>
        public static int FourCCFromString(string s)
        {
            int a1 = s[0];
            int b1 = s[1];
            int c1 = s[2];
            int d1 = s[3];
            return a1 | (b1 << 8) | (c1 << 16) | (d1 << 24);
        }
    }
}
