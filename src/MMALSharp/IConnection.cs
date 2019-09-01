using MMALSharp.Callbacks;
using MMALSharp.Components;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;

namespace MMALSharp
{
    public interface IConnection : IMMALObject
    {
        IConnectionCallbackHandler CallbackHandler { get; }
        IDownstreamComponent DownstreamComponent { get; }
        IComponent UpstreamComponent { get; }
        IInputPort InputPort { get; }
        IOutputPort OutputPort { get; }
        IBufferPool ConnectionPool { get; set; }
        string Name { get; }
        bool Enabled { get; }
        uint Flags { get; }
        long TimeSetup { get; }
        long TimeEnable { get; }
        long TimeDisable { get; }

        void ManagedConnectionCallback(IBuffer buffer);

        void Enable();

        void Disable();

        void Destroy();
    }
}
