using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        }

        public static IEnumerable<object[]> TakePictureData
        {
            get
            {
                var list = new List<object[]>();

                list.AddRange(TestData.JpegEncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.GifEncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.PngEncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.TgaEncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.PpmEncoderData.Cast<object[]>().ToList());
                list.AddRange(TestData.BmpEncoderData.Cast<object[]>().ToList());

                return list;
            }
        }

        public static IEnumerable<object[]> TakePictureDataJpeg => TestData.JpegEncoderData.Cast<object[]>().ToList();
        
        #region Configuration tests

        [Theory]
        [InlineData(40)]
        [InlineData(45)]
        [InlineData(-100)]
        public void SetThenGetBrightness(double brightness)
        {
            TestHelper.SetConfigurationDefaults();
            MMALCameraConfig.Brightness = brightness;
            
            if (brightness >= 0 && brightness <= 100)
            {
                MMALCameraConfig.Reload();
                
                Assert.True(fixture.MMALCamera.Camera.GetBrightness() == brightness / 100);
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => MMALCameraConfig.Reload());
            }
        }

        [Theory]
        [InlineData(20)]
        [InlineData(38)]
        [InlineData(101)]
        public void SetThenGetSharpness(double sharpness)
        {
            TestHelper.SetConfigurationDefaults();
            MMALCameraConfig.Sharpness = sharpness;

            if (sharpness >= -100 && sharpness <= 100)
            {
                MMALCameraConfig.Reload();
                
                Assert.True(fixture.MMALCamera.Camera.GetSharpness() == sharpness / 100);
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => MMALCameraConfig.Reload());
            }
        }

        [Theory]
        [InlineData(10)]
        [InlineData(54)]
        [InlineData(-200)]
        public void SetThenGetContrast(double contrast)
        {
            TestHelper.SetConfigurationDefaults();
            MMALCameraConfig.Contrast = contrast;
            
            if (contrast >= -100 && contrast <= 100)
            {
                MMALCameraConfig.Reload();
                
                Assert.True(fixture.MMALCamera.Camera.GetContrast() == contrast / 100);
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => MMALCameraConfig.Reload());
            }
        }

        [Theory]
        [InlineData(30)]
        [InlineData(55)]
        [InlineData(90)]
        public void SetThenGetSaturation(double saturation)
        {
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.Saturation = saturation;
            MMALCameraConfig.Reload();
            
            Assert.True(fixture.MMALCamera.Camera.GetSaturation() == saturation / 100);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(900)]
        [InlineData(0)]
        public void SetThenGetISO(int iso)
        {
            TestHelper.SetConfigurationDefaults();
            MMALCameraConfig.ISO = iso;

            if ((iso >= 100 && iso <= 800) || iso == 0)
            {
                MMALCameraConfig.Reload();
                
                Assert.True(fixture.MMALCamera.Camera.GetISO() == iso);
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => MMALCameraConfig.Reload());
            }
        }

        [Theory]
        [InlineData(5)]
        [InlineData(50)]
        [InlineData(-30)]
        public void SetThenGetExposureCompensation(int expCompensation)
        {
            TestHelper.SetConfigurationDefaults();
            MMALCameraConfig.ExposureCompensation = expCompensation;

            if (expCompensation >= -10 && expCompensation <= 10)
            {
                MMALCameraConfig.Reload();
                Assert.True(fixture.MMALCamera.Camera.GetExposureCompensation() == expCompensation);
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => MMALCameraConfig.Reload());
            }
        }

        [Theory]
        [InlineData(MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_BEACH)]
        [InlineData(MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_FIREWORKS)]
        [InlineData(MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_ANTISHAKE)]
        public void SetThenGetExposureMode(MMAL_PARAM_EXPOSUREMODE_T expMode)
        {
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.ExposureMode = expMode;
            MMALCameraConfig.Reload();

            Assert.True(fixture.MMALCamera.Camera.GetExposureMode() == expMode);
        }

        [Theory]
        [InlineData(MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_BACKLIT)]
        [InlineData(MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_MATRIX)]
        [InlineData(MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE)]
        public void SetThenGetExposureMeteringMode(MMAL_PARAM_EXPOSUREMETERINGMODE_T expMetMode)
        {
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.ExposureMeterMode = expMetMode;
            MMALCameraConfig.Reload();

            Assert.True(fixture.MMALCamera.Camera.GetExposureMeteringMode() == expMetMode);
        }

        [Theory]
        [InlineData(MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_AUTO)]
        [InlineData(MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_FLUORESCENT)]
        [InlineData(MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_CLOUDY)]
        public void SetThenGetAwbMode(MMAL_PARAM_AWBMODE_T awbMode)
        {
            TestHelper.SetConfigurationDefaults();
            
            MMALCameraConfig.AwbMode = awbMode;
            MMALCameraConfig.Reload();

            Assert.True(fixture.MMALCamera.Camera.GetAwbMode() == awbMode);
        }

        [Theory]
        [InlineData(MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_CARTOON)]
        [InlineData(MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_COLOURBALANCE)]
        [InlineData(MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_OILPAINT)]
        public void SetThenGetImageFx(MMAL_PARAM_IMAGEFX_T imgFx)
        {
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.ImageFx = imgFx;
            MMALCameraConfig.Reload();
            Assert.True(fixture.MMALCamera.Camera.GetImageFx() == imgFx);
        }

        [Theory]
        [InlineData(true, 128, 128)]
        [InlineData(true, 50, 100)]
        [InlineData(false, 128, 128)]
        public void SetThenGetColourFx(bool enable, int u, int v)
        {
            TestHelper.SetConfigurationDefaults();

            var colFx = new ColourEffects { Enable = enable, U = u, V = v };
            MMALCameraConfig.ColourFx = colFx;
            MMALCameraConfig.Reload();
            Assert.True(fixture.MMALCamera.Camera.GetColourFx().Enable == enable && 
                        fixture.MMALCamera.Camera.GetColourFx().U == u &&
                        fixture.MMALCamera.Camera.GetColourFx().V == v);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(90, 90)]
        [InlineData(140, 90)]
        [InlineData(250, 180)]
        [InlineData(270, 270)]
        public void SetThenGetRotation(int rotation, int expectedResult)
        {
            TestHelper.SetConfigurationDefaults();
            
            MMALCameraConfig.Rotation = rotation;
            MMALCameraConfig.Reload();
            
            Assert.True(fixture.MMALCamera.Camera.GetRotation() == expectedResult);
        }

        [Theory]
        [InlineData(MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_HORIZONTAL)]
        [InlineData(MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_BOTH)]
        [InlineData(MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_VERTICAL)]

        public void SetThenGetFlips(MMAL_PARAM_MIRROR_T flips)
        {
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.Flips = flips;
            MMALCameraConfig.Reload();
            Assert.True(fixture.MMALCamera.Camera.GetFlips() == flips);
        }

        [Theory]
        [InlineData(0.1, 0.1, 0.5, 1.0)]
        [InlineData(0.5, 0.1, 0.5, 1.0)]
        [InlineData(0.1, 1.1, 0.5, 1.0)]
        public void SetThenGetZoom(double x, double y, double width, double height)
        {
            TestHelper.SetConfigurationDefaults();

            var zoom = new Zoom { Height = height, Width = width, X = x, Y = y};

            MMALCameraConfig.ROI = zoom;

            if (x <= 1.0 && y <= 1.0 && height <= 1.0 && width <= 1.0)
            {
                MMALCameraConfig.Reload();
                
                Assert.True(fixture.MMALCamera.Camera.GetZoom().Height == Convert.ToInt32(height * 65536) &&
                            fixture.MMALCamera.Camera.GetZoom().Width == Convert.ToInt32(width * 65536) &&
                            fixture.MMALCamera.Camera.GetZoom().X == Convert.ToInt32(x * 65536) &&
                            fixture.MMALCamera.Camera.GetZoom().Y == Convert.ToInt32(y * 65536));

            }
            else
            {
                Assert.ThrowsAny<Exception>(() => MMALCameraConfig.Reload());
            }
        }

        [Theory]
        [InlineData(2500)]
        public void SetThenGetShutterSpeed(int shutterSpeed)
        {
            TestHelper.SetConfigurationDefaults();
            
            MMALCameraConfig.ShutterSpeed = shutterSpeed;
            MMALCameraConfig.Reload();
            
            Assert.True(fixture.MMALCamera.Camera.GetShutterSpeed() == shutterSpeed);
        }

        [Theory]
        [InlineData(MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_HIGH)]
        [InlineData(MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_LOW)]
        [InlineData(MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_MEDIUM)]
        public void SetThenGetDRC(MMAL_PARAMETER_DRC_STRENGTH_T drc)
        {
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.DrcLevel = drc;
            MMALCameraConfig.Reload();
            Assert.True(fixture.MMALCamera.Camera.GetDRC() == drc);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SetThenGetStatsPass(bool statsPass)
        {
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.StatsPass = statsPass;
            MMALCameraConfig.Reload();
            Assert.True(fixture.MMALCamera.Camera.GetStatsPass() == statsPass);
        }
        
        #endregion

        [Theory, MemberData(nameof(TakePictureData))]        
        public void TakePicture(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            TestHelper.SetConfigurationDefaults();

            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension);
                
                TestHelper.CleanDirectory("/home/pi/images/tests");

                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                {
                    imgEncoder.ConfigureOutputPort(0, encodingType, pixelFormat, 90);

                    //Create our component pipeline.         
                    fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(new MMALNullSinkComponent());

                    fixture.MMALCamera.ConfigureCameraSettings();

                    await fixture.MMALCamera.BeginProcessing(fixture.MMALCamera.Camera.StillPort, imgEncoder);
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
            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension);

                TestHelper.CleanDirectory("/home/pi/images/tests");

                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler, true))
                {
                    imgEncoder.ConfigureOutputPort(0, encodingType, pixelFormat, 90);

                    //Create our component pipeline.         
                    fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(new MMALNullSinkComponent());

                    fixture.MMALCamera.ConfigureCameraSettings();

                    await fixture.MMALCamera.BeginProcessing(fixture.MMALCamera.Camera.StillPort, imgEncoder);
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
        public void TakePictureTimelapse(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests/split_tests", extension);

                TestHelper.CleanDirectory("/home/pi/images/tests/split_tests");

                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                {
                    imgEncoder.ConfigureOutputPort(0, encodingType, pixelFormat, 90);

                    //Create our component pipeline.         
                    fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(new MMALNullSinkComponent());

                    fixture.MMALCamera.ConfigureCameraSettings();

                    Timelapse tl = new Timelapse
                    {
                        Mode = TimelapseMode.Second,
                        Timeout = DateTime.Now.AddSeconds(30),
                        Value = 5
                    };

                    while (DateTime.Now.CompareTo(tl.Timeout) < 0)
                    {
                        int interval = tl.Value * 1000;
                        
                        await Task.Delay(interval);


                        await fixture.MMALCamera.BeginProcessing(fixture.MMALCamera.Camera.StillPort, imgEncoder);
                    }
                }
            });
        }

        [Theory, MemberData(nameof(TakePictureDataJpeg))]
        public void TakePictureTimeout(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests/split_tests", extension);

                TestHelper.CleanDirectory("/home/pi/images/tests/split_tests");
                
                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                {
                    imgEncoder.ConfigureOutputPort(0, encodingType, pixelFormat, 90);

                    //Create our component pipeline.         
                    fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(new MMALNullSinkComponent());

                    fixture.MMALCamera.ConfigureCameraSettings();

                    var timeout = DateTime.Now.AddSeconds(30);

                    while (DateTime.Now.CompareTo(timeout) < 0)
                    {
                        await fixture.MMALCamera.BeginProcessing(fixture.MMALCamera.Camera.StillPort, imgEncoder);
                    }}
                
            });
        }

        [Fact]
        public void ChangeEncodingType()
        {
            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", "jpg");

                TestHelper.CleanDirectory("/home/pi/images/tests");

                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                {
                    imgEncoder.ConfigureOutputPort(0, MMALEncoding.JPEG, MMALEncoding.I420, 90);

                    //Create our component pipeline.         
                    fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(new MMALNullSinkComponent());

                    fixture.MMALCamera.ConfigureCameraSettings();

                    await fixture.MMALCamera.BeginProcessing(fixture.MMALCamera.Camera.StillPort, imgEncoder);
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

                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                {
                    imgEncoder.ConfigureOutputPort(0, MMALEncoding.BMP, MMALEncoding.I420, 90);

                    //Create our component pipeline.         
                    fixture.MMALCamera.Camera.StillPort
                        .ConnectTo(imgEncoder);
                    fixture.MMALCamera.Camera.PreviewPort
                        .ConnectTo(new MMALNullSinkComponent());

                    await fixture.MMALCamera.BeginProcessing(fixture.MMALCamera.Camera.StillPort, imgEncoder);
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
