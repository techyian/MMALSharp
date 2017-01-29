using MMALSharp.Native;
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

    public abstract unsafe class MMALPortBase : MMALObject
    {
        public MMAL_PORT_T* Ptr { get; set; }
        public MMAL_COMPONENT_T* Comp { get; set; }
        public MMALComponentBase ComponentReference { get; set; }
        public MMALConnectionImpl Connection { get; set; }
        public MMALPoolImpl BufferPool { get; set; }        
        public string ObjName { get; set; }

        #region Port struct wrapper properties

        public string Name {
            get
            {
                return Marshal.PtrToStringAnsi((IntPtr)(this.Ptr->name));
            }
        }
        public bool Enabled
        {
            get
            {
                return this.Ptr->isEnabled == 1;
            }
        }
        public uint BufferNumMin
        {
            get
            {
                return this.Ptr->bufferNumMin;
            }
        }
        public uint BufferSizeMin
        {
            get
            {
                return this.Ptr->bufferSizeMin;
            }
        }
        public uint BufferAlignmentMin
        {
            get
            {
                return this.Ptr->bufferAlignmentMin;
            }
        }
        public uint BufferNumRecommended
        {
            get
            {
                return this.Ptr->bufferNumRecommended;
            }
        }
        public uint BufferSizeRecommended
        {
            get
            {
                return this.Ptr->bufferSizeRecommended;
            }
        }
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
        public MMAL_ES_FORMAT_T Format
        {
            get
            {
                return *this.Ptr->format;
            }
        }

        #endregion

        public CountdownEvent Trigger = new CountdownEvent(1);
        
        protected static Object mLock = new object();
        
        public MMALPort.MMAL_PORT_BH_CB_T NativeCallback { get; set; }
        public Action<MMALBufferImpl> ManagedCallback { get; set; }

        protected MMALPortBase(MMAL_PORT_T* ptr, MMALComponentBase comp)
        {
            this.Ptr = ptr;
            this.Comp = ptr->component;            
            this.ComponentReference = comp;
        }

        public abstract void EnablePort(Action<MMALBufferImpl> callback);

        public void DisablePort()
        {
            if (Enabled)
                MMALCheck(MMALPort.mmal_port_disable(this.Ptr), "Unable to disable port.");
        }
        
        public void Commit()
        {
            MMALCheck(MMALPort.mmal_port_format_commit(this.Ptr), "Unable to commit port changes.");
        }

        public void ShallowCopy(MMALPortBase destination)
        {
            MMALFormat.mmal_format_copy(destination.Ptr->format, this.Ptr->format);
        }

        public void FullCopy(MMALPortBase destination)
        {
            MMALFormat.mmal_format_full_copy(destination.Ptr->format, this.Ptr->format);
        }

        public void Flush()
        {
            MMALCheck(MMALPort.mmal_port_flush(this.Ptr), "Unable to flush port.");
        }

        public void SendBuffer(MMALBufferImpl buffer)
        {
            MMALCheck(MMALPort.mmal_port_send_buffer(this.Ptr, buffer.Ptr), "Unable to send buffer header.");
        }

        

    }
}
