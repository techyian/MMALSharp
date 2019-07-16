// <copyright file="ImageFileDecodeOutputPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Threading.Tasks;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp.Ports.Outputs
{
    /// <summary>
    /// A custom port definition used specifically when using encoder conversion functionality.
    /// </summary>
    public unsafe class VideoFileDecodeOutputPort : OutputPort
    {
        /// <summary>
        /// Creates a new instance of <see cref="VideoFileDecodeOutputPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this port.</param>
        public VideoFileDecodeOutputPort(IntPtr ptr, MMALComponentBase comp, PortType type, Guid guid)
            : base(ptr, comp, type, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="VideoFileDecodeOutputPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this port.</param>
        /// <param name="handler">The capture handler for this port.</param>
        public VideoFileDecodeOutputPort(IntPtr ptr, MMALComponentBase comp, PortType type, Guid guid, ICaptureHandler handler)
            : base(ptr, comp, type, guid, handler)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="VideoFileDecodeOutputPort"/>.
        /// </summary>
        /// <param name="copyFrom">The port to copy data from.</param>
        public VideoFileDecodeOutputPort(PortBase copyFrom)
            : base((IntPtr)copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.PortType, copyFrom.Guid, copyFrom.Handler)
        {
        }
        
        /// <summary>
        /// The native callback MMAL passes buffer headers to.
        /// </summary>
        /// <param name="port">The port the buffer is sent to.</param>
        /// <param name="buffer">The buffer header.</param>
        internal override void NativeOutputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            var bufferImpl = new MMALBufferImpl(buffer);

            if (bufferImpl.CheckState() && MMALVideoFileDecoder.WorkingQueue != null)
            {
                if (MMALCameraConfig.Debug)
                {
                    MMALLog.Logger.Debug($"Putting output port buffer back into queue {((IntPtr)MMALVideoFileDecoder.WorkingQueue.Ptr).ToString()}");
                }

                bufferImpl.PrintProperties();

                MMALVideoFileDecoder.WorkingQueue.Put(bufferImpl);
            }
            else
            {
                MMALLog.Logger.Debug($"Invalid output buffer received");
            }
            
            if (port->IsEnabled == 1 && !this.Trigger.Task.IsCompleted)
            {
                Task.Run(() => { this.Trigger.SetResult(true); });
            }
        }
    }
}
