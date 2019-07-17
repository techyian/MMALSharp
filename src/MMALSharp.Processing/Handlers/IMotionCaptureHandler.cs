using MMALSharp.Processors.Motion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    public interface IMotionCaptureHandler
    {        
        void DetectMotion(MotionConfig config, Action onDetect);
        void StartRecording(string filepath);
    }
}
