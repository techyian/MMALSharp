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
    public unsafe class MMALControlPortImpl : MMALPortBase
    {
        public MMALControlPortImpl(MMAL_PORT_T* ptr, MMALComponentBase comp) : base(ptr, comp) { }

        public Action<MMALBufferImpl> Callback { get; set; }
        public void EnablePort(Action<MMALBufferImpl> callback)
        {
            this.Callback = callback;

            MMALControlPortImpl.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(NativePortCallback);

            IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(MMALControlPortImpl.NativeCallback);

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

            MMALPortImpl portObj = null;
            foreach (WeakReference<MMALObject> obj in Objects)
            {
                MMALObject target;

                if (obj.TryGetTarget(out target))
                {
                    if (target is MMALPortImpl && ((MMALPortImpl)target).Ptr == port)
                    {
                        portObj = target as MMALPortImpl;
                    }
                }
            }

            if (portObj == null)
            {
                Console.WriteLine("Cannot retrieve port object. Releasing buffer");

                bufferImpl.Release();
                return;
            }

            portObj.Callback(bufferImpl);

            Console.WriteLine("CONTROL CALLBACK");

            Console.WriteLine("Releasing buffer");
            bufferImpl.Release();

            if (bufferImpl.Cmd == 0)
            {
                if (portObj.Enabled && portObj.ComponentReference.BufferPool != null)
                {
                    Console.WriteLine("Port still enabled and buffer pool not null.");

                    var newBuffer = MMALQueueImpl.GetBuffer(portObj.ComponentReference.BufferPool.Queue.Ptr);

                    Console.WriteLine("Got buffer. Sending to port.");

                    portObj.SendBuffer(newBuffer);

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

        public System.Timers.Timer CountdownTimer { get; set; }

        public MMALBufferImpl CurrentBuffer { get; set; }


        public void EnablePort(Func<MMALBufferImpl, byte[]> callback)
        {
            this.Callback = callback;

            MMALPortImpl.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(NativePortCallback);

            IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(MMALPortImpl.NativeCallback);

            Console.WriteLine("Enabling port.");

            if (callback == null)
            {
                Console.WriteLine("Callback null");
                MMALCheck(MMALPort.mmal_port_enable(this.Ptr, IntPtr.Zero), "Unable to enable port.");
            }
            else
                MMALCheck(MMALPort.mmal_port_enable(this.Ptr, ptrCallback), "Unable to enable port.");

        }

        public void SignalDisable(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("in signal disable");
            this.DisableTrigger.Signal();
        }

        public void NativePortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {            
            //lock(MMALPortImpl.mLock)
            //{
                var bufferImpl = new MMALBufferImpl(buffer);

                this.CurrentBuffer = bufferImpl;

                bufferImpl.PrintProperties();
                          
                if (bufferImpl.Length > 0)
                {
                    var data = this.Callback(bufferImpl);

                    if (data != null && this.Storage != null)
                        this.Storage = this.Storage.Concat(data).ToArray();
                    else if (data != null && this.Storage == null)
                        this.Storage = data;
                }

                if (this.CurrentBuffer.Properties().Any(c => c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS ||
                                                    c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED))
                {
                    Console.WriteLine("Setting triggered flag");

                    this.Trigger.Signal();
                    this.Finished = true;                   
                }

                Console.WriteLine("Releasing buffer");
                bufferImpl.Release();
                                
                try
                {
                    if (this.Enabled && this.ComponentReference.BufferPool != null)
                    {
                        var newBuffer = MMALQueueImpl.GetBuffer(this.ComponentReference.BufferPool.Queue.Ptr);

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
        //}

    }
}
