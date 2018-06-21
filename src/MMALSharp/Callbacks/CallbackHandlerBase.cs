using System;
using MMALSharp.Native;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// The base class for Output port callback handlers.
    /// </summary>
    public abstract class CallbackHandlerBase
    {
        /// <summary>
        /// A whitelisted Encoding Type that this callback handler will operate on.
        /// </summary>
        public MMALEncoding EncodingType { get; }

        /// <summary>
        /// The port this callback handler is used with.
        /// </summary>
        public MMALPortBase WorkingPort { get; internal set; }
        
        protected CallbackHandlerBase()
        {
        }

        protected CallbackHandlerBase(MMALEncoding encodingType)
        {
            this.EncodingType = encodingType;
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
