namespace MMALSharp.Callbacks
{
    public interface IConnectionCallbackHandler
    {
        MMALConnectionImpl WorkingConnection { get; }
        void InputCallback(MMALBufferImpl buffer);
        void OutputCallback(MMALBufferImpl buffer);
    }
}