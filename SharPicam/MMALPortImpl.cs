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

        public Action<MMALBufferImpl> Callback { get; set; }
        public void EnablePort(Action<MMALBufferImpl> callback)
        {
            this.Callback = callback;

            this.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(NativePortCallback);

            IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);

            Console.WriteLine("Enabling port.");

            if (callback == null)
            {
                Console.WriteLine("Callback null");
                MMALCheck(MMALPort.mmal_port_enable(this.Ptr, IntPtr.Zero), "Unable to enable port.");
            }
            else
                MMALCheck(MMALPort.mmal_port_enable(this.Ptr, ptrCallback), "Unable to enable port.");

        }

        public static void NativePortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
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

                    this.SendBuffer(newBuffer);

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
        public byte[] Storage { get; set; }
        public CancellationTokenSource TokenSource { get; set; }

        public MMALPortImpl(MMAL_PORT_T* ptr, MMALComponentBase comp) : base(ptr, comp) { }


        public Func<MMALBufferImpl, byte[]> Callback { get; set; }
        public void EnablePort(Func<MMALBufferImpl, byte[]> callback)
        {
            this.Callback = callback;

            this.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(NativePortCallback);

            IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);

            Console.WriteLine("Enabling port.");

            if (callback == null)
            {
                Console.WriteLine("Callback null");
                MMALCheck(MMALPort.mmal_port_enable(this.Ptr, IntPtr.Zero), "Unable to enable port.");
            }
            else
                MMALCheck(MMALPort.mmal_port_enable(this.Ptr, ptrCallback), "Unable to enable port.");

        }

        public static void NativePortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            var bufferImpl = new MMALBufferImpl(buffer);

            Console.WriteLine("Port object name = " + this.ObjName);

            bufferImpl.PrintProperties();
                                                
            if(bufferImpl.Length > 0)
            {
                var data = this.Callback(bufferImpl);

                if (data != null && Storage != null)
                    Storage = Storage.Concat(data).ToArray();
                else if (data != null && Storage == null)
                    Storage = data;
            }
            
            Console.WriteLine("Releasing buffer");
            bufferImpl.Release();
                                    
            if (this.Enabled && this.ComponentReference.BufferPool != null)
            {
                Console.WriteLine("Port still enabled and buffer pool not null.");

                var newBuffer = MMALQueueImpl.GetBuffer(this.ComponentReference.BufferPool.Queue.Ptr);

                if (newBuffer != null)
                {                    
                    Console.WriteLine("Got buffer. Sending to port.");

                    this.SendBuffer(newBuffer);

                    Console.WriteLine("Buffer sent to port.");
                }
                else
                    Console.WriteLine("Buffer null. Continuing.");
                                
            }
            else
            {
                Console.WriteLine("Not enabled or component buffer pool null.");
            }

            if(bufferImpl.Properties().Any(c => c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END || 
                                                c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED || 
                                                c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS))
            {
                Console.WriteLine("Setting triggered flag");
                TokenSource.Cancel();
            }            
        }

    }
}
