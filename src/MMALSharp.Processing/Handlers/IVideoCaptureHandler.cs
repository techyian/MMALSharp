using MMALSharp.Processors.Motion;

namespace MMALSharp.Handlers
{
    public interface IVideoCaptureHandler : IOutputCaptureHandler
    {
        MotionType MotionType { get; set; }
        void Split();
    }
}
