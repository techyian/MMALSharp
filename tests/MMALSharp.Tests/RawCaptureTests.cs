// <copyright file="RawCaptureTests.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using MMALSharp.Common;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Ports;
using MMALSharp.Ports.Outputs;
using Xunit;

namespace MMALSharp.Tests
{
    public class RawCaptureTests : TestBase
    {
        public RawCaptureTests(MMALFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task RecordVideoDirectlyFromResizer()
        {
            TestHelper.BeginTest("RecordVideoDirectlyFromResizer");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests");

            using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "raw"))
            using (var preview = new MMALVideoRenderer())
            using (var resizer = new MMALResizerComponent())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                // Use the resizer to resize 1080p to 640x480.
                var portConfig = new MMALPortConfig(MMALEncoding.I420, MMALEncoding.I420, width: 640, height: 480);

                resizer.ConfigureOutputPort<VideoPort>(0, portConfig, vidCaptureHandler);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.VideoPort
                    .ConnectTo(resizer);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                // Camera warm up time
                await Task.Delay(2000);

                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

                // Record video for 20 seconds
                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort, cts.Token);

                Fixture.CheckAndAssertFilepath(vidCaptureHandler.GetFilepath());
            }
        }

        [Fact]
        public async Task RecordVideoDirectlyFromSplitter()
        {
            TestHelper.BeginTest("RecordVideoDirectlyFromSplitter");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests");

            // I am only using a single output here because due to the disk IO performance on the Pi you ideally need to be
            // using a faster storage medium such as the ramdisk to output to multiple files.
            using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "raw"))
            using (var preview = new MMALVideoRenderer())
            using (var splitter = new MMALSplitterComponent())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var splitterPortConfig = new MMALPortConfig(MMALEncoding.I420, MMALEncoding.I420);

                // Create our component pipeline.         
                splitter.ConfigureInputPort(new MMALPortConfig(MMALEncoding.OPAQUE, MMALEncoding.I420, 0), Fixture.MMALCamera.Camera.VideoPort, null);
                splitter.ConfigureOutputPort(0, splitterPortConfig, vidCaptureHandler);
               
                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.VideoPort
                    .ConnectTo(splitter);
                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                // Camera warm up time
                await Task.Delay(2000);

                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

                // Record video for 20 seconds
                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort, cts.Token);

                Fixture.CheckAndAssertFilepath(vidCaptureHandler.GetFilepath());
            }
        }

        [Fact]
        public async Task RecordVideoDirectlyFromResizerWithSplitterComponent()
        {
            TestHelper.BeginTest("RecordVideoDirectlyFromResizerWithSplitterComponent");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests");

            // I am only using a single output here because due to the disk IO performance on the Pi you ideally need to be
            // using a faster storage medium such as the ramdisk to output to multiple files.
            using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "h264"))
            using (var preview = new MMALVideoRenderer())
            using (var splitter = new MMALSplitterComponent())
            using (var resizer = new MMALResizerComponent())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var splitterPortConfig = new MMALPortConfig(MMALEncoding.OPAQUE, MMALEncoding.I420);
                var resizerPortConfig = new MMALPortConfig(MMALEncoding.I420, MMALEncoding.I420, width: 1024, height: 768, timeout: DateTime.Now.AddSeconds(15));

                // Create our component pipeline.         
                splitter.ConfigureInputPort(new MMALPortConfig(MMALEncoding.OPAQUE, MMALEncoding.I420), Fixture.MMALCamera.Camera.VideoPort, null);
                splitter.ConfigureOutputPort(0, splitterPortConfig, null);
                
                resizer.ConfigureOutputPort<VideoPort>(0, resizerPortConfig, vidCaptureHandler);
                
                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.VideoPort
                    .ConnectTo(splitter);

                splitter.Outputs[0].ConnectTo(resizer);

                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                // Camera warm up time
                await Task.Delay(2000);

                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort);

                Fixture.CheckAndAssertFilepath(vidCaptureHandler.GetFilepath());
            }
        }

        [Fact]
        public async Task TakePicturesDirectlyFromSplitterComponent()
        {
            TestHelper.BeginTest("TakePicturesDirectlyFromSplitterComponent");
            TestHelper.SetConfigurationDefaults();

            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", "raw"))
            using (var imgCaptureHandler2 = new ImageStreamCaptureHandler("/home/pi/images/tests", "raw"))
            using (var imgCaptureHandler3 = new ImageStreamCaptureHandler("/home/pi/images/tests", "raw"))
            using (var imgCaptureHandler4 = new ImageStreamCaptureHandler("/home/pi/images/tests", "raw"))            
            using (var splitter = new MMALSplitterComponent())            
            using (var nullSink = new MMALNullSinkComponent())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();
                                
                var splitterConfig = new MMALPortConfig(MMALEncoding.I420, MMALEncoding.I420);
                                
                // Create our component pipeline.      
                splitter.ConfigureOutputPort<SplitterStillPort>(0, splitterConfig, imgCaptureHandler);
                splitter.ConfigureOutputPort<SplitterStillPort>(1, splitterConfig, imgCaptureHandler2);
                splitter.ConfigureOutputPort<SplitterStillPort>(2, splitterConfig, imgCaptureHandler3);
                splitter.ConfigureOutputPort<SplitterStillPort>(3, splitterConfig, imgCaptureHandler4);
                                
                Fixture.MMALCamera.Camera.StillPort.ConnectTo(splitter);
                Fixture.MMALCamera.Camera.PreviewPort.ConnectTo(nullSink);

                // Camera warm up time
                await Task.Delay(2000);
                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.StillPort);

                Fixture.CheckAndAssertFilepath(imgCaptureHandler.GetFilepath());
                Fixture.CheckAndAssertFilepath(imgCaptureHandler2.GetFilepath());
                Fixture.CheckAndAssertFilepath(imgCaptureHandler3.GetFilepath());
                Fixture.CheckAndAssertFilepath(imgCaptureHandler4.GetFilepath());
            }
        }
    }
}
