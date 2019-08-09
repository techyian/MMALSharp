// <copyright file="ImageOutputCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Components;
using MMALSharp.Components.EncoderComponents;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// A callback handler specifically for rapid image capture from the camera's video port.
    /// </summary>
    public class FastImageOutputCallbackHandler : DefaultPortCallbackHandler
    {
        /// <summary>
        /// Creates a new instance of <see cref="FastImageOutputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IOutputPort"/>.</param>
        public FastImageOutputCallbackHandler(IOutputPort port)
            : base(port)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="FastImageOutputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IOutputPort"/>.</param>
        /// <param name="encoding">The <see cref="MMALEncoding"/> type to restrict on.</param>
        public FastImageOutputCallbackHandler(IOutputPort port, MMALEncoding encoding)
            : base(port, encoding)
        {
        }

        /// <inheritdoc />
        public override void Callback(IBuffer buffer)
        {
            if (this.WorkingPort.ComponentReference.GetType() != typeof(IImageEncoder))
            {
                throw new ArgumentException($"Working port component is not of type {nameof(IImageEncoder)}");
            }

            base.Callback(buffer);
            
            var eos = buffer.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END) ||
                      buffer.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS);

            if (eos)
            {
                // In rapid capture mode, provide the ability to do post-processing once we have a complete frame.
                this.WorkingPort.Handler?.PostProcess();
            }

            if (eos && this.WorkingPort.Handler?.GetType() == typeof(ImageStreamCaptureHandler))
            {
                ((ImageStreamCaptureHandler)this.WorkingPort.Handler).NewFile();
            }
        }
    }
}