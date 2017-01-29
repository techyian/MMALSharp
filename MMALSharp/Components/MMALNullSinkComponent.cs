using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Components
{
    public abstract class MMALRendererBase : MMALDownstreamComponent
    {
        public MMALRendererBase(string name) : base(name)
        {
        }
    }

    public class MMALNullSinkComponent : MMALRendererBase
    {
        public MMALNullSinkComponent() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_NULL_SINK)
        {
            this.EnableComponent();
        }

        public override void Initialize()
        {            
        }
    }

    public class MMALVideoRenderer : MMALRendererBase
    {
        public MMALVideoRenderer() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_RENDERER)
        {
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
