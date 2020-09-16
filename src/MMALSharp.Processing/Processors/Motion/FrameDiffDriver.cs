// <copyright file="FrameDiffDriver.cs" company="Techyian">
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
    /// A frame difference motion detection class which buffers a test frame and a current frame,
    /// and invokes an <see cref="IMotionAlgorithm"/> to detect motion.
    /// </summary>
    public class FrameDiffDriver : FrameAnalyser
    {
        // Some members are fields rather than properties for parallel processing performance reasons.
        // Array-based fields are threadsafe as long as multiple threads access unique array indices.

        /// <summary>
        /// Fully black pixels are skipped when comparing the test frame to the current frame.
        /// </summary>
        internal byte[] FrameMask;

        /// <summary>
        ///  This is the image we are comparing against new incoming frames.
        /// </summary>
        internal byte[] TestFrame;

        /// <summary>
        /// The number of pixels that differ in each cell between the test frame and current frame.
        /// </summary>
        internal int[] CellDiff;

        private Action _onDetect;
        private MotionConfig _motionConfig;
        private bool _fullTestFrame;
        private Stopwatch _testFrameAge;

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
            base.Apply(context);

            if (context.Eos)
            {
                // if zero bytes buffered, EOS is the end of a physical input video filestream
                if (this.WorkingData.Count > 0)
                {
                    if (!_fullTestFrame)
                    {
                        MMALLog.Logger.LogDebug("EOS reached for test frame.");

                        this.PrepareTestFrame();
                        _fullTestFrame = true;
                    }
                    else
                    {
                        MMALLog.Logger.LogDebug("Have full frame, invoking motion algorithm.");

                        // frameCounter++;
                        // frameTimer.Restart();

                        var detected = _motionConfig.MotionAlgorithm.DetectMotion(this, Metadata);

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

            _motionConfig.MotionAlgorithm.ResetAnalyser(this, this.Metadata);
        }

        /// <inheritdoc />
        protected override void ProcessFirstFrame(ImageContext context)
        {
            base.ProcessFirstFrame(context);

            this.PrepareTestFrame();

            CellDiff = new int[CellRect.Length];

            this.PrepareMask();

            // Provide a copy with raw full-frame defaults that the algorithm can safely store and reuse
            // if the algorithm is configured to output analysis frames to a capture handler's Apply method.
            var fullFrameContextTemplate = new ImageContext
            {
                Eos = true,
                IFrame = true,
                Resolution = new Resolution(Metadata.Width, Metadata.Height),
                Encoding = context.Encoding,
                PixelFormat = context.PixelFormat,
                Raw = context.Raw,
                Pts = null,
                Stride = Metadata.Stride
            };

            _motionConfig.MotionAlgorithm.FirstFrameCompleted(this, this.Metadata, fullFrameContextTemplate);
        }

        private void PrepareTestFrame()
        {
            this.TestFrame = this.WorkingData.ToArray();

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
                if (mask.Width != Metadata.Width || mask.Height != Metadata.Height || maskBpp != Metadata.Bpp)
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
