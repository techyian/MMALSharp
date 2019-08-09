using MMALSharp.Callbacks;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Ports.Inputs;

namespace MMALSharp.Ports.Outputs
{
    public interface IOutputPort : IPort
    {
        ICallbackHandler ManagedCallback { get; }

        void Configure(MMALPortConfig config, IInputPort copyFrom);

        IInputPort ConnectTo(IDownstreamComponent component, int inputPort = 0, bool useCallback = false);
        
        void Start();

        void Enable(bool sendBuffers = true);
        void ReleaseBuffer(IBuffer bufferImpl);
        void SetCaptureHandler(ICaptureHandler handler);
    }
}
