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
            byte[] rgbValues = null;
            
            using (var ms = new MemoryStream(store))
            {
                var bmp = new Bitmap(ms);
                
                // Lock the bitmap's bits.  
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, context.PixelFormat);

                // Declare an array to hold the bytes of the bitmap.
                int bytes = bmpData.Stride * bmp.Height;
                rgbValues = new byte[bytes];
                
                int count = 0;
                int stride = bmpData.Stride;
             
                unsafe
                {
                    IntPtr ptr = bmpData.Scan0;
                 
                    // Copy the RGB values into the array.
                    Marshal.Copy(ptr, rgbValues, 0, bytes);
    
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

                                rgbValues[(column * stride) + (row * 3)] = (byte) Math.Max(0, r1);
                                rgbValues[(column * stride) + (row * 3) + 1] = (byte) Math.Max(0, g1);
                                rgbValues[(column * stride) + (row * 3) + 2] = (byte) Math.Max(0, b1);
                            }
                            else
                            {
                                rgbValues[(column * stride) + (row * 3)] = 0;
                                rgbValues[(column * stride) + (row * 3) + 1] = 0;
                                rgbValues[(column * stride) + (row * 3) + 2] = 0;
                            }
                        }
                    }
                }

                store = rgbValues;
                bmp.UnlockBits(bmpData);
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