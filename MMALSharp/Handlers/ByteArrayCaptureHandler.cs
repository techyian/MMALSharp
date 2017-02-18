using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// This handler simply returns the processed data as a byte array
    /// </summary>
    public class ByteArrayCaptureHandler : ICaptureHandler<byte[]>
    {
        public byte[] Process(byte[] data)
        {
            return data;
        }
    }
}
