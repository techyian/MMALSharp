// <copyright file="MMALSplitterComponent.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
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
    }
}
