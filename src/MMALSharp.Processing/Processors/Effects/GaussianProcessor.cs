// <copyright file="GaussianProcessor.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;

namespace MMALSharp.Processors.Effects
{
    public enum GaussianMatrix
    {
        Matrix3x3,
        Matrix5x5,
    }

    /// <summary>
    /// A image processor used to apply a Gaussian blur effect.
    /// </summary>
    public class GaussianProcessor : ConvolutionBase, IFrameProcessor
    {
        private readonly int _kernelWidth = 3;
        private readonly int _kernelHeight = 3;

        private double[,] Kernel { get; }

        /// <summary>
        /// Creates a new instance of <see cref="GaussianProcessor"/>.
        /// </summary>
        /// <param name="matrix">The Gaussian matrix to apply.</param>
        public GaussianProcessor(GaussianMatrix matrix)
        {
            switch (matrix)
            {
                case GaussianMatrix.Matrix3x3:
                    _kernelWidth = 3;
                    _kernelHeight = 3;
                    Kernel = new double[3, 3]
                    {
                        { 0.0625, 0.125, 0.0625 },
                        { 0.125,  0.25,  0.125 },
                        { 0.0625, 0.125, 0.0625 }
                    };
                    break;
                case GaussianMatrix.Matrix5x5:
                    _kernelWidth = 5;
                    _kernelHeight = 5;
                    Kernel = new double[5, 5]
                    {
                        { 0.00390625, 0.015625, 0.0234375, 0.015625, 0.00390625 },
                        { 0.015625, 0.0625, 0.09375, 0.0625, 0.015625 },
                        { 0.0234375, 0.09375, 0.140625, 0.09375, 0.0234375 },
                        { 0.015625, 0.0625, 0.09375, 0.0625, 0.015625 },
                        { 0.00390625, 0.015625, 0.0234375, 0.015625, 0.00390625 },
                    };
                    break;
            }
        }

        /// <inheritdoc />
        public void Apply(IImageContext context)
        {
            this.ApplyConvolution(this.Kernel, _kernelWidth, _kernelHeight, context);
        }
    }
}