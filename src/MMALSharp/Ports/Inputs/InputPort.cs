// <copyright file="InputPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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
    public class InputPort : PortBase<IInputCallbackHandler>, IInputPort
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
        /// Creates a new instance of <see cref="InputPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public InputPort(IntPtr ptr, IComponent comp, PortType type, Guid guid)
            : base(ptr, comp, type, guid)
        {
        }

        public void ConnectTo(IOutputPort outputPort, IConnection connection)
        {
            this.ConnectedReference = connection;
        }

        public virtual void Configure(MMALPortConfig config, IInputCaptureHandler handler)
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
                this.Resolution = new Resolution(config.Width, config.Height);
            }
            else
            {
                // Use config.
                this.Resolution = new Resolution(0, 0);
            }

            if (config.Framerate > 0)
            {
                this.FrameRate = new MMAL_RATIONAL_T(config.Framerate, 1);
            }

            if (config.Bitrate > 0)
            {
                this.Bitrate = config.Bitrate;
            }

            this.EncodingType = config.EncodingType;

            this.Commit();

            if (config.ZeroCopy)
            {
                this.ZeroCopy = true;
                this.SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);
            }

            this.BufferNum = Math.Max(this.BufferNumMin, config.BufferNum > 0 ? config.BufferNum : this.BufferNumRecommended);
            this.BufferSize = Math.Max(this.BufferSizeMin, config.BufferSize > 0 ? config.BufferSize : this.BufferSizeRecommended);
            this.CallbackHandler = new DefaultInputPortCallbackHandler(this, handler);
        }

        public void Configure(MMALPortConfig config, IPort copyPort, IInputCaptureHandler handler, bool zeroCopy = false)
        {
            copyPort?.ShallowCopy(this);

            if (config.EncodingType != null)
            {
                this.NativeEncodingType = config.EncodingType.EncodingVal;
            }

            if (config.PixelFormat != null)
            {
                this.NativeEncodingSubformat = config.PixelFormat.EncodingVal;
            }

            this.Commit();

            if (zeroCopy)
            {
                this.ZeroCopy = true;
                this.SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);
            }

            this.BufferNum = Math.Max(this.BufferNumMin, config.BufferNum > 0 ? config.BufferNum : this.BufferNumRecommended);
            this.BufferSize = Math.Max(this.BufferSizeMin, config.BufferSize > 0 ? config.BufferSize : this.BufferSizeRecommended);

            if (this.CallbackHandler == null)
            {
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

                MMALLog.Logger.Debug("Enabling input port.");

                if (this.CallbackHandler == null)
                {
                    MMALLog.Logger.Warn("Callback null");
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

            this.ReleaseBuffer(bufferImpl);
        }

        /// <summary>
        /// Releases an input port buffer and reads further data from user provided image data if not reached end of file.
        /// </summary>
        /// <param name="bufferImpl">A managed buffer object.</param>
        public virtual void ReleaseBuffer(IBuffer bufferImpl)
        {
            bufferImpl.Release();

            if (this.Enabled && this.BufferPool != null)
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
        public void Start()
        {
            MMALLog.Logger.Debug($"Starting input port {this.Name}");
            this.Trigger = new TaskCompletionSource<bool>();
            this.Enable();
        }

        public void RegisterCallbackHandler(IInputCallbackHandler callbackHandler)
        {
            this.CallbackHandler = callbackHandler;
        }
    }
}
