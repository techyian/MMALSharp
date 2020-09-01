// <copyright file="MotionAnalysisCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.IO;
using MMALSharp.Common;
using MMALSharp.Processors.Motion;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Writes a raw RGB image stream consisting of frames modified by an <see cref="IMotionAlgorithm"/>.
    /// </summary>
    public class MotionAnalysisCaptureHandler : IOutputCaptureHandler, IVideoCaptureHandler
    {
        private FrameDiffDriver _driver;
        private FileStream _stream;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pathname">The raw RGB image stream pathname to create</param>
        /// <param name="config">The motion configuration object.</param>
        /// <param name="onDetect">A callback when changes are detected.</param>
        public MotionAnalysisCaptureHandler(string pathname, MotionConfig config, Action onDetect = null)
        {
            _stream = new FileStream(pathname, FileMode.Create, FileAccess.Write);
            config.MotionAlgorithm.EnableAnalysis(WriteProcessedFrame);
            _driver = new FrameDiffDriver(config, onDetect);
        }

        /// <inheritdoc />
        public void Process(ImageContext context)
        {
            _driver.Apply(context);
        }

        /// <summary>
        /// Outputs a raw RGB frame buffer to the file stream
        /// </summary>
        /// <param name="fullFrame">The raw RGB frame buffer to store</param>
        public void WriteProcessedFrame(byte[] fullFrame)
        {
            if (_stream != null && _stream.CanWrite)
            {
                _stream.Write(fullFrame, 0, fullFrame.Length);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _stream?.Flush();
            _stream?.Close();
            _stream?.Dispose();
        }

        // unused, required by IOutputCaptureHandler
        public void PostProcess() { }
        public string TotalProcessed() => string.Empty;

        // unused, required by IVideoCaptureHandler
        public void Split() { }
    }
}
