using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Components;
using MMALSharp.Native;
using Xunit;

namespace MMALSharp.Tests
{
    [Collection("MMALCollection")]
    public class ConfigurationTests
    {
        MMALFixture fixture;

        public ConfigurationTests(MMALFixture fixture)
        {
            this.fixture = fixture;
            TestData.Fixture = fixture;
        }
        
        [Theory]
        [InlineData(MMALSensorMode.Mode0)]
        [InlineData(MMALSensorMode.Mode2)]
        [InlineData(MMALSensorMode.Mode4)]
        public void SetThenGetSensorMode(MMALSensorMode mode)
        {
            TestHelper.BeginTest("SetThenGetSensorMode");
            TestHelper.SetConfigurationDefaults();
            MMALCameraConfig.SensorMode = mode;

            fixture.MMALCamera.ConfigureCameraSettings();
            Assert.True(fixture.MMALCamera.Camera.GetSensorMode() == mode);
        }

        [Theory]
        [InlineData(40)]
        [InlineData(45)]
        [InlineData(-100)]
        public void SetThenGetBrightness(double brightness)
        {
            TestHelper.BeginTest("SetThenGetBrightness");
            TestHelper.SetConfigurationDefaults();
            MMALCameraConfig.Brightness = brightness;

            if (brightness >= 0 && brightness <= 100)
            {
                fixture.MMALCamera.ConfigureCameraSettings();

                Assert.True(fixture.MMALCamera.Camera.GetBrightness() == brightness / 100);
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => fixture.MMALCamera.ConfigureCameraSettings());
            }
        }

