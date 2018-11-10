// <copyright file="OutputPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Linq;
using System.Runtime.InteropServices;
using MMALSharp.Callbacks;
using MMALSharp.Callbacks.Providers;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    public class OutputPort : GenericPort, IOutputPort
    {
        public OutputCallbackHandlerBase ManagedOutputCallback { get; set; }
        
        /// <summary>
        /// Monitor lock for output port callback method.
        /// </summary>
        internal static object OutputLock = new object();

        public unsafe OutputPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid) 
            : base(ptr, comp, type, guid)
        {
        }
        
        public unsafe OutputPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid, ICaptureHandler handler) 
            : base(ptr, comp, type, guid, handler)
        {
        }
        
        /// <summary>
        /// Connects two components together by their input and output ports.
        /// </summary>
        /// <param name="destinationComponent">The component we want to connect to.</param>
        /// <param name="inputPort">The input port of the component we want to connect to.</param>
        /// <param name="useCallback">Flag to use connection callback (adversely affects performance).</param>
        /// <returns>The input port of the component we're connecting to - allows chain calling of this method.</returns>
        public IInputPort ConnectTo(MMALDownstreamComponent destinationComponent, int inputPort = 0, bool useCallback = false)
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

        /// <summary>
        /// Connects two components together by their input and output ports.
        /// </summary>
        /// <param name="destinationComponent">The component we want to connect to.</param>
        /// <param name="inputPort">The input port of the component we want to connect to.</param>
        /// <param name="callback">An operation we would like to carry out after connecting these components together.</param>
        /// <returns>The input port of the component we're connecting to - allows chain calling of this method.</returns>
        public IInputPort ConnectTo(MMALDownstreamComponent destinationComponent, int inputPort, Func<IPort> callback)
        {
            this.ConnectTo(destinationComponent, inputPort);
            callback();
            return destinationComponent.Inputs[inputPort];
        }
        
        /// <summary>
        /// Release an output port buffer, get a new one from the queue and send it for processing.
        /// </summary>
        /// <param name="bufferImpl">A managed buffer object.</param>
        public unsafe void ReleaseOutputBuffer(MMALBufferImpl bufferImpl)
        {
            bufferImpl.Release();
            bufferImpl.Dispose();
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
        
        /// <summary>
        /// Enables processing on an output port.
        /// </summary>
        /// <param name="sendBuffers">Indicates whether we want to send all the buffers in the port pool or simply create the pool.</param>
        public virtual unsafe void EnableOutputPort(bool sendBuffers = true)
        {            
            if (!this.Enabled)
            {
                this.ManagedOutputCallback = OutputCallbackProvider.FindCallback(this);

                this.NativeCallback = this.NativeOutputPortCallback;
                
                IntPtr ptrCallback = IntPtr.Zero;
                if (sendBuffers)
                {
                    ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);
                    this.PtrCallback = ptrCallback;
                }
                else
                {
                    ptrCallback = this.PtrCallback;
                }
                
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
        
        /// <summary>
        /// Enable the port specified.
        /// </summary>
        /// <param name="port">The port.</param>
        public void Start()
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
                
                MMALLog.Logger.Debug("Here check 1");
                
                var bufferImpl = new MMALBufferImpl(buffer);

                if (MMALCameraConfig.Debug)
                {
                    bufferImpl.PrintProperties();
                }
                
                var failed = bufferImpl.Properties.Any(c => c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED);
                var eos = bufferImpl.Properties.Any(c => c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END ||
                                                         c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS) || this.ComponentReference.ForceStopProcessing;

                if ((bufferImpl.CheckState() && bufferImpl.Length > 0 && !eos && !failed && !this.Trigger) || (eos && !this.Trigger))
                {
                    this.ManagedOutputCallback.Callback(bufferImpl);
                }
                
                MMALLog.Logger.Debug("Here check 2");
                
                // Ensure we release the buffer before any signalling or we will cause a memory leak due to there still being a reference count on the buffer.
                this.ReleaseOutputBuffer(bufferImpl);

                MMALLog.Logger.Debug("Here check 3");
                
                // If this buffer signals the end of data stream, allow waiting thread to continue.
                if (eos || failed)
                {
                    MMALLog.Logger.Debug($"{this.ComponentReference.Name} {this.Name} End of stream. Signaling completion...");
                    this.Trigger = true;
                    MMALLog.Logger.Debug("Here check 4");
                }
            }
        }
        
    }
}