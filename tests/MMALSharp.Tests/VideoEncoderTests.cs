// <copyright file="VideoEncoderTests.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using Nito.AsyncEx;
using Xunit;

namespace MMALSharp.Tests
{
    [Collection("MMALCollection")]
    public class VideoEncoderTests
    {
        private readonly MMALFixture _fixture;

        public VideoEncoderTests(MMALFixture fixture)
        {
            _fixture = fixture;
            TestData.Fixture = fixture;
        }
        
        public static IEnumerable<object[]> TakeVideoData
        {
            get
            {
                var list = new List<object[]>();

                list.AddRange(TestData.H264EncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.MjpegEncoderData.Cast<object[]>().ToList());

                return list;
            }
        }

        public static IEnumerable<object[]> TakeVideoDataH264 => TestData.H264EncoderData.Cast<object[]>().ToList();

        #region Configuration tests

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SetThenGetVideoStabilisation(bool vstab)
        {
            TestHelper.BeginTest("SetThenGetVideoStabilisation");
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.VideoStabilisation = vstab;
            _fixture.MMALCamera.ConfigureCameraSettings();
            Assert.True(_fixture.MMALCamera.Camera.GetVideoStabilisation() == vstab);
        }

        #endregion

        [Theory]
        [MemberData(nameof(TakeVideoData))]
        public void TakeVideo(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakeVideo", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                TestHelper.CleanDirectory("/home/pi/videos/tests");
                
                using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", extension))
                using (var preview = new MMALVideoRenderer())
                using (var vidEncoder = new MMALVideoEncoder(vidCaptureHandler))
                {
                    _fixture.MMALCamera.ConfigureCameraSettings();

                    vidEncoder.ConfigureOutputPort(encodingType, pixelFormat, 10, 25000000);

                    // Create our component pipeline.         
                    _fixture.MMALCamera.Camera.VideoPort
                        .ConnectTo(vidEncoder);
                    _fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);
                    
                    // Camera warm up time
                    await Task.Delay(2000);

                    CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

                    // Record video for 20 seconds
                    await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.VideoPort, cts.Token);

