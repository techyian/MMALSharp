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
    /// <summary>
    /// Represents a control port
    /// </summary>
    public unsafe class MMALControlPortImpl : MMALPortBase
    {
        public MMALControlPortImpl(MMAL_PORT_T* ptr, MMALComponentBase comp) : base(ptr, comp) { }

        public override void EnablePort(Action<MMALBufferImpl> callback)
        {
            if(!this.Enabled)
            {
                this.ManagedCallback = callback;

                this.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(NativePortCallback);

                IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);
                if (MMALCameraConfigImpl.Config.Debug)
                    Console.WriteLine("Enabling port.");
                
                if (callback == null)
                {
                    if (MMALCameraConfigImpl.Config.Debug)
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
           
            this.ManagedCallback(bufferImpl);

            if (MMALCameraConfigImpl.Config.Debug)
                Console.WriteLine("Releasing buffer");
            bufferImpl.Release();                        
        }

    }

    /// <summary>
    /// Represents a port
    /// </summary>
    public unsafe class MMALPortImpl : MMALPortBase
    {  
        public MMALPortImpl(MMAL_PORT_T* ptr, MMALComponentBase comp) : base(ptr, comp) { }
                
        public override void EnablePort(Action<MMALBufferImpl> callback)
        {
            if(!this.Enabled)
            {
                this.ManagedCallback = callback;

                this.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(NativePortCallback);

                IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);

                if (MMALCameraConfigImpl.Config.Debug)
                    Console.WriteLine("Enabling port.");

                this.BufferPool = new MMALPoolImpl(this);
                                                
                if (callback == null)
                {
                    if (MMALCameraConfigImpl.Config.Debug)
                        Console.WriteLine("Callback null");
                    MMALCheck(MMALPort.mmal_port_enable(this.Ptr, IntPtr.Zero), "Unable to enable port.");
                }
                else
                    MMALCheck(MMALPort.mmal_port_enable(this.Ptr, ptrCallback), "Unable to enable port.");

                var length = this.BufferPool.Queue.QueueLength();

                for (int i = 0; i < length; i++)
                {
                    var buffer = this.BufferPool.Queue.GetBuffer();
                    this.SendBuffer(buffer);
                }
            }            
        }

        public void NativePortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (MMALPortImpl.mLock)
            {                
                var bufferImpl = new MMALBufferImpl(buffer);

                if (MMALCameraConfigImpl.Config.Debug)
                    bufferImpl.PrintProperties();

                //Process buffer frame into a byte array
                if (bufferImpl.Length > 0)
                {
                    this.ManagedCallback(bufferImpl);                    
                }

                //If this buffer signals the end of data stream, allow waiting thread to continue.
                if (bufferImpl.Properties.Any(c => c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END ||
                                                    c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED))
                {
                    if (MMALCameraConfigImpl.Config.Debug)
                        Console.WriteLine("End of stream. Signaling completion...");
                    this.Trigger.Signal();                    
                }
                
                bufferImpl.Release();

                try
                {
                    if (this.Enabled && this.BufferPool != null)
                    {
                        var newBuffer = MMALQueueImpl.GetBuffer(this.BufferPool.Queue.Ptr);

                        if (newBuffer != null)
                        {
                            if (MMALCameraConfigImpl.Config.Debug)
                                Console.WriteLine("Got buffer. Sending to port.");

                            this.SendBuffer(newBuffer);
                        }
                        else
                        {
                            if (MMALCameraConfigImpl.Config.Debug)
                                Console.WriteLine("Buffer null. Continuing.");
                        }
                            

                    }
                    else
                    {
                        if (MMALCameraConfigImpl.Config.Debug)
                            Console.WriteLine("Not enabled or component buffer pool null.");
                    }
                }
                catch
                {
                    if (MMALCameraConfigImpl.Config.Debug)
                        Console.WriteLine("Unable to send buffer header");
                }

                Thread.Sleep(100);
            }
        }

    }
}
