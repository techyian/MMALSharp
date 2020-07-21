// <copyright file="IBufferPool.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Native;

namespace MMALSharp
{
    /// <summary>
    /// Represents a pool of buffer headers.
    /// </summary>
    public interface IBufferPool : IMMALObject
    {
        /// <summary>
        /// Native pointer that represents this buffer header pool.
        /// </summary>
        unsafe MMAL_POOL_T* Ptr { get; }

        /// <summary>
        /// Accessor to the queue of buffer headers this pool has.
        /// </summary>
        IBufferQueue Queue { get; }

        /// <summary>
        /// The number of buffer headers in this pool.
        /// </summary>
        uint HeadersNum { get; }

        /// <summary>
        /// Resize a pool of MMAL_BUFFER_HEADER_T. This allows modifying either the number of allocated buffers, the payload size or both at the same time.
        /// </summary>
        /// <param name="numHeaders">Number of headers to be contained in this pool.</param>
        /// <param name="size">The size of the headers.</param>
        void Resize(uint numHeaders, uint size);
    }
}
