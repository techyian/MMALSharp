// <copyright file="MMALBootstrapper.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Collections.Generic;
using MMALSharp.Components;

namespace MMALSharp
{
    /// <summary>
    /// Used as a common class to store downstream component references between standalone mode and camera connected mode.
    /// </summary>
    public static class MMALBootstrapper
    {
        /// <summary>
        /// List of all encoders currently in the pipeline.
        /// </summary>
        public static List<MMALDownstreamComponent> DownstreamComponents { get; } = new List<MMALDownstreamComponent>();
    }
}