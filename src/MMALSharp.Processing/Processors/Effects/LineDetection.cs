// <copyright file="LineDetection.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Common;
using System;
using System.Diagnostics;

namespace MMALSharp.Processors.Effects
{
    /// <summary>
    /// The type of line highlighted by the processor.
    /// </summary>
    public enum LineDetectionType
    {
        /// <summary>
        /// Highlights horizontal lines.
        /// </summary>
        Horizontal = 0,

        /// <summary>
        /// Higlights vertical lines.
        /// </summary>
        Vertical,

        /// <summary>
        /// Highlights horizontal lines with a slight smoothing effect (less sensitive to noise).
        /// </summary>
        SobelHorizontal,

        /// <summary>
        /// Highlights vertical lines with a slight smoothing effect (less sensitive to noise).
        /// </summary>
        SobelVertical,

        /// <summary>
        /// Highlights diagonal lines sloping down left-to-right (135 degrees).
        /// </summary>
        DiagonalDown,

        /// <summary>
        /// Highlights diagonal lines sloping up left-to-right (45 degrees).
        /// </summary>
        DiagonalUp,
    }

    /// <summary>
    /// An image processor used to highlight straight lines.
    /// </summary>
    public class LineDetection : ConvolutionBase, IFrameProcessor
    {
        private const int _kernelWidth = 3;
        private const int _kernelHeight = 3;

        private readonly double[][,] _kernels =
        {
            new double[3, 3] // 0 - Horizontal
            {
                { -1,  2, -1 },
                { -1,  2, -1 },
                { -1,  2, -1 }
            },
            new double[3, 3] // 1 - Vertical
            {
                { -1, -1, -1 },
                {  2,  2,  2 },
                { -1, -1, -1 }
            },
            new double[3, 3] // 2 - SobelHorizontal
            {
                { -1,  0,  1 },
                { -2,  0,  2 },
                { -1,  0,  1 }
            },
            new double[3, 3] // 3 - SobelVertical
            {
                { -1, -2, -1 },
                {  0,  0,  0 },
                {  1,  2,  1 }
            },
            new double[3, 3] // 4 - DiagonalDown
            {
                { -1, -1,  2 },
                { -1,  2, -1 },
                {  2, -1, -1 }
            },
            new double[3, 3] // 5 - DiagonalUp
            {
                {  2, -1, -1 },
                { -1,  2, -1 },
                { -1, -1,  2 }
            }
        };

        private readonly int _kernelType;

        /// <inheritdoc />
        public LineDetection(LineDetectionType lineType)
            : base()
        {
            _kernelType = (int)lineType;
        }

        /// <inheritdoc />
        public LineDetection(LineDetectionType lineType, int horizontalCellCount, int verticalCellCount)
            : base(horizontalCellCount, verticalCellCount)
        {
            _kernelType = (int)lineType;
        }

        /// <inheritdoc />
        public void Apply(ImageContext context)
        {
            this.ApplyConvolution(_kernels[_kernelType], _kernelWidth, _kernelHeight, context);
        }
    }
}
