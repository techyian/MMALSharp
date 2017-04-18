using MMALSharp.Native;
using Nito.AsyncEx;
using System;
using System.Runtime.InteropServices;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp
{
    /// <summary>
    /// Base class for port objects
    /// </summary>
    public abstract unsafe class MMALPortBase : MMALObject
    {
        /// <summary>
        /// Native pointer that represents this port
        /// </summary>
        internal MMAL_PORT_T* Ptr { get; set; }

        /// <summary>
        /// Native pointer that represents the component this port is associated with
        /// </summary>
        internal MMAL_COMPONENT_T* Comp { get; set; }

        /// <summary>
        /// Managed reference to the component this port is associated with
        /// </summary>
        public MMALComponentBase ComponentReference { get; set; }

        /// <summary>
        /// Managed reference to the pool of buffer headers associated with this port
        /// </summary>                                
        public MMALPoolImpl BufferPool { get; set; }

        /// <summary>
        /// Managed name given to this object (user defined)
        /// </summary>
        public string ObjName { get; set; }

        #region Port struct wrapper properties

        /// <summary>
        /// Native name of port
        /// </summary>
        public string Name => Marshal.PtrToStringAnsi((IntPtr)(this.Ptr->Name));

        /// <summary>
        /// Indicates whether this port is enabled
        /// </summary>
        public bool Enabled => this.Ptr->IsEnabled == 1;

        /// <summary>
        /// Specifies minimum number of buffer headers required for this port 
        /// </summary>
        public int BufferNumMin
        {
            get
            {
                return this.Ptr->BufferNumMin;
            }
        }

        /// <summary>
        /// Specifies minimum size of buffer headers required for this port
        /// </summary>
        public int BufferSizeMin => this.Ptr->BufferSizeMin;

        /// <summary>
        /// Specifies minimum alignment value for buffer headers required for this port
        /// </summary>
        public int BufferAlignmentMin => this.Ptr->BufferAlignmentMin;

        /// <summary>
        /// Specifies recommended number of buffer headers for this port
        /// </summary>
        public int BufferNumRecommended => this.Ptr->BufferNumRecommended;

        /// <summary>
        /// Specifies recommended size of buffer headers for this port
        /// </summary>
        public int BufferSizeRecommended => this.Ptr->BufferSizeRecommended;

        /// <summary>
        /// Indicates the currently set number of buffer headers for this port
        /// </summary>
        public int BufferNum
        {
            get
            {
                return this.Ptr->BufferNum;
            }
            set
            {
                this.Ptr->BufferNum = value;
            }
        }

        /// <summary>
        /// Indicates the currently set size of buffer headers for this port
        /// </summary>
        public int BufferSize
        {
            get
            {
                return this.Ptr->BufferSize;
            }
            set
            {
                this.Ptr->BufferSize = value;
            }
        }

        /// <summary>
        /// Accessor for the elementary stream
        /// </summary>
        public MMAL_ES_FORMAT_T Format => *this.Ptr->Format;

        #endregion

        /// <summary>
        /// Asynchronous trigger which is set when processing has completed on this port.
        /// </summary>
        public AsyncCountdownEvent Trigger { get; set; }

        /// <summary>
        /// Monitor lock for port callback method
        /// </summary>
        protected static Object mLock = new object();

        /// <summary>
        /// Delegate for native port callback
        /// </summary>
        public MMALSharp.Native.MMALPort.MMAL_PORT_BH_CB_T NativeCallback { get; set; }

        /// <summary>
        /// Delegate we use to do further processing on buffer headers when they're received by the native callback delegate
        /// </summary>
        public Action<MMALBufferImpl, MMALPortBase> ManagedCallback { get; set; }
                
        protected MMALPortBase(MMAL_PORT_T* ptr, MMALComponentBase comp)
        {
            this.Ptr = ptr;
            this.Comp = ptr->Component;
            this.ComponentReference = comp;
        }

        /// <summary>
        /// Provides functionality to enable processing on a port.
        /// </summary>
        /// <param name="managedCallback"></param>        
        internal virtual void EnablePort(Action<MMALBufferImpl, MMALPortBase> managedCallback) { }
        
        /// <summary>
        /// Represents the native callback method that's called by MMAL
        /// </summary>
        /// <param name="port"></param>
        /// <param name="buffer"></param>
        internal virtual void NativePortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer) { }

        /// <summary>
        /// Disable processing on a port. Disabling a port will stop all processing on this port and return all (non-processed) 
        /// buffer headers to the client. If this is a connected output port, the input port to which it is connected shall also be disabled.
        /// Any buffer pool shall be released.
        /// </summary>
        internal void DisablePort()
        {
            if (Enabled)
            {
                if (MMALCameraConfig.Debug)
                    Console.WriteLine("Disabling port");
                MMALCheck(MMALPort.mmal_port_disable(this.Ptr), "Unable to disable port.");
            }
                
        }

        /// <summary>
        /// Commit format changes on this port.        
        /// </summary>
        internal void Commit()
        {
            MMALCheck(MMALPort.mmal_port_format_commit(this.Ptr), "Unable to commit port changes.");
        }
        
        /// <summary>
        /// Shallow copy a format structure. It is worth noting that the extradata buffer will not be copied in the new format.
        /// </summary>
        /// <param name="destination">The destination port we're copying to</param>
        internal void ShallowCopy(MMALPortBase destination)
        {
            MMALFormat.mmal_format_copy(destination.Ptr->Format, this.Ptr->Format);
        }

        /// <summary>
        /// Fully copy a format structure, including the extradata buffer.
        /// </summary>
        /// <param name="destination">The destination port we're copying to</param>
        internal void FullCopy(MMALPortBase destination)
        {
            MMALFormat.mmal_format_full_copy(destination.Ptr->Format, this.Ptr->Format);
        }

        /// <summary>
        /// Ask a port to release all the buffer headers it currently has. This is an asynchronous operation and the 
        /// flush call will return before all the buffer headers are returned to the client.
        /// </summary>
        internal void Flush()
        {
            MMALCheck(MMALPort.mmal_port_flush(this.Ptr), "Unable to flush port.");
        }

        /// <summary>
        /// Send a buffer header to a port.
        /// </summary>
        /// <param name="buffer"></param>
        internal void SendBuffer(MMALBufferImpl buffer)
        {
            MMALCheck(MMALPort.mmal_port_send_buffer(this.Ptr, buffer.Ptr), "Unable to send buffer header.");
        }

        /// <summary>
        /// Destroy a pool of MMAL_BUFFER_HEADER_T associated with a specific port. This will also deallocate all of the memory 
        /// which was allocated when creating or resizing the pool.
        /// </summary>
        internal void DestroyPortPool()
        {
            if(this.BufferPool != null)
                MMALUtil.mmal_port_pool_destroy(this.Ptr, this.BufferPool.Ptr);
        }
        

    }
}
