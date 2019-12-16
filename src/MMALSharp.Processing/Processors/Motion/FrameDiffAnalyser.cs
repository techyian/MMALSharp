// <copyright file="FrameDiffAnalyser.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Processors.Effects;

namespace MMALSharp.Processors.Motion
{
    /// <summary>
    /// The <see cref="FrameDiffAnalyser"/> is used to detect changes between two image frames.
    /// </summary>
    public class FrameDiffAnalyser : FrameAnalyser
    {
        internal Action OnDetect { get; set; }

        /// <summary>
        /// Working storage for the Test Frame. This is the original static image we are comparing to.
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
        /// Creates a new instance of <see cref="FrameDiffAnalyser"/>.
        /// </summary>
        /// <param name="config">The motion configuration object.</param>
        /// <param name="onDetect">A callback when changes are detected.</param>
        /// <param name="imageContext">The image metadata.</param>
        public FrameDiffAnalyser(MotionConfig config, Action onDetect, IImageContext imageContext)
            : base(imageContext)
        {
            this.TestFrame = new List<byte>();
            this.MotionConfig = config;
            this.OnDetect = onDetect;
        }

        /// <inheritdoc />
        public override void Apply(byte[] data, bool eos)
        {
            if (this.FullTestFrame)
            {
                MMALLog.Logger.LogInformation("Have full test frame");
                
                // If we have a full test frame stored then we can start storing subsequent frame data to check.
                base.Apply(data, eos);
            }
            else
            {
                this.TestFrame.AddRange(data);

                if (eos)
                {
                    this.FullTestFrame = true;
                    MMALLog.Logger.LogInformation("EOS reached for test frame. Applying edge detection.");

                    // We want to apply Edge Detection to the test frame to make it easier to detect changes.
                    var edgeDetection = new EdgeDetection(this.MotionConfig.Sensitivity);
                    this.ImageContext.Data = this.TestFrame.ToArray();
                    edgeDetection.ApplyConvolution(edgeDetection.Kernel, EdgeDetection.KernelWidth, EdgeDetection.KernelHeight, this.ImageContext);
                }
            }

            if (this.FullFrame)
            {
                MMALLog.Logger.LogInformation("Have full frame, checking for changes.");

                // TODO: Check for changes.
                this.CheckForChanges(this.OnDetect);
            }
        }
        
        private void CheckForChanges(Action onDetect)
        {
            var edgeDetection = new EdgeDetection(EDStrength.Medium);
            this.ImageContext.Data = this.WorkingData.ToArray();
            edgeDetection.ApplyConvolution(EdgeDetection.MediumStrengthKernel, 3, 3, this.ImageContext);
            var diff = this.Analyse();

            MMALLog.Logger.LogInformation($"Diff size: {diff}");

            if (diff >= this.MotionConfig.Threshold)
            {
                MMALLog.Logger.LogInformation("Motion detected!");
                onDetect();
            }
        }

        private Bitmap LoadBitmap(MemoryStream stream)
        {
            if (this.ImageContext.Raw)
            {
                return new Bitmap(this.ImageContext.Resolution.Width, this.ImageContext.Resolution.Height, this.ImageContext.PixelFormat);
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
                        
                        if (rgb2 > rgb1 + threshold)
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
                    }
                }

                return diff;
            }
        }
    }
}
