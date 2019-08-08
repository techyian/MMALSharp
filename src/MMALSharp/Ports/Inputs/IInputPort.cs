using MMALSharp.Callbacks;

namespace MMALSharp.Ports.Inputs
{
    public interface IInputPort : IPort
    {
        ICallbackHandler ManagedCallback { get; }

        void Configure(MMALPortConfig config);
        void Enable();
        void ReleaseBuffer(IBuffer bufferImpl);
        void Start();
    }
}
