using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Represents a still image port
    /// </summary>
    public unsafe class MMALStillPort : MMALPortImpl
    {
        public MMALStillPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type) : base(ptr, comp, type) { }
    }

    /// <summary>
    /// A custom port definition used specifically when using encoder conversion functionality
    /// </summary>
    public unsafe class MMALStillConvertPort : MMALStillPort
    {
        public MMALStillConvertPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type) : base(ptr, comp, type) { }

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
                if (bufferImpl.Properties.Any(c => c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS ||
                                                   c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED))
                {
                    Debugger.Print("End of stream. Signaling completion...");

                    if (this.Trigger != null && this.Trigger.CurrentCount > 0)
                    {
                        this.Trigger.Signal();
                    }
                }

                this.ReleaseOutputBuffer(bufferImpl);
            }
        }

    }
}
