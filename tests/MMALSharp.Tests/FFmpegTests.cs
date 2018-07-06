// <copyright file="FFmpegTests.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using MMALSharp.Components;
using MMALSharp.FFmpeg;
using MMALSharp.Handlers;
using MMALSharp.Native;
using Nito.AsyncEx;
using Xunit;

namespace MMALSharp.Tests
{
    [Collection("MMALCollection")]
    public class FFmpegTests
    {
        private readonly MMALFixture _fixture;

        public FFmpegTests(MMALFixture fixture)
        {
            _fixture = fixture;
            TestData.Fixture = fixture;
        }

        [Fact]
        public void RawVideoConvert()
        {
            TestHelper.BeginTest("RawVideoConvert");
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                TestHelper.CleanDirectory("/home/pi/videos/tests");

                using (var ffCaptureHandler = FFmpegCaptureHandler.RawVideoToAvi("/home/pi/videos/tests", "testing1234"))
                using (var vidEncoder = new MMALVideoEncoder(ffCaptureHandler))
                using (var renderer = new MMALVideoRenderer())
                {
                    _fixture.MMALCamera.ConfigureCameraSettings();

                    vidEncoder.ConfigureOutputPort(0, MMALEncoding.H264, MMALEncoding.I420, 0, 25000000);

                    _fixture.MMALCamera.Camera.VideoPort.ConnectTo(vidEncoder);
                    _fixture.MMALCamera.Camera.PreviewPort.ConnectTo(renderer);

                    // Camera warm up time
                    await Task.Delay(2000);

                    var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));

                    // Take video for 3 minutes.
                    await _fixture.MMALCamera.ProcessAsync(_fixture.MMALCamera.Camera.VideoPort, cts.Token);

                    _fixture.CheckAndAssertFilepath("/home/pi/videos/tests/testing1234.avi");
                }
            });
        }

        [Fact]
        public void ImagesToVideo()
        {
            TestHelper.BeginTest("ImagesToVideo");
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                TestHelper.CleanDirectory("/home/pi/videos/tests");
                TestHelper.CleanDirectory("/home/pi/images/tests");

                // This example will take an image every 5 seconds for 1 minute.
                using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "jpg"))
                {
                    var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

                    var tl = new Timelapse { Mode = TimelapseMode.Second, CancellationToken = cts.Token, Value = 5 };
                    await _fixture.MMALCamera.TakePictureTimelapse(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420, tl);

                    // Process all images captured into a video at 2fps.
                    imgCaptureHandler.ImagesToVideo("/home/pi/videos/tests", 2);

                    _fixture.CheckAndAssertFilepath("/home/pi/videos/tests/out.avi");
                }
            });
        }
    }
}
