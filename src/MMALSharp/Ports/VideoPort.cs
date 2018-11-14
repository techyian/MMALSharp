// <copyright file="VideoPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Common.Utility;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Represents a video encode/decode port
    /// </summary>
    public unsafe class VideoPort : OutputPort
    {
        /// <summary>
        /// This is used when the user provides a timeout DateTime and
        /// will signal an end to video recording.
        /// </summary>
        public DateTime? Timeout { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="VideoPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public VideoPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid)
            : base(ptr, comp, type, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="VideoPort"/>.
        /// </summary>
        /// <param name="copyFrom">The port to copy data from.</param>
        public VideoPort(IPort copyFrom)
            : base(copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.PortType, copyFrom.Guid, copyFrom.Handler)
        {
        }

        /// <summary>
        /// The native callback MMAL passes buffer headers to.
        /// </summary>
        /// <param name="port">The port the buffer is sent to.</param>
        /// <param name="buffer">The buffer header.</param>
        internal override void NativeOutputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (OutputLock)
            {
                if (MMALCameraConfig.Debug)
                {
                    MMALLog.Logger.Debug("In native output callback");
                }

                var bufferImpl = new MMALBufferImpl(buffer);

                if (MMALCameraConfig.Debug)
                {
                    bufferImpl.PrintProperties();
                }
                
                var eos = (this.Timeout.HasValue && DateTime.Now.CompareTo(this.Timeout.Value) > 0) || this.ComponentReference.ForceStopProcessing;

                if (bufferImpl.Ptr != null && (IntPtr)bufferImpl.Ptr != IntPtr.Zero && bufferImpl.Length > 0 && !eos && !this.Trigger)
                {
                    this.ManagedOutputCallback.Callback(bufferImpl);
                }
                
                // Ensure we release the buffer before any signalling or we will cause a memory leak due to there still being a reference count on the buffer.
                this.ReleaseOutputBuffer(bufferImpl);

                if (eos)
                {
                    if (!this.Trigger)
                    {
                        MMALLog.Logger.Debug($"{this.ComponentReference.Name} {this.Name} Timeout exceeded, triggering signal.");
                        this.Trigger = true;
                    }
                }
            }
        }
    }
}
