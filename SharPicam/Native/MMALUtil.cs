using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharPicam.Native
{
    public static class MMALUtil
    {
        public static uint VCOS_ALIGN_UP(uint value, uint roundTo)
        {
            return (value + (roundTo - 1u) & ~(roundTo - 1u));
        }

        public static uint MMAL_FOURCC(string s)
        {
            int a1 = s[0];
            int b1 = s[1];
            int c1 = s[2];
            int d1 = s[3];
            return (uint) (((a1) | (b1 << 8) | (c1 << 16) | (d1 << 24)));
        }

        public enum MMAL_STATUS_T
        {
            MMAL_SUCCESS,
            MMAL_ENOMEM,
            MMAL_ENOSPC,
            MMAL_EINVAL,
            MMAL_ENOSYS,
            MMAL_ENOENT,
            MMAL_ENXIO,
            MMAL_EIO,
            MMAL_ESPIPE,
            MMAL_ECORRUPT,
            MMAL_ENOTREADY,
            MMAL_ECONFIG,
            MMAL_EISCONN,
            MMAL_ENOTCONN,
            MMAL_EAGAIN,
            MMAL_EFAULT,
            MMAL_STATUS_MAX = 0x7FFFFFFF
        }        
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_RECT_T
    {
        public uint x, y, height, width;

        public MMAL_RECT_T(uint x, uint y, uint height, uint width)
        {
            this.x = x;
            this.y = y;
            this.height = height;
            this.width = width;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMAL_RATIONAL_T
    {
        public uint num, den;

        public MMAL_RATIONAL_T(uint num, uint den)
        {
            this.num = num;
            this.den = den;
        }
    }
}
