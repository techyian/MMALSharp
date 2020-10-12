// <copyright file="SharpenProcessor.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;
using System;
using System.Diagnostics;

namespace MMALSharp.Processors.Effects
{
    /// <summary>
    /// A image processor used to apply a sharpen effect.
    /// </summary>
    public class SharpenProcessor : ConvolutionBase, IFrameProcessor
    {
        private const int KernelWidth = 3;
        private const int KernelHeight = 3;

        private double[,] _kernel = new double[KernelWidth, KernelHeight]
        {
            {  0, -1,  0 },
            { -1,  5, -1 },
            {  0, -1,  0 },
        };

        /// <inheritdoc />
        public SharpenProcessor()
            : base()
        { }

        /// <inheritdoc />
        public SharpenProcessor(int horizontalCellCount, int verticalCellCount)
            : base(horizontalCellCount, verticalCellCount)
        { }

        /// <inheritdoc />
        public void Apply(ImageContext context)
        {
            this.ApplyConvolution(_kernel, KernelWidth, KernelHeight, context);
        }
    }
}
