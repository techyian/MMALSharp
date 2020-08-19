﻿// <copyright file="FrameBufferCaptureHandler.cs" company="Techyian">
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
    /// A capture handler focused on high-speed frame buffering, either for on-demand snapshots
    /// or for motion detection.
    /// </summary>
    public class FrameBufferCaptureHandler : MemoryStreamCaptureHandler, IMotionCaptureHandler, IVideoCaptureHandler
    {
        private MotionConfig _motionConfig;
        private bool _detectingMotion;
        private FrameDiffAnalyser _motionAnalyser;

        private bool _waitForFullFrame = true;
        private bool _writeFrameRequested = false;

        /// <summary>
        /// Creates a new <see cref="FrameBufferCaptureHandler"/> optionally configured to write on-demand snapshots.
        /// </summary>
        /// <param name="directory">Target path for image files</param>
        /// <param name="extension">Extension for image files</param>
        /// <param name="fileDateTimeFormat">Filename DateTime formatting string</param>
        public FrameBufferCaptureHandler(string directory = "", string extension = "", string fileDateTimeFormat = "yyyy-MM-dd HH.mm.ss.ffff")
            : base()
        {
            this.FileDirectory = directory.TrimEnd('/');
            this.FileExtension = extension;
            this.FileDateTimeFormat = fileDateTimeFormat;
            Directory.CreateDirectory(this.FileDirectory);
        }

        /// <summary>
        /// Creates a new <see cref="FrameBufferCaptureHandler"/> configured for motion detection using a raw video stream.
        /// </summary>
        public FrameBufferCaptureHandler()
            : base()
        { }

        /// <summary>
        /// Target directory when <see cref="WriteFrame"/> is invoked without a directory argument.
        /// </summary>
        public string FileDirectory { get; set; } = string.Empty;

        /// <summary>
        /// File extension when <see cref="WriteFrame"/> is invoked without an extension argument.
        /// </summary>
        public string FileExtension { get; set; } = string.Empty;

        /// <summary>
        /// Filename format when <see cref="WriteFrame"/> is invoked without a format argument.
        /// </summary>
        public string FileDateTimeFormat { get; set; } = string.Empty;

        /// <summary>
        /// The filename (without extension) most recently created by <see cref="WriteFrame"/>, if any.
        /// </summary>
        public string MostRecentFilename { get; set; } = string.Empty;

        /// <summary>
        /// The full pathname to the most recent file created by <see cref="WriteFrame"/>, if any.
        /// </summary>
        public string MostRecentPathname { get; set; } = string.Empty;

        /// <inheritdoc />
        public MotionType MotionType { get; set; } = MotionType.FrameDiff;

        /// <summary>
        /// Outputs an image file to the specified location and filename.
        /// </summary>
        public void WriteFrame()
        {
            if (string.IsNullOrWhiteSpace(this.FileDirectory) || string.IsNullOrWhiteSpace(this.FileDateTimeFormat))
                throw new Exception($"The {nameof(FileDirectory)} and {nameof(FileDateTimeFormat)} must be set before calling {nameof(WriteFrame)}");

            _writeFrameRequested = true;
        }

        /// <inheritdoc />
        public override void Process(ImageContext context)
        {
            // guard against partial frame data at startup
            if (_waitForFullFrame)
            {
                _waitForFullFrame = !context.Eos;
                if (_waitForFullFrame)
                {
                    return;
                }
            }

            if (_detectingMotion)
            {
                _motionAnalyser.Apply(context);
            }

            // accumulate frame data in the underlying memory stream
            base.Process(context);

            if (context.Eos)
            {
                // write a full frame if a request is pending
                if (_writeFrameRequested)
                {
                    this.WriteStreamToFile();
                    _writeFrameRequested = false;
                }

                // reset the stream to begin the next frame
                this.CurrentStream.SetLength(0);
            }
        }

        /// <inheritdoc />
        public void ConfigureMotionDetection(MotionConfig config, Action onDetect)
        {
            _motionConfig = config;
            _motionAnalyser = new FrameDiffAnalyser(config, onDetect);
            this.EnableMotionDetection();
        }

        /// <inheritdoc />
        public void EnableMotionDetection()
        {
            _detectingMotion = true;
            _motionAnalyser?.ResetAnalyser();
        }

        /// <inheritdoc />
        public void DisableMotionDetection()
        {
            _detectingMotion = false;
        }

        /// <inheritdoc />
        public void Split()
        { } // Unused, but required to handle a video stream.

        private void WriteStreamToFile()
        {
            string directory = this.FileDirectory.TrimEnd('/');
            string filename = DateTime.Now.ToString(this.FileDateTimeFormat);
            string pathname = $"{directory}/{filename}.{this.FileExtension}";

            using (var fs = new FileStream(pathname, FileMode.Create, FileAccess.Write))
            {
                CurrentStream.WriteTo(fs);
            }

            this.MostRecentFilename = filename;
            this.MostRecentPathname = pathname;
        }
    }
}
