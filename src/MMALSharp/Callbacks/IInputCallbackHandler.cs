using MMALSharp.Handlers;

namespace MMALSharp.Callbacks
{
    public interface IInputCallbackHandler
    {
        ProcessResult Callback(MMALBufferImpl buffer);
    }
}
