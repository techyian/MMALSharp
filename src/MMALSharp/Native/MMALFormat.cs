// <copyright file="MMALFormat.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace MMALSharp.Native
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1132 // Each field should be declared on its own line

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

        public static int MMAL_ES_FORMAT_FLAG_FRAMED = 0x1;

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

#pragma warning disable IDE1006 // Naming Styles
        [DllImport("libmmal.so", EntryPoint = "mmal_format_alloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMAL_ES_FORMAT_T* mmal_format_alloc();

        [DllImport("libmmal.so", EntryPoint = "mmal_format_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void mmal_format_free(MMAL_ES_FORMAT_T* format);

        [DllImport("libmmal.so", EntryPoint = "mmal_format_extradata_alloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_format_extradata_alloc(MMAL_ES_FORMAT_T* format, uint extradata_size);

        [DllImport("libmmal.so", EntryPoint = "mmal_format_copy", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void mmal_format_copy(MMAL_ES_FORMAT_T* fmt_dst, MMAL_ES_FORMAT_T* fmt_src);

        [DllImport("libmmal.so", EntryPoint = "mmal_format_full_copy", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MMALUtil.MMAL_STATUS_T mmal_format_full_copy(MMAL_ES_FORMAT_T* fmt_dst, MMAL_ES_FORMAT_T* fmt_src);

        [DllImport("libmmal.so", EntryPoint = "mmal_format_compare", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe uint mmal_format_compare(MMAL_ES_FORMAT_T* ptr, MMAL_ES_FORMAT_T* ptr2);
#pragma warning restore IDE1006 // Naming Styles
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_VIDEO_FORMAT_T
    {
        public int Width, Height;
        public MMAL_RECT_T Crop;
        public MMAL_RATIONAL_T FrameRate, Par;
        public int ColorSpace;

        public MMAL_VIDEO_FORMAT_T(int width, int height, MMAL_RECT_T crop, MMAL_RATIONAL_T frameRate,
                                    MMAL_RATIONAL_T par, int colorSpace)
        {
            this.Width = width;
            this.Height = height;
            this.Crop = crop;
            this.FrameRate = frameRate;
            this.Par = par;
            this.ColorSpace = colorSpace;
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

    // Union type.
    [StructLayout(LayoutKind.Explicit)]
    public struct MMAL_ES_SPECIFIC_FORMAT_T
    {
        [FieldOffset(0)]
        public MMAL_AUDIO_FORMAT_T Audio;
        [FieldOffset(0)]
        public MMAL_VIDEO_FORMAT_T Video;
        [FieldOffset(0)]
        public MMAL_SUBPICTURE_FORMAT_T Subpicture;

        public MMAL_ES_SPECIFIC_FORMAT_T(MMAL_AUDIO_FORMAT_T audio, MMAL_VIDEO_FORMAT_T video, MMAL_SUBPICTURE_FORMAT_T subpicture)
        {
            this.Audio = audio;
            this.Video = video;
            this.Subpicture = subpicture;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MMAL_ES_FORMAT_T
    {
        public MMALFormat.MMAL_ES_TYPE_T Type;
        public int Encoding, EncodingVariant;
        public MMAL_ES_SPECIFIC_FORMAT_T* Es;
        public int Bitrate, Flags, ExtraDataSize;

        // byte*
        public IntPtr ExtraData;

        public MMAL_ES_FORMAT_T(MMALFormat.MMAL_ES_TYPE_T type, int encoding, int encodingVariant,
                                MMAL_ES_SPECIFIC_FORMAT_T* es, int bitrate, int flags, int extraDataSize,
                                IntPtr extraData)
        {
            this.Type = type;
            this.Encoding = encoding;
            this.EncodingVariant = encodingVariant;
            this.Es = es;
            this.Bitrate = bitrate;
            this.Flags = flags;
            this.ExtraDataSize = extraDataSize;
            this.ExtraData = extraData;
        }
    }
}