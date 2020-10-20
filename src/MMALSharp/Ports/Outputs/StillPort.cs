// <copyright file="StillPort.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using Microsoft.Extensions.Logging;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports.Inputs;

namespace MMALSharp.Ports.Outputs
{
    /// <summary>
    /// Represents a still image encoder/decoder port.
    /// </summary>
    public unsafe class StillPort : OutputPort, IStillPort
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
        /// Creates a new instance of <see cref="StillPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public StillPort(IntPtr ptr, IComponent comp, Guid guid)
            : base(ptr, comp, guid)
        {
        }
        
        /// <summary>
        /// Creates a new instance of <see cref="StillPort"/>.
        /// </summary>
        /// <param name="copyFrom">The port to copy data from.</param>
        public StillPort(IPort copyFrom)
            : base((IntPtr)copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.Guid)
        {
        }

        /// <inheritdoc />
        public override void Configure(IMMALPortConfig config, IInputPort copyFrom, IOutputCaptureHandler handler)
        {
            base.Configure(config, copyFrom, handler);

            if (config != null && config.EncodingType == MMALEncoding.JPEG)
            {
                this.SetParameter(MMALParametersCamera.MMAL_PARAMETER_JPEG_Q_FACTOR, config.Quality);
            }
        }

        internal override void NativeOutputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.LogDebug($"{this.Name}: In native {nameof(StillPort)} output callback");
            }

            base.NativeOutputPortCallback(port, buffer);
        }
    }    
}
