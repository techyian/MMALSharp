using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    public interface IMotionVectorCaptureHandler
    {
        void InitialiseMotionStore(FileStream stream);
    }
}
