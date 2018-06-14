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
                /*list.AddRange(TestData.MVCEncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.H263EncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.MP4EncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.MP2EncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.MP1EncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.WMV3EncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.WMV2EncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.WMV1EncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.WVC1EncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.VP8EncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.VP7EncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.VP6EncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.TheoraEncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.SparkEncoderData.Cast<object[]>().ToList());*/
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
                var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", extension);

                TestHelper.CleanDirectory("/home/pi/videos/tests");

                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

                using (var preview = new MMALVideoRenderer())
                using (var vidEncoder = new MMALVideoEncoder(vidCaptureHandler))
                {
                    _fixture.MMALCamera.ConfigureCameraSettings();

                    vidEncoder.ConfigureOutputPort(0, encodingType, pixelFormat, 10, 25000000);

                    // Create our component pipeline.         
                    _fixture.MMALCamera.Camera.VideoPort
                        .ConnectTo(vidEncoder);
                    _fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);

                    // Camera warm up time
                    await Task.Delay(2000);

                    // Record video for 20 seconds
                    await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.VideoPort, cts.Token);

                    if (System.IO.File.Exists(vidCaptureHandler.GetFilepath()))
                    {
                        var length = new System.IO.FileInfo(vidCaptureHandler.GetFilepath()).Length;
                        Assert.True(length > 0);
                    }
                    else
                    {
                        Assert.True(false, $"File {vidCaptureHandler.GetFilepath()} was not created");
                    }
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
                var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests/split_test", "avi");

                TestHelper.CleanDirectory("/home/pi/videos/tests/split_test");

                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

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
                var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "avi");

                TestHelper.CleanDirectory("/home/pi/videos/tests");

                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

                using (var preview = new MMALVideoRenderer())
                using (var vidEncoder = new MMALVideoEncoder(vidCaptureHandler))
                {
                    _fixture.MMALCamera.ConfigureCameraSettings();

                    vidEncoder.ConfigureOutputPort(0, MMALEncoding.MJPEG, MMALEncoding.I420, 10, 25000000);

                    // Create our component pipeline.         
                    _fixture.MMALCamera.Camera.VideoPort
                        .ConnectTo(vidEncoder);
                    _fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);

                    // Camera warm up time
                    await Task.Delay(2000);

                    // Record video for 20 seconds
                    await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.VideoPort, cts.Token);

                    if (System.IO.File.Exists(vidCaptureHandler.GetFilepath()))
                    {
                        var length = new System.IO.FileInfo(vidCaptureHandler.GetFilepath()).Length;
                        Assert.True(length > 0);
                    }
                    else
                    {
                        Assert.True(false, $"File {vidCaptureHandler.GetFilepath()} was not created");
                    }
                }

                vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "mjpeg");

                cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

                using (var preview = new MMALVideoRenderer())
                using (var vidEncoder = new MMALVideoEncoder(vidCaptureHandler))
                {
                    _fixture.MMALCamera.ConfigureCameraSettings();

                    vidEncoder.ConfigureOutputPort(0, MMALEncoding.MJPEG, MMALEncoding.I420, 90, 25000000);

                    // Create our component pipeline.         
                    _fixture.MMALCamera.Camera.VideoPort
                        .ConnectTo(vidEncoder);
                    _fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);
                    
                    // Record video for 20 seconds
                    await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.VideoPort, cts.Token);

                    if (System.IO.File.Exists(vidCaptureHandler.GetFilepath()))
                    {
                        var length = new System.IO.FileInfo(vidCaptureHandler.GetFilepath()).Length;
                        Assert.True(length > 0);
                    }
                    else
                    {
                        Assert.True(false, $"File {vidCaptureHandler.GetFilepath()} was not created");
                    }
                }
            });
        }
    }
}
