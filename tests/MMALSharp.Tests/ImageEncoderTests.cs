using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xunit;

namespace MMALSharp.Tests
{
    [Collection("MMALCollection")]
    public class ImageEncoderTests
    {
        MMALFixture fixture;

        public ImageEncoderTests(MMALFixture fixture)
        {
            this.fixture = fixture;
            TestData.Fixture = fixture;
        }

        public static IEnumerable<object[]> TakePictureData
        {
            get
            {
                yield return new object[] { TestData.JpegEncoderData.Cast<object[]>() };
                yield return new object[] { TestData.GifEncoderData.Cast<object[]>() };
                yield return new object[] { TestData.PngEncoderData.Cast<object[]>() };
                yield return new object[] { TestData.TgaEncoderData.Cast<object[]>() };
                yield return new object[] { TestData.PpmEncoderData.Cast<object[]>() };
                yield return new object[] { TestData.BmpEncoderData.Cast<object[]>() };                
            }
        }

        public static IEnumerable<object[]> TakeRawPictureData
        {
            get
            {
                yield return new object[] { TestData.YUV420EncoderData.Cast<object[]>() };
                yield return new object[] { TestData.YUV422EncoderData.Cast<object[]>() };
                yield return new object[] { TestData.RGB24EncoderData.Cast<object[]>() };
                yield return new object[] { TestData.RGB24EncoderData.Cast<object[]>() };
                yield return new object[] { TestData.RGBAEncoderData.Cast<object[]>() };                
            }
        }

        public static IEnumerable<object[]> TakePictureDataJpeg => TestData.JpegEncoderData.Cast<object[]>().ToList();
       
        [Theory, MemberData(nameof(TakePictureData))]        
        public void TakePicture(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakePicture", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension);
                
                TestHelper.CleanDirectory("/home/pi/images/tests");

                using (var preview = new MMALNullSinkComponent())
                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                {
                    fixture.MMALCamera.ConfigureCameraSettings();

                    imgEncoder.ConfigureOutputPort(0, encodingType, pixelFormat, 90);

                    //Create our component pipeline.         
                    fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);
                                        
                    //Camera warm up time
                    await Task.Delay(2000);

                    await fixture.MMALCamera.ProcessAsync(fixture.MMALCamera.Camera.StillPort);
                }

