// <copyright file="PortCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using Microsoft.Extensions.Logging;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// The base class for Output port callback handlers.
    /// </summary>
    /// <typeparam name="TPort">The port type.</typeparam>
    /// <typeparam name="TCaptureHandler">The capture handler type.</typeparam>
    public abstract class PortCallbackHandler<TPort, TCaptureHandler> : IOutputCallbackHandler
        where TPort : IPort
        where TCaptureHandler : IOutputCaptureHandler
    {
        private long? _ptsStartTime;
        private long? _ptsLastTime;

        /// <summary>
        /// The working port.
        /// </summary>
        public TPort WorkingPort { get; }

        /// <summary>
        /// The active capture handler.
        /// </summary>
        public TCaptureHandler CaptureHandler { get; }

        /// <summary>
        /// Creates a new instance of <see cref="PortCallbackHandler{TPort,TCaptureHandler}"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IPort"/>.</param>
        /// <param name="handler">The port capture handler.</param>
        protected PortCallbackHandler(TPort port, TCaptureHandler handler)
        {
            this.WorkingPort = port;
            this.CaptureHandler = handler;
        }
        
        /// <inheritdoc />
        public virtual void Callback(IBuffer buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.LogDebug($"In managed {this.WorkingPort.PortType.GetPortType()} callback");
            }

            long? pts = null;

            var data = buffer.GetBufferData();
            var eos = buffer.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END) || buffer.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS);
            var containsIFrame = buffer.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CONFIG);

            if (this is IVideoOutputCallbackHandler &&
                !buffer.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CONFIG) &&
                buffer.Pts != MMALUtil.MMAL_TIME_UNKNOWN &&
                (buffer.Pts != _ptsLastTime || !_ptsLastTime.HasValue))
            {
                if (!_ptsStartTime.HasValue)
                {
                    _ptsStartTime = buffer.Pts;
                }

                _ptsLastTime = buffer.Pts;
                pts = buffer.Pts - _ptsStartTime.Value;
            }

            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.LogDebug("Attempting to process data.");
            }
            
            this.CaptureHandler?.Process(new ImageContext
            {
                Data = data,
                Eos = eos,
                IFrame = containsIFrame,
                Resolution = this.WorkingPort.Resolution,
                Encoding = this.WorkingPort.EncodingType,
                PixelFormat = this.WorkingPort.PixelFormat,
                Raw = this.WorkingPort.EncodingType.EncType == MMALEncoding.EncodingType.PixelFormat,
                Pts = pts,
                Stride = MMALUtil.mmal_encoding_width_to_stride(WorkingPort.PixelFormat?.EncodingVal ?? this.WorkingPort.EncodingType.EncodingVal, this.WorkingPort.Resolution.Width)
            });

            if (eos)
            {
                // Once we have a full frame, perform any post processing as required.
                this.CaptureHandler?.PostProcess();
            }
        }
    }
}
