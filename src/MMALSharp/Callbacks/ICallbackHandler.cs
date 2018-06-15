namespace MMALSharp.Callbacks
{
    public interface ICallbackHandler
    {
        MMALPortBase WorkingPort { get; }
        void Callback(MMALBufferImpl buffer);
    }
}
