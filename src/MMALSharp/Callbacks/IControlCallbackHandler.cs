using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Callbacks
{
    public interface IControlCallbackHandler
    {
        MMALEncoding EncodingType { get; }
        IControlPort WorkingPort { get; }
        void Callback(MMALBufferImpl buffer);
    }
}