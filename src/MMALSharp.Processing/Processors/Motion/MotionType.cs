// <copyright file="MotionType.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Processors.Motion
{
    /// <summary>
    /// Describes motion detection type to use.
    /// </summary>
    public enum MotionType
    {
        /// <summary>
        /// Frame difference.
        /// </summary>
        FrameDiff,

        /// <summary>
        /// Motion vector comparison.
        /// </summary>
        MotionVector
    }
}
