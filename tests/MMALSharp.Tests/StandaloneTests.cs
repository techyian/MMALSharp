using System.IO;
using System.Threading.Tasks;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using MMALSharp.Ports.Outputs;
using Xunit;

namespace MMALSharp.Tests
{
    [Collection("MMALStandaloneCollection")]
    public class StandaloneTests
    {
        private static MMALStandaloneFixture _fixture;
        public static MMALStandaloneFixture Fixture
        {
            get
            {
                if (_fixture == null)
                {
                    _fixture = new MMALStandaloneFixture();
                }

                return _fixture;
            }
            set => _fixture = value;
        }

        public StandaloneTests(MMALStandaloneFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task EncodeDecodeFromFile()
        {
            TestHelper.BeginTest("Image - EncodeDecodeFromFile");
            TestHelper.SetConfigurationDefaults();
            TestHelper.CleanDirectory("/home/pi/images/tests");

            string imageFilepath = string.Empty;
            string decodedFilepath = string.Empty;

            // First take a new JPEG picture using RGB24 encoding.
            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", "jpg"))
            using (var preview = new MMALNullSinkComponent())
            using (var imgEncoder = new MMALImageEncoder())
            {
                Fixture.MMALCamera.ConfigureCameraSettings();

                var portConfig = new MMALPortConfig(MMALEncoding.JPEG, MMALEncoding.RGB24, 90);

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

                imageFilepath = imgCaptureHandler.GetFilepath();
            }

            // Next decode the JPEG to raw YUV420.
            using (var stream = File.OpenRead(imageFilepath))
            using (var inputCaptureHandler = new InputCaptureHandler(stream))
            using (var outputCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "raw"))
            using (var imgDecoder = new MMALImageDecoder())
            {
                // We do not pass the resolution to the input port. Doing so will cause a MMAL exception.
                var inputConfig = new MMALPortConfig(MMALEncoding.JPEG, MMALEncoding.RGB24, 0, 0, 0, 0, 0, true, null);
                var outputConfig = new MMALPortConfig(MMALEncoding.I420, null, 640, 480, 0, 0, 0, true, null);

                // Create our component pipeline.
                imgDecoder.ConfigureInputPort(inputConfig, inputCaptureHandler)
                    .ConfigureOutputPort<FileEncodeOutputPort>(0, outputConfig, outputCaptureHandler);

                await Fixture.MMALStandalone.ProcessAsync(imgDecoder);
                
                Fixture.CheckAndAssertFilepath(outputCaptureHandler.GetFilepath());

                decodedFilepath = outputCaptureHandler.GetFilepath();
            }

            // Finally re-encode to BMP using YUV420.
            using (var stream = File.OpenRead(decodedFilepath))
            using (var inputCaptureHandler = new InputCaptureHandler(stream))
            using (var outputCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "bmp"))
            using (var imgEncoder = new MMALImageEncoder())
            {
                var inputConfig = new MMALPortConfig(MMALEncoding.I420, null, 640, 480, 0, 0, 0, true, null);
                var outputConfig = new MMALPortConfig(MMALEncoding.BMP, MMALEncoding.I420, 640, 480, 0, 0, 0, true, null);

                imgEncoder.ConfigureInputPort(inputConfig, inputCaptureHandler)
                    .ConfigureOutputPort(outputConfig, outputCaptureHandler);

                await Fixture.MMALStandalone.ProcessAsync(imgEncoder);
                
                Fixture.CheckAndAssertFilepath(outputCaptureHandler.GetFilepath());
            }
        }
    }
}
