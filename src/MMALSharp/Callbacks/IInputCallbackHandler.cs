using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Callbacks
{
    public interface IInputCallbackHandler
    {
        MMALEncoding EncodingType { get; }
        IInputPort WorkingPort { get; }
        ProcessResult Callback(MMALBufferImpl buffer);
    }
}