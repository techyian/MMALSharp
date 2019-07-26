// <copyright file="VideoStreamCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Diagnostics;
using System.IO;
using MMALSharp.Common;
using MMALSharp.Processors.Motion;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Processes the video data to a FileStream.
    /// </summary>
    public class VideoStreamCaptureHandler : FileStreamCaptureHandler, IMotionVectorCaptureHandler, IVideoCaptureHandler
    {
        public MotionType MotionType { get; set; }

        protected FileStream MotionVectorStore { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="VideoStreamCaptureHandler"/> class with the specified directory and filename extension.
        /// </summary>
        /// <param name="directory">The directory to save captured videos.</param>
        /// <param name="extension">The filename extension for saving files.</param>
        public VideoStreamCaptureHandler(string directory, string extension)
            : base(directory, extension) { }

        /// <summary>
        /// Creates a new instance of the <see cref="VideoStreamCaptureHandler"/> class with the specified file path.
        /// </summary>
        /// <param name="fullPath">The absolute full path to save captured data to.</param>
        public VideoStreamCaptureHandler(string fullPath)
            : base(fullPath) { }

        /// <summary>
        /// Splits the current file by closing the current stream and opening a new one.
        /// </summary>
        public void Split() => this.NewFile();

        public override void Process(byte[] data, bool eos)
        {
            base.Process(data, eos);

            if (this.MotionType == MotionType.MotionVector && this.MotionVectorStore != null)
            {
                // TODO: Process inline motion vector to FileStream.
            }
        }

        public void InitialiseMotionStore(FileStream stream)
        {
            this.MotionVectorStore = stream;
        }
    }
}
