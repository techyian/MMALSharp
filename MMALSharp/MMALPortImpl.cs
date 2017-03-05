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
    public unsafe class MMALControlPort : MMALPortBase
    {
        public MMALControlPort(MMAL_PORT_T* ptr, MMALComponentBase comp) : base(ptr, comp) { }

        internal override void EnablePort(Action<MMALBufferImpl, MMALPortBase> managedCallback, Action<byte[]> processCallback)
        {
            if(!this.Enabled)
            {
                this.ManagedCallback = managedCallback;
                this.ProcessCallback = processCallback;
                this.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(NativePortCallback);

                IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);
                if (MMALCameraConfig.Debug)
                    Console.WriteLine("Enabling port.");
                
                if (managedCallback == null)
                {
                    if (MMALCameraConfig.Debug)
                        Console.WriteLine("Callback null");
                    MMALCheck(MMALPort.mmal_port_enable(this.Ptr, IntPtr.Zero), "Unable to enable port.");
                }
                else
                    MMALCheck(MMALPort.mmal_port_enable(this.Ptr, ptrCallback), "Unable to enable port.");
            }            
        }
        
        internal override void NativePortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            var bufferImpl = new MMALBufferImpl(buffer);
            
            this.ManagedCallback(bufferImpl, this);

            if (MMALCameraConfig.Debug)
                Console.WriteLine("Releasing buffer");
            bufferImpl.Release();                        
        }

    }

    /// <summary>
    /// Represents a generic port
    /// </summary>
    public unsafe class MMALPortImpl : MMALPortBase
    {
        public MMALPortImpl(MMAL_PORT_T* ptr, MMALComponentBase comp) : base(ptr, comp) { }

        internal override void EnablePort(Action<MMALBufferImpl, MMALPortBase> managedCallback, Action<byte[]> processCallback)
        {
            if (!this.Enabled)
            {
                this.ManagedCallback = managedCallback;
                this.ProcessCallback = processCallback;
                this.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(NativePortCallback);

                IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);

                if (MMALCameraConfig.Debug)
                    Console.WriteLine("Enabling port.");

                this.BufferPool = new MMALPoolImpl(this);

                if (managedCallback == null)
                {
                    if (MMALCameraConfig.Debug)
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

            if (!this.Enabled)
                throw new PiCameraError("Unknown error occurred whilst enabling port");

        }

        internal override void NativePortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (MMALPortBase.mLock)
            {
                var bufferImpl = new MMALBufferImpl(buffer);

                if (MMALCameraConfig.Debug)
                    bufferImpl.PrintProperties();

                if (bufferImpl.Length > 0)
                {
                    this.ManagedCallback(bufferImpl, this);
                }

                //If this buffer signals the end of data stream, allow waiting thread to continue.
                if (bufferImpl.Properties.Any(c => c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END ||
                                                    c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED))
                {
                    if (MMALCameraConfig.Debug)
                        Console.WriteLine("End of stream. Signaling completion...");
                    if (this.Trigger != null && this.Trigger.CurrentCount > 0)
                        this.Trigger.Signal();
                }

                bufferImpl.Release();

                try
                {
                    if (MMALCameraConfig.Debug && !this.Enabled)
                        Console.WriteLine("Port not enabled.");
                    if (MMALCameraConfig.Debug && this.BufferPool == null)
                        Console.WriteLine("Buffer pool null.");

                    if (this.Enabled && this.BufferPool != null)
                    {
                        var newBuffer = MMALQueueImpl.GetBuffer(this.BufferPool.Queue.Ptr);

                        if (newBuffer != null)
                        {
                            if (MMALCameraConfig.Debug)
                                Console.WriteLine("Got buffer. Sending to port.");

                            this.SendBuffer(newBuffer);
                        }
                        else
                        {
                            if (MMALCameraConfig.Debug)
                                Console.WriteLine("Buffer null. Continuing.");
                        }
                    }
                }
                catch
                {
                    if (MMALCameraConfig.Debug)
                        Console.WriteLine("Unable to send buffer header");
                }

                Thread.Sleep(50);
            }
        }

    }

    /// <summary>
    /// Represents a still image port
    /// </summary>
    public unsafe class MMALStillPort : MMALPortImpl
    {  
        public MMALStillPort(MMAL_PORT_T* ptr, MMALComponentBase comp) : base(ptr, comp) { }         
    }

    /// <summary>
    /// Represents a video port
    /// </summary>
    public unsafe class MMALVideoPort : MMALPortImpl
    {
        public MMALVideoPort(MMAL_PORT_T* ptr, MMALComponentBase comp) : base(ptr, comp) { }        
    }

}
