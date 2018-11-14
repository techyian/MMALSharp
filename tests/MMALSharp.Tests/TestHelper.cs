using System.IO;
using MMALSharp.Components;
using MMALSharp.Native;

namespace MMALSharp.Tests
{
    public class TestHelper
    {
        public static void SetConfigurationDefaults()
        {
            MMALCameraConfig.Debug = true;
            MMALCameraConfig.Brightness = 50;
            MMALCameraConfig.Sharpness = 0;
            MMALCameraConfig.Contrast = 0;
            MMALCameraConfig.Saturation = 0;
            MMALCameraConfig.AwbGainsB = 0;
            MMALCameraConfig.AwbGainsR = 0;
            MMALCameraConfig.AwbMode = MMAL_PARAM_AWBMODE_T.MMAL_PARAM_AWBMODE_AUTO;
            MMALCameraConfig.ColourFx = new ColourEffects();
            MMALCameraConfig.ExposureCompensation = -1;
            MMALCameraConfig.ExposureMeterMode = MMAL_PARAM_EXPOSUREMETERINGMODE_T.MMAL_PARAM_EXPOSUREMETERINGMODE_AVERAGE;
            MMALCameraConfig.ExposureMode = MMAL_PARAM_EXPOSUREMODE_T.MMAL_PARAM_EXPOSUREMODE_AUTO;
            MMALCameraConfig.ROI = new Zoom();
            MMALCameraConfig.ISO = 0;
            MMALCameraConfig.StatsPass = false;
            MMALCameraConfig.Flips = MMAL_PARAM_MIRROR_T.MMAL_PARAM_MIRROR_NONE;
            MMALCameraConfig.ImageFx = MMAL_PARAM_IMAGEFX_T.MMAL_PARAM_IMAGEFX_NONE;
            MMALCameraConfig.Rotation = 0;
            MMALCameraConfig.DrcLevel = MMAL_PARAMETER_DRC_STRENGTH_T.MMAL_PARAMETER_DRC_STRENGTH_OFF;
            MMALCameraConfig.ShutterSpeed = 0;
            MMALCameraConfig.SensorMode = MMALSensorMode.Mode0;
            MMALCameraConfig.VideoStabilisation = true; 
            MMALCameraConfig.StillEncoding = MMALEncoding.OPAQUE;
            MMALCameraConfig.StillSubFormat = MMALEncoding.I420;
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
        
        public static void BeginTest(string name) => MMALLog.Logger.Info($"Running test: {name}.");
        
        public static void BeginTest(string name, string encodingType, string pixelFormat)
            => MMALLog.Logger.Info($"Running test: {name}. Encoding type: {encodingType}. Pixel format: {pixelFormat}.");
            
    }
}