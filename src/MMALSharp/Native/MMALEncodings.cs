using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Native
{
    public static class MMALEncodingHelpers
    {
        public static List<MMALEncoding> EncodingList => new List<MMALEncoding>
        {
            MMALEncoding.H264,
            MMALEncoding.MVC,
            MMALEncoding.H263,
            MMALEncoding.MP4V,
            MMALEncoding.MP2V,
            MMALEncoding.MP1V,
            MMALEncoding.WMV3,
            MMALEncoding.WMV2,
            MMALEncoding.WMV1,
            MMALEncoding.WVC1,
            MMALEncoding.VP8,
            MMALEncoding.VP7,
            MMALEncoding.VP6,
            MMALEncoding.THEORA,
            MMALEncoding.SPARK,
            MMALEncoding.MJPEG,
            MMALEncoding.JPEG,
            MMALEncoding.GIF,
            MMALEncoding.PNG,
            MMALEncoding.PPM,
            MMALEncoding.TGA,
            MMALEncoding.BMP,
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
            MMALEncoding.BAYER_SBGGR16,
            MMALEncoding.BAYER_SBGGR10DPCM8,
            MMALEncoding.YUVUV128,
            MMALEncoding.OPAQUE,
            MMALEncoding.EGL_IMAGE,
            MMALEncoding.PCM_UNSIGNED_BE,
            MMALEncoding.PCM_UNSIGNED_LE,
            MMALEncoding.PCM_SIGNED_BE,
            MMALEncoding.PCM_SIGNED_LE,
            MMALEncoding.PCM_FLOAT_BE,
            MMALEncoding.PCM_FLOAT_LE,
            MMALEncoding.PCM_UNSIGNED,
            MMALEncoding.PCM_SIGNED,
            MMALEncoding.PCM_FLOAT,
            MMALEncoding.MP4A,
            MMALEncoding.MPGA,
            MMALEncoding.ALAW,
            MMALEncoding.MULAW,
            MMALEncoding.ADPCM_MS,
            MMALEncoding.ADPCM_IMA_MS,
            MMALEncoding.ADPCM_SWF,
            MMALEncoding.WMA1,
            MMALEncoding.WMA2,
            MMALEncoding.WMAP,
            MMALEncoding.WMAL,
            MMALEncoding.WMAV,
            MMALEncoding.AMRNB,
            MMALEncoding.AMRWB,
            MMALEncoding.AMRWBP,
            MMALEncoding.AC3,
            MMALEncoding.EAC3,
            MMALEncoding.DTS,
            MMALEncoding.MLP,
            MMALEncoding.FLAC,
            MMALEncoding.VORBIS,
            MMALEncoding.SPEEX,
            MMALEncoding.ATRAC3,
            MMALEncoding.ATRACX,
            MMALEncoding.ATRACL,
            MMALEncoding.MIDI,
            MMALEncoding.EVRC,
            MMALEncoding.NELLYMOSER,
            MMALEncoding.QCELP,
            MMALEncoding.MP4V_DIVX_DRM,
            MMALEncoding.VARIANT_H264_DEFAULT,
            MMALEncoding.VARIANT_H264_AVC1,
            MMALEncoding.VARIANT_H264_RAW,
            MMALEncoding.VARIANT_MP4A_DEFAULT,
            MMALEncoding.VARIANT_MP4A_ADTS,
            MMALEncoding.MMAL_COLOR_SPACE_UNKNOWN,
            MMALEncoding.MMAL_COLOR_SPACE_ITUR_BT601,
            MMALEncoding.MMAL_COLOR_SPACE_ITUR_BT709,
            MMALEncoding.MMAL_COLOR_SPACE_JPEG_JFIF,
            MMALEncoding.MMAL_COLOR_SPACE_FCC,
            MMALEncoding.MMAL_COLOR_SPACE_SMPTE240M,
            MMALEncoding.MMAL_COLOR_SPACE_BT470_2_M,
            MMALEncoding.MMAL_COLOR_SPACE_BT470_2_BG,
            MMALEncoding.MMAL_COLOR_SPACE_JFIF_Y16_255
        };
        
        /// <summary>
        /// Parses an integer encoding value to an MMALEncoding object.
        /// </summary>
        /// <param name="encodingType">The encoding type value</param>
        /// <returns>The MMALEncoding object</returns>
        public static MMALEncoding ParseEncoding(this int encodingType)
        {
            return EncodingList.Where(c => c.EncodingVal == encodingType).FirstOrDefault();            
        }
    }

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

        public static MMALEncoding H264 = new MMALEncoding("H264", EncodingType.Video);
        public static MMALEncoding MVC = new MMALEncoding("MVC ", EncodingType.Video);
        public static MMALEncoding H263 = new MMALEncoding("H263", EncodingType.Video);
        public static MMALEncoding MP4V = new MMALEncoding("MP4V", EncodingType.Video);
        public static MMALEncoding MP2V = new MMALEncoding("MP2V", EncodingType.Video);
        public static MMALEncoding MP1V = new MMALEncoding("MP1V", EncodingType.Video);
        public static MMALEncoding WMV3 = new MMALEncoding("WMV3", EncodingType.Video);
        public static MMALEncoding WMV2 = new MMALEncoding("WMV2", EncodingType.Video);
        public static MMALEncoding WMV1 = new MMALEncoding("WMV1", EncodingType.Video);
        public static MMALEncoding WVC1 = new MMALEncoding("WVC1", EncodingType.Video);
        public static MMALEncoding VP8 = new MMALEncoding("VP8 ", EncodingType.Video);
        public static MMALEncoding VP7 = new MMALEncoding("VP7 ", EncodingType.Video);
        public static MMALEncoding VP6 = new MMALEncoding("VP6 ", EncodingType.Video);
        public static MMALEncoding THEORA = new MMALEncoding("THEO", EncodingType.Video);
        public static MMALEncoding SPARK = new MMALEncoding("SPRK", EncodingType.Video);
        public static MMALEncoding MJPEG = new MMALEncoding("MJPG", EncodingType.Video);
        public static MMALEncoding JPEG = new MMALEncoding("JPEG", EncodingType.Image);
        public static MMALEncoding GIF = new MMALEncoding("GIF ", EncodingType.Image);
        public static MMALEncoding PNG = new MMALEncoding("PNG ", EncodingType.Image);
        public static MMALEncoding PPM = new MMALEncoding("PPM ", EncodingType.Image);
        public static MMALEncoding TGA = new MMALEncoding("TGA ", EncodingType.Image);
        public static MMALEncoding BMP = new MMALEncoding("BMP ", EncodingType.Image);
        public static MMALEncoding I420 = new MMALEncoding("I420", EncodingType.PixelFormat);
        public static MMALEncoding I420_SLICE = new MMALEncoding("S420", EncodingType.PixelFormat);
        public static MMALEncoding YV12 = new MMALEncoding("YV12", EncodingType.PixelFormat);
        public static MMALEncoding I422 = new MMALEncoding("I422", EncodingType.PixelFormat);
        public static MMALEncoding I422_SLICE = new MMALEncoding("S422", EncodingType.PixelFormat);
        public static MMALEncoding YUYV = new MMALEncoding("YUYV", EncodingType.PixelFormat);
        public static MMALEncoding YVYU = new MMALEncoding("YVYU", EncodingType.PixelFormat);
        public static MMALEncoding UYVY = new MMALEncoding("UYVY", EncodingType.PixelFormat);
        public static MMALEncoding VYUY = new MMALEncoding("VYUY", EncodingType.PixelFormat);
        public static MMALEncoding NV12 = new MMALEncoding("NV12", EncodingType.PixelFormat);
        public static MMALEncoding NV21 = new MMALEncoding("NV21", EncodingType.PixelFormat);
        public static MMALEncoding ARGB = new MMALEncoding("ARGB", EncodingType.PixelFormat);
        public static MMALEncoding RGBA = new MMALEncoding("RGBA", EncodingType.PixelFormat);
        public static MMALEncoding ABGR = new MMALEncoding("ABGR", EncodingType.PixelFormat);
        public static MMALEncoding BGRA = new MMALEncoding("BGRA", EncodingType.PixelFormat);
        public static MMALEncoding RGB16 = new MMALEncoding("RGB2", EncodingType.PixelFormat);
        public static MMALEncoding RGB24 = new MMALEncoding("RGB3", EncodingType.PixelFormat);
        public static MMALEncoding RGB32 = new MMALEncoding("RGB4", EncodingType.PixelFormat);
        public static MMALEncoding BGR16 = new MMALEncoding("BGR2", EncodingType.PixelFormat);
        public static MMALEncoding BGR24 = new MMALEncoding("BGR3", EncodingType.PixelFormat);
        public static MMALEncoding BGR32 = new MMALEncoding("BGR4", EncodingType.PixelFormat);
        public static MMALEncoding BAYER_SBGGR10P = new MMALEncoding("pBAA", EncodingType.PixelFormat);
        public static MMALEncoding BAYER_SBGGR8 = new MMALEncoding("BA81", EncodingType.PixelFormat);
        public static MMALEncoding BAYER_SBGGR12P = new MMALEncoding("BY12", EncodingType.PixelFormat);
        public static MMALEncoding BAYER_SBGGR16 = new MMALEncoding("BYR2", EncodingType.PixelFormat);
        public static MMALEncoding BAYER_SBGGR10DPCM8 = new MMALEncoding("bBA8", EncodingType.PixelFormat);
        public static MMALEncoding YUVUV128 = new MMALEncoding("SAND", EncodingType.PixelFormat);
        public static MMALEncoding OPAQUE = new MMALEncoding("OPQV", EncodingType.Internal);
        public static MMALEncoding EGL_IMAGE = new MMALEncoding("EGLI", EncodingType.PixelFormat);
        public static MMALEncoding PCM_UNSIGNED_BE = new MMALEncoding("PCMU", EncodingType.PixelFormat);
        public static MMALEncoding PCM_UNSIGNED_LE = new MMALEncoding("pcmu", EncodingType.PixelFormat);
        public static MMALEncoding PCM_SIGNED_BE = new MMALEncoding("PCMS", EncodingType.PixelFormat);
        public static MMALEncoding PCM_SIGNED_LE = new MMALEncoding("pcms", EncodingType.PixelFormat);
        public static MMALEncoding PCM_FLOAT_BE = new MMALEncoding("PCMF", EncodingType.PixelFormat);
        public static MMALEncoding PCM_FLOAT_LE = new MMALEncoding("pcmf", EncodingType.PixelFormat);
        public static MMALEncoding PCM_UNSIGNED = PCM_UNSIGNED_LE;
        public static MMALEncoding PCM_SIGNED = PCM_SIGNED_LE;
        public static MMALEncoding PCM_FLOAT = PCM_FLOAT_LE;
        public static MMALEncoding MP4A = new MMALEncoding("MP4A", EncodingType.Audio);
        public static MMALEncoding MPGA = new MMALEncoding("MPGA", EncodingType.Audio);
        public static MMALEncoding ALAW = new MMALEncoding("ALAW", EncodingType.Audio);
        public static MMALEncoding MULAW = new MMALEncoding("ULAW", EncodingType.Audio);
        public static MMALEncoding ADPCM_MS = new MMALEncoding("MS\x00\x02", EncodingType.Audio);
        public static MMALEncoding ADPCM_IMA_MS = new MMALEncoding("MS\x00\x01", EncodingType.Audio);
        public static MMALEncoding ADPCM_SWF = new MMALEncoding("ASWF", EncodingType.Audio);
        public static MMALEncoding WMA1 = new MMALEncoding("WMA1", EncodingType.Audio);
        public static MMALEncoding WMA2 = new MMALEncoding("WMA2", EncodingType.Audio);
        public static MMALEncoding WMAP = new MMALEncoding("WMAP", EncodingType.Audio);
        public static MMALEncoding WMAL = new MMALEncoding("WMAL", EncodingType.Audio);
        public static MMALEncoding WMAV = new MMALEncoding("WMAV", EncodingType.Audio);
        public static MMALEncoding AMRNB = new MMALEncoding("AMRN", EncodingType.Audio);
        public static MMALEncoding AMRWB = new MMALEncoding("AMRW", EncodingType.Audio);
        public static MMALEncoding AMRWBP = new MMALEncoding("AMRP", EncodingType.Audio);
        public static MMALEncoding AC3 = new MMALEncoding("AC3 ", EncodingType.Audio);
        public static MMALEncoding EAC3 = new MMALEncoding("EAC3", EncodingType.Audio);
        public static MMALEncoding DTS = new MMALEncoding("DTS ", EncodingType.Audio);
        public static MMALEncoding MLP = new MMALEncoding("MLP ", EncodingType.Audio);
        public static MMALEncoding FLAC = new MMALEncoding("FLAC", EncodingType.Audio);
        public static MMALEncoding VORBIS = new MMALEncoding("VORB", EncodingType.Audio);
        public static MMALEncoding SPEEX = new MMALEncoding("SPX ", EncodingType.Audio);
        public static MMALEncoding ATRAC3 = new MMALEncoding("ATR3", EncodingType.Audio);
        public static MMALEncoding ATRACX = new MMALEncoding("ATRX", EncodingType.Audio);
        public static MMALEncoding ATRACL = new MMALEncoding("ATRL", EncodingType.Audio);
        public static MMALEncoding MIDI = new MMALEncoding("MIDI", EncodingType.Audio);
        public static MMALEncoding EVRC = new MMALEncoding("EVRC", EncodingType.Audio);
        public static MMALEncoding NELLYMOSER = new MMALEncoding("NELY", EncodingType.Audio);
        public static MMALEncoding QCELP = new MMALEncoding("QCEL", EncodingType.Audio);
        public static MMALEncoding MP4V_DIVX_DRM = new MMALEncoding("M4VD", EncodingType.Video);
        public static MMALEncoding VARIANT_H264_DEFAULT = new MMALEncoding(0, "VARIANT_H264_DEFAULT", EncodingType.Video);
        public static MMALEncoding VARIANT_H264_AVC1 = new MMALEncoding("AVC1", EncodingType.Video);
        public static MMALEncoding VARIANT_H264_RAW = new MMALEncoding("RAW ", EncodingType.Video);
        public static MMALEncoding VARIANT_MP4A_DEFAULT = new MMALEncoding(0, "VARIANT_MP4A_DEFAULT", EncodingType.Video);
        public static MMALEncoding VARIANT_MP4A_ADTS = new MMALEncoding("ADTS", EncodingType.Video);
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
