// <copyright file="MMALFastStillPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Linq;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    public unsafe class MMALFastStillPort : MMALPortImpl
    {
        public MMALFastStillPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid) : base(ptr, comp, type, guid)
        {
        }
        
        public MMALFastStillPort(MMALPortImpl copyFrom)
            : base(copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.PortType, copyFrom.Guid)
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
                
                var failed = bufferImpl.Properties.Any(c => c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED);
                
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
}