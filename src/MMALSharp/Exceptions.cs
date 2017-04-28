using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp
{
    public class PiCameraError : Exception
    {
        public PiCameraError() : base() { }
        public PiCameraError(string msg) : base(msg) { }
    }

    public class MMALException : Exception
    {
        public MMALException(MMALUtil.MMAL_STATUS_T status, string prefix) : base(prefix)
        {
        }
    }

    public class MMALNoMemoryException : MMALException
    {
        public MMALNoMemoryException() : base(MMALUtil.MMAL_STATUS_T.MMAL_ENOMEM, "Out of memory")
        {
        }
    }

    public class MMALNoSpaceException : MMALException
    {
        public MMALNoSpaceException() : base(MMALUtil.MMAL_STATUS_T.MMAL_ENOSPC, "Out of resources")
        {
        }
    }

    public class MMALInvalidException : MMALException
    {
        public MMALInvalidException() : base(MMALUtil.MMAL_STATUS_T.MMAL_EINVAL, "Argument is invalid")
        {
        }
    }

    public class MMALNotImplementedException : MMALException
    {
        public MMALNotImplementedException() : base(MMALUtil.MMAL_STATUS_T.MMAL_ENOSYS, "Function not implemented")
        {
        }
    }

    public class MMALInvalidDirectoryException : MMALException
    {
        public MMALInvalidDirectoryException() : base(MMALUtil.MMAL_STATUS_T.MMAL_ENOENT, "No such file or directory")
        {
        }
    }

    public class MMALInvalidDeviceException : MMALException
    {
        public MMALInvalidDeviceException() : base(MMALUtil.MMAL_STATUS_T.MMAL_ENXIO, "No such device or address")
        {
        }
    }

    public class MMALIOException : MMALException
    {
        public MMALIOException() : base(MMALUtil.MMAL_STATUS_T.MMAL_EIO, "I/O error")
        {
        }
    }

    public class MMALIllegalSeekException : MMALException
    {
        public MMALIllegalSeekException() : base(MMALUtil.MMAL_STATUS_T.MMAL_ESPIPE, "Illegal seek")
        {
        }
    }

    public class MMALCorruptException : MMALException
    {
        public MMALCorruptException() : base(MMALUtil.MMAL_STATUS_T.MMAL_ECORRUPT, "Data is corrupt")
        {
        }
    }

    public class MMALComponentNotReadyException : MMALException
    {
        public MMALComponentNotReadyException() : base(MMALUtil.MMAL_STATUS_T.MMAL_ENOTREADY, "Component is not ready")
        {
        }
    }

    public class MMALComponentNotConfiguredException : MMALException
    {
        public MMALComponentNotConfiguredException() : base(MMALUtil.MMAL_STATUS_T.MMAL_ECONFIG, "Component is not configured")
        {
        }
    }

    public class MMALPortConnectedException : MMALException
    {
        public MMALPortConnectedException() : base(MMALUtil.MMAL_STATUS_T.MMAL_EISCONN, "Port is already connected")
        {
        }
    }

    public class MMALPortNotConnectedException : MMALException
    {
        public MMALPortNotConnectedException() : base(MMALUtil.MMAL_STATUS_T.MMAL_ENOTCONN, "Port is disconnected")
        {
        }
    }

    public class MMALResourceUnavailableException : MMALException
    {
        public MMALResourceUnavailableException() : base(MMALUtil.MMAL_STATUS_T.MMAL_EAGAIN, "Resource temporarily unavailable; try again later")
        {
        }
    }

    public class MMALBadAddressException : MMALException
    {
        public MMALBadAddressException() : base(MMALUtil.MMAL_STATUS_T.MMAL_EFAULT, "Bad address")
        {
        }
    }


    public static class MMALCallerHelper
    {
        public static void MMALCheck(MMALUtil.MMAL_STATUS_T status, string prefix)
        {
            if (status != MMALUtil.MMAL_STATUS_T.MMAL_SUCCESS)
            {
                switch (status)
                {
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENOMEM:
                        throw new MMALNoMemoryException();
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENOSPC:
                        throw new MMALNoSpaceException();
                    case MMALUtil.MMAL_STATUS_T.MMAL_EINVAL:
                        throw new MMALInvalidException();
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENOSYS:
                        throw new MMALNotImplementedException();
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENOENT:
                        throw new MMALInvalidDirectoryException();
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENXIO:
                        throw new MMALInvalidDeviceException();
                    case MMALUtil.MMAL_STATUS_T.MMAL_EIO:
                        throw new MMALIOException();
                    case MMALUtil.MMAL_STATUS_T.MMAL_ESPIPE:
                        throw new MMALIllegalSeekException();
                    case MMALUtil.MMAL_STATUS_T.MMAL_ECORRUPT:
                        throw new MMALCorruptException();
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENOTREADY:
                        throw new MMALComponentNotReadyException();
                    case MMALUtil.MMAL_STATUS_T.MMAL_ECONFIG:
                        throw new MMALComponentNotConfiguredException();
                    case MMALUtil.MMAL_STATUS_T.MMAL_EISCONN:
                        throw new MMALPortConnectedException();
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENOTCONN:
                        throw new MMALPortNotConnectedException();
                    case MMALUtil.MMAL_STATUS_T.MMAL_EAGAIN:
                        throw new MMALResourceUnavailableException();
                    case MMALUtil.MMAL_STATUS_T.MMAL_EFAULT:
                        throw new MMALBadAddressException();
                    default:
                        throw new MMALException(status, "Unknown error occurred");
                }


                throw new MMALException(status, prefix);
            }
        }
    }
    
}
