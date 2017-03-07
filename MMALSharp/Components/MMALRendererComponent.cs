using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a base class for all renderer components
    /// </summary>
    public unsafe abstract class MMALRendererBase : MMALDownstreamComponent
    {
        public MMALRendererBase(string name) : base(name)
        {
            if (this.Ptr->InputNum > 0)
            {
                for (int i = 0; i < this.Ptr->InputNum; i++)
                {
                    Inputs.Add(new MMALPortImpl(&(*this.Ptr->Input[i]), this));
                }
            }

            if (this.Ptr->OutputNum > 0)
            {
                for (int i = 0; i < this.Ptr->OutputNum; i++)
                {
                    Outputs.Add(new MMALPortImpl(&(*this.Ptr->Output[i]), this));
                }
            }
        }
    }

    /// <summary>
    /// Represents a Null Sink component. This component should be used when a preview component is not required in order to measure exposure.
    /// </summary>
    public class MMALNullSinkComponent : MMALRendererBase
    {
        public MMALNullSinkComponent() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_NULL_SINK)
        {
            this.EnableComponent();
        }
        
    }

    /// <summary>
    /// Represents a Video Renderer component
    /// </summary>
    public class MMALVideoRenderer : MMALRendererBase
    {
        public MMALVideoRenderer() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_RENDERER)
        {
            this.EnableComponent();
        }
        
    }
}
