// <copyright file="ImageEncoderTests.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Config;
using MMALSharp.Ports;
using MMALSharp.Processors;
using MMALSharp.Tests.Data;
using Xunit;

namespace MMALSharp.Tests
{
    public class ImageEncoderTests : TestBase
    {
        public ImageEncoderTests(MMALFixture fixture)
            : base(fixture)
        {
        }

        [Theory]
        [MemberData(nameof(ImageData.Data), MemberType = typeof(ImageData))]
        public async Task TakePicture(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakePicture", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");
                
            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension))
            using (var preview = new MMALNullSinkComponent())
            using (var imgEncoder = new MMALImageEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(encodingType, pixelFormat, 90);

                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.StillPort
                    .ConnectTo(imgEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);
                                        
                // Camera warm up time
                await Task.Delay(2000);

                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.StillPort);

                Fixture.CheckAndAssertFilepath(imgCaptureHandler.GetFilepath());
            }
        }

        [Theory]
        [MemberData(nameof(BasicImageData.Data), MemberType = typeof(BasicImageData))]
        public async Task TakePictureRawBayer(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakePictureRawBayer", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");

            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension))
            using (var preview = new MMALNullSinkComponent())
            using (var imgEncoder = new MMALImageEncoder(true))
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(encodingType, pixelFormat, 90);

                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.StillPort
                    .ConnectTo(imgEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);
                                        
                // Camera warm up time
                await Task.Delay(2000);

                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.StillPort);

                Fixture.CheckAndAssertFilepath(imgCaptureHandler.GetFilepath());
            }
        }

        [Theory]
        [MemberData(nameof(RawImageData.Data), MemberType = typeof(RawImageData))]
        public async Task TakePictureRawSensor(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakePictureRawSensor", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.StillEncoding = encodingType;
            MMALCameraConfig.StillSubFormat = pixelFormat;
            
            TestHelper.CleanDirectory("/home/pi/images/tests");
            
            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension))
            {
                await Fixture.MMALCamera.TakeRawPicture(imgCaptureHandler);

                var encodings = Fixture.MMALCamera.Camera.StillPort.GetSupportedEncodings();

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
        [MemberData(nameof(BasicImageData.Data), MemberType = typeof(BasicImageData))]
        public async Task TakePicturesFromVideoPort(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakePicturesFromVideoPort", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");
            
            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension))
            using (var splitter = new MMALSplitterComponent())
            using (var preview = new MMALNullSinkComponent())
            using (var imgEncoder = new MMALImageEncoder(continuousCapture: true))
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(encodingType, pixelFormat, 90);

                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                // Create our component pipeline.
                Fixture.MMALCamera.Camera.VideoPort
                    .ConnectTo(splitter);
                splitter.Outputs[0].ConnectTo(imgEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);
                                      
                // Camera warm up time
                await Task.Delay(2000);

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                
                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort, cts.Token);
                
                Fixture.CheckAndAssertDirectory(imgCaptureHandler.Directory);
            }
        }

        [Theory]
        [MemberData(nameof(BasicImageData.Data), MemberType = typeof(BasicImageData))]
        public async Task TakePicturesFromVideoPortWithCustomFilename(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakePicturesFromVideoPortWithCustomFilename", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");

            using (var imgCaptureHandler = new ImageStreamCaptureHandler($"/home/pi/images/tests/fromVideoPort.{extension}"))
            using (var splitter = new MMALSplitterComponent())
            using (var preview = new MMALNullSinkComponent())
            using (var imgEncoder = new MMALImageEncoder(continuousCapture: true))
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(encodingType, pixelFormat, 90);

                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                // Create our component pipeline.
                Fixture.MMALCamera.Camera.VideoPort
                    .ConnectTo(splitter);
                splitter.Outputs[0].ConnectTo(imgEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                // Camera warm up time
                await Task.Delay(2000);

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort, cts.Token);

                Fixture.CheckAndAssertDirectory(imgCaptureHandler.Directory);
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
            using (var imgEncoder = new MMALImageEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(MMALEncoding.JPEG, MMALEncoding.I420, 90);

                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.StillPort
                    .ConnectTo(imgEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);
                  
                // Camera warm up time
                await Task.Delay(2000);

                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                Timelapse tl = new Timelapse
                {
                    Mode = TimelapseMode.Second,
                    CancellationToken = cts.Token,
                    Value = 5
                };
                
                while (!tl.CancellationToken.IsCancellationRequested)
                {
                    int interval = tl.Value * 1000;
                        
                    await Task.Delay(interval);
                        
                    await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.StillPort);
                }

                Fixture.CheckAndAssertDirectory(imgCaptureHandler.Directory);
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
            using (var imgEncoder = new MMALImageEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(MMALEncoding.JPEG, MMALEncoding.I420, 90);

                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.StillPort
                    .ConnectTo(imgEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                // Camera warm up time
                await Task.Delay(2000);

                var timeout = DateTime.Now.AddSeconds(10);
                while (DateTime.Now.CompareTo(timeout) < 0)
                {
                    await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.StillPort);
                }

                Fixture.CheckAndAssertDirectory(imgCaptureHandler.Directory);
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
            using (var imgEncoder = new MMALImageEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(MMALEncoding.JPEG, MMALEncoding.I420, 90);

                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.StillPort
                    .ConnectTo(imgEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);
                                        
                // Camera warm up time
                await Task.Delay(2000);

                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.StillPort);
                
                Fixture.CheckAndAssertFilepath(imgCaptureHandler.GetFilepath());
            }
            
            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", "bmp"))
            using (var preview = new MMALNullSinkComponent())
            using (var imgEncoder = new MMALImageEncoder())
            {
                var portConfig = new MMALPortConfig(MMALEncoding.BMP, MMALEncoding.I420, 0);

                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.StillPort
                    .ConnectTo(imgEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);
                    
                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.StillPort);
            }
            
            Fixture.CheckAndAssertDirectory("/home/pi/images/tests");
        }

        [Fact]
        public async Task StaticOverlay()
        {
            TestHelper.BeginTest("StaticOverlay");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");
            TestHelper.CleanDirectory("/home/pi/images/tests/staticoverlay");

            MMALCameraConfig.StillResolution = Resolution.As03MPixel;
            MMALCameraConfig.StillEncoding = MMALEncoding.I420;
            MMALCameraConfig.StillSubFormat = MMALEncoding.I420;
            
            var filename = string.Empty;
            
            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests/staticoverlay", "raw"))
            {
                await Fixture.MMALCamera.TakeRawPicture(imgCaptureHandler);

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
            using (var imgEncoder = new MMALImageEncoder())
            using (var video = new MMALVideoRenderer(previewConfig))
            {
                Fixture.MMALCamera.ConfigureCameraSettings();
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

                var overlay = Fixture.MMALCamera.AddOverlay(video, overlayConfig, File.ReadAllBytes(filename));
                overlay.ConfigureRenderer();
                overlay.UpdateOverlay();

                // Create our component pipeline.
                var portConfig = new MMALPortConfig(MMALEncoding.JPEG, MMALEncoding.I420, 90);

                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                Fixture.MMALCamera.Camera.StillPort.ConnectTo(imgEncoder);
                Fixture.MMALCamera.Camera.PreviewPort.ConnectTo(video);

                Fixture.MMALCamera.PrintPipeline();

                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.StillPort);

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
            
            var imgCaptureHandler = new InMemoryCaptureHandler();
            using (var preview = new MMALNullSinkComponent())
            using (var imgEncoder = new MMALImageEncoder(true))
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(MMALEncoding.JPEG, MMALEncoding.I420, 90);

                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                // Create our component pipeline.
                Fixture.MMALCamera.Camera.StillPort
                    .ConnectTo(imgEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);
                                      
                // Camera warm up time
                await Task.Delay(2000);
                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.StillPort);

                Assert.True(imgCaptureHandler.WorkingData.Count > 0);
            }
        }

        [Fact]
        public async Task JpegThumbnail()
        {
            TestHelper.BeginTest("Image - JpegThumbnail");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");
            
            JpegThumbnail tm = new JpegThumbnail(true, 200, 200, 90);
            
            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", "jpg"))
            using (var preview = new MMALNullSinkComponent())
            using (var imgEncoder = new MMALImageEncoder(thumbnailConfig: tm))
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(MMALEncoding.JPEG, MMALEncoding.I420, 90);

                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.StillPort
                    .ConnectTo(imgEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                imgCaptureHandler.Manipulate(context =>
                {
                    context.StripBayerMetadata(CameraVersion.OV5647);
                }, ImageFormat.Jpeg);
                
                // Camera warm up time
                await Task.Delay(2000);
                
                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.StillPort);
            }
        }

        [Theory]
        [MemberData(nameof(ImageFxData.Data), MemberType = typeof(ImageFxData))]
        public async Task ImageFxComponentFromCameraStillPort(MMAL_PARAM_IMAGEFX_T effect, bool throwsException)
        {
            TestHelper.BeginTest($"Image - ImageFxComponentFromCameraStillPort - {effect}");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");

            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", "jpg"))
            using (var preview = new MMALNullSinkComponent())
            using (var imageFx = new MMALImageFxComponent())
            using (var imgEncoder = new MMALImageEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(MMALEncoding.JPEG, MMALEncoding.I420, 90);
                var fxConfig = new MMALPortConfig(MMALEncoding.I420, MMALEncoding.I420);

                imageFx.ConfigureOutputPort(fxConfig, null);
                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                if (throwsException)
                {
                    Assert.Throws<MMALInvalidException>(() =>
                    {
                        imageFx.ImageEffect = effect;
                    });
                }
                else
                {
                    imageFx.ImageEffect = effect;
                }

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.StillPort
                    .ConnectTo(imageFx);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                imageFx.Outputs[0].ConnectTo(imgEncoder);
                
                // Camera warm up time
                await Task.Delay(2000);

                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.StillPort);
            }
        }

        [Fact]
        public async Task ImageFxComponentFromCameraStillPortSetColourEnhancement()
        {
            TestHelper.BeginTest("Image - ImageFxComponentFromCameraStillPortSetColourEnhancement");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");

            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", "jpg"))
            using (var preview = new MMALNullSinkComponent())
            using (var imageFx = new MMALImageFxComponent())
            using (var imgEncoder = new MMALImageEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(MMALEncoding.JPEG, MMALEncoding.I420, 90);
                var fxConfig = new MMALPortConfig(MMALEncoding.I420, MMALEncoding.I420);

                imageFx.ConfigureOutputPort(fxConfig, null);
                imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);
                
                imageFx.ImageEffect = MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_SOLARIZE;
                imageFx.ColourEnhancement = new ColourEffects(true, Color.Blue);
                
                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.StillPort
                    .ConnectTo(imageFx);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                imageFx.Outputs[0].ConnectTo(imgEncoder);

                // Camera warm up time
                await Task.Delay(2000);

                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.StillPort);
            }
        }
    }
}
