using System;
using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp.Callbacks
{
    public abstract class InputCallbackHandlerBase
    {
        /// <summary>
        /// A whitelisted Encoding Type that this callback handler will operate on.
        /// </summary>
        public MMALEncoding EncodingType { get; }

        /// <summary>
        /// The port this callback handler is used with.
        /// </summary>
        public MMALPortBase WorkingPort { get; internal set; }

        protected InputCallbackHandlerBase()
        {
        }

        protected InputCallbackHandlerBase(MMALEncoding encodingType)
        {
            this.EncodingType = encodingType;
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
