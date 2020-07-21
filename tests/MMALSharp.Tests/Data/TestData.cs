// <copyright file="TestData.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using MMALSharp.Common;
using Xunit;

namespace MMALSharp.Tests
{
    [Collection("MMALCollection")]
    public class TestBase
    {
        private static MMALFixture _fixture;
        public static MMALFixture Fixture
        {
            get
            {
                if (_fixture == null)
                {
                    _fixture = new MMALFixture();
                }

                return _fixture;
            }
            set => _fixture = value;
        }
        
        public TestBase(MMALFixture fixture)
        {
            Fixture = fixture;
        }
        
        public static List<MMALEncoding> PixelFormats = MMALEncodingHelpers.EncodingList.Where(c => c.EncType == MMALEncoding.EncodingType.PixelFormat).ToList();

        private static IEnumerable<object[]> GetVideoEncoderData(MMALEncoding encodingType, string extension)
        {
            var supportedEncodings = Fixture.MMALCamera.Camera.VideoPort.GetSupportedEncodings();
            return PixelFormats.Where(c => supportedEncodings.Contains(c.EncodingVal) && c != MMALEncoding.OPAQUE).Select(pixFormat => new object[] { extension, encodingType, pixFormat }).ToList();
        }

        private static IEnumerable<object[]> GetImageEncoderData(MMALEncoding encodingType, string extension)
        {
            var supportedEncodings = Fixture.MMALCamera.Camera.StillPort.GetSupportedEncodings();
            return PixelFormats.Where(c => supportedEncodings.Contains(c.EncodingVal) && c != MMALEncoding.OPAQUE).Select(pixFormat => new object[] { extension, encodingType, pixFormat }).ToList();
        }
        
        private static object[] GetEncoderData(MMALEncoding encodingType, MMALEncoding pixelFormat, string extension)
        {
            var supportedEncodings = Fixture.MMALCamera.Camera.StillPort.GetSupportedEncodings();

            if (!supportedEncodings.Contains(pixelFormat.EncodingVal))
            {
                throw new ArgumentException("Unsupported pixel format requested.");
            }

            return new object[] { extension, encodingType, pixelFormat };
        } 
        
        #region Still image encoders

        public static IEnumerable<object[]> JpegEncoderData => GetImageEncoderData(MMALEncoding.JPEG, "jpg");
        
        public static IEnumerable<object[]> GifEncoderData => GetImageEncoderData(MMALEncoding.GIF, "gif");
        
        public static IEnumerable<object[]> PngEncoderData => GetImageEncoderData(MMALEncoding.PNG, "png");
        
        public static IEnumerable<object[]> PpmEncoderData => GetImageEncoderData(MMALEncoding.PPM, "ppm");
        
        public static IEnumerable<object[]> TgaEncoderData => GetImageEncoderData(MMALEncoding.TGA, "tga");
        
        public static IEnumerable<object[]> BmpEncoderData => GetImageEncoderData(MMALEncoding.BMP, "bmp");
        
        #endregion

        #region Video encoders

        public static IEnumerable<object[]> H264EncoderData => GetVideoEncoderData(MMALEncoding.H264, "avi");

        public static IEnumerable<object[]> MvcEncoderData => GetVideoEncoderData(MMALEncoding.MVC, "mvc");

        public static IEnumerable<object[]> H263EncoderData => GetVideoEncoderData(MMALEncoding.H263, "h263");

        public static IEnumerable<object[]> Mp4EncoderData => GetVideoEncoderData(MMALEncoding.MP4V, "mp4");

        public static IEnumerable<object[]> Mp2EncoderData => GetVideoEncoderData(MMALEncoding.MP2V, "mp2");

        public static IEnumerable<object[]> Mp1EncoderData => GetVideoEncoderData(MMALEncoding.MP1V, "mp1");

        public static IEnumerable<object[]> Wmv3EncoderData => GetVideoEncoderData(MMALEncoding.WMV3, "wmv");

        public static IEnumerable<object[]> Wmv2EncoderData => GetVideoEncoderData(MMALEncoding.WMV2, "wmv");

        public static IEnumerable<object[]> Wmv1EncoderData => GetVideoEncoderData(MMALEncoding.WMV1, "wmv");

        public static IEnumerable<object[]> Wvc1EncoderData => GetVideoEncoderData(MMALEncoding.WVC1, "asf");

        public static IEnumerable<object[]> Vp8EncoderData => GetVideoEncoderData(MMALEncoding.VP8, "webm");

        public static IEnumerable<object[]> Vp7EncoderData => GetVideoEncoderData(MMALEncoding.VP7, "webm");

        public static IEnumerable<object[]> Vp6EncoderData => GetVideoEncoderData(MMALEncoding.VP6, "webm");

        public static IEnumerable<object[]> TheoraEncoderData => GetVideoEncoderData(MMALEncoding.THEORA, "ogv");

        public static IEnumerable<object[]> SparkEncoderData => GetVideoEncoderData(MMALEncoding.SPARK, "flv");

        public static IEnumerable<object[]> MjpegEncoderData => GetVideoEncoderData(MMALEncoding.MJPEG, "mjpeg");

        #endregion

        #region Raw image encode

        public static object[] Yuv420EncoderData => GetEncoderData(MMALEncoding.I420, MMALEncoding.I420, "i420");
        public static object[] Yuv422EncoderData => GetEncoderData(MMALEncoding.I422, MMALEncoding.I422, "i422");
        public static object[] Rgb16EncoderData => GetEncoderData(MMALEncoding.RGB16, MMALEncoding.RGB16, "rgb16");
        public static object[] Rgb24EncoderData => GetEncoderData(MMALEncoding.RGB24, MMALEncoding.RGB24, "rgb24");
        public static object[] RgbaEncoderData => GetEncoderData(MMALEncoding.RGBA, MMALEncoding.RGBA, "rgba");
        
        #endregion

    }
}
