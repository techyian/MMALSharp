// <copyright file="ConvolutionBase.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MMALSharp.Common;
using MMALSharp.Common.Utility;

namespace MMALSharp.Processors.Effects
{
    /// <summary>
    /// Base class for image processors using matrix convolution.
    /// </summary>
    public abstract class ConvolutionBase
    {
        private readonly int _horizontalCellCount;
        private readonly int _verticalCellCount;

        /// <summary>
        /// Creates a <see cref="ConvolutionBase"/> object. This uses the default parallel processing
        /// cell count based on the image resolution and the recommended values defined by the
        /// <see cref="FrameAnalyser"/>. Requires use of one of the standard camera image resolutions.
        /// </summary>
        public ConvolutionBase()
        {
            _horizontalCellCount = 0;
            _verticalCellCount = 0;
        }

        /// <summary>
        /// Creates a <see cref="ConvolutionBase"/> object with custom parallel processing cell counts.
        /// You must use this constructor if you are processing non-standard image resolutions.
        /// </summary>
        /// <param name="horizontalCellCount">The number of columns to divide the image into.</param>
        /// <param name="verticalCellCount">The number of rows to divide the image into.</param>
        public ConvolutionBase(int horizontalCellCount, int verticalCellCount)
        {
            _horizontalCellCount = horizontalCellCount;
            _verticalCellCount = verticalCellCount;
        }

        /// <summary>
        /// Apply a convolution based on the kernel passed in.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="kernelWidth">The kernel's width.</param>
        /// <param name="kernelHeight">The kernel's height.</param>
        /// <param name="context">An image context providing additional metadata on the data passed in.</param>
        public void ApplyConvolution(double[,] kernel, int kernelWidth, int kernelHeight, ImageContext context)
        {
            if (!context.Raw)
            {
                throw new Exception("Convolution effects require raw frame data");
            }

            var analyser = new FrameAnalyser
            {
                HorizonalCellCount = _horizontalCellCount,
                VerticalCellCount = _verticalCellCount,
            };
            analyser.Apply(context);

            Parallel.ForEach(analyser.CellRect, (cell, loopState, loopIndex)
                => ProcessCell(cell, context.Data, kernel, kernelWidth, kernelHeight, analyser.Metadata));

            if(context.StoreFormat != null)
            {
                context.FormatRawImage();
            }
        }

        private void ProcessCell(Rectangle rect, byte[] image, double[,] kernel, int kernelWidth, int kernelHeight, FrameAnalysisMetadata metadata)
        {
            // Rectangle and FrameAnalysisMetadata are structures; they are by-value copies and all fields are value-types which makes them thread safe

            int x2 = rect.X + rect.Width;
            int y2 = rect.Y + rect.Height;

            int index;

            for (var x = rect.X; x < x2; x++)
            {
                for (var y = rect.Y; y < y2; y++)
                {
                    double r = 0;
                    double g = 0;
                    double b = 0;

                    if (x > kernelWidth && y > kernelHeight)
                    {
                        for (var t = 0; t < kernelWidth; t++)
                        {
                            for(var u = 0; u < kernelHeight; u++)
                            {
                                double k = kernel[t, u];

                                index = (Clamp(y + u, y2) * metadata.Stride) + (Clamp(x + t, x2) * metadata.Bpp);

                                r += image[index] * k;
                                g += image[index + 1] * k;
                                b += image[index + 2] * k;
                            }
                        }

                        r = (r < 0) ? 0 : r;
                        g = (g < 0) ? 0 : g;
                        b = (b < 0) ? 0 : b;
                    }

                    index = (x * metadata.Bpp) + (y * metadata.Stride);

                    image[index] = (byte)r;
                    image[index + 1] = (byte)g;
                    image[index + 2] = (byte)b;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Clamp(int value, int endIndex)
        {
            if (value < 0)
            {
                return 0;
            }

            if (value < endIndex)
            {
                return value;
            }

            return endIndex - 1;
        }
    }
}