// <copyright file="ConfigurationTests.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Components;
using MMALSharp.Native;
using MMALSharp.Utility;
using Xunit;

namespace MMALSharp.Tests
{
    [Collection("MMALCollection")]
    public class ConfigurationTests
    {
        private readonly MMALFixture _fixture;

        public ConfigurationTests(MMALFixture fixture)
        {
            _fixture = fixture;
            TestData.Fixture = fixture;
        }
        
        [Theory]
        [InlineData(MMALSensorMode.Mode0)]
        [InlineData(MMALSensorMode.Mode2)]
        [InlineData(MMALSensorMode.Mode4)]
        [DisplayTestMethodName]
        public void SetThenGetSensorMode(MMALSensorMode mode)
        {            
            TestHelper.SetConfigurationDefaults();
            MMALCameraConfig.SensorMode = mode;

            _fixture.MMALCamera.ConfigureCameraSettings();
            Assert.True(_fixture.MMALCamera.Camera.GetSensorMode() == mode);
        }

        [Theory]
        [InlineData(40)]
        [InlineData(45)]
        [InlineData(-100)]
        [DisplayTestMethodName]
        public void SetThenGetBrightness(double brightness)
        {            
            TestHelper.SetConfigurationDefaults();
            MMALCameraConfig.Brightness = brightness;

            if (brightness >= 0 && brightness <= 100)
            {
                _fixture.MMALCamera.ConfigureCameraSettings();

                Assert.True(_fixture.MMALCamera.Camera.GetBrightness() == brightness / 100);
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => _fixture.MMALCamera.ConfigureCameraSettings());
            }
        }

