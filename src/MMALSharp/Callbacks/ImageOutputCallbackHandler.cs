// <copyright file="ImageOutputCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Linq;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Callbacks
{
    public class ImageOutputCallbackHandler : DefaultOutputCallbackHandler
    {
        public ImageOutputCallbackHandler(IOutputPort port)
            : base(port)
        {
        }

        public ImageOutputCallbackHandler(IOutputPort port, MMALEncoding encoding)
            : base(port, encoding)
        {
        }

        /// <summary>
        /// The callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        public override void Callback(MMALBufferImpl buffer)
        {
            if (this.WorkingPort.ComponentReference.GetType() != typeof(MMALImageEncoder))
            {
                throw new ArgumentException($"Working port component is not of type {nameof(MMALImageEncoder)}");
            }

            base.Callback(buffer);
            
            var component = (MMALImageEncoder) this.WorkingPort.ComponentReference;
            var eos = buffer.Properties.Any(c => c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_FRAME_END ||
                                                 c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS);

            if (eos)
            {
                // In rapid capture mode, provide the ability to do post-processing once we have a complete frame.
                this.WorkingPort.Handler?.PostProcess();
            }

            if (eos && this.WorkingPort.Handler.GetType() == typeof(ImageStreamCaptureHandler))
            {
                ((ImageStreamCaptureHandler) this.WorkingPort.Handler).NewFile();
            }
        }
    }
}