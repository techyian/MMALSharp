using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharPicam
{
    public unsafe class MMALQueueImpl : MMALObject
    {
        public MMAL_QUEUE_T* Ptr { get; set; }

        public MMALQueueImpl(MMAL_QUEUE_T* ptr)
        {
            this.Ptr = ptr;
        }

        public MMALBufferImpl GetBuffer()
        {
            var ptr = MMALQueue.mmal_queue_get(this.Ptr);
            return new MMALBufferImpl(ptr);
        }

        public uint QueueLength()
        {
            var length = MMALQueue.mmal_queue_length(this.Ptr);
            return length;
        }

        public void Destroy()
        {
            MMALQueue.mmal_queue_destroy(this.Ptr);
        }

    }
}