                if (System.IO.File.Exists(imgCaptureHandler.GetFilepath()))
                {
                    var length = new System.IO.FileInfo(imgCaptureHandler.GetFilepath()).Length;
                    Assert.True(length > 0);
                }
                else
                {
                    Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");
                }
                                                
            });
        }

        [Theory, MemberData(nameof(TakePictureDataJpeg))]
        public void TakePictureRawBayer(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakePictureRawBayer", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension);

                TestHelper.CleanDirectory("/home/pi/images/tests");

                using (var preview = new MMALNullSinkComponent())
                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler, true))
                {
                    fixture.MMALCamera.ConfigureCameraSettings();

                    imgEncoder.ConfigureOutputPort(0, encodingType, pixelFormat, 90);

                    //Create our component pipeline.         
                    fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);
                                        
                    //Camera warm up time
                    await Task.Delay(2000);

                    await fixture.MMALCamera.ProcessAsync(fixture.MMALCamera.Camera.StillPort);
                }

                if (System.IO.File.Exists(imgCaptureHandler.GetFilepath()))
                {
                    var length = new System.IO.FileInfo(imgCaptureHandler.GetFilepath()).Length;
                    Assert.True(length > 0);
                }
                else
                {
                    Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");
                }

            });
        }

        [Theory, MemberData(nameof(TakeRawPictureData))]
        public void TakePictureRawSensor(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakePictureRawSensor", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension);

                TestHelper.CleanDirectory("/home/pi/images/tests");

                await fixture.MMALCamera.TakeRawPicture(imgCaptureHandler);

                var encodings = fixture.MMALCamera.Camera.StillPort.GetSupportedEncodings();

                if (System.IO.File.Exists(imgCaptureHandler.GetFilepath()))
                {
                    var length = new System.IO.FileInfo(imgCaptureHandler.GetFilepath()).Length;

                    if (encodings.Contains(encodingType.EncodingVal))
                    {
                        Assert.True(length > 1);
                    }
                }
                else
                {
                    Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");
                }
            });
        }

        [Theory, MemberData(nameof(TakePictureDataJpeg))]
        public void TakePictureTimelapse(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakePictureTimelapse", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();
            
            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests/split_tests", extension);

                TestHelper.CleanDirectory("/home/pi/images/tests/split_tests");

                using (var preview = new MMALNullSinkComponent())
                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                {
                    fixture.MMALCamera.ConfigureCameraSettings();

                    imgEncoder.ConfigureOutputPort(0, encodingType, pixelFormat, 90);

                    //Create our component pipeline.         
                    fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);
                                        
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

                        //Camera warm up time
                        await Task.Delay(2000);

                        await fixture.MMALCamera.ProcessAsync(fixture.MMALCamera.Camera.StillPort);
                    }
                }

                DirectoryInfo info = new DirectoryInfo(imgCaptureHandler.Directory);

                if (info.Exists)
                {
                    var files = info.EnumerateFiles();

                    Assert.True(files != null && files.Count() == 6);
                }
                else
                {
                    Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");
                }
            });
        }

        [Theory, MemberData(nameof(TakePictureDataJpeg))]
        public void TakePictureTimeout(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.BeginTest("TakePictureTimeout", encodingType.EncodingName, pixelFormat.EncodingName);
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests/split_tests", extension);

                TestHelper.CleanDirectory("/home/pi/images/tests/split_tests");

                using (var preview = new MMALNullSinkComponent())
                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                {
                    fixture.MMALCamera.ConfigureCameraSettings();

                    imgEncoder.ConfigureOutputPort(0, encodingType, pixelFormat, 90);

                    //Create our component pipeline.         
                    fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);

                    //Camera warm up time
                    await Task.Delay(2000);

                    var timeout = DateTime.Now.AddSeconds(10);
                    while (DateTime.Now.CompareTo(timeout) < 0)
                    {
                        await fixture.MMALCamera.ProcessAsync(fixture.MMALCamera.Camera.StillPort);
                    }
                }
            });
        }

        [Fact]
        public void ChangeEncodingType()
        {
            TestHelper.BeginTest("Image - ChangeEncodingType");
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", "jpg");

                TestHelper.CleanDirectory("/home/pi/images/tests");

                using (var preview = new MMALNullSinkComponent())
                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                {
                    fixture.MMALCamera.ConfigureCameraSettings();

                    imgEncoder.ConfigureOutputPort(0, MMALEncoding.JPEG, MMALEncoding.I420, 90);

                    //Create our component pipeline.         
                    fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);
                                        
                    //Camera warm up time
                    await Task.Delay(2000);

                    await fixture.MMALCamera.ProcessAsync(fixture.MMALCamera.Camera.StillPort);
                }

                if (System.IO.File.Exists(imgCaptureHandler.GetFilepath()))
                {
                    var length = new System.IO.FileInfo(imgCaptureHandler.GetFilepath()).Length;
                    Assert.True(length > 0);
                }
                else
                {
                    Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");
                }

                imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", "bmp");

                using (var preview = new MMALNullSinkComponent())
                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                {
                    imgEncoder.ConfigureOutputPort(0, MMALEncoding.BMP, MMALEncoding.I420, 90);

                    //Create our component pipeline.         
                    fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(preview);

                    //Camera warm up time
                    await Task.Delay(2000);

                    await fixture.MMALCamera.ProcessAsync(fixture.MMALCamera.Camera.StillPort);
                }

                if (System.IO.File.Exists(imgCaptureHandler.GetFilepath()))
                {
                    var length = new System.IO.FileInfo(imgCaptureHandler.GetFilepath()).Length;
                    Assert.True(length > 0);
                }
                else
                {
                    Assert.True(false, $"File {imgCaptureHandler.GetFilepath()} was not created");
                }

            });
        }

    }
}
