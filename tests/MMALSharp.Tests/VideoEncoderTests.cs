// <copyright file="VideoEncoderTests.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MMALSharp.Common;
using MMALSharp.Components;
using MMALSharp.Config;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using MMALSharp.Ports.Outputs;
using MMALSharp.Tests.Data;
using Xunit;

namespace MMALSharp.Tests
{
    public class VideoEncoderTests : TestBase
    {
        public VideoEncoderTests(MMALFixture fixture)
           : base(fixture)
        {
        }

        #region Configuration tests

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [MMALTestsAttribute]
        public void SetThenGetVideoStabilisation(bool vstab)
        {
            MMALCameraConfig.VideoStabilisation = vstab;
            Fixture.MMALCamera.ConfigureCameraSettings();
            Assert.True(Fixture.MMALCamera.Camera.GetVideoStabilisation() == vstab);
        }

        #endregion

        [Theory]
        [MemberData(nameof(VideoData.Data), MemberType = typeof(VideoData))]
        public async Task TakeVideo(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakeVideo", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests");
                
            using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", extension))
            using (var preview = new MMALVideoRenderer())
            using (var vidEncoder = new MMALVideoEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(encodingType, pixelFormat, quality: 10, bitrate: 25000000);

                vidEncoder.ConfigureOutputPort(portConfig, vidCaptureHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.VideoPort
                    .ConnectTo(vidEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);
                    
                // Camera warm up time
                await Task.Delay(2000);

                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

                // Record video for 15 seconds
                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort, cts.Token);

                Fixture.CheckAndAssertFilepath(vidCaptureHandler.GetFilepath());
            }
        }

