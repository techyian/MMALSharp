// <copyright file="ConvolutionBase.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
            var localContext = context.Raw ? context : CloneToRawBitmap(context);

            bool storeFromRaw = context.Raw && context.StoreFormat != null;

            var analyser = new FrameAnalyser
            {
                HorizonalCellCount = _horizontalCellCount,
                VerticalCellCount = _verticalCellCount,
            };
            analyser.Apply(localContext);

            Parallel.ForEach(analyser.CellRect, (cell)
                => ProcessCell(cell, localContext.Data, kernel, kernelWidth, kernelHeight, analyser.Metadata, storeFromRaw));

            if (context.StoreFormat != null)
            {
                FormatRawBitmap(localContext, context);
                context.Raw = false; // context is never raw after formatting
            }
            else
            {
                if(!context.Raw)
                {
                    // TakePicture doesn't set the Resolution, copy it from the cloned version which stored it from Bitmap
                    context.Resolution = new Resolution(localContext.Resolution.Width, localContext.Resolution.Height);

                    context.Data = new byte[localContext.Data.Length];
                    Array.Copy(localContext.Data, context.Data, context.Data.Length);
                    context.Raw = true; // we just copied raw data to the source context
                }
            }
        }

        private void ProcessCell(Rectangle rect, byte[] image, double[,] kernel, int kernelWidth, int kernelHeight, FrameAnalysisMetadata metadata, bool storeFromRaw)
        {
            // Rectangle and FrameAnalysisMetadata are structures; they are by-value copies and all fields are value-types which makes them thread safe

            int x2 = rect.X + rect.Width;
            int y2 = rect.Y + rect.Height;

            int index;

            // Indicates RGB needs to be swapped to BGR so that Bitmap.Save works correctly.
            if (storeFromRaw)
            {
                for (var x = rect.X; x < x2; x++)
                {
                    for (var y = rect.Y; y < y2; y++)
                    {
                        index = (x * metadata.Bpp) + (y * metadata.Stride);
                        byte swap = image[index];
                        image[index] = image[index + 2];
                        image[index + 2] = swap;
                    }
                }
            }

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
        private int Clamp(int value, int maxIndex)
        {
            if (value < 0)
            {
                return 0;
            }

            if (value < maxIndex)
            {
                return value;
            }

            return maxIndex - 1;
        }

        private ImageContext CloneToRawBitmap(ImageContext sourceContext)
        {
            var newContext = new ImageContext
            {
                Raw = true,
                Eos = sourceContext.Eos,
                IFrame = sourceContext.IFrame,
                Encoding = sourceContext.Encoding,
                Pts = sourceContext.Pts,
                StoreFormat = sourceContext.StoreFormat
            };

            using (var ms = new MemoryStream(sourceContext.Data))
            {
                using (var sourceBmp = new Bitmap(ms))
                {
                    // sourceContext.Resolution isn't set by TakePicture (width,height is 0,0)
                    newContext.Resolution = new Resolution(sourceBmp.Width, sourceBmp.Height);

                    // If the source bitmap has a raw-compatible format, use it, otherwise default to RGBA
                    newContext.PixelFormat = PixelFormatToMMALEncoding(sourceBmp.PixelFormat, MMALEncoding.RGBA);
                    var bmpTargetFormat = MMALEncodingToPixelFormat(newContext.PixelFormat);
                    var rect = new Rectangle(0, 0, sourceBmp.Width, sourceBmp.Height);

                    using (var newBmp = sourceBmp.Clone(rect, bmpTargetFormat))
                    {
                        BitmapData bmpData = null;
                        try
                        {
                            bmpData = newBmp.LockBits(rect, ImageLockMode.ReadOnly, bmpTargetFormat);
                            var ptr = bmpData.Scan0;
                            int size = bmpData.Stride * newBmp.Height;
                            newContext.Data = new byte[size];
                            newContext.Stride = bmpData.Stride;
                            Marshal.Copy(ptr, newContext.Data, 0, size);
                        }
                        finally
                        {
                            newBmp.UnlockBits(bmpData);
                        }
                    }
                }
            }

            return newContext;
        }

        private void FormatRawBitmap(ImageContext sourceContext, ImageContext targetContext)
        {
            var pixfmt = MMALEncodingToPixelFormat(sourceContext.PixelFormat);

            using (var bitmap = new Bitmap(sourceContext.Resolution.Width, sourceContext.Resolution.Height, pixfmt))
            {
                BitmapData bmpData = null;
                try
                {
                    bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                    var ptr = bmpData.Scan0;
                    int size = bmpData.Stride * bitmap.Height;
                    var data = sourceContext.Data;
                    Marshal.Copy(data, 0, ptr, size);
                }
                finally
                {
                    bitmap.UnlockBits(bmpData);
                }

                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, targetContext.StoreFormat);
                    targetContext.Data = new byte[ms.Length];
                    Array.Copy(ms.ToArray(), 0, targetContext.Data, 0, ms.Length);
                }
            }
        }

        private PixelFormat MMALEncodingToPixelFormat(MMALEncoding encoding)
        {
            if (encoding == MMALEncoding.RGB24)
            {
                return PixelFormat.Format24bppRgb;
            }

            if (encoding == MMALEncoding.RGB32)
            {
                return PixelFormat.Format32bppRgb;
            }

            if (encoding == MMALEncoding.RGBA)
            {
                return PixelFormat.Format32bppArgb;
            }

            throw new Exception($"Unsupported pixel format: {encoding}");
        }

        private MMALEncoding PixelFormatToMMALEncoding(PixelFormat format, MMALEncoding defaultEncoding)
        {
            if (format == PixelFormat.Format24bppRgb)
            {
                return MMALEncoding.RGB24;
            }

            if (format == PixelFormat.Format32bppRgb)
            {
                return MMALEncoding.RGB32;
            }

            if (format == PixelFormat.Format32bppArgb)
            {
                return MMALEncoding.RGBA;
            }

            return defaultEncoding;
        }
    }
}