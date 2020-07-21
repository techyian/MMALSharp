// <copyright file="VideoOutputCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using Microsoft.Extensions.Logging;
using MMALSharp.Common;
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
    public class VideoOutputCallbackHandler : PortCallbackHandler<IVideoPort, IVideoCaptureHandler>, IVideoOutputCallbackHandler
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
        /// Property to indicate whether we should store motion vectors when processing image frames. 
        /// Motion vector data will be present when the buffer header equals "MMAL_BUFFER_HEADER_FLAG_CODECSIDEINFO".
        /// </summary>
        private bool StoreMotionVectors { get; }

        /// <summary>
        /// Creates a new instance of <see cref="VideoOutputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IOutputPort"/>.</param>
        /// <param name="handler">The output port capture handler.</param>
        /// <param name="split">Configure to split into multiple files.</param>
        /// <param name="storeMotionVectors">Indicates whether we should store motion vectors.</param>
        public VideoOutputCallbackHandler(
            IVideoPort port, 
            IVideoCaptureHandler handler, 
            Split split, 
            bool storeMotionVectors = false) 
            : base(port, handler)
        {
            var motionType = this.WorkingPort.EncodingType == MMALEncoding.H264
                ? MotionType.MotionVector
                : MotionType.FrameDiff;

            if (handler != null && handler is IMotionCaptureHandler)
            {
                var motionHandler = handler as IMotionCaptureHandler;
                motionHandler.MotionType = motionType;
            }

            this.Split = split;
            this.StoreMotionVectors = storeMotionVectors;
        }
        
        /// <summary>
        /// The callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        public override void Callback(IBuffer buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.LogDebug("In video output callback");
            }
            
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
                    MMALLog.Logger.LogInformation("Preparing to split.");
                    this.PrepareSplit = true;
                    this.WorkingPort.SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_REQUEST_I_FRAME, true);
                }
            }

            if (this.StoreMotionVectors && buffer.AssertProperty(MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CODECSIDEINFO))
            {
                // This is data containing Motion vectors. Check if the capture handler supports storing motion vectors.
                if (this.CaptureHandler is IMotionVectorCaptureHandler)
                {
                    var handler = this.CaptureHandler as IMotionVectorCaptureHandler;
                    handler?.ProcessMotionVectors(buffer.GetBufferData());
                }
            }
            else
            {
                // If user has requested to store motion vectors separately, do not store the motion vector data in the same file
                // as image frame data.
                base.Callback(buffer);
            }
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