        [Theory]
        [InlineData(20)]
        [InlineData(38)]
        [InlineData(101)]
        public void SetThenGetSharpness(double sharpness)
        {
            TestHelper.BeginTest("SetThenGetSharpness");
            TestHelper.SetConfigurationDefaults();
            MMALCameraConfig.Sharpness = sharpness;

            if (sharpness >= -100 && sharpness <= 100)
            {
                fixture.MMALCamera.ConfigureCameraSettings();

                Assert.True(fixture.MMALCamera.Camera.GetSharpness() == sharpness / 100);
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => fixture.MMALCamera.ConfigureCameraSettings());
            }
        }

        [Theory]
        [InlineData(10)]
        [InlineData(54)]
        [InlineData(-200)]
        public void SetThenGetContrast(double contrast)
        {
            TestHelper.BeginTest("SetThenGetContrast");
            TestHelper.SetConfigurationDefaults();
            MMALCameraConfig.Contrast = contrast;

            if (contrast >= -100 && contrast <= 100)
            {
                fixture.MMALCamera.ConfigureCameraSettings();

                Assert.True(fixture.MMALCamera.Camera.GetContrast() == contrast / 100);
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => fixture.MMALCamera.ConfigureCameraSettings());
            }
        }

        [Theory]
        [InlineData(30)]
        [InlineData(55)]
        [InlineData(90)]
        public void SetThenGetSaturation(double saturation)
        {
            TestHelper.BeginTest("SetThenGetSaturation");
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.Saturation = saturation;
            fixture.MMALCamera.ConfigureCameraSettings();

            Assert.True(fixture.MMALCamera.Camera.GetSaturation() == saturation / 100);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(900)]
        [InlineData(0)]
        public void SetThenGetISO(int iso)
        {
            TestHelper.BeginTest("SetThenGetISO");
            TestHelper.SetConfigurationDefaults();
            MMALCameraConfig.ISO = iso;

            if ((iso >= 100 && iso <= 800) || iso == 0)
            {
                fixture.MMALCamera.ConfigureCameraSettings();

                Assert.True(fixture.MMALCamera.Camera.GetISO() == iso);
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => fixture.MMALCamera.ConfigureCameraSettings());
            }
        }

        [Theory]
        [InlineData(5)]
        [InlineData(50)]
        [InlineData(-30)]
        public void SetThenGetExposureCompensation(int expCompensation)
        {
            TestHelper.BeginTest("SetThenGetExposureCompensation");
            TestHelper.SetConfigurationDefaults();
            MMALCameraConfig.ExposureCompensation = expCompensation;

            if (expCompensation >= -10 && expCompensation <= 10)
            {
                fixture.MMALCamera.ConfigureCameraSettings();
                Assert.True(fixture.MMALCamera.Camera.GetExposureCompensation() == expCompensation);
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => fixture.MMALCamera.ConfigureCameraSettings());
            }
        }

        [Theory]
        [InlineData(MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_BEACH)]
        [InlineData(MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_FIREWORKS)]
        [InlineData(MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_ANTISHAKE)]
        public void SetThenGetExposureMode(MMAL_PARAM_EXPOSUREMODE_T expMode)
        {
            TestHelper.BeginTest("SetThenGetExposureMode");
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.ExposureMode = expMode;
            fixture.MMALCamera.ConfigureCameraSettings();

            Assert.True(fixture.MMALCamera.Camera.GetExposureMode() == expMode);
        }

        [Theory]
        [InlineData(MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_BACKLIT)]
        [InlineData(MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_MATRIX)]
        [InlineData(MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE)]
        public void SetThenGetExposureMeteringMode(MMAL_PARAM_EXPOSUREMETERINGMODE_T expMetMode)
        {
            TestHelper.BeginTest("SetThenGetExposureMeteringMode");
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.ExposureMeterMode = expMetMode;
            fixture.MMALCamera.ConfigureCameraSettings();

            Assert.True(fixture.MMALCamera.Camera.GetExposureMeteringMode() == expMetMode);
        }

        [Theory]
        [InlineData(MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_AUTO)]
        [InlineData(MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_FLUORESCENT)]
        [InlineData(MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_CLOUDY)]
        public void SetThenGetAwbMode(MMAL_PARAM_AWBMODE_T awbMode)
        {
            TestHelper.BeginTest("SetThenGetAWBMode");
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.AwbMode = awbMode;
            fixture.MMALCamera.ConfigureCameraSettings();

            Assert.True(fixture.MMALCamera.Camera.GetAwbMode() == awbMode);
        }

        [Theory]
        [InlineData(MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_CARTOON)]
        [InlineData(MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_COLOURBALANCE)]
        [InlineData(MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_OILPAINT)]
        public void SetThenGetImageFx(MMAL_PARAM_IMAGEFX_T imgFx)
        {
            TestHelper.BeginTest("SetThenGetImageFx");
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.ImageFx = imgFx;
            fixture.MMALCamera.ConfigureCameraSettings();
            Assert.True(fixture.MMALCamera.Camera.GetImageFx() == imgFx);
        }

        [Theory]
        [InlineData(true, 128, 128)]
        [InlineData(true, 50, 100)]
        [InlineData(false, 128, 128)]
        public void SetThenGetColourFx(bool enable, int u, int v)
        {
            TestHelper.BeginTest("SetThenGetColourFx");
            TestHelper.SetConfigurationDefaults();

            var colFx = new ColourEffects { Enable = enable, U = u, V = v };
            MMALCameraConfig.ColourFx = colFx;
            fixture.MMALCamera.ConfigureCameraSettings();
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
            TestHelper.BeginTest("SetThenGetRotation");
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.Rotation = rotation;
            fixture.MMALCamera.ConfigureCameraSettings();

            Assert.True(fixture.MMALCamera.Camera.GetRotation() == expectedResult);
        }

        [Theory]
        [InlineData(MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_HORIZONTAL)]
        [InlineData(MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_BOTH)]
        [InlineData(MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_VERTICAL)]

        public void SetThenGetFlips(MMAL_PARAM_MIRROR_T flips)
        {
            TestHelper.BeginTest("SetThenGetFlips");
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.Flips = flips;
            fixture.MMALCamera.ConfigureCameraSettings();
            Assert.True(fixture.MMALCamera.Camera.GetFlips() == flips);
        }

        [Theory]
        [InlineData(0.1, 0.1, 0.5, 1.0)]
        [InlineData(0.5, 0.1, 0.5, 1.0)]
        [InlineData(0.1, 1.1, 0.5, 1.0)]
        public void SetThenGetZoom(double x, double y, double width, double height)
        {
            TestHelper.BeginTest("SetThenGetZoom");
            TestHelper.SetConfigurationDefaults();

            var zoom = new Zoom { Height = height, Width = width, X = x, Y = y };

            MMALCameraConfig.ROI = zoom;

            if (x <= 1.0 && y <= 1.0 && height <= 1.0 && width <= 1.0)
            {
                fixture.MMALCamera.ConfigureCameraSettings();

                Assert.True(fixture.MMALCamera.Camera.GetZoom().Height == Convert.ToInt32(height * 65536) &&
                            fixture.MMALCamera.Camera.GetZoom().Width == Convert.ToInt32(width * 65536) &&
                            fixture.MMALCamera.Camera.GetZoom().X == Convert.ToInt32(x * 65536) &&
                            fixture.MMALCamera.Camera.GetZoom().Y == Convert.ToInt32(y * 65536));

            }
            else
            {
                Assert.ThrowsAny<Exception>(() => fixture.MMALCamera.ConfigureCameraSettings());
            }
        }

        [Theory]
        [InlineData(2500)]
        public void SetThenGetShutterSpeed(int shutterSpeed)
        {
            TestHelper.BeginTest("SetThenGetShutterSpeed");
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.ShutterSpeed = shutterSpeed;
            fixture.MMALCamera.ConfigureCameraSettings();

            Assert.True(fixture.MMALCamera.Camera.GetShutterSpeed() == shutterSpeed);
        }

        [Theory]
        [InlineData(MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_HIGH)]
        [InlineData(MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_LOW)]
        [InlineData(MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_MEDIUM)]
        public void SetThenGetDRC(MMAL_PARAMETER_DRC_STRENGTH_T drc)
        {
            TestHelper.BeginTest("SetThenGetDRC");
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.DrcLevel = drc;
            fixture.MMALCamera.ConfigureCameraSettings();
            Assert.True(fixture.MMALCamera.Camera.GetDRC() == drc);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SetThenGetStatsPass(bool statsPass)
        {
            TestHelper.BeginTest("SetThenGetStatsPass");
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.StatsPass = statsPass;
            fixture.MMALCamera.ConfigureCameraSettings();
            Assert.True(fixture.MMALCamera.Camera.GetStatsPass() == statsPass);
        }
        
    }
}
