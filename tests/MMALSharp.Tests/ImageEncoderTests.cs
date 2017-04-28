using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
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
        public void SetThenGetBrightness(int brightness)
        {
            MMALCameraConfig.Brightness = brightness;
            
            if (brightness >= 0 && brightness <= 100)
            {
                MMALCameraConfig.Reload();
                Assert.True(fixture.MMALCamera.GetBrightness() == brightness);
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
        public void SetThenGetSharpness(int sharpness)
        {
            MMALCameraConfig.Sharpness = sharpness;

            if (sharpness >= -100 && sharpness <= 100)
            {
                MMALCameraConfig.Reload();
                Assert.True(fixture.MMALCamera.GetSharpness() == sharpness);
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
        public void SetThenGetContrast(int contrast)
        {
            MMALCameraConfig.Contrast = contrast;
            
            if (contrast >= -100 && contrast <= 100)
            {
                MMALCameraConfig.Reload();
                Assert.True(fixture.MMALCamera.GetContrast() == 30);
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
        public void SetThenGetSaturation(int saturation)
        {
            MMALCameraConfig.Saturation = saturation;
            MMALCameraConfig.Reload();

            Assert.True(fixture.MMALCamera.GetSaturation() == saturation);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(900)]
        [InlineData(0)]
        public void SetThenGetISO(int iso)
        {
            MMALCameraConfig.ISO = iso;

            if ((iso < 100 || iso > 800) && iso > 0)
            {
                MMALCameraConfig.Reload();
                Assert.True(fixture.MMALCamera.GetISO() == iso);
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
            MMALCameraConfig.ExposureCompensation = expCompensation;

            if (expCompensation < -10 || expCompensation > 10)
            {
                MMALCameraConfig.Reload();
                Assert.True(fixture.MMALCamera.GetExposureCompensation() == expCompensation);
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
            MMALCameraConfig.ExposureMode = expMode;
            MMALCameraConfig.Reload();

            Assert.True(fixture.MMALCamera.GetExposureMode() == expMode);
        }

        [Theory]
        [InlineData(MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_BACKLIT)]
        [InlineData(MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_MATRIX)]
        [InlineData(MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE)]
        public void SetThenGetExposureMeteringMode(MMAL_PARAM_EXPOSUREMETERINGMODE_T expMetMode)
        {
            MMALCameraConfig.ExposureMeterMode = expMetMode;
            MMALCameraConfig.Reload();

            Assert.True(fixture.MMALCamera.GetExposureMeteringMode() == expMetMode);
        }

        [Theory]
        [InlineData(MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_AUTO)]
        [InlineData(MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_FLUORESCENT)]
        [InlineData(MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_CLOUDY)]
        public void SetThenGetAwbMode(MMAL_PARAM_AWBMODE_T awbMode)
        {
            MMALCameraConfig.AwbMode = awbMode;
            MMALCameraConfig.Reload();

            Assert.True(fixture.MMALCamera.GetAwbMode() == awbMode);
        }

        [Theory]
        [InlineData(1.0, 1.0)]
        [InlineData(5.5, 1.0)]
        [InlineData(0.0, 1.0)]
        [InlineData(-1.0, -1.0)]
        public void SetThenGetAwbGains(double rGain, double bGain)
        {
            MMALCameraConfig.AwbGainsR = (int)rGain;
            MMALCameraConfig.AwbGainsB = (int)bGain;

            if (rGain >= 0 && bGain >= 0)
            {
                MMALCameraConfig.Reload();
                Assert.True(fixture.MMALCamera.GetAwbGains().Item1 == rGain && fixture.MMALCamera.GetAwbGains().Item2 == bGain);
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => MMALCameraConfig.Reload());
            }
        }

        [Theory]
        [InlineData(MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_CARTOON)]
        [InlineData(MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_COLOURBALANCE)]
        [InlineData(MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_OILPAINT)]
        public void SetThenGetImageFx(MMAL_PARAM_IMAGEFX_T imgFx)
        {
            MMALCameraConfig.ImageFx = imgFx;
            MMALCameraConfig.Reload();
            Assert.True(fixture.MMALCamera.GetImageFx() == imgFx);
        }

        [Theory]
        [InlineData(true, 128, 128)]
        [InlineData(true, 50, 100)]
        [InlineData(false, 128, 128)]
        public void SetThenGetColourFx(bool enable, int u, int v)
        {
            var colFx = new ColourEffects { Enable = enable, U = u, V = v };
            MMALCameraConfig.ColourFx = colFx;
            MMALCameraConfig.Reload();
            Assert.True(fixture.MMALCamera.GetColourFx().Enable == enable && 
                        fixture.MMALCamera.GetColourFx().U == u &&
                        fixture.MMALCamera.GetColourFx().V == v);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(90, 90)]
        [InlineData(140, 180)]
        [InlineData(250, 270)]
        [InlineData(270, 270)]
        public void SetThenGetRotation(int rotation, int expectedResult)
        {
            MMALCameraConfig.Rotation = rotation;
            MMALCameraConfig.Reload();
            Assert.True(fixture.MMALCamera.GetRotation() == expectedResult);
        }

        [Theory]
        [InlineData(MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_HORIZONTAL)]
        [InlineData(MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_BOTH)]
        [InlineData(MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_VERTICAL)]

        public void SetThenGetFlips(MMAL_PARAM_MIRROR_T flips)
        {
            MMALCameraConfig.Flips = flips;
            MMALCameraConfig.Reload();
            Assert.True(fixture.MMALCamera.GetFlips() == flips);
        }

        [Theory]
        [InlineData(0.1, 0.1, 0.5, 1.0)]
        [InlineData(0.5, 0.1, 0.5, 1.0)]
        [InlineData(0.1, 1.1, 0.5, 1.0)]
        public void SetThenGetZoom(double x, double y, double width, double height)
        {
            var zoom = new Zoom { Height = height, Width = width, X = x, Y = y};

            MMALCameraConfig.ROI = zoom;

            if (x <= 1.0 || y <= 1.0 || height <= 1.0 || width <= 1.0)
            {
                MMALCameraConfig.Reload();
                Assert.True(fixture.MMALCamera.GetZoom().Height == height &&
                            fixture.MMALCamera.GetZoom().Width == width &&
                            fixture.MMALCamera.GetZoom().X == x &&
                            fixture.MMALCamera.GetZoom().Y == y);
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => MMALCameraConfig.Reload());
            }
        }

        [Theory]
        [InlineData(100000)]
        [InlineData(1000000)]
        [InlineData(6000000)]
        public void SetThenGetShutterSpeed(int shutterSpeed)
        {
            MMALCameraConfig.ShutterSpeed = shutterSpeed;
            MMALCameraConfig.Reload();
            Assert.True(fixture.MMALCamera.GetShutterSpeed() == shutterSpeed);
        }

        [Theory]
        [InlineData(MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_HIGH)]
        [InlineData(MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_LOW)]
        [InlineData(MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_MEDIUM)]
        public void SetThenGetDRC(MMAL_PARAMETER_DRC_STRENGTH_T drc)
        {
            MMALCameraConfig.DrcLevel = drc;
            MMALCameraConfig.Reload();
            Assert.True(fixture.MMALCamera.GetDRC() == drc);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SetThenGetStatsPass(bool statsPass)
        {
            MMALCameraConfig.StatsPass = statsPass;
            MMALCameraConfig.Reload();
            Assert.True(fixture.MMALCamera.GetStatsPass() == statsPass);
        }
        
        #endregion

        [Theory, MemberData(nameof(TakePictureData))]        
        public void TakePicture(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", extension);
                
                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler, encodingType, pixelFormat, 90))
                {
                    //Create our component pipeline.         
                    fixture.MMALCamera
                       .AddEncoder(imgEncoder, fixture.MMALCamera.Camera.StillPort)
                       .CreatePreviewComponent(new MMALNullSinkComponent())
                       .ConfigureCamera();

                    await fixture.MMALCamera.TakePicture(fixture.MMALCamera.Camera.StillPort);
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

                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler, encodingType, pixelFormat, 90))
                {
                    //Create our component pipeline.         
                    fixture.MMALCamera
                        .AddEncoder(imgEncoder, fixture.MMALCamera.Camera.StillPort)
                        .CreatePreviewComponent(new MMALNullSinkComponent())
                        .ConfigureCamera();

                    await fixture.MMALCamera.TakePicture(fixture.MMALCamera.Camera.StillPort, true);
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

                try
                {
                    var files = Directory.GetFiles("/home/pi/images/tests/split_test");

                    //Clear directory first
                    foreach (string file in files)
                    {
                        File.Delete(file);
                    }
                }
                catch
                {
                }


                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler, encodingType, pixelFormat, 90))
                {
                    //Create our component pipeline.         
                    fixture.MMALCamera
                        .AddEncoder(imgEncoder, fixture.MMALCamera.Camera.StillPort)
                        .CreatePreviewComponent(new MMALNullSinkComponent())
                        .ConfigureCamera();

                    await fixture.MMALCamera.TakePictureTimelapse(fixture.MMALCamera.Camera.StillPort, 
                                                                    new Timelapse { Mode = TimelapseMode.Second, Value = 5, Timeout = DateTime.Now.AddMinutes(1) });

                    Assert.True(Directory.GetFiles("/home/pi/images/tests/split_test").Length == 2);
                }
            });
        }

        [Theory, MemberData(nameof(TakePictureDataJpeg))]
        public void TakePictureTimeout(string extension, MMALEncoding encodingType, MMALEncoding pixelFormat)
        {
            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests/split_tests", extension);

                try
                {
                    var files = Directory.GetFiles("/home/pi/images/tests/split_test");

                    //Clear directory first
                    foreach (string file in files)
                    {
                        File.Delete(file);
                    }
                }
                catch
                {
                }


                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler, encodingType, pixelFormat, 90))
                {
                    //Create our component pipeline.         
                    fixture.MMALCamera
                        .AddEncoder(imgEncoder, fixture.MMALCamera.Camera.StillPort)
                        .CreatePreviewComponent(new MMALNullSinkComponent())
                        .ConfigureCamera();

                    await fixture.MMALCamera.TakePictureTimeout(fixture.MMALCamera.Camera.StillPort, DateTime.Now.AddMinutes(1));

                    Assert.True(Directory.GetFiles("/home/pi/images/tests/split_test").Length == 2);
                }
                
            });
        }

        [Fact]
        public void ChangeEncodingType()
        {
            AsyncContext.Run(async () =>
            {
                var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/tests", "jpg");

                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler, MMALEncoding.MMAL_ENCODING_JPEG, MMALEncoding.MMAL_ENCODING_I420, 90))
                {
                    //Create our component pipeline.         
                    fixture.MMALCamera
                        .AddEncoder(imgEncoder, fixture.MMALCamera.Camera.StillPort)
                        .CreatePreviewComponent(new MMALNullSinkComponent())
                        .ConfigureCamera();

                    await fixture.MMALCamera.TakePicture(fixture.MMALCamera.Camera.StillPort);
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

                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler, MMALEncoding.MMAL_ENCODING_BMP, MMALEncoding.MMAL_ENCODING_I420, 90))
                {
                    //Create our component pipeline.         
                    fixture.MMALCamera
                        .AddEncoder(imgEncoder, fixture.MMALCamera.Camera.StillPort)
                        .CreatePreviewComponent(new MMALNullSinkComponent())
                        .ConfigureCamera();

                    await fixture.MMALCamera.TakePicture(fixture.MMALCamera.Camera.StillPort);
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
