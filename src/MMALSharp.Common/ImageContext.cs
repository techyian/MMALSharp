using System.Drawing.Imaging;
using MMALSharp.Common.Utility;

namespace MMALSharp.Common
{
    public class ImageContext : IImageContext
    {
        public Resolution Resolution { get; }
        public PixelFormat PixelFormat { get; }

        public ImageContext(Resolution res, PixelFormat format)
        {
            this.Resolution = res;
            this.PixelFormat = format;
        }
    }
}