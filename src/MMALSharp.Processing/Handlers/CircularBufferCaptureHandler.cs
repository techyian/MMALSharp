// <copyright file="CircularBufferCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Diagnostics;
using System.IO;
using MMALSharp.Common;
using MMALSharp.Processors;
using MMALSharp.Processors.Motion;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Represents a capture handler working as a circular buffer.
    /// </summary>
    public sealed class CircularBufferCaptureHandler : VideoStreamCaptureHandler, IMotionCaptureHandler
    {
        private bool _isRecordingMotion;
        private int _bufferSize;
                
        private CircularBuffer<byte> Buffer { get; }

        private bool ShouldDetectMotion { get; set; }

        private Stopwatch RecordingElapsed { get; set; }

        private IFrameAnalyser Analyser { get; set; }

        private MotionConfig Config { get; set; }
                
        /// <summary>
        /// Creates a new instance of the <see cref="CircularBufferCaptureHandler"/> class with the specified Circular buffer capacity and directory/extension of the working file.
        /// </summary>
        /// <param name="bufferSize">The buffer's size.</param>
        /// <param name="directory">The directory to save captured videos.</param>
        /// <param name="extension">The filename extension for saving files.</param>
        public CircularBufferCaptureHandler(int bufferSize, string directory, string extension)
            : base(directory, extension) 
        {
            _bufferSize = bufferSize;
            this.Buffer = new CircularBuffer<byte>(bufferSize);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CircularBufferCaptureHandler"/> class with the specified Circular buffer capacity and working file path.
        /// </summary>
        /// <param name="bufferSize">The buffer's size.</param>
        /// <param name="fullPath">The absolute full path to save captured data to.</param>
        public CircularBufferCaptureHandler(int bufferSize, string fullPath)
            : base(fullPath) 
        {
            _bufferSize = bufferSize;
            this.Buffer = new CircularBuffer<byte>(bufferSize);
        }
       
        /// <inheritdoc />
        public override void Process(byte[] data, bool eos)
        {
            if (!_isRecordingMotion)
            {
                for (var i = 0; i < data.Length; i++)
                {
                    this.Buffer.PushBack(data[i]);
                }

                this.CheckRecordingProgress();
            }
            else
            {
                this.CurrentStream.Write(data, 0, data.Length);
                this.Processed += data.Length;
            }

            if (this.ShouldDetectMotion && !_isRecordingMotion)
            {
                this.Analyser.Apply(data, eos);
            }
        }

        /// <summary>
        /// Call to enable motion detection.
        /// </summary>
        /// <param name="config">The motion configuration.</param>
        /// <param name="onDetect">A callback for when motion is detected.</param>
        /// <param name="imageContext">The frame metadata.</param>
        public void DetectMotion(MotionConfig config, Action onDetect, IImageContext imageContext)
        {
            this.Config = config;
            this.ShouldDetectMotion = true;

            if (this.MotionType == MotionType.FrameDiff)
            {
                this.Analyser = new FrameDiffAnalyser(config, onDetect, imageContext);
            }
            else
            {
                // TODO: Motion vector analyser
            }
        }

        /// <summary>
        /// Call to start recording.
        /// </summary>        
        public void StartRecording()
        {
            _isRecordingMotion = true;            
            this.RecordingElapsed = new Stopwatch();
            this.RecordingElapsed.Start();
            
            this.CurrentStream.Write(this.Buffer.ToArray(), 0, this.Buffer.Size);
            this.Processed += this.Buffer.Size;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            if (this.ShouldDetectMotion)
            {
                this.CurrentStream?.Dispose();
            }
        }

        /// <inheritdoc />
        public override string TotalProcessed()
        {
            return $"{this.Processed}";
        }

        private void CheckRecordingProgress()
        {
            if (this.RecordingElapsed != null)
            {
                if (this.RecordingElapsed.Elapsed >= this.Config.RecordDuration.TimeOfDay)
                {
                    _isRecordingMotion = false;
                    this.RecordingElapsed.Stop();
                    this.RecordingElapsed.Reset();
                }
            }
        }

        private string ProvideFilename(string directory, string extension)
        {
            var dir = directory.TrimEnd('/');
            var ext = extension.TrimStart('.');

            System.IO.Directory.CreateDirectory(dir);

            var now = DateTime.Now.ToString("dd-MMM-yy HH-mm-ss");

            int i = 1;

            var fileName = $"{dir}/{now}.{ext}";

            while (File.Exists(fileName))
            {
                fileName = $"{dir}/{now} {i}.{ext}";
                i++;
            }

            return fileName;
        }
    }
}
