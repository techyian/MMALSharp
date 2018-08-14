// <copyright file="Helpers.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
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
    }
}
