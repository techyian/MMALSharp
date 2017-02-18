using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp
{
    /// <summary>
    /// Represents a pool of buffer headers. An instance of this class can be created via a MMALPortImpl.
    /// </summary>
    public unsafe class MMALPoolImpl : MMALObject
    {
        /// <summary>
        /// Native pointer that represents this buffer header pool
        /// </summary>
        internal MMAL_POOL_T* Ptr { get; set; }
        
        /// <summary>
        /// Accessor to the queue of buffer headers this pool has
        /// </summary>                                
        public MMALQueueImpl Queue { get; set; }

        public MMALPoolImpl(MMALPortImpl port)
        {
            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine(string.Format("Creating buffer pool with {0} buffers of size {1}", port.BufferNum, port.BufferSize));
            this.Ptr = MMALUtil.mmal_port_pool_create(port.Ptr, port.BufferNum, port.BufferSize);
            this.Queue = new MMALQueueImpl((*this.Ptr).queue);            
        }

        /// <summary>
        /// Destroy a pool of MMAL_BUFFER_HEADER_T. This will also deallocate all of the memory which was allocated when creating or resizing the pool.
        /// </summary>
        public void Destroy()
        {
            MMALPool.mmal_pool_destroy(this.Ptr);
        }

        /// <summary>
        /// Resize a pool of MMAL_BUFFER_HEADER_T. This allows modifying either the number of allocated buffers, the payload size or both at the same time.
        /// </summary>
        /// <param name="numHeaders"></param>
        /// <param name="size"></param>
        public void Resize(uint numHeaders, uint size)
        {
            MMALCheck(MMALPool.mmal_pool_resize(this.Ptr, numHeaders, size), "Unable to resize pool");
        }

        public override void Dispose()
        {
            if(MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine("Disposing pool.");
            this.Destroy();
            base.Dispose();
        }
    }
}
