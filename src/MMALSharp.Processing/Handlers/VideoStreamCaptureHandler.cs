// <copyright file="VideoStreamCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.IO;
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
        
        /// <summary>
        /// Used to set the current working motion vector store.
        /// </summary>
        /// <param name="stream">The <see cref="FileStream"/> to write to.</param>
        public void InitialiseMotionStore(Stream stream)
        {
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
