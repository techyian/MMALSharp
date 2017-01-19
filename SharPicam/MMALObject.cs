using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharPicam
{
    public class MMALObject
    {
        public static List<WeakReference> Objects = new List<WeakReference>();

        public MMALObject()
        {
            Objects.Add(new WeakReference(this));
        }

    }
}
