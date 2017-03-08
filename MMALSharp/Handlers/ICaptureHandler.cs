using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    public interface ICaptureHandler
    {
        void Process(byte[] data);
        bool CanSplit();
        void Split(string fileNameConstant);
        string GetDirectory();
        void PostProcess();
    }
}
