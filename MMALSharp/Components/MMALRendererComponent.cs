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
    public abstract class MMALRendererBase : MMALDownstreamComponent
    {
        public MMALRendererBase(string name) : base(name)
        {
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
        }
        
    }
}
