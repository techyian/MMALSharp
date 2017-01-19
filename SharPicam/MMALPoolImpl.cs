using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharPicam.MMALCallerHelper;

namespace SharPicam
{
    public unsafe class MMALPoolImpl : MMALObject, IDisposable
    {
        public MMAL_POOL_T* Ptr { get; set; }
        public MMALPortImpl Port { get; set; }
        public MMALQueueImpl Queue { get; set; }

        public MMALPoolImpl(MMALPortImpl port)
        {
            this.Ptr = MMALUtil.mmal_port_pool_create(port.Ptr, port.BufferNum, port.BufferSize);
            this.Queue = new MMALQueueImpl((*this.Ptr).queue);            
        }
        
        public void Destroy()
        {
            MMALPool.mmal_pool_destroy(this.Ptr);
        }

        public void Resize(uint numHeaders, uint size)
        {
            MMALCheck(MMALPool.mmal_pool_resize(this.Ptr, numHeaders, size), "Unable to resize pool");
        }

        public override void Dispose()
        {
            Console.WriteLine("Disposing pool.");
            this.Destroy();
            base.Dispose();
        }
    }
}
