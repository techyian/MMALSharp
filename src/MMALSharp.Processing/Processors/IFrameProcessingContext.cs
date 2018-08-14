using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Processors
{
    public interface IFrameProcessingContext
    {
        IFrameProcessingContext Apply(IFrameProcessor processor);
    }
}
