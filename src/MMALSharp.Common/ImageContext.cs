using MMALSharp.Common.PixelFormats;
using MMALSharp.Common.Utility;

namespace MMALSharp.Common
{
    public class ImageContext : IImageContext
    {
        public Resolution Resolution { get; }
        public IPixelFormat PixelFormat { get; }

        public ImageContext(Resolution res, IPixelFormat format)
        {
            this.Resolution = res;
            this.PixelFormat = format;
        }
    }
}