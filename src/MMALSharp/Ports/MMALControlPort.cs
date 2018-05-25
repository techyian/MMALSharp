// <copyright file="MMALControlPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;
using MMALSharp.Callbacks;
using MMALSharp.Native;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Represents a control port.
    /// </summary>
    public unsafe class MMALControlPort : MMALPortImpl
    {
        public MMALControlPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type)
            : base(ptr, comp, type)
        {
        }

        /// <summary>
        /// Managed Control port callback delegate.
        /// </summary>
        public ICallbackHandler ManagedControlCallback { get; set; }

        /// <summary>
        /// Monitor lock for control port callback method.
        /// </summary>
        internal static object ControlLock = new object();

        /// <summary>
        /// Enables processing on a port.
        /// </summary>
        internal override void EnableControlPort()
        {
            if (!this.Enabled)
            {
                this.ManagedControlCallback = OutputCallbackProvider.FindCallback(this);

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

        /// <summary>
        /// The native callback MMAL passes buffer headers to.
        /// </summary>
        /// <param name="port">The port the buffer is sent to.</param>
        /// <param name="buffer">The buffer header.</param>
        internal override void NativeControlPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (MMALControlPort.ControlLock)
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
                        bufferImpl.PrintProperties();
                    }

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
