// <copyright file="ImageEncoderTests.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        [Theory]
        [MemberData("Data", MemberType = typeof(ImageData))]
        public async Task TakePicture(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakePicture", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");
                
            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension))
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

                if (File.Exists(imgCaptureHandler.GetFilepath()))
                {
                    var length = new FileInfo(imgCaptureHandler.GetFilepath()).Length;
                    Assert.True(length > 0);
                }
                else
                {
                    Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");
                }
            }
           
        }

        [Theory]
        [MemberData("Data", MemberType = typeof(BasicImageData))]
        public async Task TakePictureRawBayer(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakePictureRawBayer", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");

            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension))
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

                if (File.Exists(imgCaptureHandler.GetFilepath()))
                {
                    var length = new FileInfo(imgCaptureHandler.GetFilepath()).Length;
                    Assert.True(length > 0);
                }
                else
                {
                    Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");
                }
            }
        }

        [Theory]
        [MemberData("Data", MemberType = typeof(RawImageData))]
        public async Task TakePictureRawSensor(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakePictureRawSensor", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();
            
            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension))
            {
                TestHelper.CleanDirectory("/home/pi/images/tests");

                await _fixture.MMALCamera.TakeRawPicture(imgCaptureHandler);

                var encodings = _fixture.MMALCamera.Camera.StillPort.GetSupportedEncodings();

                if (File.Exists(imgCaptureHandler.GetFilepath()))
                {
                    var length = new FileInfo(imgCaptureHandler.GetFilepath()).Length;

                    if (encodings.Contains(encodingType.EncodingVal))
                    {
                        Assert.True(length > 1);
                    }
                }
                else
                {
                    Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");
                }
            }
        }
        
        [Theory]
        [MemberData("Data", MemberType = typeof(BasicImageData))]
        public async Task TakePicturesFromVideoPort(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakePicturesFromVideoPort", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");
            
            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension))
            using (var splitter = new MMALSplitterComponent(null))
            using (var preview = new MMALNullSinkComponent())
            using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler, continuousCapture: true))
            {
                _fixture.MMALCamera.ConfigureCameraSettings();

                imgEncoder.ConfigureOutputPort(encodingType, pixelFormat, 90);

                // Create our component pipeline.
                _fixture.MMALCamera.Camera.VideoPort
                    .ConnectTo(splitter);
                splitter.Outputs[0].ConnectTo(imgEncoder);
                _fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);
                                      
                // Camera warm up time
                await Task.Delay(2000);

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                
                await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.VideoPort, cts.Token);

                if (File.Exists(imgCaptureHandler.GetFilepath()))
                {
                    var length = new FileInfo(imgCaptureHandler.GetFilepath()).Length;
                    Assert.True(length > 0);
                }
                else
                {
                    Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");
                }
            }
        }

        [Fact]
        public async Task TakePictureTimelapse()
        {
            TestHelper.BeginTest("TakePictureTimelapse");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests/split_tests");

            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests/split_tests", "jpg"))
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
            }
        }

        [Fact]
        public async Task TakePictureTimeout()
        {
            TestHelper.BeginTest("TakePictureTimeout");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests/split_tests");

            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests/split_tests", "jpg"))
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
            
        }

        [Fact]
        public async Task ChangeEncodingType()
        {
            TestHelper.BeginTest("Image - ChangeEncodingType");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");

            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", "jpg"))
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

                _fixture.CheckAndAssertFilepath(imgCaptureHandler.GetFilepath());
            }
                
            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", "bmp"))
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

                _fixture.CheckAndAssertFilepath(imgCaptureHandler.GetFilepath());
            }
        }

        [Fact]
        public async Task StaticOverlay()
        {
            TestHelper.BeginTest("StaticOverlay");
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.StillResolution = Resolution.As03MPixel;
            MMALCameraConfig.StillEncoding = MMALEncoding.I420;
            MMALCameraConfig.StillSubFormat = MMALEncoding.I420;
            
            var filename = string.Empty;

            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests/staticoverlay", "raw"))
            {
                TestHelper.CleanDirectory("/home/pi/images/tests");
                TestHelper.CleanDirectory("/home/pi/images/tests/staticoverlay");

                await _fixture.MMALCamera.TakeRawPicture(imgCaptureHandler);

                filename = imgCaptureHandler.GetFilepath();
            }
                
            PreviewConfiguration previewConfig = new PreviewConfiguration
            {
                FullScreen = false,
                PreviewWindow = new Rectangle(160, 0, 640, 480),
                Layer = 2,
                Opacity = 1
            };

            MMALCameraConfig.StillResolution = Resolution.As1080p;
            MMALCameraConfig.StillEncoding = MMALEncoding.OPAQUE;
                
            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", "jpg"))
            using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
            using (var video = new MMALVideoRenderer(previewConfig))
            {
                _fixture.MMALCamera.ConfigureCameraSettings();
                video.ConfigureRenderer();

                PreviewOverlayConfiguration overlayConfig = new PreviewOverlayConfiguration
                {
                    FullScreen = true,
                    PreviewWindow = new Rectangle(50, 0, 640, 480),
                    Layer = 1,
                    Resolution = new Resolution(640, 480),
                    Encoding = MMALEncoding.I420,
                    Opacity = 255
                };

                var overlay = _fixture.MMALCamera.AddOverlay(video, overlayConfig, File.ReadAllBytes(filename));
                overlay.ConfigureRenderer();
                overlay.UpdateOverlay();

                // Create our component pipeline.
                imgEncoder.ConfigureOutputPort(0, MMALEncoding.JPEG, MMALEncoding.I420, 90);

                _fixture.MMALCamera.Camera.StillPort.ConnectTo(imgEncoder);
                _fixture.MMALCamera.Camera.PreviewPort.ConnectTo(video);

                _fixture.MMALCamera.PrintPipeline();

                await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.StillPort);

                if (File.Exists(imgCaptureHandler.GetFilepath()))
                {
                    var length = new FileInfo(imgCaptureHandler.GetFilepath()).Length;
                    Assert.True(length > 0);
                }
                else
                {
                    Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");
                }
            }
        }

        [Fact]
        public async Task TakePictureWithInMemoryHandler()
        {
            TestHelper.BeginTest("TakePictureWithInMemoryHandler");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");
            
            var imgCaptureHandler = new InMemoryCaptureHandler();
            using (var preview = new MMALNullSinkComponent())
            using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler, true))
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
                await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.StillPort);

                Assert.True(imgCaptureHandler.WorkingData.Count > 0);
            }
        }
    }
}
