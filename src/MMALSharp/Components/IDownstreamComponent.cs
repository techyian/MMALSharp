using System.Collections.Generic;
using MMALSharp.Callbacks;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    public interface IDownstreamComponent : IComponent
    {
        Dictionary<int, IOutputPort> ProcessingPorts { get; }
      
        IDownstreamComponent ConfigureInputPort(MMALPortConfig config, IPort copyPort, IInputCaptureHandler handler, bool zeroCopy = false);

        IDownstreamComponent ConfigureInputPort(MMALPortConfig config, IInputCaptureHandler handler);

        IDownstreamComponent ConfigureOutputPort(MMALPortConfig config, IOutputCaptureHandler handler);

        IDownstreamComponent ConfigureOutputPort(int outputPort, MMALPortConfig config, IOutputCaptureHandler handler);
    }
}
