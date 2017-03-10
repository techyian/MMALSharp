using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    public interface ICaptureHandler : IDisposable
    {
        void Process(byte[] data);
        bool CanSplit();
        void Split();
        string GetDirectory();
        void PostProcess();
    }
}
