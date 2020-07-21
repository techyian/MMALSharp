// <copyright file="Zoom.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Config
{
    /// <summary>
    /// Allows a user to specify a Region of Interest with Still captures.
    /// </summary>
    public struct Zoom
    {
        /// <summary>
        /// The X coordinate between 0 - 1.0.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// The Y coordinate between 0 - 1.0.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// The Width value between 0 - 1.0.
        /// </summary>
        public double Width { get; }

        /// <summary>
        /// The Height value between 0 - 1.0.
        /// </summary>
        public double Height { get; }

        /// <summary>
        /// Intialises a new <see cref="Zoom"/> struct.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="width">The Width value.</param>
        /// <param name="height">The Height value.</param>
        public Zoom(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
