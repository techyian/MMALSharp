// <copyright file="FastImageOutputCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// A callback handler specifically for rapid image capture from the camera's video port.
    /// </summary>
    public class FastImageOutputCallbackHandler : PortCallbackHandler<IVideoPort, IOutputCaptureHandler>
    {
        /// <summary>
        /// Creates a new instance of <see cref="FastImageOutputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IOutputPort"/>.</param>
        /// <param name="handler">The output port capture handler.</param>
        public FastImageOutputCallbackHandler(IVideoPort port, IOutputCaptureHandler handler)
            : base(port, handler)
        {
        }

        /// <inheritdoc />
        public override void Callback(IBuffer buffer)
        {
            base.Callback(buffer);
            
            var eos = buffer.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END) ||
                      buffer.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS);

            if (eos && this.CaptureHandler is IFileStreamCaptureHandler)
            {
                ((IFileStreamCaptureHandler)this.CaptureHandler).NewFile();
            }
        }
    }
}