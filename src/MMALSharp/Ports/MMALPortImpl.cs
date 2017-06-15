using MMALSharp.Native;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using MMALSharp.Handlers;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp
{
    /// <summary>
    /// Represents a generic port
    /// </summary>
    public unsafe class MMALPortImpl : MMALPortBase
    {
        public MMALPortImpl(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type) : base(ptr, comp, type) { }

        /// <summary>
        /// Enable processing on an input port
        /// </summary>
        /// <param name="managedCallback">A managed callback method we can do further processing on</param>
        internal override void EnablePort(Func<MMALBufferImpl, MMALPortBase, ProcessResult> managedCallback)
        {
            if (!this.Enabled)
            {
                this.ManagedInputCallback = managedCallback;
                
                this.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(NativeInputPortCallback);
                
                IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);

                Debugger.Print("Enabling port.");
                
                if (managedCallback == null)
                {
                    Debugger.Print("Callback null");
                    
                    MMALCheck(MMALPort.mmal_port_enable(this.Ptr, IntPtr.Zero), "Unable to enable port.");
                }
                else
                {
                    MMALCheck(MMALPort.mmal_port_enable(this.Ptr, ptrCallback), "Unable to enable port.");
                }

                base.EnablePort(managedCallback);
            }

            if (!this.Enabled)
            {
                throw new PiCameraError("Unknown error occurred whilst enabling port");
            }

        }

        /// <summary>
        /// Enable processing on an output port
        /// </summary>
        /// <param name="managedCallback">A managed callback method we can do further processing on</param>
        internal override void EnablePort(Action<MMALBufferImpl, MMALPortBase> managedCallback)
        {
            if (!this.Enabled)
            {
                this.ManagedOutputCallback = managedCallback;
                
                this.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(NativeOutputPortCallback);
                
                IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);

                Debugger.Print("Enabling port.");
                
                if (managedCallback == null)
                {
                    Debugger.Print("Callback null");
                    
                    MMALCheck(MMALPort.mmal_port_enable(this.Ptr, IntPtr.Zero), "Unable to enable port.");
                }
                else
                {
                    MMALCheck(MMALPort.mmal_port_enable(this.Ptr, ptrCallback), "Unable to enable port.");
                }

                base.EnablePort(managedCallback);
            }

            if (!this.Enabled)
            {
                throw new PiCameraError("Unknown error occurred whilst enabling port");
            }
                
        }

        internal override void NativeInputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (MMALPortBase.InputLock)
            {
                var bufferImpl = new MMALBufferImpl(buffer);

                if (MMALCameraConfig.Debug)
                {
                    bufferImpl.PrintProperties();
                }

                bufferImpl.Release();

                if (this.Enabled && this.BufferPool != null)
                {
                    var newBuffer = MMALQueueImpl.GetBuffer(this.BufferPool.Queue.Ptr);

                    Console.WriteLine("Input native callback");

                    //Populate the new input buffer with user provided image data.
                    var result = this.ManagedInputCallback(newBuffer, this);
                    bufferImpl.ReadIntoBuffer(result.BufferFeed, result.EOF);

                    try
                    {
                        if (this.Trigger != null && this.Trigger.CurrentCount > 0 && result.EOF)
                        {
                            Debugger.Print("Received EOF. Releasing.");
                            
                            this.Trigger.Signal();
                            newBuffer.Release();
                        }

                        if (newBuffer != null)
                        {
                            Debugger.Print("Got buffer. Sending to port.");
                            
                            this.SendBuffer(newBuffer);
                        }
                        else
                        {
                            Debugger.Print("Buffer null. Continuing.");
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Buffer handling failed. {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// The native callback MMAL passes buffer headers to
        /// </summary>
        /// <param name="port">The port the buffer is sent to</param>
        /// <param name="buffer">The buffer header</param>
        internal override void NativeOutputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (MMALPortBase.OutputLock)
            {
                var bufferImpl = new MMALBufferImpl(buffer);

                if (MMALCameraConfig.Debug)
                {
                    bufferImpl.PrintProperties();
                }
                    
                if (bufferImpl.Length > 0)
                {
                    this.ManagedOutputCallback(bufferImpl, this);
                }

                //If this buffer signals the end of data stream, allow waiting thread to continue.
                if (bufferImpl.Properties.Any(c => c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END ||
                                                    c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED))
                {
                    Debugger.Print("End of stream. Signaling completion...");
                    
                    if (this.Trigger != null && this.Trigger.CurrentCount > 0)
                    {
                        this.Trigger.Signal();
                    }                        
                }

                bufferImpl.Release();

                try
                {
                    if (MMALCameraConfig.Debug && !this.Enabled)
                    {
                        Console.WriteLine("Port not enabled.");
                    }
                        
                    if (MMALCameraConfig.Debug && this.BufferPool == null)
                    {
                        Console.WriteLine("Buffer pool null.");
                    }
                        
                    if (this.Enabled && this.BufferPool != null)
                    {
                        Console.WriteLine("Output native callback");

                        var newBuffer = MMALQueueImpl.GetBuffer(this.BufferPool.Queue.Ptr);

                        if (newBuffer != null)
                        {
                            Debugger.Print("Got buffer. Sending to port.");

                            this.SendBuffer(newBuffer);
                        }
                        else
                        {
                            Debugger.Print("Buffer null. Continuing.");
                        }
                    }
                }
                catch(Exception e)
                {
                    Debugger.Print($"Unable to send buffer header. {e.Message}");
                }                                
            }
        }

    }

    
    
    

}
