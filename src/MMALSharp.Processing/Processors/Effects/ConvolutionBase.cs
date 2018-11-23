// <copyright file="ConvolutionBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
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
        /// <param name="store">The image data.</param>
        /// <param name="kernel">The kernel.</param>
        /// <param name="kernelWidth">The kernel's width.</param>
        /// <param name="kernelHeight">The kernel's height.</param>
        /// <param name="context">An image context providing additional metadata on the data passed in.</param>
        public void Convolute(byte[] store, double[,] kernel, int kernelWidth, int kernelHeight, IImageContext context)
        {
            Bitmap bmp = null;
            BitmapData bmpData = null;
            IntPtr pNative = IntPtr.Zero;
            int bytes, stride;
            byte[] rgbValues = null;
            
            using (var ms = new MemoryStream(store))
            {
                if (context.Raw)
                {
                    bmp = new Bitmap(context.Resolution.Width, context.Resolution.Height, context.PixelFormat);
                    bmpData = bmp.LockBits(new Rectangle(0, 0,
                            bmp.Width,
                            bmp.Height),
                        ImageLockMode.ReadWrite,
                        bmp.PixelFormat);

                    pNative = bmpData.Scan0;
                    Marshal.Copy(store, 0, pNative, store.Length);
                }
                else
                {
                    bmp = new Bitmap(ms);
                    bmpData = bmp.LockBits(new Rectangle(0, 0,
                            bmp.Width,
                            bmp.Height),
                        ImageLockMode.ReadWrite,
                        bmp.PixelFormat);
                    pNative = bmpData.Scan0;
                }

                unsafe
                {
                    // Declare an array to hold the bytes of the bitmap.
                    bytes = bmpData.Stride * bmp.Height;
                    stride = bmpData.Stride;
                    
                    byte* ptr1 = (byte*)bmpData.Scan0;
                    
                    rgbValues = new byte[bytes];
                    
                    // Copy the RGB values into the array.
                    Marshal.Copy(pNative, rgbValues, 0, bytes);
    
                    for (int column = 0; column < bmpData.Height; column++)
                    {
                        for (int row = 0; row < bmpData.Width; row++)
                        {
                            if (column > 3 && row > 3)
                            {
                                int r1 = 0, g1 = 0, b1 = 0;
    
                                for (var l = 0; l < kernelWidth; l++)
                                {
                                    for (var m = 0; m < kernelHeight; m++)
                                    {
                                        r1 += (int)(rgbValues[(this.Bound(column + m, bmpData.Height) * stride) + (this.Bound(row + l, bmpData.Width) * 3)] * kernel[l, m]);
                                        g1 += (int)(rgbValues[(this.Bound(column + m, bmpData.Height) * stride) + (this.Bound(row + l, bmpData.Width) * 3) + 1] * kernel[l, m]);
                                        b1 += (int)(rgbValues[(this.Bound(column + m, bmpData.Height) * stride) + (this.Bound(row + l, bmpData.Width) * 3) + 2] * kernel[l, m]);
                                    }
                                }
                                
                                ptr1[(row * 3) + column * stride] = (byte)Math.Max(0,r1);
                                ptr1[(row * 3) + column * stride + 1] = (byte)Math.Max(0, g1);
                                ptr1[(row * 3) + column * stride + 2] = (byte)Math.Max(0, b1);
                            }
                            else
                            {
                                ptr1[(row * 3) + column * stride] = 0;
                                ptr1[(row * 3) + column * stride + 1] = 0;
                                ptr1[(row * 3) + column * stride + 2] = 0;
                            }
                        }
                    }
                }
               
                bmp.UnlockBits(bmpData);
            }
            
            if (context.Raw)
            {
                Marshal.Copy(pNative, store, 0, bytes);
            }
            else
            {
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Jpeg);
                    Array.Copy(ms.ToArray(), 0, store, 0, ms.Length);
                }
            }
            
            bmp.Dispose();
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