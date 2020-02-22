using MMALSharp.Common;
using MMALSharp.Native;

namespace MMALSharp.Tests
{
    public class TestMember
    {
        public string Extension { get; set; }
        public MMALEncoding EncodingType { get; set; }
        public MMALEncoding PixelFormat { get; set; }

        public TestMember(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            this.Extension = extension;
            this.EncodingType = encodingType;
            this.PixelFormat = pixelFormat;
        }
    }
}