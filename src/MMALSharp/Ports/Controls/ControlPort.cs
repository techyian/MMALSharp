// <copyright file="ControlPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;
using MMALSharp.Callbacks;
using MMALSharp.Common.Utility;
using MMALSharp.Native;

namespace MMALSharp.Ports.Controls
{
    /// <summary>
    /// Represents a control port.
    /// </summary>
    public unsafe class ControlPort : PortBase<IOutputCallbackHandler>, IControlPort
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
        /// Monitor lock for control port callback method.
        /// </summary>
        private static object ControlLock = new object();

        /// <summary>
        /// Enables processing on a control port.
        /// </summary>
        public void Enable()
        {
            if (!this.Enabled)
            {
                this.CallbackHandler = new DefaultPortCallbackHandler(this, null);
                
                this.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(this.NativeControlPortCallback);

                IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);

                MMALLog.Logger.Debug("Enabling control port.");

                if (this.CallbackHandler == null)
                {
                    MMALLog.Logger.Debug("Callback null");
                    this.EnablePort(IntPtr.Zero);
                }
                else
                {
                    this.EnablePort(ptrCallback);
                }
            }
        }

        /// <summary>
        /// Starts the control port.
        /// </summary>
        public void Start()
        {
            this.Enable();
        }

        /// <summary>
        /// This is the camera's control port callback function. The callback is used if
        /// MMALCameraConfig.SetChangeEventRequest is set to true.
        /// </summary>
        /// <param name="port">Native port struct pointer.</param>
        /// <param name="buffer">Native buffer header pointer.</param>
        private void NativeControlPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            lock (ControlLock)
            {
                if (buffer->Cmd == MMALEvents.MMAL_EVENT_PARAMETER_CHANGED)
                {
                    var data = (MMAL_EVENT_PARAMETER_CHANGED_T*)buffer->Data;

                    if (data->Hdr.Id == MMALParametersCamera.MMAL_PARAMETER_CAMERA_SETTINGS)
                    {
                        var settings = (MMAL_PARAMETER_CAMERA_SETTINGS_T*)data;

                        MMALLog.Logger.Debug($"Analog gain num {settings->AnalogGain.Num}");
                        MMALLog.Logger.Debug($"Analog gain den {settings->AnalogGain.Den}");
                        MMALLog.Logger.Debug($"Exposure {settings->Exposure}");
                        MMALLog.Logger.Debug($"Focus position {settings->FocusPosition}");
                    }
                }
                else if (buffer->Cmd == MMALEvents.MMAL_EVENT_ERROR)
                {
                    MMALLog.Logger.Info("No data received from sensor. Check all connections, including the Sunny one on the camera board");
                }
                else
                {
                    MMALLog.Logger.Info("Received unexpected camera control callback event");
                }

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

                    this.CallbackHandler.Callback(bufferImpl);

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
