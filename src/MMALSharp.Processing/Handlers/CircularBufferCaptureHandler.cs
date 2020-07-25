// <copyright file="CircularBufferCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
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
        private bool _receivedIFrame;
        private Stopwatch _recordingElapsed;
        private IFrameAnalyser _analyser;
        private MotionConfig _motionConfig;
        private Action _onStopDetect;
                
        /// <summary>
        /// The circular buffer object responsible for storing image data.
        /// </summary>
        public CircularBuffer<byte> Buffer { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="CircularBufferCaptureHandler"/> class with the specified Circular buffer capacity without file output.
        /// </summary>
        /// <param name="bufferSize">The buffer's size.</param>
        public CircularBufferCaptureHandler(int bufferSize)
            : base()
        {
            _bufferSize = bufferSize;
            this.Buffer = new CircularBuffer<byte>(bufferSize);
        }

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
            }
            else
            {
                if (context.Encoding == MMALEncoding.H264)
                {
                    if (context.IFrame)
                    {
                        _receivedIFrame = true;
                    }
                    
                    if (_receivedIFrame)
                    {
                        // We need to have received an IFrame for the recording to be valid.
                        this.CurrentStream.Write(context.Data, 0, context.Data.Length);
                        this.Processed += context.Data.Length;
                    }

                    if (_receivedIFrame && this.Buffer.Size > 0)
                    {
                        // The buffer contains data.
                        MMALLog.Logger.LogInformation($"Buffer contains data. Writing {this.Buffer.Size} bytes.");
                        this.CurrentStream.Write(this.Buffer.ToArray(), 0, this.Buffer.Size);
                        this.Processed += this.Buffer.Size;
                        this.Buffer = new CircularBuffer<byte>(this.Buffer.Capacity);
                    }
                }
                else
                {
                    if (this.Buffer.Size > 0)
                    {
                        // The buffer contains data.
                        this.CurrentStream.Write(this.Buffer.ToArray(), 0, this.Buffer.Size);
                        this.Processed += this.Buffer.Size;
                        this.Buffer = new CircularBuffer<byte>(this.Buffer.Capacity);
                    }

                    this.CurrentStream.Write(context.Data, 0, context.Data.Length);
                    this.Processed += context.Data.Length;
                }
            }
            
            if (_shouldDetectMotion && !_recordToFileStream)
            {
                _analyser?.Apply(context);
            }

            this.CheckRecordingProgress();

            // Not calling base method to stop data being written to the stream when not recording.
            this.ImageContext = context;
        }

        /// <summary>
        /// Call to enable motion detection.
        /// </summary>
        /// <param name="config">The motion configuration.</param>
        /// <param name="onDetect">A callback for when motion is detected.</param>
        /// <param name="onStopDetect">An optional callback for when the record duration has passed.</param>
        public void ConfigureMotionDetection(MotionConfig config, Action onDetect, Action onStopDetect = null)
        {
            _motionConfig = config;
            _onStopDetect = onStopDetect;
            
            switch(this.MotionType)
            {
                case MotionType.FrameDiff:
                    _analyser = new FrameDiffAnalyser(config, onDetect);
                    break;

                case MotionType.MotionVector:
                    // TODO Motion vector analyser
                    break;
            }

            this.EnableMotionDetection();
        }

        /// <summary>
        /// Enables motion detection. When configured, this will instruct the capture handler to detect motion.
        /// </summary>
        public void EnableMotionDetection()
        {
            _shouldDetectMotion = true;

            MMALLog.Logger.LogInformation("Enabling motion detection.");
        }

        /// <summary>
        /// Disables motion detection. When configured, this will instruct the capture handler not to detect motion.
        /// </summary>
        public void DisableMotionDetection()
        {
            _shouldDetectMotion = false;

            (_analyser as FrameDiffAnalyser)?.ResetAnalyser();

            MMALLog.Logger.LogInformation("Disabling motion detection.");
        }

        /// <summary>
        /// Call to start recording to FileStream.
        /// </summary>        
        public void StartRecording()
        {
            if (this.CurrentStream == null)
            {
                throw new InvalidOperationException($"Recording unavailable, {nameof(CircularBufferCaptureHandler)} was not created with output-file arguments");
            }

            _recordToFileStream = true;

            // If this handler isn't doing motion detection, or no recording
            // duration is defined, leave the stopwatch null which skips all
            // of the stop-recording logic in CheckRecordingProgress.
            if(_motionConfig == null || _motionConfig.RecordDuration == TimeSpan.Zero)
            {
                _recordingElapsed = null;
            }
            else
            {
                _recordingElapsed = new Stopwatch();
                _recordingElapsed.Start();
            }
        }

        /// <summary>
        /// Call to stop recording to FileStream.
        /// </summary>
        public void StopRecording()
        {
            if (this.CurrentStream == null)
            {
                throw new InvalidOperationException($"Recording unavailable, {nameof(CircularBufferCaptureHandler)} was not created with output-file arguments");
            }

            MMALLog.Logger.LogInformation("Stop recording.");

            _recordToFileStream = false;
            _receivedIFrame = false;
            _recordingElapsed?.Stop();
            _recordingElapsed = null;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            this.CurrentStream?.Dispose();
        }

        /// <inheritdoc />
        public override string TotalProcessed()
        {
            return $"{this.Processed}";
        }

        private void CheckRecordingProgress()
        {
            if (_recordingElapsed != null && _motionConfig != null)
            {
                if (_recordingElapsed.Elapsed >= _motionConfig.RecordDuration)
                {
                    if (_onStopDetect != null)
                    {
                        _onStopDetect();
                    }
                    else
                    {
                        this.StopRecording();
                    }

                    if (_analyser is FrameDiffAnalyser)
                    {
                        var fdAnalyser = _analyser as FrameDiffAnalyser;
                        fdAnalyser?.ResetAnalyser();
                    }
                }
            }
        }
    }
}
