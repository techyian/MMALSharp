// <copyright file="MMALEncodings.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MMALSharp.Common.Utility;

namespace MMALSharp.Common
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class MMALEncodingHelpers
    {
        /// <summary>
        /// Gets a list of all supported encodings.
        /// Call <see cref="PortExtensions.GetSupportedEncodings"/> to get supported encodings for a specific port.
        /// </summary>
        public static IReadOnlyCollection<MMALEncoding> EncodingList { get; } = new ReadOnlyCollection<MMALEncoding>(new List<MMALEncoding>
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
            MMALEncoding.YUV10COL,
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
            MMALEncoding.MMAL_COLOR_SPACE_JFIF_Y16_255,
            MMALEncoding.MMAL_COLOR_SPACE_REC2020
        });

        /// <summary>
        /// Parses an integer encoding value to an MMALEncoding object.
        /// </summary>
        /// <param name="encodingType">The encoding type value.</param>
        /// <returns>The MMALEncoding object</returns>
        public static MMALEncoding ParseEncoding(this int encodingType)
        {
            return EncodingList.FirstOrDefault(c => c.EncodingVal == encodingType);
        }

        /// <summary>
        /// Parses a string encoding name to an MMALEncoding object.
        /// </summary>
        /// <param name="encodingName">The encoding type name.</param>
        /// <returns>The MMALEncoding object</returns>
        public static MMALEncoding ParseEncoding(this string encodingName)
        {
            return EncodingList.FirstOrDefault(c => c.EncodingName.TrimEnd().Equals(encodingName, StringComparison.InvariantCultureIgnoreCase));
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
            ColorSpace,
            Internal
        }

        public int EncodingVal { get; }

        public string EncodingName { get; }

        public EncodingType EncType { get; }

        public override string ToString()
        {
            var type = string.Empty;
            
            switch (this.EncType)
            {
                case EncodingType.Audio:
                    type = "Audio";
                    break;
                case EncodingType.Image:
                    type = "Image";
                    break;
                case EncodingType.Internal:
                    type = "Internal";
                    break;
                case EncodingType.PixelFormat:
                    type = "Pixel Format";
                    break;
                case EncodingType.Video:
                    type = "Video";
                    break;
            }
            
            return $"Name: {this.EncodingName}. FourCC: {this.EncodingVal}. Type: {type}";
        }
            
        private MMALEncoding(string s, EncodingType type)
        {
            this.EncodingVal = Helpers.FourCCFromString(s);
            this.EncodingName = s;
            this.EncType = type;
        }

        private MMALEncoding(int val, string name, EncodingType type)
        {
            this.EncodingVal = val;
            this.EncodingName = name;
            this.EncType = type;
        }

        public static readonly MMALEncoding H264 = new MMALEncoding("H264", EncodingType.Video);
        public static readonly MMALEncoding MVC = new MMALEncoding("MVC ", EncodingType.Video);
        public static readonly MMALEncoding H263 = new MMALEncoding("H263", EncodingType.Video);
        public static readonly MMALEncoding MP4V = new MMALEncoding("MP4V", EncodingType.Video);
        public static readonly MMALEncoding MP2V = new MMALEncoding("MP2V", EncodingType.Video);
        public static readonly MMALEncoding MP1V = new MMALEncoding("MP1V", EncodingType.Video);
        public static readonly MMALEncoding WMV3 = new MMALEncoding("WMV3", EncodingType.Video);
        public static readonly MMALEncoding WMV2 = new MMALEncoding("WMV2", EncodingType.Video);
        public static readonly MMALEncoding WMV1 = new MMALEncoding("WMV1", EncodingType.Video);
        public static readonly MMALEncoding WVC1 = new MMALEncoding("WVC1", EncodingType.Video);
        public static readonly MMALEncoding VP8 = new MMALEncoding("VP8 ", EncodingType.Video);
        public static readonly MMALEncoding VP7 = new MMALEncoding("VP7 ", EncodingType.Video);
        public static readonly MMALEncoding VP6 = new MMALEncoding("VP6 ", EncodingType.Video);
        public static readonly MMALEncoding THEORA = new MMALEncoding("THEO", EncodingType.Video);
        public static readonly MMALEncoding SPARK = new MMALEncoding("SPRK", EncodingType.Video);
        public static readonly MMALEncoding MJPEG = new MMALEncoding("MJPG", EncodingType.Video);
        public static readonly MMALEncoding JPEG = new MMALEncoding("JPEG", EncodingType.Image);
        public static readonly MMALEncoding GIF = new MMALEncoding("GIF ", EncodingType.Image);
        public static readonly MMALEncoding PNG = new MMALEncoding("PNG ", EncodingType.Image);
        public static readonly MMALEncoding PPM = new MMALEncoding("PPM ", EncodingType.Image);
        public static readonly MMALEncoding TGA = new MMALEncoding("TGA ", EncodingType.Image);
        public static readonly MMALEncoding BMP = new MMALEncoding("BMP ", EncodingType.Image);
        public static readonly MMALEncoding I420 = new MMALEncoding("I420", EncodingType.PixelFormat);
        public static readonly MMALEncoding I420_SLICE = new MMALEncoding("S420", EncodingType.PixelFormat);
        public static readonly MMALEncoding YV12 = new MMALEncoding("YV12", EncodingType.PixelFormat);
        public static readonly MMALEncoding I422 = new MMALEncoding("I422", EncodingType.PixelFormat);
        public static readonly MMALEncoding I422_SLICE = new MMALEncoding("S422", EncodingType.PixelFormat);
        public static readonly MMALEncoding YUYV = new MMALEncoding("YUYV", EncodingType.PixelFormat);
        public static readonly MMALEncoding YVYU = new MMALEncoding("YVYU", EncodingType.PixelFormat);
        public static readonly MMALEncoding UYVY = new MMALEncoding("UYVY", EncodingType.PixelFormat);
        public static readonly MMALEncoding VYUY = new MMALEncoding("VYUY", EncodingType.PixelFormat);
        public static readonly MMALEncoding NV12 = new MMALEncoding("NV12", EncodingType.PixelFormat);
        public static readonly MMALEncoding NV21 = new MMALEncoding("NV21", EncodingType.PixelFormat);
        public static readonly MMALEncoding ARGB = new MMALEncoding("ARGB", EncodingType.PixelFormat);
        public static readonly MMALEncoding RGBA = new MMALEncoding("RGBA", EncodingType.PixelFormat);
        public static readonly MMALEncoding ABGR = new MMALEncoding("ABGR", EncodingType.PixelFormat);
        public static readonly MMALEncoding BGRA = new MMALEncoding("BGRA", EncodingType.PixelFormat);
        public static readonly MMALEncoding RGB16 = new MMALEncoding("RGB2", EncodingType.PixelFormat);
        public static readonly MMALEncoding RGB24 = new MMALEncoding("RGB3", EncodingType.PixelFormat);
        public static readonly MMALEncoding RGB32 = new MMALEncoding("RGB4", EncodingType.PixelFormat);
        public static readonly MMALEncoding BGR16 = new MMALEncoding("BGR2", EncodingType.PixelFormat);
        public static readonly MMALEncoding BGR24 = new MMALEncoding("BGR3", EncodingType.PixelFormat);
        public static readonly MMALEncoding BGR32 = new MMALEncoding("BGR4", EncodingType.PixelFormat);
        public static readonly MMALEncoding BAYER_SBGGR10P = new MMALEncoding("pBAA", EncodingType.PixelFormat);
        public static readonly MMALEncoding BAYER_SBGGR8 = new MMALEncoding("BA81", EncodingType.PixelFormat);
        public static readonly MMALEncoding BAYER_SBGGR12P = new MMALEncoding("BY12", EncodingType.PixelFormat);
        public static readonly MMALEncoding BAYER_SBGGR16 = new MMALEncoding("BYR2", EncodingType.PixelFormat);
        public static readonly MMALEncoding BAYER_SBGGR10DPCM8 = new MMALEncoding("bBA8", EncodingType.PixelFormat);
        public static readonly MMALEncoding YUVUV128 = new MMALEncoding("SAND", EncodingType.PixelFormat);
        public static readonly MMALEncoding YUV10COL = new MMALEncoding("Y10C", EncodingType.PixelFormat);

        /// <summary>
        /// An opaque buffer is a Broadcom specific format that references a GPU internal bitmap. It is typed as <see cref="EncodingType.Internal"/>.
        /// </summary>
        /// <remarks>https://www.raspberrypi.org/forums/viewtopic.php?t=53698</remarks>
        public static readonly MMALEncoding OPAQUE = new MMALEncoding("OPQV", EncodingType.Internal);
        public static readonly MMALEncoding EGL_IMAGE = new MMALEncoding("EGLI", EncodingType.PixelFormat);
        public static readonly MMALEncoding PCM_UNSIGNED_BE = new MMALEncoding("PCMU", EncodingType.PixelFormat);
        public static readonly MMALEncoding PCM_UNSIGNED_LE = new MMALEncoding("pcmu", EncodingType.PixelFormat);
        public static readonly MMALEncoding PCM_SIGNED_BE = new MMALEncoding("PCMS", EncodingType.PixelFormat);
        public static readonly MMALEncoding PCM_SIGNED_LE = new MMALEncoding("pcms", EncodingType.PixelFormat);
        public static readonly MMALEncoding PCM_FLOAT_BE = new MMALEncoding("PCMF", EncodingType.PixelFormat);
        public static readonly MMALEncoding PCM_FLOAT_LE = new MMALEncoding("pcmf", EncodingType.PixelFormat);
        public static readonly MMALEncoding PCM_UNSIGNED = PCM_UNSIGNED_LE;
        public static readonly MMALEncoding PCM_SIGNED = PCM_SIGNED_LE;
        public static readonly MMALEncoding PCM_FLOAT = PCM_FLOAT_LE;
        public static readonly MMALEncoding MP4A = new MMALEncoding("MP4A", EncodingType.Audio);
        public static readonly MMALEncoding MPGA = new MMALEncoding("MPGA", EncodingType.Audio);
        public static readonly MMALEncoding ALAW = new MMALEncoding("ALAW", EncodingType.Audio);
        public static readonly MMALEncoding MULAW = new MMALEncoding("ULAW", EncodingType.Audio);
        public static readonly MMALEncoding ADPCM_MS = new MMALEncoding("MS\x00\x02", EncodingType.Audio);
        public static readonly MMALEncoding ADPCM_IMA_MS = new MMALEncoding("MS\x00\x01", EncodingType.Audio);
        public static readonly MMALEncoding ADPCM_SWF = new MMALEncoding("ASWF", EncodingType.Audio);
        public static readonly MMALEncoding WMA1 = new MMALEncoding("WMA1", EncodingType.Audio);
        public static readonly MMALEncoding WMA2 = new MMALEncoding("WMA2", EncodingType.Audio);
        public static readonly MMALEncoding WMAP = new MMALEncoding("WMAP", EncodingType.Audio);
        public static readonly MMALEncoding WMAL = new MMALEncoding("WMAL", EncodingType.Audio);
        public static readonly MMALEncoding WMAV = new MMALEncoding("WMAV", EncodingType.Audio);
        public static readonly MMALEncoding AMRNB = new MMALEncoding("AMRN", EncodingType.Audio);
        public static readonly MMALEncoding AMRWB = new MMALEncoding("AMRW", EncodingType.Audio);
        public static readonly MMALEncoding AMRWBP = new MMALEncoding("AMRP", EncodingType.Audio);
        public static readonly MMALEncoding AC3 = new MMALEncoding("AC3 ", EncodingType.Audio);
        public static readonly MMALEncoding EAC3 = new MMALEncoding("EAC3", EncodingType.Audio);
        public static readonly MMALEncoding DTS = new MMALEncoding("DTS ", EncodingType.Audio);
        public static readonly MMALEncoding MLP = new MMALEncoding("MLP ", EncodingType.Audio);
        public static readonly MMALEncoding FLAC = new MMALEncoding("FLAC", EncodingType.Audio);
        public static readonly MMALEncoding VORBIS = new MMALEncoding("VORB", EncodingType.Audio);
        public static readonly MMALEncoding SPEEX = new MMALEncoding("SPX ", EncodingType.Audio);
        public static readonly MMALEncoding ATRAC3 = new MMALEncoding("ATR3", EncodingType.Audio);
        public static readonly MMALEncoding ATRACX = new MMALEncoding("ATRX", EncodingType.Audio);
        public static readonly MMALEncoding ATRACL = new MMALEncoding("ATRL", EncodingType.Audio);
        public static readonly MMALEncoding MIDI = new MMALEncoding("MIDI", EncodingType.Audio);
        public static readonly MMALEncoding EVRC = new MMALEncoding("EVRC", EncodingType.Audio);
        public static readonly MMALEncoding NELLYMOSER = new MMALEncoding("NELY", EncodingType.Audio);
        public static readonly MMALEncoding QCELP = new MMALEncoding("QCEL", EncodingType.Audio);
        public static readonly MMALEncoding MP4V_DIVX_DRM = new MMALEncoding("M4VD", EncodingType.Video);
        public static readonly MMALEncoding VARIANT_H264_DEFAULT = new MMALEncoding(0, "VARIANT_H264_DEFAULT", EncodingType.Video);
        public static readonly MMALEncoding VARIANT_H264_AVC1 = new MMALEncoding("AVC1", EncodingType.Video);
        public static readonly MMALEncoding VARIANT_H264_RAW = new MMALEncoding("RAW ", EncodingType.Video);
        public static readonly MMALEncoding VARIANT_MP4A_DEFAULT = new MMALEncoding(0, "VARIANT_MP4A_DEFAULT", EncodingType.Video);
        public static readonly MMALEncoding VARIANT_MP4A_ADTS = new MMALEncoding("ADTS", EncodingType.Video);
        public static readonly MMALEncoding MMAL_COLOR_SPACE_UNKNOWN = new MMALEncoding(0, "MMAL_COLOR_SPACE_UNKNOWN", EncodingType.ColorSpace);
        public static readonly MMALEncoding MMAL_COLOR_SPACE_ITUR_BT601 = new MMALEncoding("Y601", EncodingType.ColorSpace);
        public static readonly MMALEncoding MMAL_COLOR_SPACE_ITUR_BT709 = new MMALEncoding("Y709", EncodingType.ColorSpace);
        public static readonly MMALEncoding MMAL_COLOR_SPACE_JPEG_JFIF = new MMALEncoding("YJFI", EncodingType.ColorSpace);
        public static readonly MMALEncoding MMAL_COLOR_SPACE_FCC = new MMALEncoding("YFCC", EncodingType.ColorSpace);
        public static readonly MMALEncoding MMAL_COLOR_SPACE_SMPTE240M = new MMALEncoding("Y240", EncodingType.ColorSpace);
        public static readonly MMALEncoding MMAL_COLOR_SPACE_BT470_2_M = new MMALEncoding("Y__M", EncodingType.ColorSpace);
        public static readonly MMALEncoding MMAL_COLOR_SPACE_BT470_2_BG = new MMALEncoding("Y_BG", EncodingType.ColorSpace);
        public static readonly MMALEncoding MMAL_COLOR_SPACE_JFIF_Y16_255 = new MMALEncoding("YY16", EncodingType.ColorSpace);
        public static readonly MMALEncoding MMAL_COLOR_SPACE_REC2020 = new MMALEncoding("2020", EncodingType.ColorSpace);
    }
}
