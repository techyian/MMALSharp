// <copyright file="StreamCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.IO;
using Microsoft.Extensions.Logging;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Processors;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Processes the image data to a stream.
    /// </summary>
    /// <typeparam name="T">The <see cref="Stream"/> type.</typeparam>
    public abstract class StreamCaptureHandler<T> : OutputCaptureHandler
        where T : Stream
    {
        /// <summary>
        /// A Stream instance that we can process image data to.
        /// </summary>
        public T CurrentStream { get; protected set; }

        /// <inheritdoc />
        public override void Process(ImageContext context)
        {
            this.Processed += context.Data.Length;
                        
            if (this.CurrentStream.CanWrite)
                this.CurrentStream.Write(context.Data, 0, context.Data.Length);
            else
                throw new IOException("Stream not writable.");

            base.Process(context);
        }

        /// <inheritdoc />
        public override void PostProcess()
        {
            try
            {
                if (this.CurrentStream != null && this.CurrentStream.CanRead && this.CurrentStream.Length > 0)
                {
                    if (this.OnManipulate != null && this.ImageContext != null)
                    {
                        byte[] arr = null;

                        using (var ms = new MemoryStream())
                        {
                            this.CurrentStream.Position = 0;
                            this.CurrentStream.CopyTo(ms);

                            arr = ms.ToArray();

                            this.ImageContext.Data = arr;
                            this.ImageContext.StoreFormat = this.StoreFormat;
                            
                            MMALLog.Logger.LogDebug("Applying image processing.");

                            this.OnManipulate(new FrameProcessingContext(this.ImageContext));
                        }

                        using (var ms = new MemoryStream(this.ImageContext.Data))
                        {
                            this.CurrentStream.SetLength(0);
                            this.CurrentStream.Position = 0;
                            ms.CopyTo(this.CurrentStream);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                MMALLog.Logger.LogWarning($"Something went wrong while processing stream: {e.Message}. {e.InnerException?.Message}. {e.StackTrace}");
            }
        }

        /// <inheritdoc />
        public override string TotalProcessed()
        {
            return $"{Helpers.ConvertBytesToMegabytes(this.Processed)}";
        }

        /// <summary>
        /// Releases the underlying stream.
        /// </summary>
        public override void Dispose()
        {
            MMALLog.Logger.LogInformation($"Successfully processed {Helpers.ConvertBytesToMegabytes(this.Processed)}.");
            CurrentStream?.Dispose();
        }
    }
}
