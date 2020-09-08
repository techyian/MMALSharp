﻿// <copyright file="FrameDiffDriver.cs" company="Techyian">
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
using Microsoft.Extensions.Logging;
using MMALSharp.Common;
using MMALSharp.Common.Utility;

namespace MMALSharp.Processors.Motion
{
    /// <summary>
    /// A frame difference motion detection base class which buffers a test frame and a current frame,
    /// stores frame metrics, and invokes an <see cref="IMotionAlgorithm"/> to analyse full frames.
    /// </summary>
    public class FrameDiffDriver : FrameAnalyser
    {
        // Prefer fields over properties for parallel processing performance reasons.
        // Parallel processing references unique array indices, so arrays do not need
        // to be stored in the passed-by-value FrameDiffMetrics struct.

        // Various frame properties are collected when the first full frame is available.
        // A by-val copy of this struct is passed into the parallel processing algorithm. It is
        // private rather than internal to prevent accidental parallel reads against this copy.
        private FrameDiffMetrics _frameMetrics;

        private Action _onDetect;
        private bool _firstFrame = true;
        private MotionConfig _motionConfig;
        private bool _fullTestFrame;
        private ImageContext _imageContext;
        private Stopwatch _testFrameAge;

        /// <summary>
        /// Fully are skipped when comparing the test frame to the current frame.
        /// </summary>
        internal byte[] FrameMask;

        /// <summary>
        ///  This is the image we are comparing against new incoming frames.
        /// </summary>
        internal byte[] TestFrame;

        /// <summary>
        /// A byte array representation of the FrameAnalyser's own WorkingData object. Required
        /// to provide fast thread-safe access for parallel analysis.
        /// </summary>
        internal byte[] CurrentFrame;

        /// <summary>
        /// The number of pixels that differ in each cell between the test frame and current frame.
        /// </summary>
        internal int[] CellDiff;

        /// <summary>
        /// Represents the coordinates of each test cell for parallel analysis.
        /// </summary>
        internal Rectangle[] CellRect;

        /// <summary>
        /// Controls how many cells the frames are divided into. The result is a power of two of this
        /// value (so the default of 32 yields 1024 cells). These cells are processed in parallel. This
        /// should be a value that divides evenly into the X and Y resolutions of the motion stream.
        /// </summary>
        public int CellDivisor { get; set; } = 32;

        /// <summary>
        /// When true, the OnDetect Action will be invoked when motion is detected. Using this instead
        /// of the capture handler's Enable/DisableMotionDetection allows ongoing motion detection.
        /// </summary>
        public bool OnDetectEnabled { get; set; }

