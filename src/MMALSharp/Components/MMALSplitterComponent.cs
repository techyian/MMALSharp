// <copyright file="MMALSplitterComponent.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Callbacks.Providers;
using MMALSharp.Common.Utility;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    /// <summary>
    /// The Splitter Component is intended on being connected to the camera video output port. In turn, it
    /// provides an additional 4 output ports which can be used to produce multiple image/video outputs
    /// from the single camera video port.
    /// </summary>
    public class MMALSplitterComponent : MMALDownstreamHandlerComponent
    {
        /// <summary>
        /// Creates a new instance of <see cref="MMALSplitterComponent"/>.
        /// </summary>
        /// <param name="handlers">The capture handlers to associate with each splitter port.</param>
        public unsafe MMALSplitterComponent(params ICaptureHandler[] handlers)
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_SPLITTER)
        {
            this.Inputs.Add(new InputPort((IntPtr)(&(*this.Ptr->Input[0])), this, PortType.Input, Guid.NewGuid()));

            if (handlers != null)
            {
                for (var i = 0; i < handlers.Length; i++)
                {
                    this.Outputs.Add(new VideoPort((IntPtr)(&(*this.Ptr->Output[i])), this, PortType.Output, Guid.NewGuid(), handlers[i]));
                }
            }
            else
            {
                for (var i = 0; i < 4; i++)
                {
                    this.Outputs.Add(new VideoPort((IntPtr)(&(*this.Ptr->Output[i])), this, PortType.Output, Guid.NewGuid(), null));
                }
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="MMALSplitterComponent"/>.
        /// </summary>
        /// <param name="handlers">The capture handlers to associate with each splitter port.</param>
        /// <param name="outputPortType">The user defined output port type to use for each splitter output port.</param>
        public unsafe MMALSplitterComponent(ICaptureHandler[] handlers, Type outputPortType)
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_SPLITTER)
        {
            this.Inputs.Add(new InputPort((IntPtr)(&(*this.Ptr->Input[0])), this, PortType.Input, Guid.NewGuid()));

            if (handlers != null)
            {
                for (int i = 0; i < handlers.Length; i++)
                {
                    this.Outputs.Add((OutputPortBase)Activator.CreateInstance(outputPortType, (IntPtr)(&(*this.Ptr->Output[i])), this, PortType.Output, Guid.NewGuid(), handlers[i]));
                }
            }
            else
            {
                for (var i = 0; i < 4; i++)
                {
                    this.Outputs.Add((OutputPortBase)Activator.CreateInstance(outputPortType, (IntPtr)(&(*this.Ptr->Output[i])), this, PortType.Output, Guid.NewGuid(), null));
                }
            }
        }

        /// <inheritdoc />
        public override unsafe MMALDownstreamComponent ConfigureInputPort(MMALEncoding encodingType, MMALEncoding pixelFormat, PortBase copyPort, bool zeroCopy = false)
        {
            base.ConfigureInputPort(encodingType, pixelFormat, copyPort, zeroCopy);

            this.Inputs[0].Ptr->BufferNum = Math.Max(this.Inputs[0].Ptr->BufferNumRecommended, 3);
            
            return this;
        }

        /// <inheritdoc />
        public override unsafe MMALDownstreamComponent ConfigureInputPort(MMALPortConfig config)
        {
            base.ConfigureInputPort(config);

            this.Inputs[0].Ptr->BufferNum = Math.Max(this.Inputs[0].Ptr->BufferNumRecommended, 3);

            return this;
        }

        /// <inheritdoc />
        public override unsafe MMALDownstreamComponent ConfigureOutputPort(int outputPort, MMALPortConfig config)
        {
            // The splitter component should not have its resolution set on the output port so override method accordingly.
            this.Outputs[outputPort].PortConfig = config;

            if (this.ProcessingPorts.ContainsKey(outputPort))
            {
                this.ProcessingPorts.Remove(outputPort);
            }

            this.ProcessingPorts.Add(outputPort, this.Outputs[outputPort]);

            this.Inputs[0].ShallowCopy(this.Outputs[outputPort]);

            if (config.EncodingType != null)
            {
                this.Outputs[outputPort].NativeEncodingType = config.EncodingType.EncodingVal;
            }

            if (config.PixelFormat != null)
            {
                this.Outputs[outputPort].NativeEncodingSubformat = config.PixelFormat.EncodingVal;
            }

            MMAL_VIDEO_FORMAT_T tempVid = this.Outputs[outputPort].Ptr->Format->Es->Video;

            try
            {
                this.Outputs[outputPort].Commit();
            }
            catch
            {
                // If commit fails using new settings, attempt to reset using old temp MMAL_VIDEO_FORMAT_T.
                MMALLog.Logger.Warn("Commit of output port failed. Attempting to reset values.");
                this.Outputs[outputPort].Ptr->Format->Es->Video = tempVid;
                this.Outputs[outputPort].Commit();
            }

            if (config.EncodingType == MMALEncoding.JPEG)
            {
                this.Outputs[outputPort].SetParameter(MMALParametersCamera.MMAL_PARAMETER_JPEG_Q_FACTOR, config.Quality);
            }

            if (config.ZeroCopy)
            {
                this.Outputs[outputPort].ZeroCopy = true;
                this.Outputs[outputPort].SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);
            }

            if (MMALCameraConfig.VideoColorSpace != null &&
                MMALCameraConfig.VideoColorSpace.EncType == MMALEncoding.EncodingType.ColorSpace)
            {
                this.Outputs[outputPort].VideoColorSpace = MMALCameraConfig.VideoColorSpace;
            }
            
            this.Outputs[outputPort].EncodingType = config.EncodingType;
            this.Outputs[outputPort].PixelFormat = config.PixelFormat;
            
            // It is important to re-commit changes to width and height.
            this.Outputs[outputPort].Commit();

            this.Outputs[outputPort].BufferNum = Math.Max(this.Outputs[outputPort].Ptr->BufferNumRecommended, this.Outputs[outputPort].Ptr->BufferNumMin);
            this.Outputs[outputPort].BufferSize = Math.Max(this.Outputs[outputPort].Ptr->BufferSizeRecommended, this.Outputs[outputPort].Ptr->BufferSizeMin);

            this.Outputs[outputPort].ManagedOutputCallback = OutputCallbackProvider.FindCallback(this.Outputs[outputPort]);

            return this;
        }
    }
}
