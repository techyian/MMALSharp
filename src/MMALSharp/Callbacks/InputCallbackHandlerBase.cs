using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp.Callbacks
{
    public abstract class InputCallbackHandlerBase : IInputCallbackHandler
    {
        protected MMALEncoding EncodingType { get; set; }
        protected MMALPortBase WorkingPort { get; set; }

        protected InputCallbackHandlerBase()
        {
        }

        protected InputCallbackHandlerBase(MMALEncoding encodingType)
        {
            this.EncodingType = encodingType;
        }

        public void Initialise(MMALPortBase port)
        {
            this.WorkingPort = port;
        }

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
