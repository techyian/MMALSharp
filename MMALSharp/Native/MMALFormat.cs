using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Native
{
    public static class MMALFormat
    {
        public enum MMAL_ES_TYPE_T
        {
            MMAL_ES_TYPE_UNKNOWN,
            MMAL_ES_TYPE_CONTROL,
            MMAL_ES_TYPE_AUDIO,
            MMAL_ES_TYPE_VIDEO,
            MMAL_ES_TYPE_SUBPICTURE
        }

        public static int MMAL_ES_FORMAT_COMPARE_FLAG_TYPE = 0x01;
        public static int MMAL_ES_FORMAT_COMPARE_FLAG_ENCODING = 0x02;
        public static int MMAL_ES_FORMAT_COMPARE_FLAG_BITRATE = 0x04;
        public static int MMAL_ES_FORMAT_COMPARE_FLAG_FLAGS = 0x08;
        public static int MMAL_ES_FORMAT_COMPARE_FLAG_EXTRADATA = 0x10;
        public static int MMAL_ES_FORMAT_COMPARE_FLAG_VIDEO_RESOLUTION = 0x0100;
        public static int MMAL_ES_FORMAT_COMPARE_FLAG_VIDEO_CROPPING = 0x0200;
        public static int MMAL_ES_FORMAT_COMPARE_FLAG_VIDEO_FRAME_RATE = 0x0400;
        public static int MMAL_ES_FORMAT_COMPARE_FLAG_VIDEO_ASPECT_RATIO = 0x0800;
        public static int MMAL_ES_FORMAT_COMPARE_FLAG_VIDEO_COLOR_SPACE = 0x1000;

        public static int MMAL_ES_FORMAT_COMPARE_FLAG_ES_OTHER = 0x10000000;

        [DllImport("libmmal.so", EntryPoint = "mmal_format_alloc", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMAL_ES_FORMAT_T* mmal_format_alloc();

        [DllImport("libmmal.so", EntryPoint = "mmal_format_free", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void mmal_format_free(MMAL_ES_FORMAT_T* format);

        [DllImport("libmmal.so", EntryPoint = "mmal_format_extradata_alloc", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMALUtil.MMAL_STATUS_T mmal_format_extradata_alloc(MMAL_ES_FORMAT_T* format, UInt32 extradata_size);

        [DllImport("libmmal.so", EntryPoint = "mmal_format_copy", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void mmal_format_copy(MMAL_ES_FORMAT_T* fmt_dst, MMAL_ES_FORMAT_T* fmt_src);

        [DllImport("libmmal.so", EntryPoint = "mmal_format_full_copy", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern MMALUtil.MMAL_STATUS_T mmal_format_full_copy(MMAL_ES_FORMAT_T* fmt_dst, MMAL_ES_FORMAT_T* fmt_src);

        [DllImport("libmmal.so", EntryPoint = "mmal_format_compare", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern uint mmal_format_compare(MMAL_ES_FORMAT_T* ptr, MMAL_ES_FORMAT_T* ptr2);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_VIDEO_FORMAT_T
    {
        private int width, height;
        private MMAL_RECT_T crop;
        private MMAL_RATIONAL_T frameRate, par;
        private int colorSpace;

        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }
        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }
        public MMAL_RECT_T Crop => crop;
        public MMAL_RATIONAL_T FrameRate => frameRate;
        public MMAL_RATIONAL_T Par => par;
        public int ColorSpace => colorSpace;

        public MMAL_VIDEO_FORMAT_T(int width, int height, MMAL_RECT_T crop, MMAL_RATIONAL_T frameRate,
                                    MMAL_RATIONAL_T par, int colorSpace)
        {
            this.width = width;
            this.height = height;
            this.crop = crop;
            this.frameRate = frameRate;
            this.par = par;
            this.colorSpace = colorSpace;
        }
        
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_AUDIO_FORMAT_T
    {
        private uint channels, sampleRate, bitsPerSample, blockAlign;

        public uint Channels => channels;
        public uint SampleRate => sampleRate;
        public uint BitsPerSample => bitsPerSample;
        public uint BlockAlign => blockAlign;

        public MMAL_AUDIO_FORMAT_T(uint channels, uint sampleRate, uint bitsPerSample, uint blockAlign)
        {
            this.channels = channels;
            this.sampleRate = sampleRate;
            this.bitsPerSample = bitsPerSample;
            this.blockAlign = blockAlign;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_SUBPICTURE_FORMAT_T
    {
        private uint xOffset, yOffset;

        public uint XOffset => xOffset;
        public uint YOffset => yOffset;

        public MMAL_SUBPICTURE_FORMAT_T(uint xOffset, uint yOffset)
        {
            this.xOffset = xOffset;
            this.yOffset = yOffset;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_ES_SPECIFIC_FORMAT_T
    {
        private MMAL_AUDIO_FORMAT_T audio;
        private MMAL_VIDEO_FORMAT_T video;
        private MMAL_SUBPICTURE_FORMAT_T subpicture;

        public MMAL_AUDIO_FORMAT_T Audio
        {
            get
            {
                return this.audio;
            }
            set
            {
                this.audio = value;
            }
        }
        public MMAL_VIDEO_FORMAT_T Video
        {
            get
            {
                return this.video;
            }
            set
            {
                this.video = value;
            }
        }        
        public MMAL_SUBPICTURE_FORMAT_T SubPicture
        {
            get
            {
                return this.subpicture;
            }
            set
            {
                this.subpicture = value;
            }
        }

        public MMAL_ES_SPECIFIC_FORMAT_T(MMAL_AUDIO_FORMAT_T audio, MMAL_VIDEO_FORMAT_T video, MMAL_SUBPICTURE_FORMAT_T subpicture)
        {
            this.audio = audio;
            this.video = video;
            this.subpicture = subpicture;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_ES_FORMAT_T
    {
        private MMALFormat.MMAL_ES_TYPE_T type;
        private int encoding, encodingVariant;
        private MMAL_ES_SPECIFIC_FORMAT_T* es;
        private int bitrate, flags, extraDataSize;
        private byte* extraData;

        public MMALFormat.MMAL_ES_TYPE_T Type => type;
        public int Encoding
        {
            get
            {
                return this.encoding;
            }
            set
            {
                this.encoding = value;
            }
        }
        public int EncodingVariant
        {
            get
            {
                return this.encodingVariant;
            }
            set
            {
                this.encodingVariant = value;
            }
        }
        public MMAL_ES_SPECIFIC_FORMAT_T* Es => es;
                
        public int Bitrate
        {
            get
            {
                return this.bitrate;
            }
            set
            {
                this.bitrate = value;
            }
        }

        public int Flags => flags;
        public int ExtraDataSize => extraDataSize;
        public byte* ExtraData => extraData;

        public MMAL_ES_FORMAT_T(MMALFormat.MMAL_ES_TYPE_T type, int encoding, int encodingVariant,
                                MMAL_ES_SPECIFIC_FORMAT_T* es, int bitrate, int flags, int extraDataSize,
                                byte* extraData)
        {
            this.type = type;
            this.encoding = encoding;
            this.encodingVariant = encodingVariant;
            this.es = es;
            this.bitrate = bitrate;
            this.flags = flags;
            this.extraDataSize = extraDataSize;
            this.extraData = extraData;
        }
    }




}
