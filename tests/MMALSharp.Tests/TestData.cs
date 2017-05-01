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
            MMALEncoding.MMAL_ENCODING_I420,
            MMALEncoding.MMAL_ENCODING_I420_SLICE,
            MMALEncoding.MMAL_ENCODING_YV12,
            MMALEncoding.MMAL_ENCODING_I422,
            MMALEncoding.MMAL_ENCODING_I422_SLICE,
            MMALEncoding.MMAL_ENCODING_YUYV,
            MMALEncoding.MMAL_ENCODING_YVYU,
            MMALEncoding.MMAL_ENCODING_UYVY,
            MMALEncoding.MMAL_ENCODING_VYUY,
            MMALEncoding.MMAL_ENCODING_NV12,
            MMALEncoding.MMAL_ENCODING_NV21,
            MMALEncoding.MMAL_ENCODING_ARGB,
            MMALEncoding.MMAL_ENCODING_RGBA,
            MMALEncoding.MMAL_ENCODING_ABGR,
            MMALEncoding.MMAL_ENCODING_BGRA,
            MMALEncoding.MMAL_ENCODING_RGB16,
            MMALEncoding.MMAL_ENCODING_RGB24,
            MMALEncoding.MMAL_ENCODING_RGB32,
            MMALEncoding.MMAL_ENCODING_BGR16,
            MMALEncoding.MMAL_ENCODING_BGR24,
            MMALEncoding.MMAL_ENCODING_BGR32,
            MMALEncoding.MMAL_ENCODING_BAYER_SBGGR10P,
            MMALEncoding.MMAL_ENCODING_BAYER_SBGGR8,
            MMALEncoding.MMAL_ENCODING_BAYER_SBGGR12P,
            MMALEncoding.MMAL_ENCODING_BAYER_SBGGR8,
            MMALEncoding.MMAL_ENCODING_BAYER_SBGGR12P,
            MMALEncoding.MMAL_ENCODING_BAYER_SBGGR16,
            MMALEncoding.MMAL_ENCODING_BAYER_SBGGR10DPCM8,
            MMALEncoding.MMAL_ENCODING_YUVUV128,
            MMALEncoding.MMAL_ENCODING_EGL_IMAGE,
            MMALEncoding.MMAL_ENCODING_PCM_UNSIGNED_BE,
            MMALEncoding.MMAL_ENCODING_PCM_UNSIGNED_LE,
            MMALEncoding.MMAL_ENCODING_PCM_SIGNED_BE,
            MMALEncoding.MMAL_ENCODING_PCM_SIGNED_LE,
            MMALEncoding.MMAL_ENCODING_PCM_FLOAT_BE,
            MMALEncoding.MMAL_ENCODING_PCM_FLOAT_LE,
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
        
        public static IEnumerable<object> JpegEncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_JPEG, "jpg");
        
        public static IEnumerable<object> GifEncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_GIF, "gif");
        
        public static IEnumerable<object> PngEncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_PNG, "png");
        
        public static IEnumerable<object> PpmEncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_PPM, "ppm");
        
        public static IEnumerable<object> TgaEncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_TGA, "tga");
        
        public static IEnumerable<object> BmpEncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_BMP, "bmp");

        #endregion

        #region Video encoders

        public static IEnumerable<object> H264EncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_H264, "avi");

        public static IEnumerable<object> MVCEncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_MVC, "mvc");

        public static IEnumerable<object> H263EncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_H263, "h263");

        public static IEnumerable<object> MP4EncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_MP4V, "mp4");

        public static IEnumerable<object> MP2EncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_MP2V, "mp2");

        public static IEnumerable<object> MP1EncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_MP1V, "mp1");

        public static IEnumerable<object> WMV3EncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_WMV3, "wmv");

        public static IEnumerable<object> WMV2EncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_WMV2, "wmv");

        public static IEnumerable<object> WMV1EncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_WMV1, "wmv");

        public static IEnumerable<object> WVC1EncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_WVC1, "asf");

        public static IEnumerable<object> VP8EncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_VP8, "webm");

        public static IEnumerable<object> VP7EncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_VP7, "webm");

        public static IEnumerable<object> VP6EncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_VP6, "webm");

        public static IEnumerable<object> TheoraEncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_THEORA, "ogv");

        public static IEnumerable<object> SparkEncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_SPARK, "flv");

        public static IEnumerable<object> MJPEGEncoderData => GetEncoderData(MMALEncoding.MMAL_ENCODING_MJPEG, "mjpeg");
        
        #endregion

    }
}
