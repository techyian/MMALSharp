using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SharPicam.MMALCallerHelper;

namespace SharPicam
{
    public unsafe class MMALControlPortImpl : MMALPortBase
    {
        public MMALControlPortImpl(MMAL_PORT_T* ptr, MMALComponentBase comp) : base(ptr, comp) { }
                
        public override void NativePortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            var bufferImpl = new MMALBufferImpl(buffer);

            Console.WriteLine(this.ObjName);
            Console.WriteLine("Inside native control port callback");

            Console.WriteLine("Am I an event buffer? " + (bufferImpl.Cmd > 0).ToString());
                                    
            this.Callback(bufferImpl);

            Console.WriteLine("Releasing buffer");

            bufferImpl.Release();

            if (bufferImpl.Cmd == 0)
            {
                if (this.Enabled && this.ComponentReference.BufferPool != null)
                {
                    Console.WriteLine("Port still enabled and buffer pool not null.");

                    var newBuffer = MMALQueueImpl.GetBuffer(this.ComponentReference.BufferPool.Queue.Ptr);

                    Console.WriteLine("Got buffer. Sending to port.");

                    this.SendBuffer(newBuffer.Ptr);

                    Console.WriteLine("Buffer sent to port.");
                }
            }
            else
            {
                Console.WriteLine("Sent event buffer. Releasing");
            }
                    
        }

    }

    public unsafe class MMALPortImpl : MMALPortBase
    {
        public MMALPortImpl(MMAL_PORT_T* ptr, MMALComponentBase comp) : base(ptr, comp) { }

        public override void NativePortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            var bufferImpl = new MMALBufferImpl(buffer);

            Console.WriteLine(this.ObjName);
                                                
            if(bufferImpl.Length > 0)
            {
                this.Callback(bufferImpl);
            }
                        
            MMALBuffer.mmal_buffer_header_mem_unlock(bufferImpl.Ptr);

            Console.WriteLine("Releasing buffer");
            bufferImpl.Release();
                                    
            if (this.Enabled && this.ComponentReference.BufferPool != null)
            {
                Console.WriteLine("Port still enabled and buffer pool not null.");

                var newBuffer = MMALQueueImpl.GetBuffer(this.ComponentReference.BufferPool.Queue.Ptr);

                if (newBuffer != null)
                {                    
                    Console.WriteLine("Got buffer. Sending to port.");

                    this.SendBuffer(newBuffer.Ptr);

                    Console.WriteLine("Buffer sent to port.");
                }
                else
                    Console.WriteLine("Buffer null. Continuing.");
                                
            }
            else
            {
                Console.WriteLine("Not enabled or component buffer pool null.");
            }

            if(bufferImpl.Properties() == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END || bufferImpl.Properties() == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED)
            {
                this.Triggered = 1;
            }
            
        }

    }
}
