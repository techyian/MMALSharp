// <copyright file="MMALQueueImpl.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using Microsoft.Extensions.Logging;
using MMALSharp.Common.Utility;
using MMALSharp.Native;

namespace MMALSharp
{
    /// <summary>
    /// Represents a queue of buffer headers.
    /// </summary>
    public unsafe class MMALQueueImpl : MMALObject, IBufferQueue
    {
        /// <summary>
        /// Native pointer to the buffer header queue this object represents.
        /// </summary>
        public MMAL_QUEUE_T* Ptr { get; }

        /// <summary>
        /// Creates a new instance of <see cref="MMALQueueImpl"/>.
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        public MMALQueueImpl(MMAL_QUEUE_T* ptr)
        {
            this.Ptr = ptr;
        }

        /// <summary>
        /// Get a new buffer from this queue.
        /// </summary>
        /// <returns>A new managed buffer header object.</returns>
        public IBuffer GetBuffer()
        {
            var ptr = MMALQueue.mmal_queue_get(this.Ptr);

            if (!this.CheckState())
            {
                MMALLog.Logger.LogWarning("Buffer retrieved null.");
                return null;
            }

            return new MMALBufferImpl(ptr);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            MMALLog.Logger.LogDebug("Disposing queue.");
            this.Destroy();
            base.Dispose();
        }

        /// <summary>
        /// Returns the pointer address of this queue.
        /// </summary>
        /// <returns>The pointer address of this queue.</returns>
        public override string ToString()
        {
            return $"Ptr address: {((IntPtr)this.Ptr).ToString()}";
        }

        /// <inheritdoc />
        public override bool CheckState()
        {
            return this.Ptr != null && (IntPtr)this.Ptr != IntPtr.Zero;
        }

        /// <summary>
        /// Get the number of buffer headers currently in this queue.
        /// </summary>
        /// <returns>The number of buffers currently in this queue.</returns>
        public uint QueueLength()
        {
            var length = MMALQueue.mmal_queue_length(this.Ptr);
            return length;
        }

        /// <summary>
        /// Waits (blocking) for a buffer header to be available in the queue and allocates it.
        /// </summary>
        /// <returns>The buffer header.</returns>
        public IBuffer Wait()
        {
            return new MMALBufferImpl(MMALQueue.mmal_queue_wait(this.Ptr));
        }

        /// <summary>
        /// Waits (blocking) for a buffer header to be available in the queue and allocates it. This is the same as a wait, except that it will abort in case of timeout.
        /// </summary>
        /// <param name="waitms">Number of milliseconds to wait before aborting.</param>
        /// <returns>The buffer header.</returns>
        public IBuffer TimedWait(int waitms)
        {
            return new MMALBufferImpl(MMALQueue.mmal_queue_timedwait(this.Ptr, waitms));
        }

        /// <summary>
        /// Puts the buffer header back into this queue.
        /// </summary>
        /// <param name="buffer">The buffer header.</param>
        public void Put(IBuffer buffer)
        {
            MMALQueue.mmal_queue_put(this.Ptr, buffer.Ptr);
        }

        internal static MMALQueueImpl Create()
        {
            var ptr = MMALQueue.mmal_queue_create();
            return new MMALQueueImpl(ptr);
        }

        /// <summary>
        /// Destroy a queue of MMAL_BUFFER_HEADER_T.
        /// </summary>
        private void Destroy()
        {
            MMALQueue.mmal_queue_destroy(this.Ptr);
        }
    }
}
