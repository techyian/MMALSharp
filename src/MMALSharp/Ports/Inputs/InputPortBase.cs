// <copyright file="InputPortBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MMALSharp.Callbacks;
using MMALSharp.Callbacks.Providers;
using MMALSharp.Common.Utility;
using MMALSharp.Handlers;
using MMALSharp.Native;
using static MMALSharp.MMALNativeExceptionHelper;

namespace MMALSharp.Ports.Inputs
{
    /// <summary>
    /// Represents an input port.
    /// </summary>
    public abstract class InputPortBase : PortBase
    {
        /// <inheritdoc />
        public override Resolution Resolution
        {
            get => new Resolution(this.Width, this.Height);
            internal set
            {
                this.Width = value.Pad().Width;
                this.Height = value.Pad().Height;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="InputPortBase"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        protected InputPortBase(IntPtr ptr, MMALComponentBase comp, PortType type, Guid guid)
            : base(ptr, comp, type, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="InputPortBase"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        /// <param name="handler">The capture handler.</param>
        protected InputPortBase(IntPtr ptr, MMALComponentBase comp, PortType type, Guid guid, ICaptureHandler handler)
            : base(ptr, comp, type, guid, handler)
        {
        }

        /// <summary>
        /// Managed callback which is called by the native function callback method.
        /// </summary>
        internal ICallbackHandler ManagedInputCallback { get; set; }

        /// <summary>
        /// Enables processing on an input port.
        /// </summary>
        internal virtual unsafe void EnableInputPort()
        {
            if (!this.Enabled)
            {
                this.ManagedInputCallback = PortCallbackProvider.FindCallback(this);

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

        internal virtual unsafe void NativeInputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.Debug("In native input callback");
            }

            var bufferImpl = new MMALBufferImpl(buffer);

            bufferImpl.PrintProperties();

            this.ReleaseInputBuffer(bufferImpl);
        }

        /// <summary>
        /// Releases an input port buffer and reads further data from user provided image data if not reached end of file.
        /// </summary>
        /// <param name="bufferImpl">A managed buffer object.</param>
        internal virtual void ReleaseInputBuffer(MMALBufferImpl bufferImpl)
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
                var result = this.ManagedInputCallback.CallbackWithResult(newBuffer);
                newBuffer.ReadIntoBuffer(result.BufferFeed, result.DataLength, result.EOF);

                try
                {
                    if (result.EOF)
                    {
                        MMALLog.Logger.Debug("Received EOF. Releasing.");

                        newBuffer.Release();
                        newBuffer = null;

                        Task.Run(() => { this.Trigger.SetResult(true); });
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
                    MMALLog.Logger.Error($"Buffer handling failed. {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts the input port.
        /// </summary>
        internal void Start()
        {
            this.EnableInputPort();
        }
    }
}