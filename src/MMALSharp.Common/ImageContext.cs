using System.Drawing.Imaging;
using MMALSharp.Common.Utility;

namespace MMALSharp.Common
{
    public class ImageContext : IImageContext
    {
        public byte[] Data { get; set; }
        public bool Raw { get; }
        public Resolution Resolution { get; }
        public PixelFormat PixelFormat { get; }

        public ImageContext(Resolution res, PixelFormat format, bool raw)
        {
            this.Resolution = res;
            this.PixelFormat = format;
            this.Raw = raw;
        }
    }
}