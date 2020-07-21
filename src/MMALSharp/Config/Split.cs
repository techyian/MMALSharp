// <copyright file="Split.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Config
{
    /// <summary>
    /// The <see cref="Split"/> type is used when taking video capture and a user wishes to split
    /// recording into multiple files. 
    /// </summary>
    public class Split
    {
        /// <summary>
        /// How often files should be split.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// The <see cref="TimelapseMode"/> mode to use.
        /// </summary>
        public TimelapseMode Mode { get; set; }
    }
}
