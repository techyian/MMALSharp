// <copyright file="ImageOutputCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Components.EncoderComponents;
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
        public FastImageOutputCallbackHandler(IVideoPort port, IOutputCaptureHandler handler)
            : base(port, handler)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="FastImageOutputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IOutputPort"/>.</param>
        /// <param name="encoding">The <see cref="MMALEncoding"/> type to restrict on.</param>
        public FastImageOutputCallbackHandler(IVideoPort port, IOutputCaptureHandler handler, MMALEncoding encoding)
            : base(port, handler, encoding)
        {
        }

        /// <inheritdoc />
        public override void Callback(IBuffer buffer)
        {
            var componentRef = this.WorkingPort.ComponentReference as IImageEncoder;

            if (componentRef == null)
            {
                throw new ArgumentException($"Working port component is not of type {nameof(IImageEncoder)}");
            }

            base.Callback(buffer);
            
            var eos = buffer.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END) ||
                      buffer.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS);

            if (eos)
            {
                // In rapid capture mode, provide the ability to do post-processing once we have a complete frame.
                this.CaptureHandler?.PostProcess();
            }

            if (eos && this.CaptureHandler?.GetType() == typeof(ImageStreamCaptureHandler))
            {
                ((ImageStreamCaptureHandler)this.CaptureHandler).NewFile();
            }
        }
    }
}