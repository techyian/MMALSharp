using MMALSharp.Native;
using System;

namespace MMALSharp
{
    /// <summary>
    /// Represents a queue of buffer headers. 
    /// </summary>
    public unsafe class MMALQueueImpl : MMALObject
    {
        /// <summary>
        /// Native pointer to the buffer header queue this object represents
        /// </summary>
        internal MMAL_QUEUE_T* Ptr { get; set; }

        public MMALQueueImpl(MMAL_QUEUE_T* ptr)
        {
            this.Ptr = ptr;
        }

        /// <summary>
        /// Get a MMAL_BUFFER_HEADER_T from a queue
        /// </summary>
        /// <returns></returns>
        internal MMALBufferImpl GetBuffer()
        {
            var ptr = MMALQueue.mmal_queue_get(this.Ptr);
            return new MMALBufferImpl(ptr);
        }

        /// <summary>
        /// Get a MMAL_BUFFER_HEADER_T from a queue
        /// </summary>
        /// <param name="ptr">The queue to get a buffer from</param>
        /// <returns></returns>
        internal static MMALBufferImpl GetBuffer(MMAL_QUEUE_T* ptr)
        {
            var bufPtr = MMALQueue.mmal_queue_get(ptr);

            if((IntPtr)bufPtr == IntPtr.Zero)            
                return null;

            return new MMALBufferImpl(bufPtr);
        }

        /// <summary>
        /// Get the number of MMAL_BUFFER_HEADER_T currently in a queue.
        /// </summary>
        /// <returns></returns>
        internal uint QueueLength()
        {
            var length = MMALQueue.mmal_queue_length(this.Ptr);
            return length;
        }

        /// <summary>
        /// Destroy a queue of MMAL_BUFFER_HEADER_T.
        /// </summary>
        internal void Destroy()
        {
            MMALQueue.mmal_queue_destroy(this.Ptr);
        }

        public override void Dispose()
        {
            MMALLog.Logger.Debug("Disposing queue.");            
            this.Destroy();
            base.Dispose();
        }
    }
}
