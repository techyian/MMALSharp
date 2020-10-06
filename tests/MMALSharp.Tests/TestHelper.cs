using System.IO;
using Microsoft.Extensions.Logging;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Config;
using MMALSharp.Native;

namespace MMALSharp.Tests
{
    public class TestHelper
    {
        public static void SetConfigurationDefaults()
        {
            MMALCameraConfig.Debug = true;
            MMALCameraConfig.Brightness = 70;
            MMALCameraConfig.Sharpness = 60;
            MMALCameraConfig.Contrast = 60;
            MMALCameraConfig.Saturation = 50;
            MMALCameraConfig.AwbGainsB = 0;
            MMALCameraConfig.AwbGainsR = 0;
            MMALCameraConfig.AwbMode = MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_AUTO;
            MMALCameraConfig.ColourFx = default(ColourEffects);
            MMALCameraConfig.ExposureCompensation = -1;
            MMALCameraConfig.ExposureMeterMode = MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE;
            MMALCameraConfig.ExposureMode = MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_AUTO;
            MMALCameraConfig.ROI = default(Zoom);
            MMALCameraConfig.ISO = 0;
            MMALCameraConfig.StatsPass = false;
            MMALCameraConfig.Flips = MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_NONE;
            MMALCameraConfig.ImageFx = MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_NONE;
            MMALCameraConfig.Rotation = 0;
            MMALCameraConfig.DrcLevel = MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_OFF;
            MMALCameraConfig.ShutterSpeed = 0;
            MMALCameraConfig.SensorMode = MMALSensorMode.Mode0;
            MMALCameraConfig.VideoStabilisation = true;
            MMALCameraConfig.Framerate = 10;
            MMALCameraConfig.Encoding = MMALEncoding.OPAQUE;
            MMALCameraConfig.EncodingSubFormat = MMALEncoding.I420;
            MMALCameraConfig.VideoColorSpace = MMALEncoding.MMAL_COLOR_SPACE_ITUR_BT709;
            MMALCameraConfig.InlineMotionVectors = false;
            MMALCameraConfig.Resolution = Resolution.As03MPixel;
            MMALCameraConfig.AnalogGain = 0;
            MMALCameraConfig.DigitalGain = 0;
            MMALCameraConfig.Annotate = null;
        }

        public static void CleanDirectory(string directory)
        {
            try
            {
                var files = Directory.GetFiles(directory);

                // Clear directory first
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }
            catch
            {
            }
        }
        
        public static void BeginTest(string name) => MMALLog.Logger.LogInformation($"Running test: {name}.");
        
        public static void BeginTest(string name, string encodingType, string pixelFormat)
            => MMALLog.Logger.LogInformation($"Running test: {name}. Encoding type: {encodingType}. Pixel format: {pixelFormat}.");
    }
}