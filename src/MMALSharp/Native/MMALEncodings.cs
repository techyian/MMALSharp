using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Native
{
    public class MMALEncoding
    {
        public enum EncodingType
        {
            Image,
            Video,
            Audio,
            PixelFormat,
            Internal
        }

        public int EncodingVal { get; set; }
        public string EncodingName { get; set; }
        public EncodingType EncType { get; set; }

        public MMALEncoding(string s, EncodingType type)
        {
            this.EncodingVal = MMALUtil.MMAL_FOURCC(s);
            this.EncodingName = s;
            this.EncType = type;
        }

        public MMALEncoding(int val, string name, EncodingType type)
        {
            this.EncodingVal = val;
            this.EncodingName = name;
            this.EncType = type;
        }

        public static MMALEncoding MMAL_ENCODING_H264 = new MMALEncoding("H264", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_MVC = new MMALEncoding("MVC ", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_H263 = new MMALEncoding("H263", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_MP4V = new MMALEncoding("MP4V", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_MP2V = new MMALEncoding("MP2V", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_MP1V = new MMALEncoding("MP1V", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_WMV3 = new MMALEncoding("WMV3", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_WMV2 = new MMALEncoding("WMV2", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_WMV1 = new MMALEncoding("WMV1", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_WVC1 = new MMALEncoding("WVC1", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_VP8 = new MMALEncoding("VP8 ", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_VP7 = new MMALEncoding("VP7 ", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_VP6 = new MMALEncoding("VP6 ", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_THEORA = new MMALEncoding("THEO", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_SPARK = new MMALEncoding("SPRK", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_MJPEG = new MMALEncoding("MJPG", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_JPEG = new MMALEncoding("JPEG", EncodingType.Image);
        public static MMALEncoding MMAL_ENCODING_GIF = new MMALEncoding("GIF ", EncodingType.Image);
        public static MMALEncoding MMAL_ENCODING_PNG = new MMALEncoding("PNG ", EncodingType.Image);
        public static MMALEncoding MMAL_ENCODING_PPM = new MMALEncoding("PPM ", EncodingType.Image);
        public static MMALEncoding MMAL_ENCODING_TGA = new MMALEncoding("TGA ", EncodingType.Image);
        public static MMALEncoding MMAL_ENCODING_BMP = new MMALEncoding("BMP ", EncodingType.Image);
        public static MMALEncoding MMAL_ENCODING_I420 = new MMALEncoding("I420", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_I420_SLICE = new MMALEncoding("S420", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_YV12 = new MMALEncoding("YV12", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_I422 = new MMALEncoding("I422", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_I422_SLICE = new MMALEncoding("S422", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_YUYV = new MMALEncoding("YUYV", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_YVYU = new MMALEncoding("YVYU", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_UYVY = new MMALEncoding("UYVY", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_VYUY = new MMALEncoding("VYUY", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_NV12 = new MMALEncoding("NV12", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_NV21 = new MMALEncoding("NV21", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_ARGB = new MMALEncoding("ARGB", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_RGBA = new MMALEncoding("RGBA", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_ABGR = new MMALEncoding("ABGR", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_BGRA = new MMALEncoding("BGRA", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_RGB16 = new MMALEncoding("RGB2", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_RGB24 = new MMALEncoding("RGB3", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_RGB32 = new MMALEncoding("RGB4", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_BGR16 = new MMALEncoding("BGR2", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_BGR24 = new MMALEncoding("BGR3", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_BGR32 = new MMALEncoding("BGR4", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_BAYER_SBGGR10P = new MMALEncoding("pBAA", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_BAYER_SBGGR8 = new MMALEncoding("BA81", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_BAYER_SBGGR12P = new MMALEncoding("BY12", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_BAYER_SBGGR16 = new MMALEncoding("BYR2", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_BAYER_SBGGR10DPCM8 = new MMALEncoding("bBA8", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_YUVUV128 = new MMALEncoding("SAND", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_OPAQUE = new MMALEncoding("OPQV", EncodingType.Internal);
        public static MMALEncoding MMAL_ENCODING_EGL_IMAGE = new MMALEncoding("EGLI", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_PCM_UNSIGNED_BE = new MMALEncoding("PCMU", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_PCM_UNSIGNED_LE = new MMALEncoding("pcmu", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_PCM_SIGNED_BE = new MMALEncoding("PCMS", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_PCM_SIGNED_LE = new MMALEncoding("pcms", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_PCM_FLOAT_BE = new MMALEncoding("PCMF", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_PCM_FLOAT_LE = new MMALEncoding("pcmf", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_ENCODING_PCM_UNSIGNED = MMAL_ENCODING_PCM_UNSIGNED_LE;
        public static MMALEncoding MMAL_ENCODING_PCM_SIGNED = MMAL_ENCODING_PCM_SIGNED_LE;
        public static MMALEncoding MMAL_ENCODING_PCM_FLOAT = MMAL_ENCODING_PCM_FLOAT_LE;
        public static MMALEncoding MMAL_ENCODING_MP4A = new MMALEncoding("MP4A", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_MPGA = new MMALEncoding("MPGA", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_ALAW = new MMALEncoding("ALAW", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_MULAW = new MMALEncoding("ULAW", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_ADPCM_MS = new MMALEncoding("MS\x00\x02", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_ADPCM_IMA_MS = new MMALEncoding("MS\x00\x01", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_ADPCM_SWF = new MMALEncoding("ASWF", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_WMA1 = new MMALEncoding("WMA1", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_WMA2 = new MMALEncoding("WMA2", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_WMAP = new MMALEncoding("WMAP", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_WMAL = new MMALEncoding("WMAL", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_WMAV = new MMALEncoding("WMAV", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_AMRNB = new MMALEncoding("AMRN", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_AMRWB = new MMALEncoding("AMRW", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_AMRWBP = new MMALEncoding("AMRP", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_AC3 = new MMALEncoding("AC3 ", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_EAC3 = new MMALEncoding("EAC3", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_DTS = new MMALEncoding("DTS ", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_MLP = new MMALEncoding("MLP ", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_FLAC = new MMALEncoding("FLAC", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_VORBIS = new MMALEncoding("VORB", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_SPEEX = new MMALEncoding("SPX ", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_ATRAC3 = new MMALEncoding("ATR3", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_ATRACX = new MMALEncoding("ATRX", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_ATRACL = new MMALEncoding("ATRL", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_MIDI = new MMALEncoding("MIDI", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_EVRC = new MMALEncoding("EVRC", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_NELLYMOSER = new MMALEncoding("NELY", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_QCELP = new MMALEncoding("QCEL", EncodingType.Audio);
        public static MMALEncoding MMAL_ENCODING_MP4V_DIVX_DRM = new MMALEncoding("M4VD", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_VARIANT_H264_DEFAULT = new MMALEncoding(0, "MMAL_ENCODING_VARIANT_H264_DEFAULT", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_VARIANT_H264_AVC1 = new MMALEncoding("AVC1", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_VARIANT_H264_RAW = new MMALEncoding("RAW ", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_VARIANT_MP4A_DEFAULT = new MMALEncoding(0, "MMAL_ENCODING_VARIANT_MP4A_DEFAULT", EncodingType.Video);
        public static MMALEncoding MMAL_ENCODING_VARIANT_MP4A_ADTS = new MMALEncoding("ADTS", EncodingType.Video);
        public static MMALEncoding MMAL_COLOR_SPACE_UNKNOWN = new MMALEncoding(0, "MMAL_COLOR_SPACE_UNKNOWN", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_COLOR_SPACE_ITUR_BT601 = new MMALEncoding("Y601", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_COLOR_SPACE_ITUR_BT709 = new MMALEncoding("Y709", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_COLOR_SPACE_JPEG_JFIF = new MMALEncoding("YJFI", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_COLOR_SPACE_FCC = new MMALEncoding("YFCC", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_COLOR_SPACE_SMPTE240M = new MMALEncoding("Y240", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_COLOR_SPACE_BT470_2_M = new MMALEncoding("Y__M", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_COLOR_SPACE_BT470_2_BG = new MMALEncoding("Y_BG", EncodingType.PixelFormat);
        public static MMALEncoding MMAL_COLOR_SPACE_JFIF_Y16_255 = new MMALEncoding("YY16", EncodingType.PixelFormat);

    }

}
