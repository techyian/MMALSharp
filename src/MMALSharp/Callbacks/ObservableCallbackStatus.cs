using System;

namespace MMALSharp.Callbacks
{
    public class ObservableCallback
    {
        public ObservableCallbackStatus Status { get; set; }
        public Exception Exception { get; set; }
    }

    public enum ObservableCallbackStatus
    {
        EOS = 1,
        Reset = 2,
        Error = 3
    }
}
