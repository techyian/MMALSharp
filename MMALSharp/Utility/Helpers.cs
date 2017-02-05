using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Utility
{
    public class Helpers
    {
        public static string ConvertBytesToMegabytes(long bytes)
        {
            return ((bytes / 1024f) / 1024f).ToString("0.00mb");
        }
    }
}
