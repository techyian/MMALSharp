// <copyright file="OutputPort.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MMALSharp.Callbacks;
using MMALSharp.Common;
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
    public unsafe class OutputPort : GenericPort<IOutputCallbackHandler>, IOutputPort
    {                
        /// <summary>
        /// Creates a new instance of <see cref="OutputPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public OutputPort(IntPtr ptr, IComponent comp, Guid guid) 
            : base(ptr, comp, PortType.Output, guid)
        {
        }

        /// <summary>
        /// Call to configure an output port.
        /// </summary>
        /// <param name="config">The port configuration object.</param>
        /// <param name="copyFrom">The port to copy from.</param>
        /// <param name="handler">The capture handler to assign to this port.</param>
        public virtual void Configure(IMMALPortConfig config, IInputPort copyFrom, IOutputCaptureHandler handler)
        {
            if (config != null)
            {
                this.PortConfig = config;

                copyFrom?.ShallowCopy(this);

                if (config.EncodingType != null)
                {
                    this.NativeEncodingType = config.EncodingType.EncodingVal;
                }

                if (config.PixelFormat != null)
                {
                    this.NativeEncodingSubformat = config.PixelFormat.EncodingVal;
                }

                this.Par = new MMAL_RATIONAL_T(1, 1);

                MMAL_VIDEO_FORMAT_T tempVid = this.Ptr->Format->Es->Video;

                try
                {
                    this.Commit();
                }
                catch
                {
                    // If commit fails using new settings, attempt to reset using old temp MMAL_VIDEO_FORMAT_T.
                    MMALLog.Logger.LogWarning($"{this.Name}: Commit of output port failed. Attempting to reset values.");
                    this.Ptr->Format->Es->Video = tempVid;
                    this.Commit();
                }
                
                if (config.ZeroCopy)
                {
                    this.ZeroCopy = true;
                    this.SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);
                }

                if (MMALCameraConfig.VideoColorSpace != null &&
                    MMALCameraConfig.VideoColorSpace.EncType == MMALEncoding.EncodingType.ColorSpace)
                {
                    this.VideoColorSpace = MMALCameraConfig.VideoColorSpace;
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
                this.PixelFormat = config.PixelFormat;

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

                this.BufferNum = Math.Max(this.BufferNumMin, config.BufferNum > 0 ? config.BufferNum : this.BufferNumRecommended);
                this.BufferSize = Math.Max(this.BufferSizeMin, config.BufferSize > 0 ? config.BufferSize : this.BufferSizeRecommended);

                // It is important to re-commit changes to width and height.
                this.Commit();
            }
            
            this.CallbackHandler = new DefaultOutputPortCallbackHandler(this, handler);
        }

        /// <summary>
        /// Connects two components together by their input and output ports.
        /// </summary>
        /// <param name="destinationComponent">The component we want to connect to.</param>
        /// <param name="inputPort">The input port of the component we want to connect to.</param>
        /// <param name="useCallback">Flag to use connection callback (adversely affects performance).</param>
        /// <returns>The connection instance between the source output and destination input ports.</returns>
        public virtual IConnection ConnectTo(IDownstreamComponent destinationComponent, int inputPort = 0, bool useCallback = false)
        {
            if (this.ConnectedReference != null)
            {
                MMALLog.Logger.LogWarning($"{this.Name}: A connection has already been established on this port");
                return this.ConnectedReference;
            }

            var connection = MMALConnectionImpl.CreateConnection(this, destinationComponent.Inputs[inputPort], destinationComponent, useCallback);
            this.ConnectedReference = connection;

            destinationComponent.Inputs[inputPort].ConnectTo(this, connection);

            return connection;
        }

        /// <summary>
        /// Release an output port buffer, get a new one from the queue and send it for processing.
        /// </summary>
        /// <param name="bufferImpl">A managed buffer object.</param>
        /// <param name="eos">Flag that this buffer is the end of stream.</param>
        public virtual void ReleaseBuffer(IBuffer bufferImpl, bool eos)
        {
            bufferImpl.Release();
            
            if (eos)
            {
                // If we have reached the end of stream, we don't want to send a buffer to the output port again.
                return;
            }

            IBuffer newBuffer = null;

            try
            {
                if (MMALCameraConfig.Debug)
                {
                    if (!this.Enabled)
                    {
                        MMALLog.Logger.LogDebug($"{this.Name}: Port not enabled.");
                    }

                    if (this.BufferPool == null)
                    {
                        MMALLog.Logger.LogDebug($"{this.Name}: Buffer pool null.");
                    }
                }
                
                if (this.Enabled && this.BufferPool != null)
                {
                    newBuffer = this.BufferPool.Queue.GetBuffer();
                    
                    if (newBuffer.CheckState())
                    {
                        this.SendBuffer(newBuffer);
                    }
                    else
                    {
                        MMALLog.Logger.LogWarning($"{this.Name}: Buffer null. Continuing.");
                    }
                }
            }
            catch (Exception e)
            {
                if (newBuffer != null && newBuffer.CheckState())
                {
                    newBuffer.Release();
                }

                MMALLog.Logger.LogWarning($"{this.Name}: Unable to send buffer header. {e.Message}");
            }
        }

        /// <summary>
        /// Call to register a new callback handler with this port.
        /// </summary>
        /// <param name="callbackHandler">The output callback handler.</param>
        public void RegisterCallbackHandler(IOutputCallbackHandler callbackHandler)
        {
            this.CallbackHandler = callbackHandler;
        }

        /// <summary>
        /// Enables processing on an output port.
        /// </summary>
        public virtual void Enable()
        {            
            if (!this.Enabled)
            {
                this.NativeCallback = this.NativeOutputPortCallback;
                
                IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);
                this.PtrCallback = ptrCallback;
                
                if (this.CallbackHandler == null)
                {
                    MMALLog.Logger.LogWarning($"{this.Name}: Callback null");

                    this.EnablePort(IntPtr.Zero);
                }
                else
                {
                    this.EnablePort(ptrCallback);
                }
                
                if (this.CallbackHandler != null)
                {
                    this.SendAllBuffers();
                }
            }
            
            if (!this.Enabled)
            {
                throw new PiCameraError($"{this.Name}: Unknown error occurred whilst enabling port");
            }
        }

        /// <summary>
        /// Enable the port specified.
        /// </summary>
        public void Start()
        {
            MMALLog.Logger.LogDebug($"{this.Name}: Starting output port.");
            this.Trigger = new TaskCompletionSource<bool>();
            this.Enable();
        }

        /// <summary>
        /// The native callback MMAL passes buffer headers to.
        /// </summary>
        /// <param name="port">The port the buffer is sent to.</param>
        /// <param name="buffer">The buffer header.</param>
        internal virtual void NativeOutputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.LogDebug($"{this.Name}: In native output callback");
            }

            var bufferImpl = new MMALBufferImpl(buffer);

            bufferImpl.PrintProperties();

            var failed = bufferImpl.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED);

            var eos = bufferImpl.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END) ||
                      bufferImpl.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS) ||
                      this.ComponentReference.ForceStopProcessing ||
                      bufferImpl.Length == 0;

            if ((bufferImpl.CheckState() && bufferImpl.Length > 0 && !eos && !failed && !this.Trigger.Task.IsCompleted) || (eos && !this.Trigger.Task.IsCompleted && bufferImpl.Length > 0))
            {
                this.CallbackHandler.Callback(bufferImpl);
            }

            // Ensure we release the buffer before any signalling or we will cause a memory leak due to there still being a reference count on the buffer.
            this.ReleaseBuffer(bufferImpl, eos);

            // If this buffer signals the end of data stream, allow waiting thread to continue.
            if (eos || failed)
            {
                MMALLog.Logger.LogDebug($"{this.Name}: End of stream. Signaling completion...");

                Task.Run(() => { this.Trigger.SetResult(true); });
            }
        }
    }
}