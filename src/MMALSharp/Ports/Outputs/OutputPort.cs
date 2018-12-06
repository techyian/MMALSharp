// <copyright file="OutputPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;
using MMALSharp.Callbacks;
using MMALSharp.Callbacks.Providers;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports.Inputs;

namespace MMALSharp.Ports.Outputs
{
    /// <summary>
    /// Represents an output port.
    /// </summary>
    public class OutputPort : OutputPortBase
    {
        /// <inheritdoc />
        internal override IOutputCallbackHandler ManagedOutputCallback { get; set; }
        
        /// <summary>
        /// Monitor lock for output port callback method.
        /// </summary>
        internal static object OutputLock = new object();

        /// <summary>
        /// Creates a new instance of <see cref="OutputPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public unsafe OutputPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid) 
            : base(ptr, comp, type, guid)
        {
        }
        
        /// <summary>
        /// Creates a new instance of <see cref="OutputPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        /// <param name="handler">The capture handler.</param>
        public unsafe OutputPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid, ICaptureHandler handler) 
            : base(ptr, comp, type, guid, handler)
        {
        }
        
        /// <inheritdoc />
        public override InputPortBase ConnectTo(MMALDownstreamComponent destinationComponent, int inputPort = 0, bool useCallback = false)
        {
            if (this.ConnectedReference != null)
            {
                MMALLog.Logger.Warn("A connection has already been established on this port");
                return destinationComponent.Inputs[inputPort];
            }

            var connection = MMALConnectionImpl.CreateConnection(this, destinationComponent.Inputs[inputPort], destinationComponent, useCallback);
            this.ConnectedReference = connection;
            destinationComponent.Inputs[inputPort].ConnectedReference = connection;

            return destinationComponent.Inputs[inputPort];
        }

        /// <inheritdoc />
        public override InputPortBase ConnectTo(MMALDownstreamComponent destinationComponent, int inputPort, Func<PortBase> callback)
        {
            this.ConnectTo(destinationComponent, inputPort);
            callback();
            return destinationComponent.Inputs[inputPort];
        }
        
        /// <inheritdoc />
        internal override unsafe void ReleaseOutputBuffer(MMALBufferImpl bufferImpl)
        {
            bufferImpl.Release();
            
            try
            {
                if (!this.Enabled)
                {
                    MMALLog.Logger.Warn("Port not enabled.");
                }

                if (this.BufferPool == null)
                {
                    MMALLog.Logger.Warn("Buffer pool null.");
                }

                if (this.Enabled && this.BufferPool != null)
                {
                    var newBuffer = MMALQueueImpl.GetBuffer(this.BufferPool.Queue.Ptr);
                    
                    if (newBuffer != null)
                    {
                        this.SendBuffer(newBuffer);
                    }
                    else
                    {
                        MMALLog.Logger.Warn("Buffer null. Continuing.");
                    }
                }
            }
            catch (Exception e)
            {
                MMALLog.Logger.Warn($"Unable to send buffer header. {e.Message}");
            }
        }
        
        /// <inheritdoc />
        internal override unsafe void EnableOutputPort(bool sendBuffers = true)
        {            
            if (!this.Enabled)
            {
                this.ManagedOutputCallback = OutputCallbackProvider.FindCallback(this);

                this.NativeCallback = this.NativeOutputPortCallback;
                
                IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);
                this.PtrCallback = ptrCallback;
                
                if (this.ManagedOutputCallback == null)
                {
                    MMALLog.Logger.Warn("Callback null");

                    this.EnablePort(IntPtr.Zero);
                }
                else
                {
                    this.EnablePort(ptrCallback);
                }
                
                if (this.ManagedOutputCallback != null)
                {
                    this.SendAllBuffers(sendBuffers);
                }
            }
            
            if (!this.Enabled)
            {
                throw new PiCameraError("Unknown error occurred whilst enabling port");
            }
        }
        
        /// <inheritdoc />
        internal override void Start()
        {
            MMALLog.Logger.Debug($"Starting output port {this.Name}");
            
            if (this.Handler != null && this.Handler.GetType().IsSubclassOf(typeof(FileStreamCaptureHandler)))
            {
                ((FileStreamCaptureHandler)this.Handler).NewFile();
            }
            
            this.EnableOutputPort();
        }
        
        /// <summary>
        /// The native callback MMAL passes buffer headers to.
        /// </summary>
        /// <param name="port">The port the buffer is sent to.</param>
        /// <param name="buffer">The buffer header.</param>
        internal virtual unsafe void NativeOutputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (OutputLock)
            {
                if (MMALCameraConfig.Debug)
                {
                    MMALLog.Logger.Debug("In native output callback");
                }
                
                var bufferImpl = new MMALBufferImpl(buffer);

                bufferImpl.PrintProperties();
                
                var failed = bufferImpl.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED);
                
                var eos = bufferImpl.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END) ||
                          bufferImpl.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS) ||
                          this.ComponentReference.ForceStopProcessing;

                if ((bufferImpl.CheckState() && bufferImpl.Length > 0 && !eos && !failed && !this.Trigger) || (eos && !this.Trigger))
                {
                    this.ManagedOutputCallback.Callback(bufferImpl);
                }
                
                // Ensure we release the buffer before any signalling or we will cause a memory leak due to there still being a reference count on the buffer.
                this.ReleaseOutputBuffer(bufferImpl);

                // If this buffer signals the end of data stream, allow waiting thread to continue.
                if (eos || failed)
                {
                    MMALLog.Logger.Debug($"{this.ComponentReference.Name} {this.Name} End of stream. Signaling completion...");
                    this.Trigger = true;
                }
            }
        }
    }
}