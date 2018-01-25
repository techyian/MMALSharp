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
