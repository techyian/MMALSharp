// <copyright file="VideoOutputCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Config;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports.Outputs;
using MMALSharp.Processors.Motion;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// Represents a callback handler specifically for <see cref="MMALVideoEncoder"/> components.
    /// </summary>
    public class VideoOutputCallbackHandler : PortCallbackHandler<IVideoPort, IVideoCaptureHandler>
    {
        /// <summary>
        /// Object containing properties used to determine when we should perform a file split.
        /// </summary>
        public Split Split { get; }

        /// <summary>
        /// States the time we last did a file split.
        /// </summary>
        public DateTime? LastSplit { get; private set; }

        /// <summary>
        /// Property to indicate whether on the next callback we should split. This is used so that we can request an I-Frame from the camera
        /// and this can be applied on the next run to the newly created file.
        /// </summary>
        private bool PrepareSplit { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="VideoOutputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IOutputPort"/>.</param>
        /// <param name="handler">The output port capture handler.</param>
        public VideoOutputCallbackHandler(IVideoPort port, IVideoCaptureHandler handler) 
            : base(port, handler)
        {
            var motionType = this.WorkingPort.EncodingType == MMALEncoding.H264
                ? MotionType.MotionVector
                : MotionType.FrameDiff;

            if (handler != null)
            {
                handler.MotionType = motionType;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="VideoOutputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IOutputPort"/>.</param>
        /// <param name="handler">The output port capture handler.</param>
        /// <param name="split">Configure to split into multiple files.</param>
        public VideoOutputCallbackHandler(IVideoPort port, Split split, IVideoCaptureHandler handler)
            : this(port, handler)
        {
            this.Split = split;
        }

        /// <summary>
        /// Creates a new instance of <see cref="VideoOutputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IOutputPort"/>.</param>
        /// <param name="handler">The output port capture handler.</param>
        /// <param name="encoding">The <see cref="MMALEncoding"/> type to restrict on.</param>
        public VideoOutputCallbackHandler(IVideoPort port, IVideoCaptureHandler handler, MMALEncoding encoding)
            : base(port, handler, encoding)
        {
            var motionType = this.WorkingPort.EncodingType == MMALEncoding.H264
                ? MotionType.MotionVector
                : MotionType.FrameDiff;

            handler.MotionType = motionType;
        }

        /// <summary>
        /// Creates a new instance of <see cref="VideoOutputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IOutputPort"/>.</param>
        /// <param name="handler">The output port capture handler.</param>
        /// <param name="encoding">The <see cref="MMALEncoding"/> type to restrict on.</param>
        /// <param name="split">Configure to split into multiple files.</param>
        public VideoOutputCallbackHandler(IVideoPort port, IVideoCaptureHandler handler, MMALEncoding encoding, Split split)
            : this(port, handler, encoding)
        {
            this.Split = split;
        }

        /// <summary>
        /// The callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        public override void Callback(IBuffer buffer)
        {
            MMALLog.Logger.Debug("In video output callback");
            
            if (this.PrepareSplit && buffer.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CONFIG))
            {
                this.CaptureHandler.Split();
                LastSplit = DateTime.Now;
                this.PrepareSplit = false;
            }

            // Ensure that if we need to split then this is done before processing the buffer data.
            if (this.Split != null)
            {
                if (!this.LastSplit.HasValue)
                {
                    LastSplit = DateTime.Now;
                }

                if (DateTime.Now.CompareTo(this.CalculateSplit()) > 0)
                {
                    this.PrepareSplit = true;
                    this.WorkingPort.SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_REQUEST_I_FRAME, true);
                }
            }
            
            base.Callback(buffer);
        }

        private DateTime CalculateSplit()
        {
            DateTime tempDt = new DateTime(this.LastSplit.Value.Ticks);
            switch (this.Split.Mode)
            {
                case TimelapseMode.Millisecond:
                    return tempDt.AddMilliseconds(this.Split.Value);
                case TimelapseMode.Second:
                    return tempDt.AddSeconds(this.Split.Value);
                case TimelapseMode.Minute:
                    return tempDt.AddMinutes(this.Split.Value);
                default:
                    return tempDt.AddMinutes(this.Split.Value);
            }
        }
    }
}
