namespace MMALSharp.Common.PixelFormats
{
    public class Rgba : IPixelFormat
    {
        public int PixelSize { get; }

        public Rgba()
        {
            this.PixelSize = 32;
        }
    }
}