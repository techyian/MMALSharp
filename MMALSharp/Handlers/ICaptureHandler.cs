using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    public interface ICaptureHandler<T>
    {
        void Process(byte[] data);
        void PostProcess();
    }
}
