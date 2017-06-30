using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Represents a video port
    /// </summary>
    public unsafe class MMALVideoPort : MMALPortImpl
    {
        /// <summary>
        /// This is used when the user provides a timeout DateTime and
        /// will signal an end to video recording.
        /// </summary>
        public DateTime? Timeout { get; set; }

        public MMALVideoPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type) : base(ptr, comp, type) { }

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

                if (this.Timeout.HasValue && DateTime.Now.CompareTo(this.Timeout.Value) > 0)
                {
                    if (this.Trigger != null && this.Trigger.CurrentCount > 0)
                    {
                        this.Trigger.Signal();
                    }
                }                                
            }
        }

    }
}
