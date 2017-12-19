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

                MMALLog.Logger.Debug("Enabling port.");
                
                if (managedCallback == null)
                {
                    MMALLog.Logger.Warn("Callback null");
                    
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

                MMALLog.Logger.Debug("Enabling port.");
                
                if (managedCallback == null)
                {
                    MMALLog.Logger.Warn("Callback null");
                    
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

                this.ReleaseInputBuffer(bufferImpl);
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

                //Ensure we release the buffer before any signalling or we will cause a memory leak due to there still being a reference count on the buffer.
                this.ReleaseOutputBuffer(bufferImpl);

                //If this buffer signals the end of data stream, allow waiting thread to continue.
                if (bufferImpl.Properties.Any(c => c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END ||
                                                    c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED))
                {
                    MMALLog.Logger.Debug("End of stream. Signaling completion...");
                    
                    if (this.Trigger != null && this.Trigger.CurrentCount > 0)
                    {
                        this.Trigger.Signal();
                    }                        
                }                                                             
            }
        }

    }

    
    
    

}
