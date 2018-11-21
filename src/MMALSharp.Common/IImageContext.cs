using System.Drawing.Imaging;
using MMALSharp.Common.Utility;

namespace MMALSharp.Common
{
    public interface IImageContext
    {
        Resolution Resolution { get; }
        PixelFormat PixelFormat { get; }
    }
}