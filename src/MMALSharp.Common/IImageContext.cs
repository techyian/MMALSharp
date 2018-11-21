using System.Drawing.Imaging;
using MMALSharp.Common.Utility;

namespace MMALSharp.Common
{
    public interface IImageContext
    {
        bool Raw { get; }
        Resolution Resolution { get; }
        PixelFormat PixelFormat { get; }
    }
}