// <copyright file="VideoPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Threading.Tasks;
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
        /// <inheritdoc />
        public override Resolution Resolution
        {
            get => new Resolution(this.Width, this.Height);
            internal set
            {
                if (value.Width == 0 || value.Height == 0)
                {
                    this.Width = MMALCameraConfig.VideoResolution.Pad().Width;
                    this.Height = MMALCameraConfig.VideoResolution.Pad().Height;
                }
                else
                {
                    this.Width = value.Pad().Width;
                    this.Height = value.Pad().Height;
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
        public override void Configure(MMALPortConfig config, IInputPort copyFrom, IOutputCaptureHandler handler)
        {
            base.Configure(config, copyFrom, handler);

            this.CallbackHandler = new VideoOutputCallbackHandler(this, (IVideoCaptureHandler)handler, null, config.Split);
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
                MMALLog.Logger.Debug($"In native {nameof(VideoPort)} output callback");
            }

            var bufferImpl = new MMALBufferImpl(buffer);

            bufferImpl.PrintProperties();
            
            var eos = (this.PortConfig.Timeout.HasValue && DateTime.Now.CompareTo(this.PortConfig.Timeout.Value) > 0) || this.ComponentReference.ForceStopProcessing;

            if (bufferImpl.CheckState() && bufferImpl.Length > 0 && !eos && !this.Trigger.Task.IsCompleted)
            {
                this.CallbackHandler.Callback(bufferImpl);
            }
            
            // Ensure we release the buffer before any signalling or we will cause a memory leak due to there still being a reference count on the buffer.
            this.ReleaseBuffer(bufferImpl);

            if (eos)
            {
                if (!this.Trigger.Task.IsCompleted)
                {
                    MMALLog.Logger.Debug($"{this.ComponentReference.Name} {this.Name} Timeout exceeded, triggering signal.");
                    Task.Run(() => { this.Trigger.SetResult(true); });
                }
            }
        }
    }
}
