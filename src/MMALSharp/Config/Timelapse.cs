// <copyright file="Timelapse.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Threading;

namespace MMALSharp.Config
{
    /// <summary>
    /// The <see cref="Timelapse"/> type is for use with Timelapse still captures.
    /// </summary>
    public class Timelapse
    {
        /// <summary>
        /// The timelapse mode.
        /// </summary>
        public TimelapseMode Mode { get; set; }

        /// <summary>
        /// Specifies when timelapse capture should finish.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// How often images should be taken (relates to the <see cref="TimelapseMode"/> chosen).
        /// </summary>
        public int Value { get; set; }
    }
}
