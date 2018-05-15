using MMALSharp.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MMALSharp.Tests
{
    public class ColorConversionTests
    {
        [Theory]
        [Fact]        
        public void FromCIE1960()
        {
            TestHelper.BeginTest("FromCIE1960");

            var cie1960 = MMALColor.RGBToCIE1960(Color.Blue);
            var from1960 = MMALColor.FromCIE1960(cie1960.Item1, cie1960.Item2);
            
            Assert.True(from1960.Equals(Color.Blue));
        }

        [Theory]
        [Fact]
        public void FromCIEXYZ()
        {
            TestHelper.BeginTest("FromCIEXYZ");

            var cieXYZ = MMALColor.RGBToCIEXYZ(Color.Blue);
            var fromXYZ = MMALColor.FromCieXYZ(cieXYZ.Item1, cieXYZ.Item2, cieXYZ.Item3);

            Assert.True(fromXYZ.Equals(Color.Blue));
        }

        [Theory]
        [Fact]
        public void FromYIQ()
        {
            TestHelper.BeginTest("FromYIQ");

            var yiq = MMALColor.RGBToYIQ(Color.Blue);
            var fromYIQ = MMALColor.FromYIQ(yiq.Item1, yiq.Item2, yiq.Item3);
            
            Assert.True(fromYIQ.Equals(Color.Blue));
        }

        [Theory]
        [Fact]
        public void FromYUV()
        {
            TestHelper.BeginTest("FromYUV");

            var yuv = MMALColor.RGBToYUV(Color.Blue);
            var fromYUV = MMALColor.FromYUV(yuv.Item1, yuv.Item2, yuv.Item3);

            Assert.True(fromYUV.Equals(Color.Blue));
        }

        [Theory]
        [Fact]
        public void FromHLS()
        {
            TestHelper.BeginTest("FromHLS");

            var hls = MMALColor.RGBToHLS(Color.Blue);
            var fromHLS = MMALColor.FromHLS(hls.Item1, hls.Item2, hls.Item3);
            
            Assert.True(fromHLS.Equals(Color.Blue));
        }

        [Theory]
        [Fact]
        public void FromHSV()
        {
            TestHelper.BeginTest("FromHSV");

            var hsv = MMALColor.RGBToHSV(Color.Blue);
            var fromHSV = MMALColor.FromHSV(hsv.Item1, hsv.Item2, hsv.Item3);

            Assert.True(fromHSV.Equals(Color.Blue));
        }        
    }
}
