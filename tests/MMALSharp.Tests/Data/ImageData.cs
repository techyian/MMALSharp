using System.Collections.Generic;

namespace MMALSharp.Tests
{
    public class ImageData
    {
        public static IEnumerable<object[]> Data
        {
            get
            {
                var list = new List<object[]>();

                list.AddRange(TestData.JpegEncoderData);
                list.AddRange(TestData.GifEncoderData);
                list.AddRange(TestData.PngEncoderData);
                list.AddRange(TestData.BmpEncoderData);

                // TGA/PPM support is enabled by performing a firmware update "sudo rpi-update".
                // See: https://github.com/techyian/MMALSharp/issues/23

                //list.AddRange(TestData.TgaEncoderData);
                //list.AddRange(TestData.PpmEncoderData);

                return list;
            }
        }
    }
}