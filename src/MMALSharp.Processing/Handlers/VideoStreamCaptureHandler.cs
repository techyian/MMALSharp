// <copyright file="VideoStreamCaptureHandler.cs" company="Techyian">
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
    /// Processes the video data to a FileStream.
    /// </summary>
    public class VideoStreamCaptureHandler : FileStreamCaptureHandler, IMotionVectorCaptureHandler, IVideoCaptureHandler
    {
        /// <summary>
        /// The motion type associated with this VideoCaptureHandler
        /// </summary>
        public MotionType MotionType { get; set; }

        /// <summary>
        /// The data store for motion vectors.
        /// </summary>
        protected Stream MotionVectorStore { get; set; }

        /// <summary>
        /// Indicates whether this capture handler stores video timestamps.
        /// </summary>
        protected bool StoreVideoTimestamps { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="VideoStreamCaptureHandler"/> class without provisions for writing to a file. Supports
        /// subclasses in which file output is optional.
        /// </summary>
        public VideoStreamCaptureHandler()
            : base()
        {
            this.StoreVideoTimestamps = false;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="VideoStreamCaptureHandler"/> class with the specified directory and filename extension.
        /// </summary>
        /// <param name="directory">The directory to save captured videos.</param>
        /// <param name="extension">The filename extension for saving files.</param>
        /// <param name="storeTimestamps">Store video timestamps.</param>
        public VideoStreamCaptureHandler(string directory, string extension, bool storeTimestamps = false)
            : base(directory, extension)
        {
            this.StoreVideoTimestamps = storeTimestamps;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="VideoStreamCaptureHandler"/> class with the specified file path.
        /// </summary>
        /// <param name="fullPath">The absolute full path to save captured data to.</param>
        /// <param name="storeTimestamps">Store video timestamps.</param>
        public VideoStreamCaptureHandler(string fullPath, bool storeTimestamps = false)
            : base(fullPath)
        {
            this.StoreVideoTimestamps = storeTimestamps;
        }

        /// <inheritdoc />
        public override void Process(ImageContext context)
        {
            if (this.CurrentStream == null)
            {
                return;
            }

            base.Process(context);

            if (this.StoreVideoTimestamps && context.Pts.HasValue)
            {
                var str = $"{context.Pts / 1000}.{context.Pts % 1000:000}" + Environment.NewLine;
               
                File.AppendAllText($"{this.Directory}/{this.CurrentFilename}.pts", str);
            }
        }

        /// <summary>
        /// Splits the current file by closing the current stream and opening a new one.
        /// </summary>
        public virtual void Split() => this.NewFile();
        
        /// <summary>
        /// Used to set the current working motion vector store.
        /// </summary>
        /// <param name="stream">The <see cref="FileStream"/> to write to.</param>
        public void InitialiseMotionStore(Stream stream)
        {
            if (this.CurrentStream == null)
            {
                return;
            }

            this.MotionVectorStore = stream;
        }

        /// <summary>
        /// Responsible for storing the motion vector data to an output stream.
        /// </summary>
        /// <param name="data">The byte array containing the motion vector data.</param>
        public void ProcessMotionVectors(byte[] data)
        {
            if (this.MotionVectorStore != null)
            {
                if (this.MotionVectorStore.CanWrite)
                {
                    this.MotionVectorStore.Write(data, 0, data.Length);
                }
                else
                {
                    throw new IOException("Stream not writable.");
                }                    
            }
        }
    }
}
