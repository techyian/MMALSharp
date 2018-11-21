// <copyright file="StreamCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.IO;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Processors;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Processes the image data to a stream.
    /// </summary>
    /// <typeparam name="T">The <see cref="Stream"/> type.</typeparam>
    public abstract class StreamCaptureHandler<T> : ICaptureHandler 
        where T : Stream
    {
        /// <summary>
        /// A Stream instance that we can process image data to.
        /// </summary>
        public T CurrentStream { get; protected set; }
        
        /// <summary>
        /// The total size of data that has been processed by this capture handler.
        /// </summary>
        protected int Processed { get; set; }
        
        /// <summary>
        /// When overridden in a derived class, returns user provided image data.
        /// </summary>
        /// <param name="allocSize">The count of bytes to return at most in the <see cref="ProcessResult"/>.</param>
        /// <returns>A <see cref="ProcessResult"/> object containing the user provided image data.</returns>
        public virtual ProcessResult Process(uint allocSize)
        {
            return new ProcessResult();
        }

        /// <summary>
        /// Processes the data passed into this method to this class' Stream instance.
        /// </summary>
        /// <param name="data">The image data.</param>
        public virtual void Process(byte[] data)
        {
            this.Processed += data.Length;
                        
            if (this.CurrentStream.CanWrite)
                this.CurrentStream.Write(data, 0, data.Length);
            else
                throw new IOException("Stream not writable.");
        }

        /// <summary>
        /// Allows us to do any further processing once the capture method has completed.
        /// </summary>
        public virtual void PostProcess()
        {
            try
            {
                MMALLog.Logger.Info($"Successfully processed {Helpers.ConvertBytesToMegabytes(this.Processed)}.");
            }
            catch(Exception e)
            {
                MMALLog.Logger.Warn($"Something went wrong while processing stream: {e.Message}");                
            }
        }

        /// <inheritdoc />
        public void Manipulate(Action<IFrameProcessingContext> context, IImageContext imageContext) 
        {
            if (this.CurrentStream != null && this.CurrentStream.Length > 0)
            {
                byte[] arr = null;

                using (var ms = new MemoryStream())
                {
                    this.CurrentStream.Position = 0;
                    this.CurrentStream.CopyTo(ms);

                    arr = ms.ToArray();

                    context(new FrameProcessingContext(arr, imageContext));
                }

                using (var ms = new MemoryStream(arr))
                {
                    ms.WriteTo(this.CurrentStream);
                }
            }
        }

        /// <summary>
        /// Releases the underlying stream.
        /// </summary>
        public void Dispose()
        {
            CurrentStream?.Dispose();
        }
    }
}
