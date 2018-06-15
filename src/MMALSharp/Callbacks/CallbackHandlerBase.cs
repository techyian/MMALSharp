using System;
using MMALSharp.Native;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// The base class for Output and Control port callback handlers.
    /// </summary>
    public abstract class CallbackHandlerBase : ICallbackHandler
    {
        /// <summary>
        /// A whitelisted Encoding Type that this callback handler will operate on.
        /// </summary>
        public MMALEncoding EncodingType { get; }

        /// <summary>
        /// The port this callback handler is used with.
        /// </summary>
        public MMALPortBase WorkingPort { get; }
        
        protected CallbackHandlerBase(MMALPortBase port)
        {
            this.WorkingPort = port;
        }

        protected CallbackHandlerBase(MMALEncoding encodingType, MMALPortBase port)
        {
            this.EncodingType = encodingType;
            this.WorkingPort = port;
        }
        
        /// <summary>
        /// The callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        public virtual void Callback(MMALBufferImpl buffer)
        {
            MMALLog.Logger.Debug($"In managed {this.WorkingPort.PortType.GetPortType()} callback");

            if (this.EncodingType != null && this.WorkingPort.EncodingType != this.EncodingType)
            {
                throw new ArgumentException("Port Encoding Type not supported for this handler.");
            }
        }
    }
}
