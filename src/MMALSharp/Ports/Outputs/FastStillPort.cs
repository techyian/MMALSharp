// <copyright file="FastStillPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Common.Utility;
using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp.Ports.Outputs
{
    /// <summary>
    /// Represents a still port used specifically when capturing rapid single image frames from the camera's video port.
    /// </summary>
    public unsafe class FastStillPort : OutputPort
    {
        /// <summary>
        /// Creates a new instance of <see cref="FastStillPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this port.</param>
        public FastStillPort(IntPtr ptr, MMALComponentBase comp, PortType type, Guid guid) 
            : base(ptr, comp, type, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="FastStillPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this port.</param>
        /// <param name="handler">The capture handler for the output port.</param>
        public FastStillPort(IntPtr ptr, MMALComponentBase comp, PortType type, Guid guid, ICaptureHandler handler)
            : base(ptr, comp, type, guid, handler)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="FastStillPort"/>.
        /// </summary>
        /// <param name="copyFrom">The port to copy data from.</param>
        public FastStillPort(PortBase copyFrom)
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
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.Debug("In native output callback");
            }
            
            var bufferImpl = new MMALBufferImpl(buffer);

            bufferImpl.PrintProperties();
            
            var failed = bufferImpl.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED);
            
            if ((bufferImpl.CheckState() && bufferImpl.Length > 0 && !this.ComponentReference.ForceStopProcessing && !failed && !this.Trigger) || 
                (this.ComponentReference.ForceStopProcessing && !this.Trigger))
            {
                this.ManagedOutputCallback.Callback(bufferImpl);
            }
            
            // Ensure we release the buffer before any signalling or we will cause a memory leak due to there still being a reference count on the buffer.
            this.ReleaseOutputBuffer(bufferImpl);

            // If this buffer signals the end of data stream, allow waiting thread to continue.
            if (this.ComponentReference.ForceStopProcessing || failed)
            {
                MMALLog.Logger.Debug($"{this.ComponentReference.Name} {this.Name} Signaling completion of continuous still frame capture...");
                this.Trigger = true;
            }
        }
    }
}