        /// <summary>
        /// Tracks the elapsed time since motion was last detected.
        /// </summary>
        public Stopwatch LastDetectionEvent { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="FrameDiffDriver"/>.
        /// </summary>
        /// <param name="config">The motion configuration object.</param>
        /// <param name="onDetect">A callback when changes are detected.</param>
        public FrameDiffDriver(MotionConfig config, Action onDetect)
        {
            _motionConfig = config;

            _onDetect = onDetect;
            this.OnDetectEnabled = _onDetect != null;

            _testFrameAge = new Stopwatch();
            this.LastDetectionEvent = new Stopwatch();
        }

        // These are used for temporary local performance-testing; uncomment these and four lines in
        // the Apply method below, and see the Dispose method at the end of FrameBufferCaptureHandler.
        // private Stopwatch frameTimer = new Stopwatch();
        // internal long frameCounter;
        // internal long totalElapsed;

        /// <inheritdoc />
        public override void Apply(ImageContext context)
        {
            _imageContext = context;

            base.Apply(context);

            if (context.Eos)
            {
                // if zero bytes buffered, EOS is the end of a physical input video filestream
                if (this.WorkingData.Count > 0)
                {
                    if (!_fullTestFrame)
                    {
                        MMALLog.Logger.LogDebug("EOS reached for test frame.");

                        _fullTestFrame = true;
                        this.PrepareTestFrame();
                    }
                    else
                    {
                        MMALLog.Logger.LogDebug("Have full frame, checking for changes.");

                        this.CurrentFrame = this.WorkingData.ToArray();

                        // frameCounter++;
                        // frameTimer.Restart();

                        var detected = _motionConfig.MotionAlgorithm.DetectMotion(this, _frameMetrics);

                        // frameTimer.Stop();
                        // totalElapsed += frameTimer.ElapsedMilliseconds;

                        if (detected)
                        {
                            this.LastDetectionEvent.Restart();

                            if(this.OnDetectEnabled)
                            {
                                _onDetect?.Invoke();
                            }
                        }

                        this.TryUpdateTestFrame();
                    }
                }
                else
                {
                    MMALLog.Logger.LogDebug("EOS reached, no working data buffered");
                }
            }
        }

        /// <summary>
        /// Resets the state of the buffers so that a new test frame is
        /// stored. Also resets any state in the motion detection algorithm.
        /// </summary>
        public void ResetAnalyser()
        {
            this.FullFrame = false;
            _fullTestFrame = false;
            this.WorkingData = new List<byte>();
            this.TestFrame = null;
            this.CurrentFrame = null;

            _testFrameAge.Reset();

            _motionConfig.MotionAlgorithm.ResetAnalyser(this, this._frameMetrics);
        }

        // Copies the working buffer to the test frame buffer, and if this is the
        // first test frame, collects various frame properties (size, BPP, etc).
        private void PrepareTestFrame()
        {
            this.TestFrame = this.WorkingData.ToArray();

            if (_firstFrame)
            {
                _firstFrame = false;

                // one-time collection of basic frame dimensions
                _frameMetrics.FrameWidth = _imageContext.Resolution.Width;
                _frameMetrics.FrameHeight = _imageContext.Resolution.Height;
                _frameMetrics.FrameBpp = this.GetBpp() / 8;
                _frameMetrics.FrameStride = _imageContext.Stride;

                // one-time setup of the diff cell parameters and arrays
                int indices = (int)Math.Pow(CellDivisor, 2);
                int cellWidth = _frameMetrics.FrameWidth / CellDivisor;
                int cellHeight = _frameMetrics.FrameHeight / CellDivisor;
                int i = 0;

                CellRect = new Rectangle[indices];
                CellDiff = new int[indices];

                for (int row = 0; row < CellDivisor; row++)
                {
                    int y = row * cellHeight;
                    for (int col = 0; col < CellDivisor; col++)
                    {
                        int x = col * cellWidth;
                        CellRect[i] = new Rectangle(x, y, cellWidth, cellHeight);
                        i++;
                    }
                }

                this.PrepareMask();

                // provide a copy (with raw full-frame defaults) that the algorithm can safely store and reuse
                var fullFrameContextTemplate = new ImageContext
                {
                    Eos = true,
                    IFrame = true,
                    Resolution = new Resolution(_frameMetrics.FrameWidth, _frameMetrics.FrameHeight),
                    Encoding = _imageContext.Encoding,
                    PixelFormat = _imageContext.PixelFormat,
                    Raw = _imageContext.Raw,
                    Pts = null,
                    Stride = _frameMetrics.FrameStride
                };

                _motionConfig.MotionAlgorithm.FirstFrameCompleted(this, this._frameMetrics, fullFrameContextTemplate);
            }

            if (_motionConfig.TestFrameInterval != TimeSpan.Zero)
            {
                _testFrameAge.Restart();
            }
        }

        // Periodically replaces the test frame with the current frame, which helps when a scene
        // changes over time (such as changing shadows throughout the day).
        private void TryUpdateTestFrame()
        {
            // Exit if the update interval has not elapsed, or if there was recent motion
            if (_motionConfig.TestFrameInterval == TimeSpan.Zero
                || _testFrameAge.Elapsed < _motionConfig.TestFrameInterval
                || (_motionConfig.TestFrameRefreshCooldown != TimeSpan.Zero
                && this.LastDetectionEvent.Elapsed < _motionConfig.TestFrameRefreshCooldown))
            {
                return;
            }

            MMALLog.Logger.LogDebug($"Updating test frame after {_testFrameAge.ElapsedMilliseconds} ms");
            this.PrepareTestFrame();
        }

        private int GetBpp()
        {
            PixelFormat format = default;

            // RGB16 doesn't appear to be supported by GDI?
            if (_imageContext.PixelFormat == MMALEncoding.RGB24)
            {
                return 24;
            }

            if (_imageContext.PixelFormat == MMALEncoding.RGB32 || _imageContext.PixelFormat == MMALEncoding.RGBA)
            {
                return 32;
            }

            if (format == default)
            {
                throw new Exception($"Unsupported pixel format: {_imageContext.PixelFormat}");
            }

            return 0;
        }

        private void PrepareMask()
        {
            if (string.IsNullOrWhiteSpace(_motionConfig.MotionMaskPathname))
            {
                return;
            }

            using (var fs = new FileStream(_motionConfig.MotionMaskPathname, FileMode.Open, FileAccess.Read))
            using (var mask = new Bitmap(fs))
            {
                // Verify it matches our frame dimensions
                var maskBpp = Image.GetPixelFormatSize(mask.PixelFormat) / 8;
                if (mask.Width != _frameMetrics.FrameWidth || mask.Height != _frameMetrics.FrameHeight || maskBpp != _frameMetrics.FrameBpp)
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
                    FrameMask = new byte[size];
                    Marshal.Copy(pNative, FrameMask, 0, size);
                }
                finally
                {
                    mask.UnlockBits(bmpData);
                }
            }
        }
    }
}
