using MMALSharp.Processors.Motion;
using System;

namespace MMALSharp.Handlers
{
    public interface IMotionCaptureHandler
    {        
        void DetectMotion(MotionConfig config, Action onDetect);
        void StartRecording(string filepath);
    }
}
