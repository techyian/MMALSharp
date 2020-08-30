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

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Represents a capture handler working as a circular buffer.
    /// </summary>
    public sealed class CircularBufferCaptureHandler : VideoStreamCaptureHandler
    {
        private bool _recordToFileStream;
        private int _bufferSize;
        private bool _receivedIFrame;

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
                    _receivedIFrame = context.IFrame;
                }

                if (this.Buffer.Size > 0)
                {
                    // The buffer contains data.
                    if (this.CurrentStream != null && this.CurrentStream.CanWrite)
                    {
                        this.CurrentStream.Write(this.Buffer.ToArray(), 0, this.Buffer.Size);
                    }

                    this.Processed += this.Buffer.Size;
                    this.Buffer = new CircularBuffer<byte>(this.Buffer.Capacity);
                }

                if (this.CurrentStream != null && this.CurrentStream.CanWrite)
                {
                    this.CurrentStream.Write(context.Data, 0, context.Data.Length);
                }

                this.Processed += context.Data.Length;
            }

            // Not calling base method to stop data being written to the stream when not recording.
            this.ImageContext = context;
        }

        /// <summary>
        /// Call to start recording to FileStream.
        /// </summary>
        /// <param name="initRecording">Optional Action to execute when recording starts, for example, to request an h.264 I-frame.</param>
        /// <param name="cancellationToken">When the token is canceled, <see cref="StopRecording"/> is called. If a token is not provided, the caller must stop the recording.</param>
        /// <returns>Task representing the recording process if a CancellationToken was provided, otherwise a completed Task.</returns>
        public async Task StartRecording(Action initRecording = null, CancellationToken cancellationToken = default)
        {
            if (this.CurrentStream == null)
            {
                throw new InvalidOperationException($"Recording unavailable, {nameof(CircularBufferCaptureHandler)} was not created with output-file arguments");
            }

            _recordToFileStream = true;

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
