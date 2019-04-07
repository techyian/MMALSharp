using System;
using System.Threading;
using System.Threading.Tasks;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using MMALSharp.Ports.Outputs;
using Xunit;

namespace MMALSharp.Tests
{
    public class RawCaptureTests : TestBase
    {
        [Fact]
        public async Task RecordVideoDirectlyFromResizer()
        {
            TestHelper.BeginTest("RecordVideoDirectlyFromResizer");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests");

            using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "raw"))
            using (var preview = new MMALVideoRenderer())
            using (var resizer = new MMALResizerComponent(vidCaptureHandler, typeof(VideoPort)))
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                // Use the resizer to resize 1080p to 640x480.
                var portConfig = new MMALPortConfig(MMALEncoding.I420, MMALEncoding.I420, 640, 480, 0, 0, 0, false, null);

                resizer.ConfigureOutputPort(portConfig);

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

            using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "raw"))
            using (var vidCaptureHandler2 = new VideoStreamCaptureHandler("/home/pi/videos/tests", "raw"))
            using (var vidCaptureHandler3 = new VideoStreamCaptureHandler("/home/pi/videos/tests", "raw"))
            using (var vidCaptureHandler4 = new VideoStreamCaptureHandler("/home/pi/videos/tests", "raw"))
            using (var preview = new MMALVideoRenderer())
            using (var splitter = new MMALSplitterComponent(new[] { vidCaptureHandler, vidCaptureHandler2, vidCaptureHandler3, vidCaptureHandler4 }, typeof(VideoPort)))
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var splitterPortConfig = new MMALPortConfig(MMALEncoding.I420, MMALEncoding.I420, 0, 0, 0, null);

                // Create our component pipeline.         
                splitter.ConfigureInputPort(MMALEncoding.OPAQUE, MMALEncoding.I420, Fixture.MMALCamera.Camera.VideoPort);
                splitter.ConfigureOutputPort(0, splitterPortConfig);
                splitter.ConfigureOutputPort(1, splitterPortConfig);
                splitter.ConfigureOutputPort(2, splitterPortConfig);
                splitter.ConfigureOutputPort(3, splitterPortConfig);

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
                Fixture.CheckAndAssertFilepath(vidCaptureHandler2.GetFilepath());
                Fixture.CheckAndAssertFilepath(vidCaptureHandler3.GetFilepath());
                Fixture.CheckAndAssertFilepath(vidCaptureHandler4.GetFilepath());
            }
        }

        [Fact]
        public async Task RecordVideoDirectlyFromResizerWithSplitterComponent()
        {
            TestHelper.BeginTest("RecordVideoDirectlyFromResizerWithSplitterComponent");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/videos/tests");

            using (var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "raw"))
            using (var vidCaptureHandler2 = new VideoStreamCaptureHandler("/home/pi/videos/tests", "raw"))
            using (var vidCaptureHandler3 = new VideoStreamCaptureHandler("/home/pi/videos/tests", "raw"))
            using (var vidCaptureHandler4 = new VideoStreamCaptureHandler("/home/pi/videos/tests", "raw"))
            using (var preview = new MMALVideoRenderer())
            using (var splitter = new MMALSplitterComponent())
            using (var resizer = new MMALResizerComponent(vidCaptureHandler, typeof(VideoPort)))
            using (var resizer2 = new MMALResizerComponent(vidCaptureHandler2, typeof(VideoPort)))
            using (var resizer3 = new MMALResizerComponent(vidCaptureHandler3, typeof(VideoPort)))
            using (var resizer4 = new MMALResizerComponent(vidCaptureHandler4, typeof(VideoPort)))
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var splitterPortConfig = new MMALPortConfig(MMALEncoding.OPAQUE, MMALEncoding.I420, 0, 0, 0, null);

                // Create our component pipeline.         
                splitter.ConfigureInputPort(MMALEncoding.OPAQUE, MMALEncoding.I420, Fixture.MMALCamera.Camera.VideoPort);
                splitter.ConfigureOutputPort(0, splitterPortConfig);
                splitter.ConfigureOutputPort(1, splitterPortConfig);
                splitter.ConfigureOutputPort(2, splitterPortConfig);
                splitter.ConfigureOutputPort(3, splitterPortConfig);

                var portConfig = new MMALPortConfig(MMALEncoding.I420, MMALEncoding.I420, 1024, 768, 0, 0, 0, false, DateTime.Now.AddSeconds(20));
                var portConfig2 = new MMALPortConfig(MMALEncoding.I420, MMALEncoding.I420, 800, 600, 0, 0, 0, false, DateTime.Now.AddSeconds(20));
                var portConfig3 = new MMALPortConfig(MMALEncoding.I420, MMALEncoding.I420, 640, 480, 0, 0, 0, false, DateTime.Now.AddSeconds(15));
                var portConfig4 = new MMALPortConfig(MMALEncoding.I420, MMALEncoding.I420, 320, 240, 0, 0, 0, false, DateTime.Now.AddSeconds(20));

                resizer.ConfigureOutputPort(portConfig);
                resizer2.ConfigureOutputPort(portConfig2);
                resizer3.ConfigureOutputPort(portConfig3);
                resizer4.ConfigureOutputPort(portConfig4);

                // Create our component pipeline.         
                Fixture.MMALCamera.Camera.VideoPort
                    .ConnectTo(splitter);

                splitter.Outputs[0].ConnectTo(resizer);
                splitter.Outputs[1].ConnectTo(resizer2);
                splitter.Outputs[2].ConnectTo(resizer3);
                splitter.Outputs[3].ConnectTo(resizer4);

                Fixture.MMALCamera.Camera.PreviewPort
                    .ConnectTo(preview);

                // Camera warm up time
                await Task.Delay(2000);

                // Record video for 20 seconds
                await Fixture.MMALCamera.ProcessAsync(Fixture.MMALCamera.Camera.VideoPort);

                Fixture.CheckAndAssertFilepath(vidCaptureHandler.GetFilepath());
                Fixture.CheckAndAssertFilepath(vidCaptureHandler2.GetFilepath());
                Fixture.CheckAndAssertFilepath(vidCaptureHandler3.GetFilepath());
                Fixture.CheckAndAssertFilepath(vidCaptureHandler4.GetFilepath());
            }
        }
    }
}
