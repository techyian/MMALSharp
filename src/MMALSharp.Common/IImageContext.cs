using System.Drawing.Imaging;
using MMALSharp.Common.Utility;

namespace MMALSharp.Common
{
    public interface IImageContext
    {
        byte[] Data { get; set; }
        bool Raw { get; }
        Resolution Resolution { get; }
        PixelFormat PixelFormat { get; }
    }
}