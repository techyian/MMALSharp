// <copyright file="FrameDiffAnalyser.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MMALSharp.Common;
using MMALSharp.Common.Utility;

namespace MMALSharp.Processors.Motion
{
    /// <summary>
    /// The <see cref="FrameDiffAnalyser"/> is used to detect changes between two image frames.
    /// </summary>
    public class FrameDiffAnalyser : FrameAnalyser
    {
        private Stopwatch _testFrameAge;
        
        internal Action OnDetect { get; set; }

        /// <summary>
        /// Working storage for the Test Frame. This is the image we are comparing against new incoming frames.
        /// </summary>
        protected List<byte> TestFrame { get; set; }
        
        /// <summary>
        /// Indicates whether we have a full test frame.
        /// </summary>
        protected bool FullTestFrame { get; set; }

        /// <summary>
        /// The motion configuration object.
        /// </summary>
        protected MotionConfig MotionConfig { get; set; }

        /// <summary>
        /// The image metadata.
        /// </summary>
        protected ImageContext ImageContext { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="FrameDiffAnalyser"/>.
        /// </summary>
        /// <param name="config">The motion configuration object.</param>
        /// <param name="onDetect">A callback when changes are detected.</param>
        public FrameDiffAnalyser(MotionConfig config, Action onDetect)
        {
            this.TestFrame = new List<byte>();
            this.MotionConfig = config;
            this.OnDetect = onDetect;

            _testFrameAge = new Stopwatch();
        }

        /// <inheritdoc />
        public override void Apply(ImageContext context)
        {
            this.ImageContext = context;

            if (this.FullTestFrame)
            {
                MMALLog.Logger.LogDebug("Have full test frame.");
                
                // If we have a full test frame stored then we can start storing subsequent frame data to check.
                base.Apply(context);
            }
            else
            {
                this.TestFrame.AddRange(context.Data);

                if (context.Eos)
                {
                    this.FullTestFrame = true;

                    if(this.MotionConfig.TestFrameInterval != TimeSpan.Zero)
                    {
                        _testFrameAge.Restart();
                    }

                    MMALLog.Logger.LogDebug("EOS reached for test frame.");
                }
            }

            if (this.FullFrame && !TestFrameExpired())
            {
                MMALLog.Logger.LogDebug("Have full frame, checking for changes.");

                this.CheckForChanges(this.OnDetect);
            }
        }

        /// <summary>
        /// Resets the test and working frames this analyser is using.
        /// </summary>
        public void ResetAnalyser()
        {
            this.TestFrame = new List<byte>();
            this.WorkingData = new List<byte>();
            this.FullFrame = false;
            this.FullTestFrame = false;

            _testFrameAge.Reset();
        }

        private bool TestFrameExpired()
        {
            if(this.MotionConfig.TestFrameInterval == TimeSpan.Zero || _testFrameAge.Elapsed < this.MotionConfig.TestFrameInterval)
            {
                return false;
            }

            MMALLog.Logger.LogDebug("Have full frame, updating test frame.");

            this.TestFrame = this.WorkingData;
            this.WorkingData = new List<byte>();

            _testFrameAge.Restart();

            return true;
        }

        private void CheckForChanges(Action onDetect)
        {
            this.PrepareDifferenceImage(this.ImageContext, this.MotionConfig.Threshold);

            var diff = this.Analyse();
            
            if (diff >= this.MotionConfig.Threshold)
            {
                MMALLog.Logger.LogInformation($"Motion detected! Frame difference {diff}.");
                onDetect();
            }
        }

        private Bitmap LoadBitmap(MemoryStream stream)
        {
            if (this.ImageContext.Raw)
            {
                PixelFormat format = default;

                // RGB16 doesn't appear to be supported by GDI?
                if (this.ImageContext.PixelFormat == MMALEncoding.RGB24)
                {
                    format = PixelFormat.Format24bppRgb;
                }

                if (this.ImageContext.PixelFormat == MMALEncoding.RGB32)
                {
                    format = PixelFormat.Format32bppRgb;
                }

                if (this.ImageContext.PixelFormat == MMALEncoding.RGBA)
                {
                    format = PixelFormat.Format32bppArgb;
                }

                if (format == default)
                {
                    throw new Exception("Unsupported pixel format for Bitmap");
                }

                return new Bitmap(this.ImageContext.Resolution.Width, this.ImageContext.Resolution.Height, format);
            }
            
            return new Bitmap(stream);
        }

        private void InitBitmapData(BitmapData bmpData, byte[] data)
        {
            var pNative = bmpData.Scan0;
            Marshal.Copy(data, 0, pNative, data.Length);
        }

        private int Analyse()
        {
            using (var testMemStream = new MemoryStream(this.TestFrame.ToArray()))
            using (var currentMemStream = new MemoryStream(this.WorkingData.ToArray()))
            using (var testBmp = this.LoadBitmap(testMemStream))
            using (var currentBmp = this.LoadBitmap(currentMemStream))
            {
                var testBmpData = testBmp.LockBits(new Rectangle(0, 0, testBmp.Width, testBmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, testBmp.PixelFormat);
                var currentBmpData = currentBmp.LockBits(new Rectangle(0, 0, currentBmp.Width, currentBmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, currentBmp.PixelFormat);

                if (this.ImageContext.Raw)
                {
                    this.InitBitmapData(testBmpData, this.TestFrame.ToArray());
                    this.InitBitmapData(currentBmpData, this.WorkingData.ToArray());
                }

                var quadA = new Rectangle(0, 0, testBmpData.Width / 2, testBmpData.Height / 2);
                var quadB = new Rectangle(testBmpData.Width / 2, 0, testBmpData.Width / 2, testBmpData.Height / 2);
                var quadC = new Rectangle(0, testBmpData.Height / 2, testBmpData.Width / 2, testBmpData.Height / 2);
                var quadD = new Rectangle(testBmpData.Width / 2, testBmpData.Height / 2, testBmpData.Width / 2, testBmpData.Height / 2);

                int diff = 0;

                var bpp = Image.GetPixelFormatSize(testBmp.PixelFormat) / 8;

                var t1 = Task.Run(() =>
                {
                    diff += this.CheckDiff(quadA, testBmpData, currentBmpData, bpp, this.MotionConfig.Threshold);
                });
                var t2 = Task.Run(() =>
                {
                    diff += this.CheckDiff(quadB, testBmpData, currentBmpData, bpp, this.MotionConfig.Threshold);
                });
                var t3 = Task.Run(() =>
                {
                    diff += this.CheckDiff(quadC, testBmpData, currentBmpData, bpp, this.MotionConfig.Threshold);
                });
                var t4 = Task.Run(() =>
                {
                    diff += this.CheckDiff(quadD, testBmpData, currentBmpData, bpp, this.MotionConfig.Threshold);
                });

                Task.WaitAll(t1, t2, t3, t4);

                testBmp.UnlockBits(testBmpData);
                currentBmp.UnlockBits(currentBmpData);

                return diff;
            }
        }

        private void PrepareDifferenceImage(ImageContext context, int threshold)
        {
            BitmapData bmpData = null;
            IntPtr pNative = IntPtr.Zero;
            int bytes;
            byte[] store = null;

            using (var ms = new MemoryStream(context.Data))
            using (var bmp = this.LoadBitmap(ms))
            {
                bmpData = bmp.LockBits(new Rectangle(0, 0,
                        bmp.Width,
                        bmp.Height),
                    ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

                if (context.Raw)
                {
                    this.InitBitmapData(bmpData, ms.ToArray());
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
                    this.ApplyThreshold(quadA, bmpData, bpp, threshold);
                });
                var t2 = Task.Run(() =>
                {
                    this.ApplyThreshold(quadB, bmpData, bpp, threshold);
                });
                var t3 = Task.Run(() =>
                {
                    this.ApplyThreshold(quadC, bmpData, bpp, threshold);
                });
                var t4 = Task.Run(() =>
                {
                    this.ApplyThreshold(quadD, bmpData, bpp, threshold);
                });

                Task.WaitAll(t1, t2, t3, t4);

                if (context.Raw)
                {
                    store = new byte[bytes];
                    Marshal.Copy(pNative, store, 0, bytes);
                }

                bmp.UnlockBits(bmpData);
            }

            context.Data = store;
        }

        private void ApplyThreshold(Rectangle quad, BitmapData bmpData, int pixelDepth, int threshold)
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
                        var rgb1 = ptr1[(column * pixelDepth) + (row * stride)] +
                                   ptr1[(column * pixelDepth) + (row * stride) + 1] +
                                   ptr1[(column * pixelDepth) + (row * stride) + 2];

                        if (rgb1 > threshold)
                        {
                            ptr1[(column * pixelDepth) + (row * stride)] = 255;
                            ptr1[(column * pixelDepth) + (row * stride) + 1] = 255;
                            ptr1[(column * pixelDepth) + (row * stride) + 2] = 255;
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

        private int CheckDiff(Rectangle quad, BitmapData bmpData, BitmapData bmpData2, int pixelDepth, int threshold)
        {
            unsafe
            {
                var stride1 = bmpData.Stride;
                var stride2 = bmpData2.Stride;

                byte* ptr1 = (byte*)bmpData.Scan0;
                byte* ptr2 = (byte*)bmpData2.Scan0;
              
                int diff = 0;
                int lowestX = 0, highestX = 0, lowestY = 0, highestY = 0;

                for (int column = quad.X; column < quad.X + quad.Width; column++)
                {
                    for (int row = quad.Y; row < quad.Y + quad.Height; row++)
                    {
                        var rgb1 = ptr1[(column * pixelDepth) + (row * stride1)] +
                        ptr1[(column * pixelDepth) + (row * stride1) + 1] +
                        ptr1[(column * pixelDepth) + (row * stride1) + 2];

                        var rgb2 = ptr2[(column * pixelDepth) + (row * stride2)] +
                        ptr2[(column * pixelDepth) + (row * stride2) + 1] +
                        ptr2[(column * pixelDepth) + (row * stride2) + 2];
                        
                        if (rgb2 - rgb1 > threshold)
                        {
                            diff++;

                            if (row < lowestY || lowestY == 0)
                            {
                                lowestY = row;
                            }

                            if (row > highestY)
                            {
                                highestY = row;
                            }

                            if (column < lowestX || lowestX == 0)
                            {
                                lowestX = column;
                            }

                            if (column > highestX)
                            {
                                highestX = column;
                            }
                        }

                        // If the threshold has been exceeded, we want to exit from this method immediately for performance reasons.
                        if (diff > threshold)
                        {
                            break;
                        }
                    }

                    if (diff > threshold)
                    {
                        break;
                    }
                }

                return diff;
            }
        }
    }
}
