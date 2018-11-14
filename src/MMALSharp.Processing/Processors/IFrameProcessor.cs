using MMALSharp.Common;

namespace MMALSharp.Processors
{
    public interface IFrameProcessor
    {
        void Apply(byte[] store, IImageContext context);
    }
}
