using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Tests
{
    public class TestHelper
    {
        public static void SetConfigurationDefaults()
        {
            MMALCameraConfig.Brightness = 0;
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
            MMALCameraConfig.Reload();
        }

        public static void CleanDirectory(string directory)
        {
            try
            {
                var files = Directory.GetFiles(directory);

                //Clear directory first
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }
            catch
            {
            }
        }
    }

    public class TestData
    {        
        public static List<MMALEncoding> PixelFormats = new List<MMALEncoding>
        {
            MMALEncoding.I420,
            MMALEncoding.I420_SLICE,
            MMALEncoding.YV12,
            MMALEncoding.I422,
            MMALEncoding.I422_SLICE,
            MMALEncoding.YUYV,
            MMALEncoding.YVYU,
            MMALEncoding.UYVY,
            MMALEncoding.VYUY,
            MMALEncoding.NV12,
            MMALEncoding.NV21,
            MMALEncoding.ARGB,
            MMALEncoding.RGBA,
            MMALEncoding.ABGR,
            MMALEncoding.BGRA,
            MMALEncoding.RGB16,
            MMALEncoding.RGB24,
            MMALEncoding.RGB32,
            MMALEncoding.BGR16,
            MMALEncoding.BGR24,
            MMALEncoding.BGR32,
            MMALEncoding.BAYER_SBGGR10P,
            MMALEncoding.BAYER_SBGGR8,
            MMALEncoding.BAYER_SBGGR12P,
            MMALEncoding.BAYER_SBGGR8,
            MMALEncoding.BAYER_SBGGR12P,
            MMALEncoding.BAYER_SBGGR16,
            MMALEncoding.BAYER_SBGGR10DPCM8,
            MMALEncoding.YUVUV128,
            MMALEncoding.EGL_IMAGE,
            MMALEncoding.PCM_UNSIGNED_BE,
            MMALEncoding.PCM_UNSIGNED_LE,
            MMALEncoding.PCM_SIGNED_BE,
            MMALEncoding.PCM_SIGNED_LE,
            MMALEncoding.PCM_FLOAT_BE,
            MMALEncoding.PCM_FLOAT_LE,
            MMALEncoding.MMAL_COLOR_SPACE_ITUR_BT601,
            MMALEncoding.MMAL_COLOR_SPACE_ITUR_BT709,
            MMALEncoding.MMAL_COLOR_SPACE_JPEG_JFIF,
            MMALEncoding.MMAL_COLOR_SPACE_FCC,
            MMALEncoding.MMAL_COLOR_SPACE_SMPTE240M,
            MMALEncoding.MMAL_COLOR_SPACE_BT470_2_M,
            MMALEncoding.MMAL_COLOR_SPACE_BT470_2_BG,
            MMALEncoding.MMAL_COLOR_SPACE_JFIF_Y16_255
        };
                
        private static IEnumerable<object> GetEncoderData(MMALEncoding encodingType, string extension)
        {
            return PixelFormats.Select(pixFormat => new object[] { extension, encodingType, pixFormat });
        }

        #region Still image encoders
        
        public static IEnumerable<object> JpegEncoderData => GetEncoderData(MMALEncoding.JPEG, "jpg");
        
        public static IEnumerable<object> GifEncoderData => GetEncoderData(MMALEncoding.GIF, "gif");
        
        public static IEnumerable<object> PngEncoderData => GetEncoderData(MMALEncoding.PNG, "png");
        
        public static IEnumerable<object> PpmEncoderData => GetEncoderData(MMALEncoding.PPM, "ppm");
        
        public static IEnumerable<object> TgaEncoderData => GetEncoderData(MMALEncoding.TGA, "tga");
        
        public static IEnumerable<object> BmpEncoderData => GetEncoderData(MMALEncoding.BMP, "bmp");

        #endregion

        #region Video encoders

        public static IEnumerable<object> H264EncoderData => GetEncoderData(MMALEncoding.H264, "avi");

        public static IEnumerable<object> MVCEncoderData => GetEncoderData(MMALEncoding.MVC, "mvc");

        public static IEnumerable<object> H263EncoderData => GetEncoderData(MMALEncoding.H263, "h263");

        public static IEnumerable<object> MP4EncoderData => GetEncoderData(MMALEncoding.MP4V, "mp4");

        public static IEnumerable<object> MP2EncoderData => GetEncoderData(MMALEncoding.MP2V, "mp2");

        public static IEnumerable<object> MP1EncoderData => GetEncoderData(MMALEncoding.MP1V, "mp1");

        public static IEnumerable<object> WMV3EncoderData => GetEncoderData(MMALEncoding.WMV3, "wmv");

        public static IEnumerable<object> WMV2EncoderData => GetEncoderData(MMALEncoding.WMV2, "wmv");

        public static IEnumerable<object> WMV1EncoderData => GetEncoderData(MMALEncoding.WMV1, "wmv");

        public static IEnumerable<object> WVC1EncoderData => GetEncoderData(MMALEncoding.WVC1, "asf");

        public static IEnumerable<object> VP8EncoderData => GetEncoderData(MMALEncoding.VP8, "webm");

        public static IEnumerable<object> VP7EncoderData => GetEncoderData(MMALEncoding.VP7, "webm");

        public static IEnumerable<object> VP6EncoderData => GetEncoderData(MMALEncoding.VP6, "webm");

        public static IEnumerable<object> TheoraEncoderData => GetEncoderData(MMALEncoding.THEORA, "ogv");

        public static IEnumerable<object> SparkEncoderData => GetEncoderData(MMALEncoding.SPARK, "flv");

        public static IEnumerable<object> MJPEGEncoderData => GetEncoderData(MMALEncoding.MJPEG, "mjpeg");
        
        #endregion

    }
}
