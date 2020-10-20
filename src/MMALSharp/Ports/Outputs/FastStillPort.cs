// <copyright file="FastStillPort.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MMALSharp.Callbacks;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports.Inputs;

namespace MMALSharp.Ports.Outputs
{
    /// <summary>
    /// Represents a still port used specifically when capturing rapid single image frames from the camera's video port.
    /// </summary>
    public unsafe class FastStillPort : VideoPort, IVideoPort
    {        
        /// <summary>
        /// Creates a new instance of <see cref="FastStillPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="guid">Managed unique identifier for this port.</param>
        public FastStillPort(IntPtr ptr, IComponent comp, Guid guid) 
            : base(ptr, comp, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="FastStillPort"/>.
        /// </summary>
        /// <param name="copyFrom">The port to copy data from.</param>
        public FastStillPort(IPort copyFrom)
            : base((IntPtr)copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.Guid)
        {
        }

        /// <inheritdoc />
        public override void Configure(IMMALPortConfig config, IInputPort copyFrom, IOutputCaptureHandler handler)
        {
            base.Configure(config, copyFrom, handler);

            this.CallbackHandler = new FastImageOutputCallbackHandler(this, handler);
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
                MMALLog.Logger.LogDebug($"{this.Name}: In native {nameof(FastStillPort)} output callback.");
            }
            
            var bufferImpl = new MMALBufferImpl(buffer);

            bufferImpl.PrintProperties();
            
            var failed = bufferImpl.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED);
            
            if ((bufferImpl.CheckState() && bufferImpl.Length > 0 && !this.ComponentReference.ForceStopProcessing && !failed && !this.Trigger.Task.IsCompleted) || 
                (this.ComponentReference.ForceStopProcessing && !this.Trigger.Task.IsCompleted))
            {
                this.CallbackHandler.Callback(bufferImpl);
            }
            
            // Ensure we release the buffer before any signalling or we will cause a memory leak due to there still being a reference count on the buffer.
            this.ReleaseBuffer(bufferImpl, this.ComponentReference.ForceStopProcessing || failed);

            // If this buffer signals the end of data stream, allow waiting thread to continue.
            if (this.ComponentReference.ForceStopProcessing || failed)
            {
                MMALLog.Logger.LogDebug($"{this.Name}: Signaling completion of continuous still frame capture...");
                Task.Run(() => { this.Trigger.SetResult(true); });
            }
        }
    }
}