// <copyright file="TimelapseMode.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Config
{
    /// <summary>
    /// The unit of time to use.
    /// </summary>
    public enum TimelapseMode
    {
        /// <summary>
        /// Uses milliseconds as unit of time. One hour equals 3'600'000 milliseconds.
        /// </summary>
        Millisecond,

        /// <summary>
        /// Uses seconds as unit of time. One hour equals 3'600 seconds.
        /// </summary>
        Second,

        /// <summary>
        /// Uses minutes as unit of time. One hour equals 60 minutes.
        /// </summary>
        Minute
    }
}
