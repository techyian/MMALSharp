// <copyright file="InputPort.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MMALSharp.Callbacks;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Ports.Inputs
{
    /// <summary>
    /// Represents an input port.
    /// </summary>
    public class InputPort : GenericPort<IInputCallbackHandler>, IInputPort
    {
        /// <summary>
        /// Creates a new instance of <see cref="InputPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public InputPort(IntPtr ptr, IComponent comp, Guid guid)
            : base(ptr, comp, PortType.Input, guid)
        {
        }

        /// <summary>
        /// Call to connect this input port to an output port. This method
        /// simply assigns the <see cref="IConnection"/> to the ConnectedReference property. 
        /// </summary>
        /// <param name="outputPort">The connected output port.</param>
        /// <param name="connection">The connection object.</param>
        public void ConnectTo(IOutputPort outputPort, IConnection connection)
        {
            this.ConnectedReference = connection;
        }
        
        /// <summary>
        /// Call to configure an input port.
        /// </summary>
        /// <param name="config">The port configuration object.</param>
        /// <param name="copyPort">The port to copy from.</param>
        /// <param name="handler">The capture handler to assign to this port.</param>
        public virtual void Configure(IMMALPortConfig config, IPort copyPort, IInputCaptureHandler handler)
        {
            copyPort?.ShallowCopy(this);

            if (config != null)
            {
                this.PortConfig = config;

                if (config.EncodingType != null)
                {
                    this.NativeEncodingType = config.EncodingType.EncodingVal;
                }

                if (config.PixelFormat != null)
                {
                    this.NativeEncodingSubformat = config.PixelFormat.EncodingVal;
                }

                if (config.Width > 0 && config.Height > 0)
                {
                    if (config.Crop.HasValue)
                    {
                        this.Crop = config.Crop.Value;
                    }
                    else
                    {
                        this.Crop = new Rectangle(0, 0, config.Width, config.Height);
                    }

                    this.Resolution = new Resolution(config.Width, config.Height);
                }
                else
                {
                    // Use config or don't set depending on port type.
                    this.Resolution = new Resolution(0, 0);

                    // Certain resolution overrides set to global config Video/Still resolutions so check here if the width and height are greater than 0.
                    if (this.Resolution.Width > 0 && this.Resolution.Height > 0)
                    {
                        this.Crop = new Rectangle(0, 0, this.Resolution.Width, this.Resolution.Height);
                    }
                }

                if (config.Framerate > 0)
                {
                    this.FrameRate = config.Framerate;
                }

                if (config.Bitrate > 0)
                {
                    this.Bitrate = config.Bitrate;
                }

                this.EncodingType = config.EncodingType;
                
                if (config.ZeroCopy)
                {
                    this.ZeroCopy = true;
                    this.SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);
                }

                this.BufferNum = Math.Max(this.BufferNumMin, config.BufferNum > 0 ? config.BufferNum : this.BufferNumRecommended);
                this.BufferSize = Math.Max(this.BufferSizeMin, config.BufferSize > 0 ? config.BufferSize : this.BufferSizeRecommended);

                this.Commit();

                this.CallbackHandler = new DefaultInputPortCallbackHandler(this, handler);
            }
        }

        /// <summary>
        /// Enables processing on an input port.
        /// </summary>
        public unsafe void Enable()
        {
            if (!this.Enabled)
            {
                this.NativeCallback = new MMALPort.MMAL_PORT_BH_CB_T(this.NativeInputPortCallback);

                IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);

                MMALLog.Logger.LogDebug($"{this.Name}: Enabling input port.");

                if (this.CallbackHandler == null)
                {
                    MMALLog.Logger.LogWarning($"{this.Name}: Callback null");
                    this.EnablePort(IntPtr.Zero);
                }
                else
                {
                    this.EnablePort(ptrCallback);
                }

                this.InitialiseBufferPool();
            }

            if (!this.Enabled)
            {
                throw new PiCameraError($"{this.Name}: Unknown error occurred whilst enabling port");
            }
        }
        
        /// <summary>
        /// Releases an input port buffer and reads further data from user provided image data if not reached end of file.
        /// </summary>
        /// <param name="bufferImpl">A managed buffer object.</param>
        public virtual void ReleaseBuffer(IBuffer bufferImpl)
        {
            bufferImpl.Release();

            if (this.Enabled && this.BufferPool != null && !this.Trigger.Task.IsCompleted)
            {
                IBuffer newBuffer;
                while (true)
                {
                    newBuffer = this.BufferPool.Queue.GetBuffer();
                    if (newBuffer != null)
                    {
                        break;
                    }
                }
                
                // Populate the new input buffer with user provided image data.
                var result = this.CallbackHandler.CallbackWithResult(newBuffer);

                if (result.Success)
                {
                    newBuffer.ReadIntoBuffer(result.BufferFeed, result.DataLength, result.EOF);
                }
              
                this.SendBuffer(newBuffer);

                if (result.EOF || this.ComponentReference.ForceStopProcessing)
                {
                    MMALLog.Logger.LogDebug($"{this.Name}: Received EOF. Releasing.");

                    Task.Run(() => { this.Trigger.SetResult(true); });
                }
            }
        }

        /// <summary>
        /// Starts the input port.
        /// </summary>
        public void Start()
        {
            MMALLog.Logger.LogDebug($"{this.Name}: Starting input port.");
            this.Trigger = new TaskCompletionSource<bool>();
            this.Enable();
        }

        /// <summary>
        /// Registers a new input callback handler with this port.
        /// </summary>
        /// <param name="callbackHandler">The callback handler.</param>
        public void RegisterCallbackHandler(IInputCallbackHandler callbackHandler)
        {
            this.CallbackHandler = callbackHandler;
        }

        internal virtual unsafe void NativeInputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.LogDebug($"{this.Name}: In native input callback.");
            }

            var bufferImpl = new MMALBufferImpl(buffer);

            if (bufferImpl.CheckState())
            {
                if (bufferImpl.Cmd > 0)
                {
                    if (bufferImpl.Cmd == MMALEvents.MMAL_EVENT_FORMAT_CHANGED)
                    {
                        MMALLog.Logger.LogInformation("EVENT FORMAT CHANGED");
                    }
                }
            }

            bufferImpl.PrintProperties();

            this.ReleaseBuffer(bufferImpl);
        }
    }
}
