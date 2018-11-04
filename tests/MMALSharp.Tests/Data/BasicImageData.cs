using System.Collections.Generic;

namespace MMALSharp.Tests
{
    public class BasicImageData
    {
        public static IEnumerable<object[]> Data
        {
            get
            {
                var list = new List<object[]>();

                list.AddRange(TestData.JpegEncoderData);
              
                return list;
            }
        }
    }
}