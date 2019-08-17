using MMALSharp.Callbacks;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Ports.Inputs;

namespace MMALSharp.Ports.Outputs
{
    public interface IOutputPort : IPort
    {
        void Configure(MMALPortConfig config, IInputPort copyFrom, IOutputCaptureHandler handler);
            
        IInputPort ConnectTo(IDownstreamComponent component, int inputPort = 0, bool useCallback = false);
        
        void Start();

        void Enable(bool sendBuffers = true);
        void ReleaseBuffer(IBuffer bufferImpl);
        void RegisterCallbackHandler(IOutputCallbackHandler callbackHandler);
    }
}
