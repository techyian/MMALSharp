using System.IO;

namespace MMALSharp.Handlers
{
    public interface IMotionVectorCaptureHandler
    {
        void InitialiseMotionStore(FileStream stream);
    }
}
