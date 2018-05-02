using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp
{
    public static class GenericExtensions
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0)
            {
                return min;
            }
            else if (val.CompareTo(max) > 0)
            {
                return max;
            }

            return val;
        }

        public static int FromByte(this byte val) => (int)Math.Floor(val >= 255 ? (double)1 : val * 255);
        public static byte ToByte(this float val) => (byte)Math.Floor(val >= 1.0 ? 255 : val * 255);
    }
}