        [Fact]
        public async Task TakeVideoSplit()
        {
            TestHelper.BeginTest("TakeVideoSplit");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests/split_test");

            MMALCameraConfig.InlineHeaders = true;
            
            using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests/split_test", "h264"))
            using (var preview = new MMALVideoRenderer())
            using (var vidEncoder = new MMALVideoEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();
                                
                var split = new Split
                {
                    Mode = TimelapseMode.Second,
                    Value = 15
                };

                var portConfig = new MMALPortConfig(MMALEncoding.H264, MMALEncoding.I420, quality: 10, bitrate: 25000000, split: split);

                vidEncoder.ConfigureOutputPort(portConfig, vidCaptureHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.VideoPort
                    .ConnectTo(vidEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                // Camera warm up time
                await Task.Delay(2000);

                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

                // 2 files should be created from this test. 
                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort, cts.Token);
                    
                Assert.True(Directory.GetFiles("/home/pi/videos/tests/split_test").Length == 2);
            }
        }

        [Fact]
        public async Task ChangeEncoderType()
        {
            TestHelper.BeginTest("Video - ChangeEncoderType");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests");
                
            using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "h264"))
            using (var preview = new MMALVideoRenderer())
            using (var vidEncoder = new MMALVideoEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(MMALEncoding.MJPEG, MMALEncoding.I420, quality: 10, bitrate: 25000000);

                vidEncoder.ConfigureOutputPort(portConfig, vidCaptureHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.VideoPort
                    .ConnectTo(vidEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                // Camera warm up time
                await Task.Delay(2000);

                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

                // Record video for 20 seconds
                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort, cts.Token);

                Fixture.CheckAndAssertFilepath(vidCaptureHandler.GetFilepath());
            }
                
            using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "mjpeg"))
            using (var preview = new MMALVideoRenderer())
            using (var vidEncoder = new MMALVideoEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(MMALEncoding.MJPEG, MMALEncoding.I420, quality: 10, bitrate: 25000000);

                vidEncoder.ConfigureOutputPort(portConfig, vidCaptureHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.VideoPort
                    .ConnectTo(vidEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

                // Record video for 20 seconds
                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort, cts.Token);

                Fixture.CheckAndAssertFilepath(vidCaptureHandler.GetFilepath());
            }
        }

        [Fact]
        public async Task VideoSplitterComponent()
        {
            TestHelper.BeginTest("VideoSplitterComponent");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests");

            using (var handler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "h264"))
            using (var handler2 = new VideoStreamCaptureHandler("/home/pi/videos/tests", "h264"))
            using (var handler3 = new VideoStreamCaptureHandler("/home/pi/videos/tests", "h264"))
            using (var handler4 = new VideoStreamCaptureHandler("/home/pi/videos/tests", "h264"))
            using (var splitter = new MMALSplitterComponent())
            using (var vidEncoder = new MMALVideoEncoder())
            using (var vidEncoder2 = new MMALVideoEncoder())
            using (var vidEncoder3 = new MMALVideoEncoder())
            using (var vidEncoder4 = new MMALVideoEncoder())
            using (var renderer = new MMALNullSinkComponent())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var splitterPortConfig = new MMALPortConfig(MMALEncoding.OPAQUE, MMALEncoding.I420, quality: 0, bitrate: 13000000);
                var portConfig1 = new MMALPortConfig(MMALEncoding.H264, MMALEncoding.I420, quality: 10, bitrate: 13000000, timeout: DateTime.Now.AddSeconds(20));
                var portConfig2 = new MMALPortConfig(MMALEncoding.H264, MMALEncoding.I420, quality: 20, bitrate: 13000000, timeout: DateTime.Now.AddSeconds(15));
                var portConfig3 = new MMALPortConfig(MMALEncoding.H264, MMALEncoding.I420, quality: 30, bitrate: 13000000, timeout: DateTime.Now.AddSeconds(10));
                var portConfig4 = new MMALPortConfig(MMALEncoding.H264, MMALEncoding.I420, quality: 40, bitrate: 13000000, timeout: DateTime.Now.AddSeconds(10));

                // Create our component pipeline.         
                splitter.ConfigureInputPort(new MMALPortConfig(MMALEncoding.OPAQUE, MMALEncoding.I420), Fixture.MMALCamera.Camera.VideoPort, null);
                splitter.ConfigureOutputPort(0, splitterPortConfig, null);
                splitter.ConfigureOutputPort(1, splitterPortConfig, null);
                splitter.ConfigureOutputPort(2, splitterPortConfig, null);
                splitter.ConfigureOutputPort(3, splitterPortConfig, null);

                vidEncoder.ConfigureInputPort(new MMALPortConfig(MMALEncoding.OPAQUE, MMALEncoding.I420), splitter.Outputs[0], null);
                vidEncoder.ConfigureOutputPort(0, portConfig1, handler);

                vidEncoder2.ConfigureInputPort(new MMALPortConfig(MMALEncoding.OPAQUE, MMALEncoding.I420), splitter.Outputs[1], null);
                vidEncoder2.ConfigureOutputPort(0, portConfig2, handler2);
                vidEncoder3.ConfigureInputPort(new MMALPortConfig(MMALEncoding.OPAQUE, MMALEncoding.I420), splitter.Outputs[2], null);
                vidEncoder3.ConfigureOutputPort(0, portConfig3, handler3);

                vidEncoder4.ConfigureInputPort(new MMALPortConfig(MMALEncoding.OPAQUE, MMALEncoding.I420), splitter.Outputs[3], null);
                vidEncoder4.ConfigureOutputPort(0, portConfig4, handler4);

                Fixture.MMALCamera.Camera.VideoPort.ConnectTo(splitter);

                splitter.Outputs[0].ConnectTo(vidEncoder);
                splitter.Outputs[1].ConnectTo(vidEncoder2);
                splitter.Outputs[2].ConnectTo(vidEncoder3);
                splitter.Outputs[3].ConnectTo(vidEncoder4);

                Fixture.MMALCamera.Camera.PreviewPort.ConnectTo(renderer);

                // Camera warm up time
                await Task.Delay(2000);

                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort);

                Fixture.CheckAndAssertFilepath(handler.GetFilepath());
                Fixture.CheckAndAssertFilepath(handler2.GetFilepath());
                Fixture.CheckAndAssertFilepath(handler3.GetFilepath());
                Fixture.CheckAndAssertFilepath(handler4.GetFilepath());
            }
        }
        
        [Fact]
        public void ChangeColorSpace()
        {
            TestHelper.BeginTest("ChangeColorSpace");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests");

            MMALCameraConfig.VideoColorSpace = MMALEncoding.MMAL_COLOR_SPACE_ITUR_BT601;
            
            using (var handler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "h264"))
            using (var handler2 = new VideoStreamCaptureHandler("/home/pi/video/tests", "h264"))
            using (var handler3 = new VideoStreamCaptureHandler("/home/pi/video/tests", "h264"))
            using (var handler4 = new VideoStreamCaptureHandler("/home/pi/video/tests", "h264"))
            using (var splitter = new MMALSplitterComponent())
            using (var vidEncoder = new MMALVideoEncoder())
            using (var vidEncoder2 = new MMALVideoEncoder())
            using (var vidEncoder3 = new MMALVideoEncoder())
            using (var vidEncoder4 = new MMALVideoEncoder())
            using (var renderer = new MMALVideoRenderer())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var splitterPortConfig = new MMALPortConfig(MMALEncoding.OPAQUE, MMALEncoding.I420, bitrate: 13000000);
                var portConfig1 = new MMALPortConfig(MMALEncoding.H264, MMALEncoding.I420, quality: 10, bitrate: 13000000, timeout: DateTime.Now.AddSeconds(20));
                var portConfig2 = new MMALPortConfig(MMALEncoding.H264, MMALEncoding.I420, quality: 20, bitrate: 13000000, timeout: DateTime.Now.AddSeconds(15));
                var portConfig3 = new MMALPortConfig(MMALEncoding.H264, MMALEncoding.I420, quality: 30, bitrate: 13000000, timeout: DateTime.Now.AddSeconds(10));
                var portConfig4 = new MMALPortConfig(MMALEncoding.H264, MMALEncoding.I420, quality: 40, bitrate: 13000000, timeout: DateTime.Now.AddSeconds(10));

                // Create our component pipeline.         
                splitter.ConfigureInputPort(new MMALPortConfig(MMALEncoding.OPAQUE, MMALEncoding.I420), Fixture.MMALCamera.Camera.VideoPort, null);
                splitter.ConfigureOutputPort(0, splitterPortConfig, null);
                splitter.ConfigureOutputPort(1, splitterPortConfig, null);
                splitter.ConfigureOutputPort(2, splitterPortConfig, null);
                splitter.ConfigureOutputPort(3, splitterPortConfig, null);

                vidEncoder.ConfigureInputPort(new MMALPortConfig(MMALEncoding.OPAQUE, MMALEncoding.I420), splitter.Outputs[0], null);
                vidEncoder.ConfigureOutputPort(0, portConfig1, handler);

                vidEncoder2.ConfigureInputPort(new MMALPortConfig(MMALEncoding.OPAQUE, MMALEncoding.I420), splitter.Outputs[1], null);
                vidEncoder2.ConfigureOutputPort(0, portConfig2, handler2);
                vidEncoder3.ConfigureInputPort(new MMALPortConfig(MMALEncoding.OPAQUE, MMALEncoding.I420), splitter.Outputs[2], null);
                vidEncoder3.ConfigureOutputPort(0, portConfig3, handler3);

                vidEncoder4.ConfigureInputPort(new MMALPortConfig(MMALEncoding.OPAQUE, MMALEncoding.I420), splitter.Outputs[3], null);
                vidEncoder4.ConfigureOutputPort(0, portConfig4, handler4);

                Fixture.MMALCamera.Camera.VideoPort.ConnectTo(splitter);

                splitter.Outputs[0].ConnectTo(vidEncoder);
                splitter.Outputs[1].ConnectTo(vidEncoder2);
                splitter.Outputs[2].ConnectTo(vidEncoder3);
                splitter.Outputs[3].ConnectTo(vidEncoder4);

                Fixture.MMALCamera.Camera.PreviewPort.ConnectTo(renderer);

                // Assert that all output ports have the same video color space.
                Assert.True(Fixture.MMALCamera.Camera.VideoPort.VideoColorSpace.EncodingVal == MMALEncoding.MMAL_COLOR_SPACE_ITUR_BT601.EncodingVal);
                
                Assert.True(splitter.Outputs[0].VideoColorSpace.EncodingVal == MMALEncoding.MMAL_COLOR_SPACE_ITUR_BT601.EncodingVal);
                Assert.True(splitter.Outputs[1].VideoColorSpace.EncodingVal == MMALEncoding.MMAL_COLOR_SPACE_ITUR_BT601.EncodingVal);
                Assert.True(splitter.Outputs[2].VideoColorSpace.EncodingVal == MMALEncoding.MMAL_COLOR_SPACE_ITUR_BT601.EncodingVal);
                Assert.True(splitter.Outputs[3].VideoColorSpace.EncodingVal == MMALEncoding.MMAL_COLOR_SPACE_ITUR_BT601.EncodingVal);
                
                Assert.True(vidEncoder.Outputs[0].VideoColorSpace.EncodingVal == MMALEncoding.MMAL_COLOR_SPACE_ITUR_BT601.EncodingVal);
                Assert.True(vidEncoder2.Outputs[0].VideoColorSpace.EncodingVal == MMALEncoding.MMAL_COLOR_SPACE_ITUR_BT601.EncodingVal);
                Assert.True(vidEncoder3.Outputs[0].VideoColorSpace.EncodingVal == MMALEncoding.MMAL_COLOR_SPACE_ITUR_BT601.EncodingVal);
                Assert.True(vidEncoder4.Outputs[0].VideoColorSpace.EncodingVal == MMALEncoding.MMAL_COLOR_SPACE_ITUR_BT601.EncodingVal);
            }
        }

        [Theory]
        [MemberData(nameof(VideoData.H264Data), MemberType = typeof(VideoData))]
        public async Task TakeVideoAndStoreMotionVectors(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakeVideoAndStoreMotionVectors", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests");

            // Ensure inline motion vectors are enabled.
            MMALCameraConfig.InlineMotionVectors = true;

            using (var motionVectorStore = File.Create("/home/pi/videos/tests/motion.dat"))
            using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", extension))
            using (var preview = new MMALVideoRenderer())
            using (var vidEncoder = new MMALVideoEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(encodingType, pixelFormat, quality: 10, bitrate: 25000000, storeMotionVectors: true);

                vidEncoder.ConfigureOutputPort(portConfig, vidCaptureHandler);

                // Initialise the motion vector stream.
                vidCaptureHandler.InitialiseMotionStore(motionVectorStore);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.VideoPort
                    .ConnectTo(vidEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                // Camera warm up time
                await Task.Delay(2000);

                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                // Record video for 10 seconds
                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort, cts.Token);

                Fixture.CheckAndAssertFilepath(vidCaptureHandler.GetFilepath());
            }
        }

        [Theory]
        [MemberData(nameof(VideoData.H264Data), MemberType = typeof(VideoData))]
        public async Task TakeVideoWithCircularBuffer(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakeVideoWithCircularBuffer", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests");
            
            using (var circularBufferHandler = new CircularBufferCaptureHandler(4096, "/home/pi/videos/tests", extension))            
            using (var preview = new MMALVideoRenderer())
            using (var vidEncoder = new MMALVideoEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(encodingType, pixelFormat, quality: 10, bitrate: 25000000, storeMotionVectors: true);

                vidEncoder.ConfigureOutputPort(portConfig, circularBufferHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.VideoPort
                    .ConnectTo(vidEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                // Camera warm up time
                await Task.Delay(2000);

                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                // Record video for 10 seconds
                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort, cts.Token);

                // Check that the circular buffer has stored some data during the recording operation.
                Assert.True(circularBufferHandler.Buffer.Size > 0);
            }
        }

        [Theory]
        [MemberData(nameof(ImageFxData.Data), MemberType = typeof(ImageFxData))]
        public async Task ImageFxComponentFromCameraVideoPort(MMAL_PARAM_IMAGEFX_T effect, bool throwsException)
        {
            TestHelper.BeginTest($"Video - ImageFxComponentFromCameraVideoPort - {effect}");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests");

            using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "raw"))
            using (var preview = new MMALNullSinkComponent())
            using (var imageFx = new MMALImageFxComponent())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();
                
                var fxConfig = new MMALPortConfig(MMALEncoding.I420, MMALEncoding.I420);

                imageFx.ConfigureOutputPort<VideoPort>(0, fxConfig, vidCaptureHandler);

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
                Fixture.MMALCamera.Camera.VideoPort
                    .ConnectTo(imageFx);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                // Camera warm up time
                await Task.Delay(2000);

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort, cts.Token);

                Fixture.CheckAndAssertFilepath(vidCaptureHandler.GetFilepath());
            }
        }

        [Theory]
        [MemberData(nameof(ImageFxData.Data), MemberType = typeof(ImageFxData))]
        public async Task ImageFxComponentFromCameraVideoPortWithSplitterAndEncoder(MMAL_PARAM_IMAGEFX_T effect, bool throwsException)
        {
            TestHelper.BeginTest($"Video - ImageFxComponentFromCameraVideoPortWithSplitterAndEncoder - {effect}");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests");

            using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "h264"))
            using (var preview = new MMALNullSinkComponent())
            using (var imageFx = new MMALImageFxComponent())
            using (var splitter = new MMALSplitterComponent())
            using (var vidEncoder = new MMALVideoEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var vidEncoderConfig = new MMALPortConfig(MMALEncoding.H264, MMALEncoding.I420);
                var splitterConfig = new MMALPortConfig(MMALEncoding.I420, MMALEncoding.I420);
                var fxConfig = new MMALPortConfig(MMALEncoding.I420, MMALEncoding.I420);

                imageFx.ConfigureOutputPort<VideoPort>(0, fxConfig, null);

                splitter.ConfigureInputPort(new MMALPortConfig(MMALEncoding.I420, MMALEncoding.I420), imageFx.Outputs[0], null);
                splitter.ConfigureOutputPort<VideoPort>(0, splitterConfig, null);

                vidEncoder.ConfigureInputPort(new MMALPortConfig(MMALEncoding.I420, MMALEncoding.I420), splitter.Outputs[0], null);
                vidEncoder.ConfigureOutputPort(0, vidEncoderConfig, vidCaptureHandler);

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
                Fixture.MMALCamera.Camera.VideoPort
                    .ConnectTo(imageFx);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                imageFx.Outputs[0].ConnectTo(splitter);
                splitter.Outputs[0].ConnectTo(vidEncoder);

                // Camera warm up time
                await Task.Delay(2000);

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort, cts.Token);

                Fixture.CheckAndAssertFilepath(vidCaptureHandler.GetFilepath());
            }
        }

        [Fact]
        public async Task TakeVideoAndStoreTimestamps()
        {
            TestHelper.BeginTest("Video - TakeVideoAndStoreTimestamps");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests");
            
            using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "h264", true))
            using (var preview = new MMALVideoRenderer())
            using (var vidEncoder = new MMALVideoEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(MMALEncoding.H264, MMALEncoding.I420, bitrate: MMALVideoEncoder.MaxBitrateLevel4);

                vidEncoder.ConfigureOutputPort(portConfig, vidCaptureHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.VideoPort
                    .ConnectTo(vidEncoder);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                // Camera warm up time
                await Task.Delay(2000);

                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

                // Record video for 15 seconds
                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort, cts.Token);

                Fixture.CheckAndAssertFilepath(vidCaptureHandler.GetFilepath());
                Fixture.CheckAndAssertFilepath($"{vidCaptureHandler.Directory}/{vidCaptureHandler.CurrentFilename}.pts");
            }
        }

        [Fact]
        public async Task AnnotateVideoRefreshSeconds()
        {
            TestHelper.BeginTest("Video - AnnotateVideo");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests");

            MMALCameraConfig.Annotate = new AnnotateImage();
            MMALCameraConfig.Annotate.RefreshRate = DateTimeTextRefreshRate.Seconds;
            MMALCameraConfig.Annotate.TimeFormat = "HH:mm:ss";

            using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "h264"))
            using (var vidEncoder = new MMALVideoEncoder())
            using (var renderer = new MMALVideoRenderer())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(MMALEncoding.H264, MMALEncoding.I420, quality: 10, bitrate: MMALVideoEncoder.MaxBitrateLevel4);

                // Create our component pipeline. Here we are using the H.264 standard with a YUV420 pixel format. The video will be taken at 25Mb/s.
                vidEncoder.ConfigureOutputPort(portConfig, vidCaptureHandler);

                Fixture.MMALCamera.Camera.VideoPort.ConnectTo(vidEncoder);
                Fixture.MMALCamera.Camera.PreviewPort.ConnectTo(renderer);

                // Camera warm up time
                await Task.Delay(2000);

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

                // Take video for 30 seconds.
                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort, cts.Token);

                Fixture.CheckAndAssertFilepath(vidCaptureHandler.GetFilepath());
            }
        }
    }
}
