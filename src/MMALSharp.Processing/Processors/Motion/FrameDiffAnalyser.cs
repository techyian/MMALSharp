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
        // When true, PrepareTestFrame does additional start-up processing
        private bool _firstFrame = true;

        // Frame dimensions collected when the first full frame is complete
        private int _frameWidth;
        private int _frameHeight;
        private int _frameStride;
        private int _frameBpp;

        private byte[] _mask;
        private Stopwatch _testFrameAge;

        internal Action OnDetect { get; set; }

        /// <summary>
        ///  This is the image we are comparing against new incoming frames.
        /// </summary>
        protected byte[] TestFrame { get; set; }

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
            this.MotionConfig = config;
            this.OnDetect = onDetect;

            _testFrameAge = new Stopwatch();
        }

        /// <inheritdoc />
        public override void Apply(ImageContext context)
        {
            this.ImageContext = context;

            base.Apply(context);

            if (!this.FullTestFrame)
            {
                if (context.Eos)
                {
                    this.FullTestFrame = true;
                    this.PrepareTestFrame();
                    MMALLog.Logger.LogDebug("EOS reached for test frame.");
                }
            }
            else
            {
                MMALLog.Logger.LogDebug("Have full test frame.");

                if (this.FullFrame && !this.TestFrameExpired())
                {
                    MMALLog.Logger.LogDebug("Have full frame, checking for changes.");

                    this.CheckForChanges(this.OnDetect);
                }
            }
        }

        /// <summary>
        /// Resets the test and working frames this analyser is using.
        /// </summary>
        public void ResetAnalyser()
        {
            this.TestFrame = null;
            this.WorkingData = new List<byte>();
            this.FullFrame = false;
            this.FullTestFrame = false;

            _testFrameAge.Reset();
        }

        private void PrepareTestFrame()
        {
            if (_firstFrame)
            {
                // one-time collection of basic frame dimensions
                _frameWidth = this.ImageContext.Resolution.Width;
                _frameHeight = this.ImageContext.Resolution.Height;
                _frameBpp = this.GetBpp() / 8;
                _frameStride = this.ImageContext.Stride;

                this.TestFrame = this.WorkingData.ToArray();

                if(!string.IsNullOrWhiteSpace(this.MotionConfig.MotionMaskPathname))
                {
                    this.PrepareMask();
                }

                _firstFrame = false;
            }
            else
            {
                this.TestFrame = this.WorkingData.ToArray();
            }

            if (this.MotionConfig.TestFrameInterval != TimeSpan.Zero)
            {
                _testFrameAge.Restart();
            }
        }

        private int GetBpp()
        {
            PixelFormat format = default;

            // RGB16 doesn't appear to be supported by GDI?
            if (this.ImageContext.PixelFormat == MMALEncoding.RGB24)
            {
                return 24;
            }

            if (this.ImageContext.PixelFormat == MMALEncoding.RGB32 || this.ImageContext.PixelFormat == MMALEncoding.RGBA)
            {
                return 32;
            }

            if (format == default)
            {
                throw new Exception("Unsupported pixel format.");
            }

            return 0;
        }

        private void PrepareMask()
        {
            using (var fs = new FileStream(this.MotionConfig.MotionMaskPathname, FileMode.Open, FileAccess.Read))
            using (var mask = new Bitmap(fs))
            {
                // Verify it matches our frame dimensions
                var maskBpp = Image.GetPixelFormatSize(mask.PixelFormat) / 8;
                if (mask.Width != _frameWidth || mask.Height != _frameHeight || maskBpp != _frameBpp)
                {
                    throw new Exception("Motion-detection mask must match raw stream width, height, and format (bits per pixel)");
                }

                // Store the byte array
                BitmapData bmpData = null;
                try
                {
                    bmpData = mask.LockBits(new Rectangle(0, 0, mask.Width, mask.Height), ImageLockMode.ReadOnly, mask.PixelFormat);
                    var pNative = bmpData.Scan0;
                    int size = bmpData.Stride * mask.Height;
                    _mask = new byte[size];
                    Marshal.Copy(pNative, _mask, 0, size);
                }
                finally
                {
                    mask.UnlockBits(bmpData);
                }
            }
        }

        private bool TestFrameExpired()
        {
            if (this.MotionConfig.TestFrameInterval == TimeSpan.Zero || _testFrameAge.Elapsed < this.MotionConfig.TestFrameInterval)
            {
                return false;
            }

            MMALLog.Logger.LogDebug("Have full frame, updating test frame.");
            this.PrepareTestFrame();
            return true;
        }

        private void CheckForChanges(Action onDetect)
        {
            var diff = this.Analyse();

            if (diff >= this.MotionConfig.Threshold)
            {
                MMALLog.Logger.LogInformation($"Motion detected! Frame difference {diff}.");
                onDetect();
            }
        }

        private int Analyse()
        {
            var quadA = new Rectangle(0, 0, _frameWidth / 2, _frameHeight / 2);
            var quadB = new Rectangle(_frameWidth / 2, 0, _frameWidth / 2, _frameHeight / 2);
            var quadC = new Rectangle(0, _frameHeight / 2, _frameWidth / 2, _frameHeight / 2);
            var quadD = new Rectangle(_frameWidth / 2, _frameHeight / 2, _frameWidth / 2, _frameHeight / 2);

            var currentBytes = this.WorkingData.ToArray();

            int diff = 0;

            var t1 = Task.Run(() =>
            {
                diff += this.CheckDiff(quadA, currentBytes);
            });
            var t2 = Task.Run(() =>
            {
                diff += this.CheckDiff(quadB, currentBytes);
            });
            var t3 = Task.Run(() =>
            {
                diff += this.CheckDiff(quadC, currentBytes);
            });
            var t4 = Task.Run(() =>
            {
                diff += this.CheckDiff(quadD, currentBytes);
            });

            Task.WaitAll(t1, t2, t3, t4);

            return diff;
        }

        private int CheckDiff(Rectangle quad, byte[] currentFrame)
        {
            int diff = 0;

            for (int column = quad.X; column < quad.X + quad.Width; column++)
            {
                for (int row = quad.Y; row < quad.Y + quad.Height; row++)
                {
                    var index = (column * _frameBpp) + (row * _frameStride);

                    if(_mask != null)
                    {
                        var rgbMask = _mask[index] + _mask[index + 1] + _mask[index + 2];

                        if (rgbMask == 0)
                        {
                            continue;
                        }
                    }

                    var rgb1 = TestFrame[index] + TestFrame[index + 1] + TestFrame[index + 2];

                    var rgb2 = currentFrame[index] + currentFrame[index + 1] + currentFrame[index + 2];

                    if (rgb2 - rgb1 > MotionConfig.Threshold)
                    {
                        diff++;
                    }

                    // If the threshold has been exceeded, we want to exit from this method immediately for performance reasons.
                    if (diff > MotionConfig.Threshold)
                    {
                        return diff;
                    }
                }

                if (diff > MotionConfig.Threshold)
                {
                    return diff;
                }
            }

            return diff;
        }
    }
}
