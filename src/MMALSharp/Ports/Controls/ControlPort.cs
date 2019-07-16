// <copyright file="ControlPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;
using MMALSharp.Callbacks;
using MMALSharp.Callbacks.Providers;
using MMALSharp.Common.Utility;
using MMALSharp.Native;
using static MMALSharp.MMALNativeExceptionHelper;

namespace MMALSharp.Ports.Controls
{
    /// <summary>
    /// Represents a control port.
    /// </summary>
    public unsafe class ControlPort : ControlPortBase
    {
        /// <inheritdoc />
        public override Resolution Resolution
        {
            get => new Resolution(0, 0);
            internal set
            {
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ControlPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public ControlPort(IntPtr ptr, MMALComponentBase comp, PortType type, Guid guid)
            : base(ptr, comp, type, guid)
        {
        }

        /// <summary>
        /// A callback handler for Control ports we use to do further processing on buffer headers after they've been received by the native callback delegate.
        /// </summary>
        internal override ICallbackHandler ManagedControlCallback { get; set; }

        /// <summary>
        /// Monitor lock for control port callback method.
        /// </summary>
        private static object ControlLock = new object();
        
        /// <inheritdoc />
        internal override void EnableControlPort()
        {
            if (!this.Enabled)
            {
                this.ManagedControlCallback = PortCallbackProvider.FindCallback(this);

                this.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(this.NativeControlPortCallback);

                IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);

                MMALLog.Logger.Debug("Enabling control port.");

                if (this.ManagedControlCallback == null)
                {
                    MMALLog.Logger.Debug("Callback null");

                    MMALCheck(MMALPort.mmal_port_enable(this.Ptr, IntPtr.Zero), "Unable to enable port.");
                }
                else
                {
                    MMALCheck(MMALPort.mmal_port_enable(this.Ptr, ptrCallback), "Unable to enable port.");
                }
            }
        }

        /// <inheritdoc />
        internal override void Start()
        {
            this.EnableControlPort();
        }

        /// <summary>
        /// Represents the native callback method for a control port that's called by MMAL.
        /// </summary>
        /// <param name="port">Native port struct pointer.</param>
        /// <param name="buffer">Native buffer header pointer.</param>
        private void NativeControlPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (ControlLock)
            {
                if (MMALCameraConfig.Debug)
                {
                    MMALLog.Logger.Debug("In native control callback.");
                }

                var bufferImpl = new MMALBufferImpl(buffer);

                if (bufferImpl.CheckState())
                {
                    if (MMALCameraConfig.Debug)
                    {
                        bufferImpl.ParseEvents();
                    }

                    bufferImpl.PrintProperties();

                    this.ManagedControlCallback.Callback(bufferImpl);

                    if (MMALCameraConfig.Debug)
                    {
                        MMALLog.Logger.Debug("Releasing buffer.");
                    }

                    bufferImpl.Release();
                }
                else
                {
                    MMALLog.Logger.Warn("Received null control buffer.");
                }
            }
        }
    }
}
