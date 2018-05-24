using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp.Callbacks
{
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
