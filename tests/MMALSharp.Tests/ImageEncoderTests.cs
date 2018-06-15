// <copyright file="ImageEncoderTests.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MMALSharp.Tests
{
    [Collection("MMALCollection")]
    public class ImageEncoderTests
    {
        private readonly MMALFixture _fixture;

        public ImageEncoderTests(MMALFixture fixture)
        {
            _fixture = fixture;
            TestData.Fixture = fixture;
        }

        public static IEnumerable<object[]> TakePictureData
        {
            get
            {
                yield return new object[] { TestData.JpegEncoderData.Cast<object[]>() };
                yield return new object[] { TestData.GifEncoderData.Cast<object[]>() };
                yield return new object[] { TestData.PngEncoderData.Cast<object[]>() };
                //yield return new object[] { TestData.TgaEncoderData.Cast<object[]>() };
                //yield return new object[] { TestData.PpmEncoderData.Cast<object[]>() };
                yield return new object[] { TestData.BmpEncoderData.Cast<object[]>() };                
            }
        }

        public static IEnumerable<object[]> TakeRawPictureData
        {
            get
            {
                yield return new object[] { TestData.Yuv420EncoderData.Cast<object[]>() };
                yield return new object[] { TestData.Yuv422EncoderData.Cast<object[]>() };
                yield return new object[] { TestData.Rgb24EncoderData.Cast<object[]>() };
                yield return new object[] { TestData.Rgb24EncoderData.Cast<object[]>() };
                yield return new object[] { TestData.RgbaEncoderData.Cast<object[]>() };                
            }
        }

        public static IEnumerable<object[]> TakePictureDataJpeg => TestData.JpegEncoderData.Cast<object[]>().ToList();

        [Theory]
        [MemberData(nameof(TakePictureData))]
        public void TakePicture(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakePicture", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension);
                
                TestHelper.CleanDirectory("/home/pi/images/tests");

                using (var preview = new MMALNullSinkComponent())
                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                {
                    _fixture.MMALCamera.ConfigureCameraSettings();

                    imgEncoder.ConfigureOutputPort(encodingType, pixelFormat, 90);

                    // Create our component pipeline.         
                    _fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    _fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);
                                        
                    // Camera warm up time
                    await Task.Delay(2000);

                    await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.StillPort);
                }

                if (System.IO.File.Exists(imgCaptureHandler.GetFilepath()))
                {
                    var length = new System.IO.FileInfo(imgCaptureHandler.GetFilepath()).Length;
                    Assert.True(length > 0);
                }
                else
                {
                    Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");
                }
            });
        }

        [Theory]
        [MemberData(nameof(TakePictureDataJpeg))]
        public void TakePictureRawBayer(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakePictureRawBayer", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension);

                TestHelper.CleanDirectory("/home/pi/images/tests");

                using (var preview = new MMALNullSinkComponent())
                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler, true))
                {
                    _fixture.MMALCamera.ConfigureCameraSettings();

                    imgEncoder.ConfigureOutputPort(encodingType, pixelFormat, 90);

                    // Create our component pipeline.         
                    _fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    _fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);
                                        
                    // Camera warm up time
                    await Task.Delay(2000);

                    await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.StillPort);
                }

                if (System.IO.File.Exists(imgCaptureHandler.GetFilepath()))
                {
                    var length = new System.IO.FileInfo(imgCaptureHandler.GetFilepath()).Length;
                    Assert.True(length > 0);
                }
                else
                {
                    Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");
                }
            });
        }

        [Theory]
        [MemberData(nameof(TakeRawPictureData))]
        public void TakePictureRawSensor(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakePictureRawSensor", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension);

                TestHelper.CleanDirectory("/home/pi/images/tests");

                await _fixture.MMALCamera.TakeRawPicture(imgCaptureHandler);

                var encodings = _fixture.MMALCamera.Camera.StillPort.GetSupportedEncodings();

                if (System.IO.File.Exists(imgCaptureHandler.GetFilepath()))
                {
                    var length = new System.IO.FileInfo(imgCaptureHandler.GetFilepath()).Length;

                    if (encodings.Contains(encodingType.EncodingVal))
                    {
                        Assert.True(length > 1);
                    }
                }
                else
                {
                    Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");
                }
            });
        }

        [Fact]
        public void TakePictureTimelapse()
        {
            TestHelper.BeginTest("TakePictureTimelapse");
            TestHelper.SetConfigurationDefaults();
            
            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests/split_tests", "jpg");

                TestHelper.CleanDirectory("/home/pi/images/tests/split_tests");

                using (var preview = new MMALNullSinkComponent())
                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                {
                    _fixture.MMALCamera.ConfigureCameraSettings();

                    imgEncoder.ConfigureOutputPort(0, MMALEncoding.JPEG, MMALEncoding.I420, 90);

                    // Create our component pipeline.         
                    _fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    _fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);
                                        
                    CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                    Timelapse tl = new Timelapse
                    {
                        Mode = TimelapseMode.Second,
                        CancellationToken = cts.Token,
                        Value = 5
                    };

                    // Camera warm up time
                    await Task.Delay(2000);

                    while (!tl.CancellationToken.IsCancellationRequested)
                    {
                        int interval = tl.Value * 1000;
                        
                        await Task.Delay(interval);
                        
                        await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.StillPort);
                    }
                }

                DirectoryInfo info = new DirectoryInfo(imgCaptureHandler.Directory);

                if (info.Exists)
                {
                    var files = info.EnumerateFiles();

                    Assert.True(files != null && files.Count() == 6);
                }
                else
                {
                    Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");
                }
            });
        }

        [Fact]
        public void TakePictureTimeout()
        {
            TestHelper.BeginTest("TakePictureTimeout");
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests/split_tests", "jpg");

                TestHelper.CleanDirectory("/home/pi/images/tests/split_tests");

                using (var preview = new MMALNullSinkComponent())
                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                {
                    _fixture.MMALCamera.ConfigureCameraSettings();

                    imgEncoder.ConfigureOutputPort(0, MMALEncoding.JPEG, MMALEncoding.I420, 90);

                    // Create our component pipeline.         
                    _fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    _fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);

                    // Camera warm up time
                    await Task.Delay(2000);

                    var timeout = DateTime.Now.AddSeconds(10);
                    while (DateTime.Now.CompareTo(timeout) < 0)
                    {
                        await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.StillPort);
                    }
                }
            });
        }

        [Fact]
        public void ChangeEncodingType()
        {
            TestHelper.BeginTest("Image - ChangeEncodingType");
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", "jpg");

                TestHelper.CleanDirectory("/home/pi/images/tests");

                using (var preview = new MMALNullSinkComponent())
                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                {
                    _fixture.MMALCamera.ConfigureCameraSettings();

                    imgEncoder.ConfigureOutputPort(MMALEncoding.JPEG, MMALEncoding.I420, 90);

                    // Create our component pipeline.         
                    _fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    _fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);
                                        
                    // Camera warm up time
                    await Task.Delay(2000);

                    await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.StillPort);
                }

                if (System.IO.File.Exists(imgCaptureHandler.GetFilepath()))
                {
                    var length = new System.IO.FileInfo(imgCaptureHandler.GetFilepath()).Length;
                    Assert.True(length > 0);
                }
                else
                {
                    Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");
                }

                imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", "bmp");

                using (var preview = new MMALNullSinkComponent())
                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                {
                    imgEncoder.ConfigureOutputPort(MMALEncoding.BMP, MMALEncoding.I420, 90);

                    // Create our component pipeline.         
                    _fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    _fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);
                    
                    await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.StillPort);
                }

                if (System.IO.File.Exists(imgCaptureHandler.GetFilepath()))
                {
                    var length = new System.IO.FileInfo(imgCaptureHandler.GetFilepath()).Length;
                    Assert.True(length > 0);
                }
                else
                {
                    Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");
                }
            });
        }
    }
}
