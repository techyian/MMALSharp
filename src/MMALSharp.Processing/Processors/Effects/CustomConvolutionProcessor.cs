// <copyright file="CustomConvolutionProcessor.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;

namespace MMALSharp.Processors.Effects
{
    /// <summary>
    /// A image processor allowing for user-defined kernels.
    /// </summary>
    public class CustomConvolutionProcessor : ConvolutionBase, IFrameProcessor
    {
        private readonly int _kernelWidth;
        private readonly int _kernelHeight;

        private double[,] Kernel { get; }

        /// <summary>
        /// Creates a new instance of <see cref="CustomConvolutionProcessor"/>.
        /// </summary>
        /// <param name="kernel">The user-defined kernel.</param>
        /// <param name="kernelWidth">The kernel's width.</param>
        /// <param name="kernelHeight">The kernel's height.</param>
        public CustomConvolutionProcessor(double[,] kernel, int kernelWidth, int kernelHeight)
        {
            this.Kernel = kernel;

            _kernelWidth = kernelWidth;
            _kernelHeight = kernelHeight;
        }

        /// <inheritdoc />
        public void Apply(ImageContext context)
        {
            this.ApplyConvolution(this.Kernel, _kernelWidth, _kernelHeight, context);
        }
    }
}
