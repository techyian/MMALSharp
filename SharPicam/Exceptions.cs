using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharPicam
{
    public class PiCameraError : Exception
    {
        public PiCameraError() : base() { }
        public PiCameraError(string msg) : base(msg) { }
    }

    public class MMALError : Exception
    {
        private MMALUtil.MMAL_STATUS_T Status { get; set; }
        private string Prefix { get; set; }

        public MMALError(MMALUtil.MMAL_STATUS_T status, string prefix)
        {
            this.Status = status;
            this.Prefix = prefix;
        }

        public override string ToString()
        {
            switch(this.Status)
            {
                case MMALUtil.MMAL_STATUS_T.MMAL_ENOMEM:
                    return Prefix + " : Out of memory";
                case MMALUtil.MMAL_STATUS_T.MMAL_ENOSPC:
                    return Prefix + " : Out of resources";
                case MMALUtil.MMAL_STATUS_T.MMAL_EINVAL:
                    return Prefix + " : Argument is invalid";
                case MMALUtil.MMAL_STATUS_T.MMAL_ENOSYS:
                    return Prefix + " : Function not implemented";
                case MMALUtil.MMAL_STATUS_T.MMAL_ENOENT:
                    return Prefix + " : No such file or directory";
                case MMALUtil.MMAL_STATUS_T.MMAL_ENXIO:
                    return Prefix + " : No such device or address";
                case MMALUtil.MMAL_STATUS_T.MMAL_EIO:
                    return Prefix + " : I/O error";
                case MMALUtil.MMAL_STATUS_T.MMAL_ESPIPE:
                    return Prefix + " : Illegal seek";
                case MMALUtil.MMAL_STATUS_T.MMAL_ECORRUPT:
                    return Prefix + " : Data is corrupt #FIXME not POSIX";
                case MMALUtil.MMAL_STATUS_T.MMAL_ENOTREADY:
                    return Prefix + " : Component is not ready #FIXME not POSIX";
                case MMALUtil.MMAL_STATUS_T.MMAL_ECONFIG:
                    return Prefix + " : Component is not configured #FIXME not POSIX";
                case MMALUtil.MMAL_STATUS_T.MMAL_EISCONN:
                    return Prefix + " : Port is already connected";
                case MMALUtil.MMAL_STATUS_T.MMAL_ENOTCONN:
                    return Prefix + " : Port is disconnected";
                case MMALUtil.MMAL_STATUS_T.MMAL_EAGAIN:
                    return Prefix + " : Resource temporarily unavailable; try again later";
                case MMALUtil.MMAL_STATUS_T.MMAL_EFAULT:
                    return Prefix + " : Bad address";
                default:
                    return Prefix + " : Unknown status error";
            }                        
        }
    }

    public static class MMALCallerHelper
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
