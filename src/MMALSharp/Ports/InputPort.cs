// <copyright file="InputPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;
using MMALSharp.Callbacks;
using MMALSharp.Callbacks.Providers;
using MMALSharp.Native;
using static MMALSharp.MMALNativeExceptionHelper;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Represents an input port.
    /// </summary>
    public class InputPort : GenericPort, IInputPort
    {
        /// <inheritdoc />
        public IInputCallbackHandler ManagedInputCallback { get; set; }
        
        /// <summary>
        /// Monitor lock for input port callback method.
        /// </summary>
        internal static object InputLock = new object();

        /// <summary>
        /// Creates a new instance of <see cref="InputPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public unsafe InputPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid) 
            : base(ptr, comp, type, guid)
        {
        }
        
        /// <inheritdoc />
        public virtual unsafe void EnableInputPort()
        {
            if (!this.Enabled)
            {
                this.ManagedInputCallback = InputCallbackProvider.FindCallback(this);

                this.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(this.NativeInputPortCallback);

                IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);

                MMALLog.Logger.Debug("Enabling input port.");

                if (this.ManagedInputCallback == null)
                {
                    MMALLog.Logger.Warn("Callback null");

                    MMALCheck(MMALPort.mmal_port_enable(this.Ptr, IntPtr.Zero), "Unable to enable port.");
                }
                else
                {
                    MMALCheck(MMALPort.mmal_port_enable(this.Ptr, ptrCallback), "Unable to enable port.");
                }

                this.InitialiseBufferPool();
            }

            if (!this.Enabled)
            {
                throw new PiCameraError("Unknown error occurred whilst enabling port");
            }
        }
        
        /// <inheritdoc />
        public void ReleaseInputBuffer(MMALBufferImpl bufferImpl)
        {            
            bufferImpl.Release();
            
            if (this.Enabled && this.BufferPool != null)
            {
                MMALBufferImpl newBuffer;
                while (true)
                {
                    newBuffer = this.BufferPool.Queue.GetBuffer();
                    if (newBuffer != null)
                    {
                        break;
                    }
                }
                
                // Populate the new input buffer with user provided image data.
                var result = this.ManagedInputCallback.Callback(newBuffer);
                newBuffer.ReadIntoBuffer(result.BufferFeed, result.DataLength, result.EOF);

                try
                {
                    if (!this.Trigger && result.EOF)
                    {
                        MMALLog.Logger.Debug("Received EOF. Releasing.");
                        
                        newBuffer.Release();                        
                        newBuffer = null;
                        this.Trigger = true;
                    }

                    if (newBuffer != null)
                    {
                        this.SendBuffer(newBuffer);
                    }
                    else
                    {
                        MMALLog.Logger.Warn("Buffer null. Continuing.");
                    }
                }
                catch (Exception ex)
                {
                    MMALLog.Logger.Warn($"Buffer handling failed. {ex.Message}");
                    throw;
                }
            }
        }

        /// <inheritdoc />
        public void Start()
        {
            this.EnableInputPort();
        }

        internal virtual unsafe void NativeInputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (InputLock)
            {
                if (MMALCameraConfig.Debug)
                {
                    MMALLog.Logger.Debug("In native input callback");
                }

                var bufferImpl = new MMALBufferImpl(buffer);

                if (MMALCameraConfig.Debug)
                {
                    bufferImpl.PrintProperties();
                }

                this.ReleaseInputBuffer(bufferImpl);
            }
        }
    }
}