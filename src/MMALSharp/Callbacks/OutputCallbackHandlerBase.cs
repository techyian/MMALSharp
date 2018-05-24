using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Native;

namespace MMALSharp.Callbacks
{
    public abstract class CallbackHandlerBase : ICallbackHandler
    {
        protected MMALEncoding EncodingType { get; set; }
        protected MMALPortBase WorkingPort { get; set; }
        
        protected CallbackHandlerBase()
        {
        }

        protected CallbackHandlerBase(MMALEncoding encodingType)
        {
            this.EncodingType = encodingType;
        }

        public void Initialise(MMALPortBase port)
        {
            this.WorkingPort = port;
        }

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
