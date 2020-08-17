// <copyright file="CircularBufferCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
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
        private int _recordNumFrames;
        private int _numFramesRecorded;
        private bool _splitFrames;
        private bool _beginRecordFrame;        
        private IFrameAnalyser _analyser;
        private MotionConfig _motionConfig;

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
            else if (_recordNumFrames > 0)
            {
                // We will begin storing data immediately after we receive an EOS, this means we're sure to record frame data from the beginning of the stream.
                if (_beginRecordFrame)
                {
                    this.CurrentStream.Write(context.Data, 0, context.Data.Length);
                    this.Processed += context.Data.Length;

                    if (context.Eos)
                    {
                        // We've reached the end of the frame. Check if we want to create a new file and increment number of recorded frames.
                        _numFramesRecorded++;

                        if (_numFramesRecorded >= _recordNumFrames)
                        {
                            // Effectively stop recording individual frames at this point.
                            _beginRecordFrame = false;
                        }            
                    }
                }

                if (context.Eos && _numFramesRecorded < _recordNumFrames)
                {                    
                    _beginRecordFrame = true;

                    if (_splitFrames)
                    {
                        this.Split();
                    }
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

                    if (_receivedIFrame && this.Buffer.Size > 0)
                    {
                        // The buffer contains data.
                        MMALLog.Logger.LogInformation($"Buffer contains data. Writing {this.Buffer.Size} bytes.");
                        this.CurrentStream.Write(this.Buffer.ToArray(), 0, this.Buffer.Size);
                        this.Processed += this.Buffer.Size;
                        this.Buffer = new CircularBuffer<byte>(this.Buffer.Capacity);
                    }

                    if (_receivedIFrame)
                    {
                        // We need to have received an IFrame for the recording to be valid.
                        this.CurrentStream.Write(context.Data, 0, context.Data.Length);
                        this.Processed += context.Data.Length;
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

            // Not calling base method to stop data being written to the stream when not recording.
            this.ImageContext = context;
        }

        /// <inheritdoc/>
        public void ConfigureMotionDetection(MotionConfig config, Action onDetect)
        {
            _motionConfig = config;

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

        /// <inheritdoc/>
        public void EnableMotionDetection()
        {
            _shouldDetectMotion = true;

            MMALLog.Logger.LogInformation("Enabling motion detection.");
        }

        /// <inheritdoc/>
        public void DisableMotionDetection()
        {
            _shouldDetectMotion = false;

            (_analyser as FrameDiffAnalyser)?.ResetAnalyser();

            MMALLog.Logger.LogInformation("Disabling motion detection.");
        }

        /// <summary>
        /// Call to start recording to FileStream.
        /// </summary>
        /// <param name="initRecording">Optional Action to execute when recording starts, for example, to request an h.264 I-frame.</param>
        /// <param name="cancellationToken">When the token is canceled, <see cref="StopRecording"/> is called. If a token is not provided, the caller must stop the recording.</param>
        /// <param name="recordNumFrames">Optional number of full frames to record. If value is 0, <paramref name="cancellationToken"/> parameter will be used to manage timeout.</param>
        /// <param name="splitFrames">Optional flag to state full frames should be split to new files.</param>
        /// <returns>Task representing the recording process if a CancellationToken was provided, otherwise a completed Task.</returns>
        public async Task StartRecording(Action initRecording = null, CancellationToken cancellationToken = default, int recordNumFrames = 0, bool splitFrames = false)
        {
            if (this.CurrentStream == null)
            {
                throw new InvalidOperationException($"Recording unavailable, {nameof(CircularBufferCaptureHandler)} was not created with output-file arguments");
            }

            _recordToFileStream = true;
            _recordNumFrames = recordNumFrames;            
            _splitFrames = splitFrames;
            
            if (initRecording != null)
            {
                initRecording.Invoke();
            }

            if (cancellationToken != CancellationToken.None)
            {
                try
                {
                    await cancellationToken.AsTask().ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    // normal, but capture here because we may be running in the async void lambda (onDetect)
                }

                StopRecording();
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
            _beginRecordFrame = false;
            _recordNumFrames = 0;
            _numFramesRecorded = 0;
            _splitFrames = false;

            (_analyser as FrameDiffAnalyser)?.ResetAnalyser();
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
    }
}
