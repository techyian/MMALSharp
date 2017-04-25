using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Tests
{
    public class TestData
    {        
        public static List<MMALEncoding> PixelFormats = new List<MMALEncoding>()
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
                
        private static object[] GetEncoderData(MMALEncoding encodingType)
        {
            var arr = new object[PixelFormats.Count()];

            int i = 0;
            foreach (MMALEncoding pixFormat in PixelFormats)
            {
                arr[i] = new object[] { "jpg", encodingType, pixFormat };

                i++;
            }

            return arr;
        }

        public static object[] JPEGEncoderData
        {
            get
            {
                return GetEncoderData(MMALEncoding.MMAL_ENCODING_JPEG);                
            }
        }

        public static object[] GIFEncoderData
        {
            get
            {
                return GetEncoderData(MMALEncoding.MMAL_ENCODING_GIF);                
            }
        }

        public static object[] PNGEncoderData
        {
            get
            {
                return GetEncoderData(MMALEncoding.MMAL_ENCODING_PNG);                
            }
        }

        public static object[] PPMEncoderData
        {
            get
            {
                return GetEncoderData(MMALEncoding.MMAL_ENCODING_PPM);                
            }
        }

        public static object[] TGAEncoderData
        {
            get
            {
                return GetEncoderData(MMALEncoding.MMAL_ENCODING_TGA);              
            }
        }

        public static object[] BMPEncoderData
        {
            get
            {
                return GetEncoderData(MMALEncoding.MMAL_ENCODING_BMP);              
            }
        }

    }
}
