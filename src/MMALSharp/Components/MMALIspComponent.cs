// <copyright file="MMALIspComponent.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;
using static MMALSharp.Native.MMALParameters;

namespace MMALSharp.Components
{
    /// <summary>
    /// This component wraps the ISP (Image Sensor Processor) hardware block to
    /// offer hardware accelerated format conversion and resizing.
    /// It has one input port taking Bayer, YUV or RGB images in numerous different formats.
    /// It has two output ports:
    /// Port 361 supports various RGB and YUV formats.
    /// Port 362 is off the low res resizer which can only downscale.Due to this
    /// port 362 must be at a lower resolution to port 361. If set to a higher resolution
    /// then the port will effectively be disabled and no ouput produced.It also doesn't have
    /// the colour conversion block, and so only supports YUV formats.
    /// https://github.com/raspberrypi/firmware/blob/master/documentation/ilcomponents/isp.html 
    /// </summary>
    public class MMALIspComponent : MMALDownstreamHandlerComponent
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MMALIspComponent"/> class that can be used to change the size
        /// and the pixel format of resulting frames. 
        /// </summary>
        public unsafe MMALIspComponent()
            : base(MMAL_COMPONENT_ISP)
        {
            // Default to use still image port behaviour.
            this.Inputs.Add(new InputPort((IntPtr)(&(*this.Ptr->Input[0])), this, Guid.NewGuid()));

            for (var i = 0; i < this.Ptr->OutputNum; i++)
            {
                this.Outputs.Add(new StillPort((IntPtr)(&(*this.Ptr->Output[i])), this, Guid.NewGuid()));
            }            
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MMALIspComponent"/> class that can be used to change the size
        /// and the pixel format of resulting frames. 
        /// </summary>
        /// <param name="outputPortType">The user defined output port type.</param>
        public unsafe MMALIspComponent(Type outputPortType)
            : base(MMAL_COMPONENT_ISP)
        {
            this.Inputs.Add(new InputPort((IntPtr)(&(*this.Ptr->Input[0])), this, Guid.NewGuid()));

            for (var i = 0; i < this.Ptr->OutputNum; i++)
            {
                this.Outputs.Add((IOutputPort)Activator.CreateInstance(outputPortType, (IntPtr)(&(*this.Ptr->Output[i])), this, Guid.NewGuid()));
            }            
        }
    }
}
