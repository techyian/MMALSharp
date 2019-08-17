
namespace MMALSharp.Handlers
{
    public interface IInputCaptureHandler : ICaptureHandler
    {
        ProcessResult Process(uint allocSize);
    }
}
