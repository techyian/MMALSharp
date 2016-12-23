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
        public static void MMALCheck(MMALUtil.MMAL_STATUS_T status, string prefix)
        {
            if (status != MMALUtil.MMAL_STATUS_T.MMAL_SUCCESS)
            {
                throw new MMALError(status, prefix);
            }
        }

    }
}
