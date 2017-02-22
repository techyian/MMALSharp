using MMALSharp.Handlers;
using MMALSharp.Native;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        public string Name {
            get
            {
                return Marshal.PtrToStringAnsi((IntPtr)(this.Ptr->name));
            }
        }

        /// <summary>
        /// Indicates whether this port is enabled
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.Ptr->isEnabled == 1;
            }
        }

        /// <summary>
        /// Specifies minimum number of buffer headers required for this port 
        /// </summary>
        public uint BufferNumMin
        {
            get
            {
                return this.Ptr->bufferNumMin;
            }
        }

        /// <summary>
        /// Specifies minimum size of buffer headers required for this port
        /// </summary>
        public uint BufferSizeMin
        {
            get
            {
                return this.Ptr->bufferSizeMin;
            }
        }

        /// <summary>
        /// Specifies minimum alignment value for buffer headers required for this port
        /// </summary>
        public uint BufferAlignmentMin
        {
            get
            {
                return this.Ptr->bufferAlignmentMin;
            }
        }

        /// <summary>
        /// Specifies recommended number of buffer headers for this port
        /// </summary>
        public uint BufferNumRecommended
        {
            get
            {
                return this.Ptr->bufferNumRecommended;
            }
        }

        /// <summary>
        /// Specifies recommended size of buffer headers for this port
        /// </summary>
        public uint BufferSizeRecommended
        {
            get
            {
                return this.Ptr->bufferSizeRecommended;
            }
        }

        /// <summary>
        /// Indicates the currently set number of buffer headers for this port
        /// </summary>
        public uint BufferNum
        {
            get
            {
                return this.Ptr->bufferNum;
            }
            set
            {
                this.Ptr->bufferNum = value;
            }
        }

        /// <summary>
        /// Indicates the currently set size of buffer headers for this port
        /// </summary>
        public uint BufferSize
        {
            get
            {
                return this.Ptr->bufferSize;
            }
            set
            {
                this.Ptr->bufferSize = value;
            }
        }

        /// <summary>
        /// Accessor for the elementary stream
        /// </summary>
        public MMAL_ES_FORMAT_T Format
        {
            get
            {
                return *this.Ptr->format;
            }
        }

        #endregion

        /// <summary>
        /// Asynchronous trigger which is set when processing has completed on this port.
        /// </summary>
        public AsyncCountdownEvent Trigger = new AsyncCountdownEvent(1);
        
        /// <summary>
        /// Monitor lock for port callback method
        /// </summary>
        protected static Object mLock = new object();
        
        /// <summary>
        /// Delegate for native port callback
        /// </summary>
        public MMALPort.MMAL_PORT_BH_CB_T NativeCallback { get; set; }

        /// <summary>
        /// Delegate we use to do further processing on buffer headers when they're received by the native callback delegate
        /// </summary>
        public Action<MMALBufferImpl, MMALPortBase> ManagedCallback { get; set; }

        /// <summary>
        /// Delegate we use to process the buffer data itself
        /// </summary>
        public Action<byte[]> ProcessCallback { get; set; }

        protected MMALPortBase(MMAL_PORT_T* ptr, MMALComponentBase comp)
        {
            this.Ptr = ptr;
            this.Comp = ptr->component;            
            this.ComponentReference = comp;
        }

        /// <summary>
        /// Provides functionality to enable processing on a port.
        /// </summary>
        /// <param name="callback"></param>
        public abstract void EnablePort(Action<MMALBufferImpl, MMALPortBase> managedCallback, Action<byte[]> processCallback);

        /// <summary>
        /// Disable processing on a port. Disabling a port will stop all processing on this port and return all (non-processed) 
        /// buffer headers to the client. If this is a connected output port, the input port to which it is connected shall also be disabled.
        /// Any buffer pool shall be released.
        /// </summary>
        internal void DisablePort()
        {
            if (Enabled)
                MMALCheck(MMALPort.mmal_port_disable(this.Ptr), "Unable to disable port.");
        }

        /// <summary>
        /// Commit format changes on a port.        
        /// </summary>
        internal void Commit()
        {
            MMALCheck(MMALPort.mmal_port_format_commit(this.Ptr), "Unable to commit port changes.");
        }

        /// <summary>
        /// Shallow copy a format structure. It is worth noting that the extradata buffer will not be copied in the new format.
        /// </summary>
        /// <param name="destination"></param>
        internal void ShallowCopy(MMALPortBase destination)
        {
            MMALFormat.mmal_format_copy(destination.Ptr->format, this.Ptr->format);
        }

        /// <summary>
        /// Fully copy a format structure, including the extradata buffer.
        /// </summary>
        /// <param name="destination"></param>
        internal void FullCopy(MMALPortBase destination)
        {
            MMALFormat.mmal_format_full_copy(destination.Ptr->format, this.Ptr->format);
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
            MMALUtil.mmal_port_pool_destroy(this.Ptr, this.BufferPool.Ptr);
        }
        

    }
}
