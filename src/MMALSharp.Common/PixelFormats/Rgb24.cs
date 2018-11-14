namespace MMALSharp.Common.PixelFormats
{
    public class Rgb24 : IPixelFormat
    {
        public int PixelSize { get; }

        public Rgb24()
        {
            this.PixelSize = 24;
        }
    }
}