using MMALSharp.Callbacks;
using MMALSharp.Native;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Ports.Inputs
{
    public interface IInputPort : IPort
    {
        ICallbackHandler ManagedCallback { get; }

        void ConnectTo(IOutputPort outputPort, IConnection connection);
        void Configure(MMALPortConfig config);
        void Configure(MMALPortConfig config, IPort copyPort, bool zeroCopy = false);
        void Enable();
        void ReleaseBuffer(IBuffer bufferImpl);
        void Start();
    }
}
