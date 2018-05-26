using MMALSharp.Native;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// A default callback handler for Input ports.
    /// </summary>
    public class DefaultInputCallbackHandler : InputCallbackHandlerBase
    {
        public DefaultInputCallbackHandler(MMALPortBase port)
            : base(port)
        {
        }

        public DefaultInputCallbackHandler(MMALEncoding encodingType, MMALPortBase port)
            : base(encodingType, port)
        {
        }
    }
}
