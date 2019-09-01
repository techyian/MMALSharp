using MMALSharp.Callbacks;
using MMALSharp.Handlers;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Ports.Inputs
{
    public interface IInputPort : IPort
    {
        void ConnectTo(IOutputPort outputPort, IConnection connection);
        void Configure(MMALPortConfig config, IPort copyPort, IInputCaptureHandler handler);
        void Enable();
        void ReleaseBuffer(IBuffer bufferImpl);
        void Start();
        void RegisterCallbackHandler(IInputCallbackHandler callbackHandler);
    }
}
