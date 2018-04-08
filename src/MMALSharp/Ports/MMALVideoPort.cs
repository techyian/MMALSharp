// <copyright file="MMALVideoPort.cs" company="Techyian">
// Copyright (c) Techyian. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Represents a video encode/decode port
    /// </summary>
    public unsafe class MMALVideoPort : MMALPortImpl
    {
        /// <summary>
        /// This is used when the user provides a timeout DateTime and
        /// will signal an end to video recording.
        /// </summary>
        public DateTime? Timeout { get; set; }

        public MMALVideoPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type)
            : base(ptr, comp, type)
        {
        }

        public MMALVideoPort(MMALPortImpl copyFrom)
            : base(copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.PortType)
        {
        }

        /// <summary>
        /// The native callback MMAL passes buffer headers to.
        /// </summary>
        /// <param name="port">The port the buffer is sent to.</param>
        /// <param name="buffer">The buffer header.</param>
        internal override void NativeOutputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (MMALPortBase.OutputLock)
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

                if (bufferImpl.Ptr != null && (IntPtr)bufferImpl.Ptr != IntPtr.Zero && bufferImpl.Length > 0)
                {
                    this.ManagedOutputCallback(bufferImpl, this);
                }

                // Ensure we release the buffer before any signalling or we will cause a memory leak due to there still being a reference count on the buffer.
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
