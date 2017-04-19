using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Native
{
    public class MMALEncoding
    {
        public int EncodingVal { get; set; }
        public string EncodingName { get; set; }

        public MMALEncoding(string s)
        {
            this.EncodingVal = MMALUtil.MMAL_FOURCC(s);
            this.EncodingName = s;            
        }

        public MMALEncoding(int val, string name)
        {
            this.EncodingVal = val;
            this.EncodingName = name;
        }

        public static MMALEncoding MMAL_ENCODING_H264 = new MMALEncoding("H264");
        public static MMALEncoding MMAL_ENCODING_MVC = new MMALEncoding("MVC ");
        public static MMALEncoding MMAL_ENCODING_H263 = new MMALEncoding("H263");
        public static MMALEncoding MMAL_ENCODING_MP4V = new MMALEncoding("MP4V");
        public static MMALEncoding MMAL_ENCODING_MP2V = new MMALEncoding("MP2V");
        public static MMALEncoding MMAL_ENCODING_MP1V = new MMALEncoding("MP1V");
        public static MMALEncoding MMAL_ENCODING_WMV3 = new MMALEncoding("WMV3");
        public static MMALEncoding MMAL_ENCODING_WMV2 = new MMALEncoding("WMV2");
        public static MMALEncoding MMAL_ENCODING_WMV1 = new MMALEncoding("WMV1");
        public static MMALEncoding MMAL_ENCODING_WVC1 = new MMALEncoding("WVC1");
        public static MMALEncoding MMAL_ENCODING_VP8 = new MMALEncoding("VP8 ");
        public static MMALEncoding MMAL_ENCODING_VP7 = new MMALEncoding("VP7 ");
        public static MMALEncoding MMAL_ENCODING_VP6 = new MMALEncoding("VP6 ");
        public static MMALEncoding MMAL_ENCODING_THEORA = new MMALEncoding("THEO");
        public static MMALEncoding MMAL_ENCODING_SPARK = new MMALEncoding("SPRK");
        public static MMALEncoding MMAL_ENCODING_MJPEG = new MMALEncoding("MJPG");
        public static MMALEncoding MMAL_ENCODING_JPEG = new MMALEncoding("JPEG");
        public static MMALEncoding MMAL_ENCODING_GIF = new MMALEncoding("GIF ");
        public static MMALEncoding MMAL_ENCODING_PNG = new MMALEncoding("PNG ");
        public static MMALEncoding MMAL_ENCODING_PPM = new MMALEncoding("PPM ");
        public static MMALEncoding MMAL_ENCODING_TGA = new MMALEncoding("TGA ");
        public static MMALEncoding MMAL_ENCODING_BMP = new MMALEncoding("BMP ");
        public static MMALEncoding MMAL_ENCODING_I420 = new MMALEncoding("I420");
        public static MMALEncoding MMAL_ENCODING_I420_SLICE = new MMALEncoding("S420");
        public static MMALEncoding MMAL_ENCODING_YV12 = new MMALEncoding("YV12");
        public static MMALEncoding MMAL_ENCODING_I422 = new MMALEncoding("I422");
        public static MMALEncoding MMAL_ENCODING_I422_SLICE = new MMALEncoding("S422");
        public static MMALEncoding MMAL_ENCODING_YUYV = new MMALEncoding("YUYV");
        public static MMALEncoding MMAL_ENCODING_YVYU = new MMALEncoding("YVYU");
        public static MMALEncoding MMAL_ENCODING_UYVY = new MMALEncoding("UYVY");
        public static MMALEncoding MMAL_ENCODING_VYUY = new MMALEncoding("VYUY");
        public static MMALEncoding MMAL_ENCODING_NV12 = new MMALEncoding("NV12");
        public static MMALEncoding MMAL_ENCODING_NV21 = new MMALEncoding("NV21");
        public static MMALEncoding MMAL_ENCODING_ARGB = new MMALEncoding("ARGB");
        public static MMALEncoding MMAL_ENCODING_RGBA = new MMALEncoding("RGBA");
        public static MMALEncoding MMAL_ENCODING_ABGR = new MMALEncoding("ABGR");
        public static MMALEncoding MMAL_ENCODING_BGRA = new MMALEncoding("BGRA");
        public static MMALEncoding MMAL_ENCODING_RGB16 = new MMALEncoding("RGB2");
        public static MMALEncoding MMAL_ENCODING_RGB24 = new MMALEncoding("RGB3");
        public static MMALEncoding MMAL_ENCODING_RGB32 = new MMALEncoding("RGB4");
        public static MMALEncoding MMAL_ENCODING_BGR16 = new MMALEncoding("BGR2");
        public static MMALEncoding MMAL_ENCODING_BGR24 = new MMALEncoding("BGR3");
        public static MMALEncoding MMAL_ENCODING_BGR32 = new MMALEncoding("BGR4");
        public static MMALEncoding MMAL_ENCODING_BAYER_SBGGR10P = new MMALEncoding("pBAA");
        public static MMALEncoding MMAL_ENCODING_BAYER_SBGGR8 = new MMALEncoding("BA81");
        public static MMALEncoding MMAL_ENCODING_BAYER_SBGGR12P = new MMALEncoding("BY12");
        public static MMALEncoding MMAL_ENCODING_BAYER_SBGGR16 = new MMALEncoding("BYR2");
        public static MMALEncoding MMAL_ENCODING_BAYER_SBGGR10DPCM8 = new MMALEncoding("bBA8");
        public static MMALEncoding MMAL_ENCODING_YUVUV128 = new MMALEncoding("SAND");
        public static MMALEncoding MMAL_ENCODING_OPAQUE = new MMALEncoding("OPQV");
        public static MMALEncoding MMAL_ENCODING_EGL_IMAGE = new MMALEncoding("EGLI");
        public static MMALEncoding MMAL_ENCODING_PCM_UNSIGNED_BE = new MMALEncoding("PCMU");
        public static MMALEncoding MMAL_ENCODING_PCM_UNSIGNED_LE = new MMALEncoding("pcmu");
        public static MMALEncoding MMAL_ENCODING_PCM_SIGNED_BE = new MMALEncoding("PCMS");
        public static MMALEncoding MMAL_ENCODING_PCM_SIGNED_LE = new MMALEncoding("pcms");
        public static MMALEncoding MMAL_ENCODING_PCM_FLOAT_BE = new MMALEncoding("PCMF");
        public static MMALEncoding MMAL_ENCODING_PCM_FLOAT_LE = new MMALEncoding("pcmf");
        public static MMALEncoding MMAL_ENCODING_PCM_UNSIGNED = MMAL_ENCODING_PCM_UNSIGNED_LE;
        public static MMALEncoding MMAL_ENCODING_PCM_SIGNED = MMAL_ENCODING_PCM_SIGNED_LE;
        public static MMALEncoding MMAL_ENCODING_PCM_FLOAT = MMAL_ENCODING_PCM_FLOAT_LE;
        public static MMALEncoding MMAL_ENCODING_MP4A = new MMALEncoding("MP4A");
        public static MMALEncoding MMAL_ENCODING_MPGA = new MMALEncoding("MPGA");
        public static MMALEncoding MMAL_ENCODING_ALAW = new MMALEncoding("ALAW");
        public static MMALEncoding MMAL_ENCODING_MULAW = new MMALEncoding("ULAW");
        public static MMALEncoding MMAL_ENCODING_ADPCM_MS = new MMALEncoding("MS\x00\x02");
        public static MMALEncoding MMAL_ENCODING_ADPCM_IMA_MS = new MMALEncoding("MS\x00\x01");
        public static MMALEncoding MMAL_ENCODING_ADPCM_SWF = new MMALEncoding("ASWF");
        public static MMALEncoding MMAL_ENCODING_WMA1 = new MMALEncoding("WMA1");
        public static MMALEncoding MMAL_ENCODING_WMA2 = new MMALEncoding("WMA2");
        public static MMALEncoding MMAL_ENCODING_WMAP = new MMALEncoding("WMAP");
        public static MMALEncoding MMAL_ENCODING_WMAL = new MMALEncoding("WMAL");
        public static MMALEncoding MMAL_ENCODING_WMAV = new MMALEncoding("WMAV");
        public static MMALEncoding MMAL_ENCODING_AMRNB = new MMALEncoding("AMRN");
        public static MMALEncoding MMAL_ENCODING_AMRWB = new MMALEncoding("AMRW");
        public static MMALEncoding MMAL_ENCODING_AMRWBP = new MMALEncoding("AMRP");
        public static MMALEncoding MMAL_ENCODING_AC3 = new MMALEncoding("AC3 ");
        public static MMALEncoding MMAL_ENCODING_EAC3 = new MMALEncoding("EAC3");
        public static MMALEncoding MMAL_ENCODING_DTS = new MMALEncoding("DTS ");
        public static MMALEncoding MMAL_ENCODING_MLP = new MMALEncoding("MLP ");
        public static MMALEncoding MMAL_ENCODING_FLAC = new MMALEncoding("FLAC");
        public static MMALEncoding MMAL_ENCODING_VORBIS = new MMALEncoding("VORB");
        public static MMALEncoding MMAL_ENCODING_SPEEX = new MMALEncoding("SPX ");
        public static MMALEncoding MMAL_ENCODING_ATRAC3 = new MMALEncoding("ATR3");
        public static MMALEncoding MMAL_ENCODING_ATRACX = new MMALEncoding("ATRX");
        public static MMALEncoding MMAL_ENCODING_ATRACL = new MMALEncoding("ATRL");
        public static MMALEncoding MMAL_ENCODING_MIDI = new MMALEncoding("MIDI");
        public static MMALEncoding MMAL_ENCODING_EVRC = new MMALEncoding("EVRC");
        public static MMALEncoding MMAL_ENCODING_NELLYMOSER = new MMALEncoding("NELY");
        public static MMALEncoding MMAL_ENCODING_QCELP = new MMALEncoding("QCEL");
        public static MMALEncoding MMAL_ENCODING_MP4V_DIVX_DRM = new MMALEncoding("M4VD");
        public static MMALEncoding MMAL_ENCODING_VARIANT_H264_DEFAULT = new MMALEncoding(0, "MMAL_ENCODING_VARIANT_H264_DEFAULT");
        public static MMALEncoding MMAL_ENCODING_VARIANT_H264_AVC1 = new MMALEncoding("AVC1");
        public static MMALEncoding MMAL_ENCODING_VARIANT_H264_RAW = new MMALEncoding("RAW ");
        public static MMALEncoding MMAL_ENCODING_VARIANT_MP4A_DEFAULT = new MMALEncoding(0, "MMAL_ENCODING_VARIANT_MP4A_DEFAULT");
        public static MMALEncoding MMAL_ENCODING_VARIANT_MP4A_ADTS = new MMALEncoding("ADTS");
        public static MMALEncoding MMAL_COLOR_SPACE_UNKNOWN = new MMALEncoding(0, "MMAL_COLOR_SPACE_UNKNOWN");
        public static MMALEncoding MMAL_COLOR_SPACE_ITUR_BT601 = new MMALEncoding("Y601");
        public static MMALEncoding MMAL_COLOR_SPACE_ITUR_BT709 = new MMALEncoding("Y709");
        public static MMALEncoding MMAL_COLOR_SPACE_JPEG_JFIF = new MMALEncoding("YJFI");
        public static MMALEncoding MMAL_COLOR_SPACE_FCC = new MMALEncoding("YFCC");
        public static MMALEncoding MMAL_COLOR_SPACE_SMPTE240M = new MMALEncoding("Y240");
        public static MMALEncoding MMAL_COLOR_SPACE_BT470_2_M = new MMALEncoding("Y__M");
        public static MMALEncoding MMAL_COLOR_SPACE_BT470_2_BG = new MMALEncoding("Y_BG");
        public static MMALEncoding MMAL_COLOR_SPACE_JFIF_Y16_255 = new MMALEncoding("YY16");

    }

}
