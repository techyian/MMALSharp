// <copyright file="FormatRawImageExtension.cs" company="Techyian">
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
    /// An extension to convert raw <see cref="ImageContext"/> data to a bitmap <see cref="ImageFormat"/>.
    /// </summary>
    public static class FormatRawImageExtension
    {
        /// <summary>
        /// Converts the raw image data in an <see cref="ImageContext"/> object to a specified bitmap
        /// <see cref="ImageFormat"/> (such as <see cref="ImageFormat.Jpeg"/> or <see cref="ImageFormat.Png"/>).
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

            throw new Exception("Unsupported encoding / pixel format");
        }

    }
}
