// <copyright file="ConvolutionBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MMALSharp.Common;

namespace MMALSharp.Processors.Effects
{
    /// <summary>
    /// Base class for image processors using matrix convolution.
    /// </summary>
    public abstract class ConvolutionBase
    {
        /// <summary>
        /// Apply a convolution based on the kernel passed in.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="kernelWidth">The kernel's width.</param>
        /// <param name="kernelHeight">The kernel's height.</param>
        /// <param name="context">An image context providing additional metadata on the data passed in.</param>
        public void ApplyConvolution(double[,] kernel, int kernelWidth, int kernelHeight, ImageContext context)
        {
            BitmapData bmpData = null;
            IntPtr pNative = IntPtr.Zero;
            int bytes;
            byte[] store = null;

            using (var ms = new MemoryStream(context.Data))
            using (var bmp = this.LoadBitmap(context, ms))
            {
                bmpData = bmp.LockBits(new Rectangle(0, 0,
                        bmp.Width,
                        bmp.Height),
                    ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

                if (context.Raw)
                {
                    this.InitBitmapData(context, bmpData);
                }

                pNative = bmpData.Scan0;

                // Split image into 4 quadrants and process individually.
                var quadA = new Rectangle(0, 0, bmpData.Width / 2, bmpData.Height / 2);
                var quadB = new Rectangle(bmpData.Width / 2, 0, bmpData.Width / 2, bmpData.Height / 2);
                var quadC = new Rectangle(0, bmpData.Height / 2, bmpData.Width / 2, bmpData.Height / 2);
                var quadD = new Rectangle(bmpData.Width / 2, bmpData.Height / 2, bmpData.Width / 2, bmpData.Height / 2);

                bytes = bmpData.Stride * bmp.Height;

                var rgbValues = new byte[bytes];

                // Copy the RGB values into the array.
                Marshal.Copy(pNative, rgbValues, 0, bytes);

                var bpp = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;

                var t1 = Task.Run(() =>
                {
                    this.ProcessQuadrant(quadA, bmp, bmpData, rgbValues, kernel, kernelWidth, kernelHeight, bpp);
                });
                var t2 = Task.Run(() =>
                {
                    this.ProcessQuadrant(quadB, bmp, bmpData, rgbValues, kernel, kernelWidth, kernelHeight, bpp);
                });
                var t3 = Task.Run(() =>
                {
                    this.ProcessQuadrant(quadC, bmp, bmpData, rgbValues, kernel, kernelWidth, kernelHeight, bpp);
                });
                var t4 = Task.Run(() =>
                {
                    this.ProcessQuadrant(quadD, bmp, bmpData, rgbValues, kernel, kernelWidth, kernelHeight, bpp);
                });

                Task.WaitAll(t1, t2, t3, t4);

                if (context.Raw)
                {
                    store = new byte[bytes];
                    Marshal.Copy(pNative, store, 0, bytes);
                }

                bmp.UnlockBits(bmpData);

                if (!context.Raw)
                {
                    using (var ms2 = new MemoryStream())
                    {
                        bmp.Save(ms2, context.StoreFormat);
                        store = new byte[ms2.Length];
                        Array.Copy(ms2.ToArray(), 0, store, 0, ms2.Length);
                    }
                }
            }
            
            context.Data = store;
        }

        private Bitmap LoadBitmap(ImageContext imageContext, MemoryStream stream)
        {
            if (imageContext.Raw)
            {
                PixelFormat format = default;

                if (imageContext.PixelFormat == MMALEncoding.RGB16)
                {
                    format = PixelFormat.Format16bppRgb565;
                }

                if (imageContext.PixelFormat == MMALEncoding.RGB24)
                {
                    format = PixelFormat.Format24bppRgb;
                }

                if (imageContext.PixelFormat == MMALEncoding.RGB32)
                {
                    format = PixelFormat.Format32bppRgb;
                }

                if (imageContext.PixelFormat == MMALEncoding.RGBA)
                {
                    format = PixelFormat.Format32bppArgb;
                }

                if (format == default)
                {
                    throw new Exception("Unsupported pixel format for Bitmap");
                }

                return new Bitmap(imageContext.Resolution.Width, imageContext.Resolution.Height, format);
            }

            return new Bitmap(stream);
        }

        private void InitBitmapData(ImageContext imageContext, BitmapData bmpData)
        {
            var pNative = bmpData.Scan0;
            Marshal.Copy(imageContext.Data, 0, pNative, imageContext.Data.Length);
        }

        private void ProcessQuadrant(Rectangle quad, Bitmap bmp, BitmapData bmpData, byte[] rgbValues, double[,] kernel, int kernelWidth, int kernelHeight, int pixelDepth)
        {
            unsafe
            {
                // Declare an array to hold the bytes of the bitmap.
                var stride = bmpData.Stride;

                byte* ptr1 = (byte*)bmpData.Scan0;

                for (int column = quad.X; column < quad.X + quad.Width; column++)
                {
                    for (int row = quad.Y; row < quad.Y + quad.Height; row++)
                    {
                        if (column > kernelWidth && row > kernelHeight)
                        {
                            int r1 = 0, g1 = 0, b1 = 0;

                            for (var l = 0; l < kernelWidth; l++)
                            {
                                for (var m = 0; m < kernelHeight; m++)
                                {
                                    r1 += (int)(rgbValues[(this.Bound(row + m, quad.Y + quad.Height) * stride) + (this.Bound(column + l, quad.X + quad.Width) * pixelDepth)] * kernel[l, m]);
                                    g1 += (int)(rgbValues[(this.Bound(row + m, quad.Y + quad.Height) * stride) + (this.Bound(column + l, quad.X + quad.Width) * pixelDepth) + 1] * kernel[l, m]);
                                    b1 += (int)(rgbValues[(this.Bound(row + m, quad.Y + quad.Height) * stride) + (this.Bound(column + l, quad.X + quad.Width) * pixelDepth) + 2] * kernel[l, m]);
                                }
                            }

                            ptr1[(column * pixelDepth) + (row * stride)] = (byte)Math.Max(0, r1);
                            ptr1[(column * pixelDepth) + (row * stride) + 1] = (byte)Math.Max(0, g1);
                            ptr1[(column * pixelDepth) + (row * stride) + 2] = (byte)Math.Max(0, b1);
                        }
                        else
                        {
                            ptr1[(column * pixelDepth) + (row * stride)] = 0;
                            ptr1[(column * pixelDepth) + (row * stride) + 1] = 0;
                            ptr1[(column * pixelDepth) + (row * stride) + 2] = 0;
                        }
                    }
                }
            }
        }
        
        private int Bound(int value, int endIndex)
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