using MMALSharp.Common.PixelFormats;
using MMALSharp.Common.Utility;

namespace MMALSharp.Common
{
    public interface IImageContext
    {
        Resolution Resolution { get; }
        IPixelFormat PixelFormat { get; }
    }
}