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
        private bool _recordToFileStream;
        private int _bufferSize;
        private bool _shouldDetectMotion;
        private Stopwatch _recordingElapsed;
        private IFrameAnalyser _analyser;
        private MotionConfig _config;
                
        /// <summary>
        /// The circular buffer object responsible for storing image data.
        /// </summary>
        public CircularBuffer<byte> Buffer { get; }
        
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
        public override void Process(ImageContext context)
        {
            if (!_recordToFileStream)
            {
                for (var i = 0; i < context.Data.Length; i++)
                {
                    this.Buffer.PushBack(context.Data[i]);
                }

                this.CheckRecordingProgress();
            }
            else
            {
                this.CurrentStream.Write(context.Data, 0, context.Data.Length);
                this.Processed += context.Data.Length;
            }

            if (_shouldDetectMotion && !_recordToFileStream)
            {
                _analyser.Apply(context);
            }
        }

        public override void Split()
        {
            if (this.ImageContext.Encoding == MMALEncoding.H264)
            {

            }
            else
            {
                base.Split();
            }
        }

        /// <summary>
        /// Call to enable motion detection.
        /// </summary>
        /// <param name="config">The motion configuration.</param>
        /// <param name="onDetect">A callback for when motion is detected.</param>
        public void DetectMotion(MotionConfig config, Action onDetect)
        {
            _config = config;
            _shouldDetectMotion = true;

            if (this.MotionType == MotionType.FrameDiff)
            {
                _analyser = new FrameDiffAnalyser(config, onDetect);
            }
            else
            {
                // TODO: Motion vector analyser
            }
        }

        /// <summary>
        /// Call to start recording to FileStream.
        /// </summary>        
        public void StartRecording()
        {
            _recordToFileStream = true;            
            _recordingElapsed = new Stopwatch();
            _recordingElapsed.Start();
            
            // Write what's currently in the Circular buffer to the FileStream.
            this.CurrentStream.Write(this.Buffer.ToArray(), 0, this.Buffer.Size);
            this.Processed += this.Buffer.Size;
        }

        /// <summary>
        /// Call to stop recording to FileStream.
        /// </summary>
        public void StopRecording()
        {
            _recordToFileStream = false;
            _recordingElapsed?.Stop();
            _recordingElapsed?.Reset();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            if (_shouldDetectMotion)
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
            if (_recordingElapsed != null && _config != null)
            {
                if (_recordingElapsed.Elapsed >= _config.RecordDuration.TimeOfDay)
                {
                    _recordToFileStream = false;
                    _recordingElapsed.Stop();
                    _recordingElapsed.Reset();
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
