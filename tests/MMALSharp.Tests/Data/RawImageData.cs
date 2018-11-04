using System.Collections.Generic;

namespace MMALSharp.Tests
{
    public class RawImageData
    {
        public static IEnumerable<object[]> Data
        {
            get
            {
                yield return TestData.Yuv420EncoderData;
                yield return TestData.Yuv422EncoderData;
                yield return TestData.Rgb24EncoderData;
                yield return TestData.Rgb24EncoderData;
                yield return TestData.RgbaEncoderData;                
            }
        }
    }
}