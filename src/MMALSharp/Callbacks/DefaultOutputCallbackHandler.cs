using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Native;

namespace MMALSharp.Callbacks
{
    public class DefaultCallbackHandler : CallbackHandlerBase
    {
        public DefaultCallbackHandler()
        {
        }

        public DefaultCallbackHandler(MMALEncoding encodingType)
            : base(encodingType)
        {
        }
        
        public override void Callback(MMALBufferImpl buffer)
        {
            base.Callback(buffer);
            
            var data = buffer.GetBufferData();
            this.WorkingPort.ComponentReference.Handler?.Process(data);
        }
    }
}
