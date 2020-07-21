// <copyright file="ColorConversionTests.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Drawing;
using MMALSharp.Common.Utility;
using Xunit;

namespace MMALSharp.Tests
{
    public class ColorConversionTests
    {        
        [Fact]
        [MMALTestsAttribute]
        public void FromCie1960()
        {          
            var cie1960 = MMALColor.RGBToCIE1960(Color.Blue);
            var from1960 = MMALColor.FromCIE1960(cie1960.Item1, cie1960.Item2, cie1960.Item3);
            
            Assert.True(from1960.R == Color.Blue.R && from1960.G == Color.Blue.G && from1960.B == Color.Blue.B);
        }
             
        [Fact]
        [MMALTestsAttribute]
        public void FromCiexyz()
        {           
            var cieXyz = MMALColor.RGBToCIEXYZ(Color.Blue);
            var fromXyz = MMALColor.FromCieXYZ(cieXyz.Item1, cieXyz.Item2, cieXyz.Item3);

            Assert.True(fromXyz.R == Color.Blue.R && fromXyz.G == Color.Blue.G && fromXyz.B == Color.Blue.B);
        }
                
        [Fact]
        [MMALTestsAttribute]
        public void FromYiq()
        {           
            var yiq = MMALColor.RGBToYIQ(Color.Blue);
            var fromYiq = MMALColor.FromYIQ(yiq.Item1, yiq.Item2, yiq.Item3);

            Assert.True(fromYiq.R == Color.Blue.R && fromYiq.G == Color.Blue.G && fromYiq.B == Color.Blue.B);
        }
                
        [Fact]
        [MMALTestsAttribute]
        public void FromYuv()
        {
            var fromYuvBytes = MMALColor.FromYUVBytes(0, 20, 20);
            var rgbToYuv = MMALColor.RGBToYUV(fromYuvBytes);
            var fromYuv = MMALColor.FromYUV(rgbToYuv.Item1, rgbToYuv.Item2, rgbToYuv.Item3);
                        
            Assert.True(fromYuv.Equals(fromYuvBytes));
        }

        [Fact]
        [MMALTestsAttribute]
        public void RgbtoYuvBytes()
        {
            var yuvBytes = MMALColor.RGBToYUVBytes(Color.Blue);
            var fromYuvBytes = MMALColor.FromYUVBytes(yuvBytes.Item1, yuvBytes.Item2, yuvBytes.Item3);

            Assert.True(fromYuvBytes.R == Color.Blue.R && fromYuvBytes.G == Color.Blue.G && fromYuvBytes.B == Color.Blue.B);
        }

        [Fact]
        [MMALTestsAttribute]
        public void FromHls()
        {            
            var hls = MMALColor.RGBToHLS(Color.Blue);
            var fromHls = MMALColor.FromHLS(hls.Item1, hls.Item2, hls.Item3);

            Assert.True(fromHls.R == Color.Blue.R && fromHls.G == Color.Blue.G && fromHls.B == Color.Blue.B);
        }
                
        [Fact]
        [MMALTestsAttribute]
        public void FromHsv()
        {            
            var hsv = MMALColor.RGBToHSV(Color.Blue);
            var fromHsv = MMALColor.FromHSV(hsv.Item1, hsv.Item2, hsv.Item3);
            
            Assert.True(fromHsv.R == Color.Blue.R && fromHsv.G == Color.Blue.G && fromHsv.B == Color.Blue.B);
        }        
    }
}
