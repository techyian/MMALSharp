// <copyright file="OutputPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MMALSharp.Callbacks;
using MMALSharp.Callbacks.Providers;
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
    public unsafe class OutputPort : PortBase, IOutputPort
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
        /// Output callback handler which is called by the native function callback.
        /// </summary>
        public virtual ICallbackHandler ManagedCallback { get; set; }
        
        /// <summary>
        /// Creates a new instance of <see cref="OutputPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public OutputPort(IntPtr ptr, MMALComponentBase comp, PortType type, Guid guid) 
            : base(ptr, comp, type, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="OutputPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        /// <param name="handler">The capture handler.</param>
        public OutputPort(IntPtr ptr, MMALComponentBase comp, PortType type, Guid guid, ICaptureHandler handler) 
            : base(ptr, comp, type, guid, handler)
        {
        }

        public void Configure(MMALPortConfig config, IInputPort copyFrom)
        {
            this.PortConfig = config;
            
            copyFrom.ShallowCopy(this);
            
            if (config.EncodingType != null)
            {
                this.NativeEncodingType = config.EncodingType.EncodingVal;
            }

            if (config.PixelFormat != null)
            {
                this.NativeEncodingSubformat = config.PixelFormat.EncodingVal;
            }

            MMAL_VIDEO_FORMAT_T tempVid = this.Ptr->Format->Es->Video;

            try
            {
                this.Commit();
            }
            catch
            {
                // If commit fails using new settings, attempt to reset using old temp MMAL_VIDEO_FORMAT_T.
                MMALLog.Logger.Warn("Commit of output port failed. Attempting to reset values.");
                this.Ptr->Format->Es->Video = tempVid;
                this.Commit();
            }

            if (config.EncodingType == MMALEncoding.JPEG)
            {
                this.SetParameter(MMALParametersCamera.MMAL_PARAMETER_JPEG_Q_FACTOR, config.Quality);
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

            if (config.Bitrate > 0)
            {
                this.Bitrate = config.Bitrate;
            }

            this.EncodingType = config.EncodingType;
            this.PixelFormat = config.PixelFormat;

            if (config.Width > 0 && config.Height > 0)
            {
                this.Resolution = new Resolution(config.Width, config.Height).Pad();
                this.Crop = new Rectangle(0, 0, this.Resolution.Width, this.Resolution.Height);
            }
            else
            {
                // Use config or don't set depending on port type.
                this.Resolution = new Resolution(0, 0);

                if (this.Resolution.Width > 0 && this.Resolution.Height > 0)
                {
                    this.Crop = new Rectangle(0, 0, this.Resolution.Width, this.Resolution.Height);
                }
            }

            // It is important to re-commit changes to width and height.
            this.Commit();

            this.BufferNum = Math.Max(this.Ptr->BufferNumRecommended, this.Ptr->BufferNumMin);
            this.BufferSize = Math.Max(this.Ptr->BufferSizeRecommended, this.Ptr->BufferSizeMin);

            this.ManagedCallback = PortCallbackProvider.FindCallback(this);
        }

        /// <summary>
        /// Connects two components together by their input and output ports.
        /// </summary>
        /// <param name="destinationComponent">The component we want to connect to.</param>
        /// <param name="inputPort">The input port of the component we want to connect to.</param>
        /// <param name="useCallback">Flag to use connection callback (adversely affects performance).</param>
        /// <returns>The input port of the component we're connecting to - allows chain calling of this method.</returns>
        public virtual IInputPort ConnectTo(IDownstreamComponent destinationComponent, int inputPort = 0, bool useCallback = false)
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
        /// Release an output port buffer, get a new one from the queue and send it for processing.
        /// </summary>
        /// <param name="bufferImpl">A managed buffer object.</param>
        public virtual void ReleaseBuffer(IBuffer bufferImpl)
        {
            bufferImpl.Release();
            
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
                    var newBuffer = this.BufferPool.Queue.GetBuffer();
                    
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
        public virtual void Enable(bool sendBuffers = true)
        {            
            if (!this.Enabled)
            {
                this.ManagedCallback = PortCallbackProvider.FindCallback(this);

                this.NativeCallback = this.NativeOutputPortCallback;
                
                IntPtr ptrCallback = Marshal.GetFunctionPointerForDelegate(this.NativeCallback);
                this.PtrCallback = ptrCallback;
                
                if (this.ManagedCallback == null)
                {
                    MMALLog.Logger.Warn("Callback null");

                    this.EnablePort(IntPtr.Zero);
                }
                else
                {
                    this.EnablePort(ptrCallback);
                }
                
                if (this.ManagedCallback != null)
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
        public void Start()
        {
            MMALLog.Logger.Debug($"Starting output port {this.Name}");
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
                MMALLog.Logger.Debug("In native output callback");
            }
            
            var bufferImpl = new MMALBufferImpl(buffer);

            bufferImpl.PrintProperties();
            
            var failed = bufferImpl.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_TRANSMISSION_FAILED);
            
            var eos = bufferImpl.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END) ||
                      bufferImpl.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS) ||
                      this.ComponentReference.ForceStopProcessing;

            if ((bufferImpl.CheckState() && bufferImpl.Length > 0 && !eos && !failed && !this.Trigger.Task.IsCompleted) || (eos && !this.Trigger.Task.IsCompleted))
            {
                this.ManagedCallback.Callback(bufferImpl);
            }
            
            // Ensure we release the buffer before any signalling or we will cause a memory leak due to there still being a reference count on the buffer.
            this.ReleaseBuffer(bufferImpl);

            // If this buffer signals the end of data stream, allow waiting thread to continue.
            if (eos || failed)
            {
                MMALLog.Logger.Debug($"{this.ComponentReference.Name} {this.Name} End of stream. Signaling completion...");
                
                Task.Run(() => { this.Trigger.SetResult(true); });
            }
        }
    }
}