                    _fixture.CheckAndAssertFilepath(vidCaptureHandler.GetFilepath());
                }
            });
        }

        [Fact]
        public void TakeVideoSplit()
        {
            TestHelper.BeginTest("TakeVideoSplit");
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.InlineHeaders = true;
            
            AsyncContext.Run(async () =>
            {
                TestHelper.CleanDirectory("/home/pi/videos/tests/split_test");
                
                using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests/split_test", "avi"))
                using (var preview = new MMALVideoRenderer())
                using (var vidEncoder = new MMALVideoEncoder(vidCaptureHandler, null, new Split { Mode = TimelapseMode.Second, Value = 15 }))
                {
                    _fixture.MMALCamera.ConfigureCameraSettings();

                    vidEncoder.ConfigureOutputPort(0, MMALEncoding.H264, MMALEncoding.I420, 0, 25000000);

                    // Create our component pipeline.         
                    _fixture.MMALCamera.Camera.VideoPort
                        .ConnectTo(vidEncoder);
                    _fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);

                    // Camera warm up time
                    await Task.Delay(2000);

                    CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

                    // 2 files should be created from this test. 
                    await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.VideoPort, cts.Token);
                    
                    Assert.True(Directory.GetFiles("/home/pi/videos/tests/split_test").Length == 2);
                }
            });
        }

        [Fact]
        public void ChangeEncoderType()
        {
            TestHelper.BeginTest("Video - ChangeEncoderType");
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                TestHelper.CleanDirectory("/home/pi/videos/tests");
                
                using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "avi"))
                using (var preview = new MMALVideoRenderer())
                using (var vidEncoder = new MMALVideoEncoder(vidCaptureHandler))
                {
                    _fixture.MMALCamera.ConfigureCameraSettings();

                    vidEncoder.ConfigureOutputPort(MMALEncoding.MJPEG, MMALEncoding.I420, 10, 25000000);

                    // Create our component pipeline.         
                    _fixture.MMALCamera.Camera.VideoPort
                        .ConnectTo(vidEncoder);
                    _fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);

                    // Camera warm up time
                    await Task.Delay(2000);

                    CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

                    // Record video for 20 seconds
                    await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.VideoPort, cts.Token);

                    _fixture.CheckAndAssertFilepath(vidCaptureHandler.GetFilepath());
                }
                
                using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "mjpeg"))
                using (var preview = new MMALVideoRenderer())
                using (var vidEncoder = new MMALVideoEncoder(vidCaptureHandler))
                {
                    _fixture.MMALCamera.ConfigureCameraSettings();

                    vidEncoder.ConfigureOutputPort(MMALEncoding.MJPEG, MMALEncoding.I420, 90, 25000000);

                    // Create our component pipeline.         
                    _fixture.MMALCamera.Camera.VideoPort
                        .ConnectTo(vidEncoder);
                    _fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);

                    CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

                    // Record video for 20 seconds
                    await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.VideoPort, cts.Token);

                    _fixture.CheckAndAssertFilepath(vidCaptureHandler.GetFilepath());
                }
            });
        }

        [Fact]
        public void VideoSplitterComponent()
        {
            TestHelper.BeginTest("VideoSplitterComponent");
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                TestHelper.CleanDirectory("/home/pi/videos/tests");

                using (var handler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "avi"))
                using (var handler2 = new VideoStreamCaptureHandler("/home/pi/video/tests", "avi"))
                using (var handler3 = new VideoStreamCaptureHandler("/home/pi/video/tests", "avi"))
                using (var handler4 = new VideoStreamCaptureHandler("/home/pi/video/tests", "avi"))
                using (var splitter = new MMALSplitterComponent(null))
                using (var vidEncoder = new MMALVideoEncoder(handler, DateTime.Now.AddSeconds(10)))
                using (var vidEncoder2 = new MMALVideoEncoder(handler2, DateTime.Now.AddSeconds(15)))
                using (var vidEncoder3 = new MMALVideoEncoder(handler3, DateTime.Now.AddSeconds(10)))
                using (var vidEncoder4 = new MMALVideoEncoder(handler4, DateTime.Now.AddSeconds(10)))
                using (var renderer = new MMALVideoRenderer())
                {
                    _fixture.MMALCamera.ConfigureCameraSettings();

                    // Create our component pipeline.         
                    splitter.ConfigureInputPort(MMALEncoding.I420, MMALEncoding.I420, _fixture.MMALCamera.Camera.VideoPort);
                    splitter.ConfigureOutputPort(0, MMALEncoding.OPAQUE, MMALEncoding.I420, 0);
                    splitter.ConfigureOutputPort(1, MMALEncoding.OPAQUE, MMALEncoding.I420, 0);
                    splitter.ConfigureOutputPort(2, MMALEncoding.OPAQUE, MMALEncoding.I420, 0);
                    splitter.ConfigureOutputPort(3, MMALEncoding.OPAQUE, MMALEncoding.I420, 0);

                    vidEncoder.ConfigureInputPort(MMALEncoding.OPAQUE, MMALEncoding.I420, splitter.Outputs[0]);
                    vidEncoder.ConfigureOutputPort(0, MMALEncoding.H264, MMALEncoding.I420, 10, 25000000);

                    vidEncoder2.ConfigureInputPort(MMALEncoding.OPAQUE, MMALEncoding.I420, splitter.Outputs[1]);
                    vidEncoder2.ConfigureOutputPort(0, MMALEncoding.H264, MMALEncoding.I420, 20, 25000000);
                    vidEncoder3.ConfigureInputPort(MMALEncoding.OPAQUE, MMALEncoding.I420, splitter.Outputs[2]);
                    vidEncoder3.ConfigureOutputPort(0, MMALEncoding.H264, MMALEncoding.I420, 30, 25000000);

                    vidEncoder4.ConfigureInputPort(MMALEncoding.OPAQUE, MMALEncoding.I420, splitter.Outputs[3]);
                    vidEncoder4.ConfigureOutputPort(0, MMALEncoding.H264, MMALEncoding.I420, 40, 25000000);

                    _fixture.MMALCamera.Camera.VideoPort.ConnectTo(splitter);

                    splitter.Outputs[0].ConnectTo(vidEncoder);
                    splitter.Outputs[1].ConnectTo(vidEncoder2);
                    splitter.Outputs[2].ConnectTo(vidEncoder3);
                    splitter.Outputs[3].ConnectTo(vidEncoder4);

                    _fixture.MMALCamera.Camera.PreviewPort.ConnectTo(renderer);

                    // Camera warm up time
                    await Task.Delay(2000);

                    await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.VideoPort);

                    _fixture.CheckAndAssertFilepath(handler.GetFilepath());
                    _fixture.CheckAndAssertFilepath(handler2.GetFilepath());
                    _fixture.CheckAndAssertFilepath(handler3.GetFilepath());
                    _fixture.CheckAndAssertFilepath(handler4.GetFilepath());
                }
            });
        }
    }
}