        [Theory]
        [InlineData(20)]
        [InlineData(38)]
        [InlineData(101)]
        [DisplayTestMethodName]
        public void SetThenGetSharpness(double sharpness)
        {            
            TestHelper.SetConfigurationDefaults();
            MMALCameraConfig.Sharpness = sharpness;

            if (sharpness >= -100 && sharpness <= 100)
            {
                _fixture.MMALCamera.ConfigureCameraSettings();

                Assert.True(_fixture.MMALCamera.Camera.GetSharpness() == sharpness / 100);
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => _fixture.MMALCamera.ConfigureCameraSettings());
            }
        }

        [Theory]
        [InlineData(10)]
        [InlineData(54)]
        [InlineData(-200)]
        [DisplayTestMethodName]
        public void SetThenGetContrast(double contrast)
        {            
            TestHelper.SetConfigurationDefaults();
            MMALCameraConfig.Contrast = contrast;

            if (contrast >= -100 && contrast <= 100)
            {
                _fixture.MMALCamera.ConfigureCameraSettings();

                Assert.True(_fixture.MMALCamera.Camera.GetContrast() == contrast / 100);
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => _fixture.MMALCamera.ConfigureCameraSettings());
            }
        }

        [Theory]
        [InlineData(30)]
        [InlineData(55)]
        [InlineData(90)]
        [DisplayTestMethodName]
        public void SetThenGetSaturation(double saturation)
        {            
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.Saturation = saturation;
            _fixture.MMALCamera.ConfigureCameraSettings();

            Assert.True(_fixture.MMALCamera.Camera.GetSaturation() == saturation / 100);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(900)]
        [InlineData(0)]
        [DisplayTestMethodName]
        public void SetThenGetIso(int iso)
        {            
            TestHelper.SetConfigurationDefaults();
            MMALCameraConfig.ISO = iso;

            if ((iso >= 100 && iso <= 800) || iso == 0)
            {
                _fixture.MMALCamera.ConfigureCameraSettings();

                Assert.True(_fixture.MMALCamera.Camera.GetISO() == iso);
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => _fixture.MMALCamera.ConfigureCameraSettings());
            }
        }

        [Theory]
        [InlineData(5)]
        [InlineData(50)]
        [InlineData(-30)]
        [DisplayTestMethodName]
        public void SetThenGetExposureCompensation(int expCompensation)
        {            
            TestHelper.SetConfigurationDefaults();
            MMALCameraConfig.ExposureCompensation = expCompensation;

            if (expCompensation >= -10 && expCompensation <= 10)
            {
                _fixture.MMALCamera.ConfigureCameraSettings();
                Assert.True(_fixture.MMALCamera.Camera.GetExposureCompensation() == expCompensation);
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => _fixture.MMALCamera.ConfigureCameraSettings());
            }
        }

        [Theory]
        [InlineData(MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_BEACH)]
        [InlineData(MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_FIREWORKS)]
        [InlineData(MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_ANTISHAKE)]
        [DisplayTestMethodName]
        public void SetThenGetExposureMode(MMAL_PARAM_EXPOSUREMODE_T expMode)
        {            
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.ExposureMode = expMode;
            _fixture.MMALCamera.ConfigureCameraSettings();

            Assert.True(_fixture.MMALCamera.Camera.GetExposureMode() == expMode);
        }

        [Theory]
        [InlineData(MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_BACKLIT)]
        [InlineData(MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_MATRIX)]
        [InlineData(MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE)]
        [DisplayTestMethodName]
        public void SetThenGetExposureMeteringMode(MMAL_PARAM_EXPOSUREMETERINGMODE_T expMetMode)
        {            
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.ExposureMeterMode = expMetMode;
            _fixture.MMALCamera.ConfigureCameraSettings();

            Assert.True(_fixture.MMALCamera.Camera.GetExposureMeteringMode() == expMetMode);
        }

        [Theory]
        [InlineData(MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_AUTO)]
        [InlineData(MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_FLUORESCENT)]
        [InlineData(MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_CLOUDY)]
        [DisplayTestMethodName]
        public void SetThenGetAwbMode(MMAL_PARAM_AWBMODE_T awbMode)
        {            
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.AwbMode = awbMode;
            _fixture.MMALCamera.ConfigureCameraSettings();

            Assert.True(_fixture.MMALCamera.Camera.GetAwbMode() == awbMode);
        }

        [Theory]
        [InlineData(MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_CARTOON)]
        [InlineData(MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_COLOURBALANCE)]
        [InlineData(MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_OILPAINT)]
        [DisplayTestMethodName]
        public void SetThenGetImageFx(MMAL_PARAM_IMAGEFX_T imgFx)
        {            
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.ImageFx = imgFx;
            _fixture.MMALCamera.ConfigureCameraSettings();
            Assert.True(_fixture.MMALCamera.Camera.GetImageFx() == imgFx);
        }

        [Theory]
        [InlineData(true, 128, 128)]
        [InlineData(true, 50, 100)]
        [InlineData(false, 128, 128)]
        [DisplayTestMethodName]
        public void SetThenGetColourFx(bool enable, byte u, byte v)
        {            
            TestHelper.SetConfigurationDefaults();

            var color = MMALColor.FromYUVBytes(0, u, v);

            var colFx = new ColourEffects(enable, color);

            MMALCameraConfig.ColourFx = colFx;
            _fixture.MMALCamera.ConfigureCameraSettings();

            var uv = MMALColor.RGBToYUVBytes(_fixture.MMALCamera.Camera.GetColourFx().Color);
            
            Assert.True(_fixture.MMALCamera.Camera.GetColourFx().Enable == enable &&
                        uv.Item2 == u &&
                        uv.Item3 == v);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(90, 90)]
        [InlineData(140, 90)]
        [InlineData(250, 180)]
        [InlineData(270, 270)]
        [DisplayTestMethodName]
        public void SetThenGetRotation(int rotation, int expectedResult)
        {            
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.Rotation = rotation;
            _fixture.MMALCamera.ConfigureCameraSettings();

            Assert.True(_fixture.MMALCamera.Camera.GetRotation() == expectedResult);
        }

        [Theory]
        [InlineData(MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_HORIZONTAL)]
        [InlineData(MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_BOTH)]
        [InlineData(MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_VERTICAL)]
        [DisplayTestMethodName]
        public void SetThenGetFlips(MMAL_PARAM_MIRROR_T flips)
        {            
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.Flips = flips;
            _fixture.MMALCamera.ConfigureCameraSettings();
            Assert.True(_fixture.MMALCamera.Camera.GetFlips() == flips &&
                        _fixture.MMALCamera.Camera.GetStillFlips() == flips &&
                        _fixture.MMALCamera.Camera.GetVideoFlips() == flips);
        }

        [Theory]
        [InlineData(0.1, 0.1, 0.5, 1.0)]
        [InlineData(0.5, 0.1, 0.5, 1.0)]
        [InlineData(0.1, 1.1, 0.5, 1.0)]
        [DisplayTestMethodName]
        public void SetThenGetZoom(double x, double y, double width, double height)
        {            
            TestHelper.SetConfigurationDefaults();

            var zoom = new Zoom(x, y, width, height);

            MMALCameraConfig.ROI = zoom;

            if (x <= 1.0 && y <= 1.0 && height <= 1.0 && width <= 1.0)
            {
                _fixture.MMALCamera.ConfigureCameraSettings();

                Assert.True(_fixture.MMALCamera.Camera.GetZoom().Height == Convert.ToInt32(height * 65536) &&
                            _fixture.MMALCamera.Camera.GetZoom().Width == Convert.ToInt32(width * 65536) &&
                            _fixture.MMALCamera.Camera.GetZoom().X == Convert.ToInt32(x * 65536) &&
                            _fixture.MMALCamera.Camera.GetZoom().Y == Convert.ToInt32(y * 65536));
            }
            else
            {
                Assert.ThrowsAny<Exception>(() => _fixture.MMALCamera.ConfigureCameraSettings());
            }
        }

        [Theory]
        [InlineData(2500)]
        [DisplayTestMethodName]
        public void SetThenGetShutterSpeed(int shutterSpeed)
        {            
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.StillFramerate = new MMAL_RATIONAL_T(0, 0);
            MMALCameraConfig.SensorMode = MMALSensorMode.Mode1;
            MMALCameraConfig.AwbMode = MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_OFF;
            MMALCameraConfig.ShutterSpeed = shutterSpeed;
            _fixture.MMALCamera.ConfigureCameraSettings();

            Assert.True(_fixture.MMALCamera.Camera.GetShutterSpeed() == shutterSpeed);
        }

        [Theory]
        [InlineData(MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_HIGH)]
        [InlineData(MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_LOW)]
        [InlineData(MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_MEDIUM)]
        [DisplayTestMethodName]
        public void SetThenGetDrc(MMAL_PARAMETER_DRC_STRENGTH_T drc)
        {            
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.DrcLevel = drc;
            _fixture.MMALCamera.ConfigureCameraSettings();
            Assert.True(_fixture.MMALCamera.Camera.GetDRC() == drc);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [DisplayTestMethodName]
        public void SetThenGetStatsPass(bool statsPass)
        {            
            TestHelper.SetConfigurationDefaults();

            MMALCameraConfig.StatsPass = statsPass;
            _fixture.MMALCamera.ConfigureCameraSettings();
            Assert.True(_fixture.MMALCamera.Camera.GetStatsPass() == statsPass);
        }
    }
}
