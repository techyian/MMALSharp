using MMALSharp.Callbacks;

namespace MMALSharp.Ports.Controls
{
    public interface IControlPort : IPort
    {
        ICallbackHandler ManagedCallback { get; }
        
        void Enable();
        void Start();
    }
}
