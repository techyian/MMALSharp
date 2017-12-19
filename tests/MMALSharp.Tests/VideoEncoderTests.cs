using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        MMALFixture fixture;

        public VideoEncoderTests(MMALFixture fixture)
        {
            this.fixture = fixture;
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
                list.AddRange(TestData.MJPEGEncoderData.Cast<object[]>().ToList());

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
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.VideoStabilisation = vstab;
            MMALCameraConfig.Reload();
            Assert.True(fixture.MMALCamera.Camera.GetVideoStabilisation() == vstab);
        }

        #endregion

        [Theory, MemberData(nameof(TakeVideoData))]
        public void TakeVideo(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", extension);

                TestHelper.CleanDirectory("/home/pi/videos/tests");

                using (var vidEncoder = new MMALVideoEncoder(vidCaptureHandler, new MMAL_RATIONAL_T(25, 1), DateTime.Now.AddSeconds(20)))
                {
                    vidEncoder.ConfigureOutputPort(0, encodingType, pixelFormat, 10, 25000000);

                    //Create our component pipeline.         
                    fixture.MMALCamera.Camera.VideoPort
                        .ConnectTo(vidEncoder);
                    fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(new MMALVideoRenderer());

                    fixture.MMALCamera.ConfigureCameraSettings();

                    //Record video for 20 seconds
                    await fixture.MMALCamera.BeginProcessing(fixture.MMALCamera.Camera.VideoPort, vidEncoder);
                    
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

        [Theory, MemberData(nameof(TakeVideoDataH264))]
        public void TakeVideoSplit(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.InlineHeaders = true;
            
            AsyncContext.Run(async () =>
            {
                var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests/split_test", extension);

                TestHelper.CleanDirectory("/home/pi/videos/tests/split_test");
                
                using (var vidEncoder = new MMALVideoEncoder(vidCaptureHandler, new MMAL_RATIONAL_T(25, 1), 
                        DateTime.Now.AddSeconds(30), new Split { Mode = TimelapseMode.Second, Value = 15 }))
                {
                    vidEncoder.ConfigureOutputPort(0, encodingType, pixelFormat, 10, 25000000);

                    //Create our component pipeline.         
                    fixture.MMALCamera.Camera.VideoPort
                        .ConnectTo(vidEncoder);
                    fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(new MMALVideoRenderer());

                    fixture.MMALCamera.ConfigureCameraSettings();

                    //2 files should be created from this test. 
                    await fixture.MMALCamera.BeginProcessing(fixture.MMALCamera.Camera.VideoPort, vidEncoder);
                    
                    Assert.True(Directory.GetFiles("/home/pi/videos/tests/split_test").Length == 2);

                }
            });
        }

        [Fact]
        public void ChangeEncoderType()
        {
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                var vidCaptureHandler = new VideoStreamCaptureHandler("/home/pi/videos/tests", "avi");

                TestHelper.CleanDirectory("/home/pi/videos/tests");
                
                using (var vidEncoder = new MMALVideoEncoder(vidCaptureHandler, new MMAL_RATIONAL_T(25, 1), DateTime.Now.AddSeconds(20)))
                {
                    vidEncoder.ConfigureOutputPort(0, MMALEncoding.MJPEG, MMALEncoding.I420, 10, 25000000);

                    //Create our component pipeline.         
                    fixture.MMALCamera.Camera.VideoPort
                        .ConnectTo(vidEncoder);
                    fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(new MMALVideoRenderer());

                    fixture.MMALCamera.ConfigureCameraSettings();

                    //Record video for 20 seconds
                    await fixture.MMALCamera.BeginProcessing(fixture.MMALCamera.Camera.VideoPort, vidEncoder);

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

                using (var vidEncoder = new MMALVideoEncoder(vidCaptureHandler, new MMAL_RATIONAL_T(25, 1), DateTime.Now.AddSeconds(20)))
                {
                    vidEncoder.ConfigureOutputPort(0, MMALEncoding.MJPEG, MMALEncoding.I420, 90, 25000000);

                    //Create our component pipeline.         
                    fixture.MMALCamera.Camera.VideoPort
                        .ConnectTo(vidEncoder);
                    fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(new MMALVideoRenderer());

                    fixture.MMALCamera.ConfigureCameraSettings();

                    //Record video for 20 seconds
                    await fixture.MMALCamera.BeginProcessing(fixture.MMALCamera.Camera.VideoPort, vidEncoder);

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
