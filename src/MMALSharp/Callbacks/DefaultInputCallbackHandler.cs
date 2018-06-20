using MMALSharp.Native;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// A default callback handler for Input ports.
    /// </summary>
    public class DefaultInputCallbackHandler : InputCallbackHandlerBase
    {
        public DefaultInputCallbackHandler()
        {
        }

        public DefaultInputCallbackHandler(MMALEncoding encodingType)
            : base(encodingType)
        {
        }
    }
}
