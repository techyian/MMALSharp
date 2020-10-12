// <copyright file="VideoPort.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MMALSharp.Callbacks;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports.Inputs;

namespace MMALSharp.Ports.Outputs
{
    /// <summary>
    /// Represents a video encode/decode port
    /// </summary>
    public unsafe class VideoPort : OutputPort, IVideoPort
    {
        private Resolution _resolution;

        /// <inheritdoc />
        public override Resolution Resolution
        {
            get
            {
                if (_resolution.Width == 0 || _resolution.Height == 0)
                {
                    _resolution = new Resolution(MMALCameraConfig.Resolution.Width, MMALCameraConfig.Resolution.Height);
                }

                return _resolution;
            }

            internal set
            {
                if (value.Width == 0 || value.Height == 0)
                {
                    this.NativeWidth = MMALCameraConfig.Resolution.Pad().Width;
                    this.NativeHeight = MMALCameraConfig.Resolution.Pad().Height;
                    _resolution = new Resolution(MMALCameraConfig.Resolution.Width, MMALCameraConfig.Resolution.Height);
                }
                else
                {
                    this.NativeWidth = value.Pad().Width;
                    this.NativeHeight = value.Pad().Height;
                    _resolution = new Resolution(value.Width, value.Height);
                }
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="VideoPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="guid">Managed unique identifier for this port.</param>
        public VideoPort(IntPtr ptr, IComponent comp, Guid guid)
            : base(ptr, comp, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="VideoPort"/>.
        /// </summary>
        /// <param name="copyFrom">The port to copy data from.</param>
        public VideoPort(IPort copyFrom)
            : base((IntPtr)copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.Guid)
        {
        }

        /// <inheritdoc />
        public override void Configure(IMMALPortConfig config, IInputPort copyFrom, IOutputCaptureHandler handler)
        {
            base.Configure(config, copyFrom, handler);
            
            this.CallbackHandler = new VideoOutputCallbackHandler(this, (IVideoCaptureHandler)handler, config.Split, config.StoreMotionVectors);
        }

        /// <summary>
        /// The native callback MMAL passes buffer headers to.
        /// </summary>
        /// <param name="port">The port the buffer is sent to.</param>
        /// <param name="buffer">The buffer header.</param>
        internal override void NativeOutputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.LogDebug($"{this.Name}: In native {nameof(VideoPort)} output callback");
            }

            var bufferImpl = new MMALBufferImpl(buffer);

            bufferImpl.PrintProperties();
            
            var eos = (this.PortConfig.Timeout.HasValue && DateTime.Now.CompareTo(this.PortConfig.Timeout.Value) > 0) || this.ComponentReference.ForceStopProcessing;

            if (bufferImpl.CheckState() && bufferImpl.Length > 0 && !eos && !this.Trigger.Task.IsCompleted)
            {
                this.CallbackHandler.Callback(bufferImpl);
            }
            
            // Ensure we release the buffer before any signalling or we will cause a memory leak due to there still being a reference count on the buffer.
            this.ReleaseBuffer(bufferImpl, eos);

            if (eos && !this.Trigger.Task.IsCompleted)
            {
                MMALLog.Logger.LogDebug($"{this.Name}: Timeout exceeded, triggering signal.");
                Task.Run(() => { this.Trigger.SetResult(true); });
            }
        }
    }
}
