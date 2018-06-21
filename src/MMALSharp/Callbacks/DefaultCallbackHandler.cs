using MMALSharp.Native;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// A default callback handler for Output and Control ports.
    /// </summary>
    public class DefaultCallbackHandler : CallbackHandlerBase
    {
        public DefaultCallbackHandler()
        {
        }

        public DefaultCallbackHandler(MMALEncoding encodingType)
            : base(encodingType)
        {
        }

        /// <summary>
        /// The callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        public override void Callback(MMALBufferImpl buffer)
        {
            base.Callback(buffer);
            
            var data = buffer.GetBufferData();
            this.WorkingPort.ComponentReference.Handler?.Process(data);
        }
    }
}
