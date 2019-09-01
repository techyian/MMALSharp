using System;
using MMALSharp.Ports;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;
using static MMALSharp.Native.MMALParameters;

namespace MMALSharp.Components
{
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
            this.Inputs.Add(new InputPort((IntPtr)(&(*this.Ptr->Input[0])), this, PortType.Input, Guid.NewGuid()));
            this.Outputs.Add(new StillPort((IntPtr)(&(*this.Ptr->Output[0])), this, PortType.Output, Guid.NewGuid()));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MMALIspComponent"/> class that can be used to change the size
        /// and the pixel format of resulting frames. 
        /// </summary>
        /// <param name="outputPortType">The user defined output port type.</param>
        public unsafe MMALIspComponent(Type outputPortType)
            : base(MMAL_COMPONENT_ISP)
        {
            this.Inputs.Add(new InputPort((IntPtr)(&(*this.Ptr->Input[0])), this, PortType.Input, Guid.NewGuid()));
            this.Outputs.Add((IOutputPort)Activator.CreateInstance(outputPortType, (IntPtr)(&(*this.Ptr->Output[0])), this, PortType.Output, Guid.NewGuid()));
        }
    }
}
