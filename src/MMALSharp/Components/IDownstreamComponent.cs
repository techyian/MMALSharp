using System.Collections.Generic;
using MMALSharp.Callbacks;
using MMALSharp.Native;
using MMALSharp.Ports;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    public interface IDownstreamComponent : IComponent
    {
        Dictionary<int, IOutputPort> ProcessingPorts { get; }
        void RegisterPortCallback(ICallbackHandler handler);

        void RemovePortCallback(IPort port);

        void RegisterConnectionCallback(IConnectionCallbackHandler handler);

        void RemoveConnectionCallback(IPort port);

        IDownstreamComponent ConfigureInputPort(MMALPortConfig config, IPort copyPort, bool zeroCopy = false);

        IDownstreamComponent ConfigureInputPort(MMALPortConfig config);

        IDownstreamComponent ConfigureOutputPort(MMALPortConfig config);

        IDownstreamComponent ConfigureOutputPort(int outputPort, MMALPortConfig config);
    }
}
