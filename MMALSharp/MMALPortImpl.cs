using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp
{
    internal unsafe class MMALControlPortImpl : MMALPortBase
    {
        public MMALControlPortImpl(MMAL_PORT_T* ptr, MMALComponentBase comp) : base(ptr, comp) { }

        public Action<MMALBufferImpl> Callback { get; set; }
        public void EnablePort(Action<MMALBufferImpl> callback)
        {
            if(!this.Enabled)
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
            
        }
        
        public void NativePortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            var bufferImpl = new MMALBufferImpl(buffer);
           
            this.Callback(bufferImpl);
            
            Console.WriteLine("Releasing buffer");
            bufferImpl.Release();                        
        }

    }

    internal unsafe class MMALPortImpl : MMALPortBase
    {
        public byte[] Storage { get; set; }
                
        public Func<MMALBufferImpl, byte[]> Callback { get; set; }

        public System.Timers.Timer CountdownTimer { get; set; }
        
        public MMALPortImpl(MMAL_PORT_T* ptr, MMALComponentBase comp) : base(ptr, comp) { }

        public void EnablePort(Func<MMALBufferImpl, byte[]> callback)
        {
            if(!this.Enabled)
            {
                this.Callback = callback;

                this.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(NativePortCallback);

                IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);

                Console.WriteLine("Enabling port.");

                this.BufferPool = new MMALPoolImpl(this);

                var length = this.BufferPool.Queue.QueueLength();

                for (int i = 0; i < length; i++)
                {
                    var buffer = this.BufferPool.Queue.GetBuffer();
                    this.SendBuffer(buffer);
                }
                
                if (callback == null)
                {
                    Console.WriteLine("Callback null");
                    MMALCheck(MMALPort.mmal_port_enable(this.Ptr, IntPtr.Zero), "Unable to enable port.");
                }
                else
                    MMALCheck(MMALPort.mmal_port_enable(this.Ptr, ptrCallback), "Unable to enable port.");
            }            
        }

        public void NativePortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (MMALPortImpl.mLock)
            {
                Console.WriteLine("Background thread: " + Thread.CurrentThread.IsBackground);

                var bufferImpl = new MMALBufferImpl(buffer);
                
                bufferImpl.PrintProperties();

                //Process buffer frame into a byte array
                if (bufferImpl.Length > 0)
                {
                    var data = this.Callback(bufferImpl);

                    if (data != null && this.Storage != null)
                        this.Storage = this.Storage.Concat(data).ToArray();
                    else if (data != null && this.Storage == null)
                        this.Storage = data;
                }

                //If this buffer signals the end of data stream, allow waiting thread to continue.
                if (bufferImpl.Properties.Any(c => c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END ||
                                                    c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED))
                {
                    Console.WriteLine("Setting triggered flag");

                    this.Trigger.Signal();                    
                }

                Console.WriteLine("Releasing buffer");
                bufferImpl.Release();

                try
                {
                    if (this.Enabled && this.BufferPool != null)
                    {
                        var newBuffer = MMALQueueImpl.GetBuffer(this.BufferPool.Queue.Ptr);

                        if (newBuffer != null)
                        {
                            Console.WriteLine("Got buffer. Sending to port.");

                            this.SendBuffer(newBuffer);
                        }
                        else
                            Console.WriteLine("Buffer null. Continuing.");

                    }
                    else
                    {
                        Console.WriteLine("Not enabled or component buffer pool null.");
                    }
                }
                catch
                {
                    Console.WriteLine("Unable to send buffer header");
                }

                Thread.Sleep(100);
            }
        }

    }
}
