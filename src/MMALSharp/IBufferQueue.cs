// <copyright file="IBufferQueue.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Native;

namespace MMALSharp
{
    /// <summary>
    /// Represents a queue of buffer headers.
    /// </summary>
    public interface IBufferQueue
    {
        /// <summary>
        /// Native pointer to the buffer header queue this object represents.
        /// </summary>
        unsafe MMAL_QUEUE_T* Ptr { get; }

        /// <summary>
        /// Get a MMAL_BUFFER_HEADER_T from a queue.
        /// </summary>
        /// <returns>A new managed buffer header object.</returns>
        IBuffer GetBuffer();

        /// <summary>
        /// Get the number of buffer headers currently in this queue.
        /// </summary>
        /// <returns>The number of buffers currently in this queue.</returns>
        uint QueueLength();

        /// <summary>
        /// Waits (blocking) for a buffer header to be available in the queue and allocates it.
        /// </summary>
        /// <returns>The buffer header.</returns>
        IBuffer Wait();

        /// <summary>
        /// Puts the buffer header back into this queue.
        /// </summary>
        /// <param name="buffer">The buffer header.</param>
        void Put(IBuffer buffer);
    }
}
