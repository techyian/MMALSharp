// <copyright file="FFmpegTests.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using MMALSharp.Common;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using Xunit;

namespace MMALSharp.Tests
{
    public class FFmpegTests : TestBase
    {
        public FFmpegTests(MMALFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task RawVideoConvert()
        {
            TestHelper.BeginTest("RawVideoConvert");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests");

            using (var ffCaptureHandler = FFmpegCaptureHandler.RawVideoToAvi("/home/pi/videos/tests", "testing1234"))
            using (var vidEncoder = new MMALVideoEncoder())
            using (var renderer = new MMALVideoRenderer())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(MMALEncoding.H264, MMALEncoding.I420, quality: 10, bitrate: 25000000);

                vidEncoder.ConfigureOutputPort(portConfig, ffCaptureHandler);

                Fixture.MMALCamera.Camera.VideoPort.ConnectTo(vidEncoder);
                Fixture.MMALCamera.Camera.PreviewPort.ConnectTo(renderer);

                // Camera warm up time
                await Task.Delay(2000);

                var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

                // Take video for 1 minute.
                await Task.WhenAll(new[]
                {
                    ffCaptureHandler.ProcessExternalAsync(cts.Token),
                    Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort, cts.Token),
                });

                Fixture.CheckAndAssertFilepath("/home/pi/videos/tests/testing1234.avi");
            }
        }

        /*[Fact]
        public async Task ImagesToVideo()
        {
            TestHelper.BeginTest("ImagesToVideo");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests");
            TestHelper.CleanDirectory("/home/pi/images/tests");

            // This example will take an image every 5 seconds for 1 minute.
            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", "jpg"))
            {
                var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

                var tl = new Timelapse { Mode = TimelapseMode.Second, CancellationToken = cts.Token, Value = 5 };
                await Fixture.MMALCamera.TakePictureTimelapse(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420, tl);

                // Process all images captured into a video at 2fps.
                imgCaptureHandler.ImagesToVideo("/home/pi/videos/tests", 2);

                Fixture.CheckAndAssertFilepath("/home/pi/videos/tests/out.avi");
            }
        }*/
    }
}
