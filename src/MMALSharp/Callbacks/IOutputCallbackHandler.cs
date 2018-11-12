using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Callbacks
{
    public interface IOutputCallbackHandler
    {
        MMALEncoding EncodingType { get; }
        IOutputPort WorkingPort { get; }
        void Callback(MMALBufferImpl buffer);
    }
}