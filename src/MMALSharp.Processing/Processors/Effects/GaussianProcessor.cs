// <copyright file="GaussianProcessor.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;
using System;
using System.Diagnostics;

namespace MMALSharp.Processors.Effects
{
    /// <summary>
    /// Represents the matrix to use when applying a Gaussian blur convolution to an image frame.
    /// </summary>
    public enum GaussianMatrix
    {
        /// <summary>
        /// Use a 3x3 matrix.
        /// </summary>
        Matrix3x3 = 0,

        /// <summary>
        /// Use a 5x5 matrix.
        /// </summary>
        Matrix5x5,
    }

    /// <summary>
    /// A image processor used to apply a Gaussian blur effect.
    /// </summary>
    public class GaussianProcessor : ConvolutionBase, IFrameProcessor
    {
        private readonly int _kernelType;

        private readonly double[][,] _kernels =
{
            new double[3, 3] // 0 - Matrix3x3
            {
                { 0.0625, 0.125, 0.0625 },
                { 0.125,  0.25,  0.125 },
                { 0.0625, 0.125, 0.0625 },
            },
            new double[5, 5] // 1 - Matrix5x5
            {
                { 0.00390625, 0.015625, 0.0234375, 0.015625, 0.00390625 },
                { 0.015625, 0.0625, 0.09375, 0.0625, 0.015625 },
                { 0.0234375, 0.09375, 0.140625, 0.09375, 0.0234375 },
                { 0.015625, 0.0625, 0.09375, 0.0625, 0.015625 },
                { 0.00390625, 0.015625, 0.0234375, 0.015625, 0.00390625 },
            },
        };

        private readonly (int width, int height)[] _sizes =
        {
            (3, 3), // 0 - Matrix3x3
            (5, 5), // 1 - Matrix5x5
        };

        /// <summary>
        /// Creates a new instance of <see cref="GaussianProcessor"/>.
        /// </summary>
        /// <param name="matrix">The Gaussian matrix to apply.</param>
        public GaussianProcessor(GaussianMatrix matrix)
            : base()
        {
            _kernelType = (int)matrix;
        }

        /// <summary>
        /// Creates a new instance of <see cref="GaussianProcessor"/>.
        /// </summary>
        /// <param name="matrix">The Gaussian matrix to apply.</param>
        /// <param name="horizontalCellCount">The number of columns to divide the image into.</param>
        /// <param name="verticalCellCount">The number of rows to divide the image into.</param>
        public GaussianProcessor(GaussianMatrix matrix, int horizontalCellCount, int verticalCellCount)
            : base(horizontalCellCount, verticalCellCount)
        {
            _kernelType = (int)matrix;
        }

        /// <inheritdoc />
        public void Apply(ImageContext context)
        {
            this.ApplyConvolution(_kernels[_kernelType], _sizes[_kernelType].width, _sizes[_kernelType].height, context);
        }
    }
}