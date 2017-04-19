using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Native;

namespace MMALSharp.Components
{
    public class MMALSplitterComponent : MMALDownstreamComponent
    {
        public MMALSplitterComponent() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_SPLITTER, null)
        {            
        }

        public override void Initialize()
        {            
        }        
    }
}
