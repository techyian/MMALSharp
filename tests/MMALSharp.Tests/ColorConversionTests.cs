// <copyright file="ColorConversionTests.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Utility;
using System.Drawing;
using Xunit;

namespace MMALSharp.Tests
{
    public class ColorConversionTests
    {        
        [Fact]
        [DisplayTestMethodName]
        public void FromCIE1960()
        {          
            var cie1960 = MMALColor.RGBToCIE1960(Color.Blue);
            var from1960 = MMALColor.FromCIE1960(cie1960.Item1, cie1960.Item2, cie1960.Item3);
            
            Assert.True(from1960.R == Color.Blue.R && from1960.G == Color.Blue.G && from1960.B == Color.Blue.B);
        }
             
        [Fact]
        [DisplayTestMethodName]
        public void FromCIEXYZ()
        {           
            var cieXYZ = MMALColor.RGBToCIEXYZ(Color.Blue);
            var fromXYZ = MMALColor.FromCieXYZ(cieXYZ.Item1, cieXYZ.Item2, cieXYZ.Item3);

            Assert.True(fromXYZ.R == Color.Blue.R && fromXYZ.G == Color.Blue.G && fromXYZ.B == Color.Blue.B);
        }
                
        [Fact]
        [DisplayTestMethodName]
        public void FromYIQ()
        {           
            var yiq = MMALColor.RGBToYIQ(Color.Blue);
            var fromYIQ = MMALColor.FromYIQ(yiq.Item1, yiq.Item2, yiq.Item3);

            Assert.True(fromYIQ.R == Color.Blue.R && fromYIQ.G == Color.Blue.G && fromYIQ.B == Color.Blue.B);
        }
                
        [Fact]
        [DisplayTestMethodName]
        public void FromYUV()
        {
            var fromYUVBytes = MMALColor.FromYUVBytes(0, 20, 20);
            var rgbToYUV = MMALColor.RGBToYUV(fromYUVBytes);
            var fromYUV = MMALColor.FromYUV(rgbToYUV.Item1, rgbToYUV.Item2, rgbToYUV.Item3);
                        
            Assert.True(fromYUV.Equals(fromYUVBytes));
        }

        [Fact]
        [DisplayTestMethodName]
        public void RGBToYUVBytes()
        {
            var yuvBytes = MMALColor.RGBToYUVBytes(Color.Blue);
            var fromYuvBytes = MMALColor.FromYUVBytes(yuvBytes.Item1, yuvBytes.Item2, yuvBytes.Item3);

            Assert.True(fromYuvBytes.R == Color.Blue.R && fromYuvBytes.G == Color.Blue.G && fromYuvBytes.B == Color.Blue.B);
        }

        [Fact]
        [DisplayTestMethodName]
        public void FromHLS()
        {            
            var hls = MMALColor.RGBToHLS(Color.Blue);
            var fromHLS = MMALColor.FromHLS(hls.Item1, hls.Item2, hls.Item3);

            Assert.True(fromHLS.R == Color.Blue.R && fromHLS.G == Color.Blue.G && fromHLS.B == Color.Blue.B);
        }
                
        [Fact]
        [DisplayTestMethodName]
        public void FromHSV()
        {            
            var hsv = MMALColor.RGBToHSV(Color.Blue);
            var fromHSV = MMALColor.FromHSV(hsv.Item1, hsv.Item2, hsv.Item3);
            
            Assert.True(fromHSV.R == Color.Blue.R && fromHSV.G == Color.Blue.G && fromHSV.B == Color.Blue.B);
        }        
    }
}
