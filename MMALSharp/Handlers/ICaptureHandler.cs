using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    public interface ICaptureHandler<T>
    {
        T Process(byte[] data);
    }
}
