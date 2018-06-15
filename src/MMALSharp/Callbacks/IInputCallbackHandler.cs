using MMALSharp.Handlers;

namespace MMALSharp.Callbacks
{
    public interface IInputCallbackHandler
    {
        MMALPortBase WorkingPort { get; }
        ProcessResult Callback(MMALBufferImpl buffer);
    }
}
