using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Utility
{
    public class ReceivedDataEventArgs : EventArgs
    {
        public MMALPortBase Port { get; set; }
        public byte[] Data { get; set; }
    }
}
