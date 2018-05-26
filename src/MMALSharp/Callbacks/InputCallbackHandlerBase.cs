using System;
using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp.Callbacks
{
    public abstract class InputCallbackHandlerBase : IInputCallbackHandler
    {
        /// <summary>
        /// A whitelisted Encoding Type that this callback handler will operate on.
        /// </summary>
        protected MMALEncoding EncodingType { get; set; }

        /// <summary>
        /// The port this callback handler is used with.
        /// </summary>
        protected MMALPortBase WorkingPort { get; set; }

        protected InputCallbackHandlerBase(MMALPortBase port)
        {
            this.WorkingPort = port;
        }

        protected InputCallbackHandlerBase(MMALEncoding encodingType, MMALPortBase port)
        {
            this.EncodingType = encodingType;
            this.WorkingPort = port;
        }

        /// <summary>
        /// The callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        /// <returns>A <see cref="ProcessResult"/> object based on the result of the callback function.</returns>
        public virtual ProcessResult Callback(MMALBufferImpl buffer)
        {
            MMALLog.Logger.Debug("In managed input callback");

            if (this.EncodingType != null && this.WorkingPort.EncodingType != this.EncodingType)
            {
                throw new ArgumentException("Port Encoding Type not supported for this handler.");
            }
            
            return this.WorkingPort.ComponentReference.Handler?.Process(buffer.AllocSize);
        }
    }
}
