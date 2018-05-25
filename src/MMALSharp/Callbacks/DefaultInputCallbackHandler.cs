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
        public DefaultInputCallbackHandler(MMALPortBase port)
            : base(port)
        {
        }

        public DefaultInputCallbackHandler(MMALEncoding encodingType, MMALPortBase port)
            : base(encodingType, port)
        {
        }
    }
}
