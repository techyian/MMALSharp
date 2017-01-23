using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Components
{
    internal class MMALNullSinkComponent : MMALComponentBase
    {
        public MMALNullSinkComponent() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_NULL_SINK)
        {
            this.EnableComponent();
        }

        public override void Initialize()
        {            
        }
    }
}
