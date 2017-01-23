using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    public class ByteArrayCaptureHandler : ICaptureHandler<byte[]>
    {
        public byte[] Process(byte[] data)
        {
            return data;
        }
    }
}
