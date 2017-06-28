using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Handlers;

namespace MMALSharp.Components
{
    public abstract class MMALDownstreamHandlerComponent : MMALDownstreamComponent
    {
        protected MMALDownstreamHandlerComponent(string name, ICaptureHandler handler) : base(name)
        {
            this.Handler = handler;
        }
    }
}
