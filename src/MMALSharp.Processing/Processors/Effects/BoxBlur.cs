// <copyright file="BoxBlur.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;
using System;
using System.Diagnostics;

namespace MMALSharp.Processors.Effects
{
    /// <summary>
    /// An image processor used to apply a box-blur effect.
    /// </summary>
    public class BoxBlur : ConvolutionBase, IFrameProcessor
    {
        private const int _kernelWidth = 3;
        private const int _kernelHeight = 3;
        private double[,] _kernel = new double[3, 3]
        {
            {0.11111111, 0.11111111, 0.11111111 },
            {0.11111111, 0.11111111, 0.11111111 },
            {0.11111111, 0.11111111, 0.11111111 },
        };

        /// <inheritdoc />
        public BoxBlur()
            : base()
        { }

        /// <inheritdoc />
        public BoxBlur(int horizontalCellCount, int verticalCellCount)
            : base(horizontalCellCount, verticalCellCount)
        { }

        /// <inheritdoc />
        public void Apply(ImageContext context)
        {
            this.ApplyConvolution(_kernel, _kernelWidth, _kernelHeight, context);
        }
    }
}
