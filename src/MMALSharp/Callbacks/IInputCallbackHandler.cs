using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Handlers;

namespace MMALSharp.Callbacks
{
    public interface IInputCallbackHandler
    {
        void Initialise(MMALPortBase port);
        ProcessResult Callback(MMALBufferImpl buffer);
    }
}
