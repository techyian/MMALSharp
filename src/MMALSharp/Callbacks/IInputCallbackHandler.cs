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
        ProcessResult Callback(MMALBufferImpl buffer);
    }
}
