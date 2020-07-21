// <copyright file="BayerOperationsExtensions.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;
using MMALSharp.Processors.Bayer;

namespace MMALSharp.Processors
{
    /// <summary>
    /// Extensions methods relating to Bayer metadata.
    /// </summary>
    public static class BayerOperationsExtensions
    {
        /// <summary>
        /// Apply a processor to strip out Bayer metadata from a JPEG frame.
        /// </summary>
        /// <param name="context">The image context.</param>
        /// <param name="version">The camera version.</param>
        /// <returns>The active image context.</returns>
        public static IFrameProcessingContext StripBayerMetadata(this IFrameProcessingContext context, CameraVersion version)
            => context.Apply(new BayerMetaProcessor(version));
        
        /// <summary>
        /// Apply a processor to apply a Demosaic to raw Bayer metadata.
        /// </summary>
        /// <param name="context">The image context.</param>
        /// <returns>The active image context.</returns>
        public static IFrameProcessingContext Demosaic(this IFrameProcessingContext context)
            => context.Apply(new DemosaicProcessor());
    }
}
