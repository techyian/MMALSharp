// <copyright file="VideoOutputCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Native;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// Represents a callback handler specifically for <see cref="MMALVideoEncoder"/> components.
    /// </summary>
    public class VideoOutputCallbackHandler : DefaultOutputCallbackHandler
    {
        /// <summary>
        /// Creates a new instance of <see cref="VideoOutputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="OutputPortBase"/>.</param>
        public VideoOutputCallbackHandler(OutputPortBase port) 
            : base(port)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="VideoOutputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="OutputPortBase"/>.</param>
        /// <param name="encoding">The <see cref="MMALEncoding"/> type to restrict on.</param>
        public VideoOutputCallbackHandler(OutputPortBase port, MMALEncoding encoding)
            : base(port, encoding)
        {
        }

        /// <summary>
        /// The callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        public override void Callback(MMALBufferImpl buffer)
        {
            if (this.WorkingPort.ComponentReference.GetType() != typeof(MMALVideoEncoder))
            {
                throw new ArgumentException($"Working port component is not of type {nameof(MMALVideoEncoder)}");
            }
            
            MMALLog.Logger.Debug("In video output callback");

            var component = (MMALVideoEncoder)this.WorkingPort.ComponentReference;

            if (component.PrepareSplit && buffer.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CONFIG))
            {
                ((VideoStreamCaptureHandler)this.WorkingPort.Handler).Split();
                component.LastSplit = DateTime.Now;
                component.PrepareSplit = false;
            }

            // Ensure that if we need to split then this is done before processing the buffer data.
            if (component.Split != null)
            {
                if (!component.LastSplit.HasValue)
                {
                    component.LastSplit = DateTime.Now;
                }

                if (DateTime.Now.CompareTo(component.CalculateSplit()) > 0)
                {
                    component.PrepareSplit = true;
                    this.WorkingPort.SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_REQUEST_I_FRAME, true);
                }
            }
            
            base.Callback(buffer);
        }
    }
}
