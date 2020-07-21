// <copyright file="MMALPoolImpl.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using Microsoft.Extensions.Logging;
using MMALSharp.Common.Utility;
using MMALSharp.Native;
using MMALSharp.Ports;
using static MMALSharp.MMALNativeExceptionHelper;

namespace MMALSharp
{
    /// <summary>
    /// Represents a pool of buffer headers. An instance of this class can be created via a MMALPortImpl.
    /// </summary>
    public unsafe class MMALPoolImpl : MMALObject, IBufferPool
    {
        /// <summary>
        /// Native pointer that represents this buffer header pool.
        /// </summary>
        public MMAL_POOL_T* Ptr { get; }

        /// <summary>
        /// Accessor to the queue of buffer headers this pool has.
        /// </summary>
        public IBufferQueue Queue { get; }

        /// <summary>
        /// The number of buffer headers in this pool.
        /// </summary>
        public uint HeadersNum => this.Ptr->HeadersNum;
        
        /// <summary>
        /// Creates a new instance of <see cref="MMALPoolImpl"/> based on a port.
        /// </summary>
        /// <param name="port">The port.</param>
        public MMALPoolImpl(IPort port)
        {            
            MMALLog.Logger.LogDebug($"Creating buffer pool with {port.BufferNum} buffers of size {port.BufferSize}");

            this.Ptr = MMALUtil.mmal_port_pool_create(port.Ptr, port.BufferNum, port.BufferSize);
            this.Queue = new MMALQueueImpl((*this.Ptr).Queue);
        }

        /// <summary>
        /// Creates a new instance of <see cref="MMALPoolImpl"/> based on an existing <see cref="MMAL_POOL_T"/> pointer.
        /// </summary>
        /// <param name="ptr">The <see cref="MMAL_POOL_T"/> pointer.</param>
        public MMALPoolImpl(MMAL_POOL_T* ptr)
        {
            MMALLog.Logger.LogDebug($"Creating buffer pool from existing instance.");

            this.Ptr = ptr;
            this.Queue = new MMALQueueImpl((*this.Ptr).Queue);
        }

        /// <inheritdoc />
        public override bool CheckState()
        {
            return this.Ptr != null && (IntPtr)this.Ptr != IntPtr.Zero;
        }

        /// <summary>
        /// Resize a pool of MMAL_BUFFER_HEADER_T. This allows modifying either the number of allocated buffers, the payload size or both at the same time.
        /// </summary>
        /// <param name="numHeaders">Number of headers to be contained in this pool.</param>
        /// <param name="size">The size of the headers.</param>
        public void Resize(uint numHeaders, uint size)
        {
            MMALCheck(MMALPool.mmal_pool_resize(this.Ptr, numHeaders, size), "Unable to resize pool");
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            MMALLog.Logger.LogDebug("Disposing buffer pool.");
            this.Destroy();
            base.Dispose();
        }

        /// <summary>
        /// Destroy a pool of MMAL_BUFFER_HEADER_T. This will also deallocate all of the memory which was allocated when creating or resizing the pool.
        /// </summary>
        private void Destroy()
        {
            MMALPool.mmal_pool_destroy(this.Ptr);
        }
    }
}
