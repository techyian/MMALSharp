using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using MMALSharp.Common;

namespace MMALSharp.Processors.Effects
{
    public abstract class ConvolutionBase
    {
        public virtual void Convolute(byte[] store, double[,] kernel, int kernelWidth, int kernelHeight, IImageContext context)
        {
            using (var bmp = new Bitmap(context.Resolution.Width, context.Resolution.Height, context.PixelFormat))
            {
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0,
                        bmp.Width,
                        bmp.Height),
                    ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

                IntPtr pNative = bmpData.Scan0;
                Marshal.Copy(store, 0, pNative, store.Length);
                
                // Declare an array to hold the bytes of the bitmap.
                int bytes = bmpData.Stride * bmp.Height;
                int stride = bmpData.Stride;

                var rgbValues = new byte[bytes];
                
                // Copy the RGB values into the array.
                Marshal.Copy(pNative, rgbValues, 0, bytes);

                for (int column = 0; column < bmpData.Height; column++)
                {
                    for (int row = 0; row < bmpData.Width; row++)
                    {
                        if (column > 3 && row > 3)
                        {
                            int acc = 0;
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

                            rgbValues[(column * stride) + (row * 3)] = (byte)Math.Max(0, r1);
                            rgbValues[(column * stride) + (row * 3) + 1] = (byte)Math.Max(0, g1);
                            rgbValues[(column * stride) + (row * 3) + 2] = (byte)Math.Max(0, b1);
                        }
                        else
                        {
                            rgbValues[(column * stride) + (row * 3)] = 0;
                            rgbValues[(column * stride) + (row * 3) + 1] = 0;
                            rgbValues[(column * stride) + (row * 3) + 2] = 0;
                        }
                    }
                }
                
                bmp.UnlockBits(bmpData);
                store = rgbValues;
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