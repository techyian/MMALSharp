// <copyright file="EdgeDetection.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;
using System;
using System.Diagnostics;

namespace MMALSharp.Processors.Effects
{
    /// <summary>
    /// Represents the strength of the Edge Detection algorithm.
    /// </summary>
    public enum EDStrength
    {
        /// <summary>
        /// Low strength.
        /// </summary>
        Low = 0,

        /// <summary>
        /// Medium strength.
        /// </summary>
        Medium,

        /// <summary>
        /// High strength.
        /// </summary>
        High
    }

    /// <summary>
    /// A kernel based image processor used to apply Edge detection convolution.
    /// </summary>
    public class EdgeDetection : ConvolutionBase, IFrameProcessor
    {
        private const int _kernelWidth = 3;
        private const int _kernelHeight = 3;

        private readonly double[][,] _kernels =
        {
            new double[,] // 0 - Low
            {
                { -1, 0, 1 },
                { 0, 0, 0 },
                { 1, 0, -1 }
            },
            new double[,] // 1 - Medium
            {
                { 0, 1, 0 },
                { 1, -4, 1 },
                { 0, 1, 0 }
            },
            new double[,] // 2 - High
            {
                { -1, -1, -1 },
                { -1, 8, -1 },
                { -1, -1, -1 }
            },
        };

        private readonly int _kernelType;

        /// <summary>
        /// Creates a new instance of <see cref="EdgeDetection"/> processor used to apply Edge detection convolution.
        /// </summary>
        /// <param name="strength">The Edge detection strength.</param>
        public EdgeDetection(EDStrength strength)
            : base()
        {
            _kernelType = (int)strength;
        }

        /// <summary>
        /// Creates a new instance of <see cref="EdgeDetection"/> processor used to apply Edge detection convolution.
        /// </summary>
        /// <param name="strength">The Edge detection strength.</param>
        /// <param name="horizontalCellCount">The number of columns to divide the image into.</param>
        /// <param name="verticalCellCount">The number of rows to divide the image into.</param>
        public EdgeDetection(EDStrength strength, int horizontalCellCount, int verticalCellCount)
            : base(horizontalCellCount, verticalCellCount)
        {
            _kernelType = (int)strength;
        }

        /// <inheritdoc />
        public void Apply(ImageContext context)
        {
            this.ApplyConvolution(_kernels[_kernelType], _kernelWidth, _kernelHeight, context);
        }
    }
}
