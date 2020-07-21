// <copyright file="IFrameAnalyser.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;

namespace MMALSharp.Processors
{
    /// <summary>
    /// Represents a frame analyser.
    /// </summary>
    public interface IFrameAnalyser
    {
        /// <summary>
        /// The operation to perform analysis.
        /// </summary>
        /// <param name="context">Contains the data and metadata for an image frame.</param>
        void Apply(ImageContext context);
    }
}
