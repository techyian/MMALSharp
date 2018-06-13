// <copyright file="Helpers.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Linq.Expressions;

namespace MMALSharp
{
    /// <summary>
    /// Provides general helper methods.
    /// </summary>
    public class Helpers
    {
        /// <summary>
        /// Converts the count of bytes to megabytes in the format "0.00mb".
        /// </summary>
        public static string ConvertBytesToMegabytes(long bytes)
        {
            return ((bytes / 1024f) / 1024f).ToString("0.00mb");
        }
    }
}
