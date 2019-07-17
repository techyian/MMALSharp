// <copyright file="VideoStreamCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.IO;
using MMALSharp.Processors.Motion;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Processes the video data to a FileStream.
    /// </summary>
    public class VideoStreamCaptureHandler : FileStreamCaptureHandler, IMotionCaptureHandler
    {
        private bool _isRecordingMotion;

        protected bool ShouldDetectMotion { get; set; }

        protected Action OnDetect { get; set; }

        protected MotionConfig MotionConfig { get; set; }

        protected FileStream MotionRecording { get; set; }

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

        public override void Process(byte[] data)
        {
            base.Process(data);

            if (this.ShouldDetectMotion)
            {
                //this.Analyse(context =>
                //{
                //    context.Apply(new SharpenProcessor());
                //}, new ImageContext(MMALCameraConfig.StillResolution));
            }                        
        }

        public void DetectMotion(MotionConfig config, Action onDetect)
        {
            this.ShouldDetectMotion = true;            
            this.MotionConfig = config;
            this.OnDetect = onDetect;
        }

        public void StartRecording(string filepath)
        {
            _isRecordingMotion = true;
            this.MotionRecording = File.Create(filepath);
            this.MotionConfig.RecordingElapsed.Start();
        }

        public override void Dispose()
        {
            if (this.ShouldDetectMotion && this.MotionRecording != null)
            {
                this.MotionRecording.Dispose();
            }

            base.Dispose();
        }
    }
}
