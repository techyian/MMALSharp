// <copyright file="MMALControlPort.cs" company="Techyian">
// Copyright (c) Techyian. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;
using MMALSharp.Native;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Represents a control port
    /// </summary>
    public unsafe class MMALControlPort : MMALPortBase
    {
        public MMALControlPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type)
            : base(ptr, comp, type)
        {
        }

        /// <summary>
        /// Enable processing on a port
        /// </summary>
        /// <param name="managedCallback">A managed callback method we can do further processing on</param>
        internal override void EnablePort(Action<MMALBufferImpl, MMALPortBase> managedCallback)
        {
            if (!this.Enabled)
            {
                this.ManagedControlCallback = managedCallback;

                this.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(this.NativeControlPortCallback);

                IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);

                MMALLog.Logger.Debug("Enabling control port.");

                if (managedCallback == null)
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
        /// The native callback MMAL passes buffer headers to
        /// </summary>
        /// <param name="port">The port the buffer is sent to</param>
        /// <param name="buffer">The buffer header</param>
        internal override void NativeControlPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (MMALPortBase.ControlLock)
            {
                if (MMALCameraConfig.Debug)
                {
                    MMALLog.Logger.Debug("In native control callback");
                }

                var bufferImpl = new MMALBufferImpl(buffer);

                if (bufferImpl.Ptr != null && (IntPtr)bufferImpl.Ptr != IntPtr.Zero)
                {
                    if (MMALCameraConfig.Debug)
                    {
                        bufferImpl.ParseEvents();
                    }

                    this.ManagedControlCallback(bufferImpl, this);

                    if (MMALCameraConfig.Debug)
                    {
                        MMALLog.Logger.Debug("Releasing buffer");
                    }

                    bufferImpl.Release();
                }
            }
        }
    }
}
