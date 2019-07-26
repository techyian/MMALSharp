using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Processors
{
    public interface IFrameAnalyser
    {
        void Apply(byte[] data, bool eos);
    }
}
