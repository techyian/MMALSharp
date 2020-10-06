// <copyright file="ImageContextFormattingExtension.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace MMALSharp.Common.Utility
{
    /// <summary>
    /// Extensions to convert <see cref="ImageContext"/> data between raw formats and bitmap <see cref="ImageFormat"/>s.
    /// </summary>
    public static class ImageContextFormattingExtension
    {
        /// <summary>
        /// Converts the formatted image data in an <see cref="ImageContext"/> object to a bitmap
        /// suitable for pixel-by-pixel processing.
        /// </summary>
        /// <param name="context">The image to convert.</param>
        public static void ToBitmap(this ImageContext context)
        {
            if(context.Raw)
            {
                return;
            }

            using(var ms = new MemoryStream(context.Data))
            {
                using(var bitmap = new Bitmap(ms))
                {
                    BitmapData bmpData = null;
                    try
                    {
                        bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                        var ptr = bmpData.Scan0;
                        int size = bmpData.Stride * bitmap.Height;
                        context.Data = new byte[size];
                        Marshal.Copy(ptr, context.Data, 0, size);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bmpData);
                    }
                }
            }
        }

        /// <summary>
        /// Converts the raw image data in an <see cref="ImageContext"/> object to a specified bitmap
        /// <see cref="ImageFormat"/> (such as <see cref="ImageFormat.Jpeg"/> or <see cref="ImageFormat.Png"/>)
        /// based on the value of the StoreFormat field.
        /// </summary>
        /// <param name="context">The image to convert. Must be raw data.</param>
        public static void FormatRawImage(this ImageContext context)
        {
            if (!context.Raw)
            {
                throw new Exception("ImageContext does not contain raw data");
            }

            if (context.StoreFormat == null)
            {
                throw new Exception("ImageContext.StoreFormat does not define a target ImageFormat");
            }

            var pixfmt = MMALEncodingToPixelFormat(context.PixelFormat);

            using (var bitmap = new Bitmap(context.Resolution.Width, context.Resolution.Height, pixfmt))
            {
                BitmapData bmpData = null;
                try
                {
                    bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                    var ptr = bmpData.Scan0;
                    int size = bmpData.Stride * bitmap.Height;
                    var data = context.Data;
                    Marshal.Copy(data, 0, ptr, size);
                }
                finally
                {
                    bitmap.UnlockBits(bmpData);
                }

                using(var ms = new MemoryStream())
                {
                    bitmap.Save(ms, context.StoreFormat);
                    context.Data = new byte[ms.Length];
                    Array.Copy(ms.ToArray(), 0, context.Data, 0, ms.Length);
                }
            }
        }

        private static PixelFormat MMALEncodingToPixelFormat(MMALEncoding encoding)
        {
            if(encoding == MMALEncoding.RGB24)
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
    }
}
