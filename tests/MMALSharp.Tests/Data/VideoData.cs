using System.Collections.Generic;

namespace MMALSharp.Tests
{
    public class VideoData
    {
        public static IEnumerable<object[]> Data
        {
            get
            {
                var list = new List<object[]>();

                list.AddRange(TestData.H264EncoderData);
                list.AddRange(TestData.MjpegEncoderData);

                return list;
            }
        }
    }
}