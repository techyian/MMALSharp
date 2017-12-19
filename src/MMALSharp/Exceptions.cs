using MMALSharp.Native;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;

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
        public MMALNoMemoryException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_ENOMEM, $"Out of memory. {prefix}")
        {
        }
    }

    public class MMALNoSpaceException : MMALException
    {
        public MMALNoSpaceException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_ENOSPC, $"Out of resources. {prefix}")
        {
        }
    }

    public class MMALInvalidException : MMALException
    {
        public MMALInvalidException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_EINVAL, $"Argument is invalid. {prefix}")
        {
        }
    }

    public class MMALNotImplementedException : MMALException
    {
        public MMALNotImplementedException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_ENOSYS, $"Function not implemented. {prefix}")
        {
        }
    }

    public class MMALInvalidDirectoryException : MMALException
    {
        public MMALInvalidDirectoryException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_ENOENT, $"No such file or directory. {prefix}")
        {
        }
    }

    public class MMALInvalidDeviceException : MMALException
    {
        public MMALInvalidDeviceException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_ENXIO, $"No such device or address. {prefix}")
        {
        }
    }

    public class MMALIOException : MMALException
    {
        public MMALIOException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_EIO, $"I/O error. {prefix}")
        {
        }
    }

    public class MMALIllegalSeekException : MMALException
    {
        public MMALIllegalSeekException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_ESPIPE, $"Illegal seek. {prefix}")
        {
        }
    }

    public class MMALCorruptException : MMALException
    {
        public MMALCorruptException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_ECORRUPT, $"Data is corrupt. {prefix}")
        {
        }
    }

    public class MMALComponentNotReadyException : MMALException
    {
        public MMALComponentNotReadyException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_ENOTREADY, $"Component is not ready. {prefix}")
        {
        }
    }

    public class MMALComponentNotConfiguredException : MMALException
    {
        public MMALComponentNotConfiguredException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_ECONFIG, $"Component is not configured. {prefix}")
        {
        }
    }

    public class MMALPortConnectedException : MMALException
    {
        public MMALPortConnectedException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_EISCONN, $"Port is already connected. {prefix}")
        {
        }
    }

    public class MMALPortNotConnectedException : MMALException
    {
        public MMALPortNotConnectedException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_ENOTCONN, $"Port is disconnected. {prefix}")
        {
        }
    }

    public class MMALResourceUnavailableException : MMALException
    {
        public MMALResourceUnavailableException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_EAGAIN, $"Resource temporarily unavailable; try again later. {prefix}")
        {
        }
    }

    public class MMALBadAddressException : MMALException
    {
        public MMALBadAddressException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_EFAULT, $"Bad address. {prefix}")
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
                        throw new MMALNoMemoryException(prefix);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENOSPC:
                        throw new MMALNoSpaceException(prefix);
                    case MMALUtil.MMAL_STATUS_T.MMAL_EINVAL:
                        throw new MMALInvalidException(prefix);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENOSYS:
                        throw new MMALNotImplementedException(prefix);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENOENT:
                        throw new MMALInvalidDirectoryException(prefix);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENXIO:
                        throw new MMALInvalidDeviceException(prefix);
                    case MMALUtil.MMAL_STATUS_T.MMAL_EIO:
                        throw new MMALIOException(prefix);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ESPIPE:
                        throw new MMALIllegalSeekException(prefix);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ECORRUPT:
                        throw new MMALCorruptException(prefix);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENOTREADY:
                        throw new MMALComponentNotReadyException(prefix);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ECONFIG:
                        throw new MMALComponentNotConfiguredException(prefix);
                    case MMALUtil.MMAL_STATUS_T.MMAL_EISCONN:
                        throw new MMALPortConnectedException(prefix);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENOTCONN:
                        throw new MMALPortNotConnectedException(prefix);
                    case MMALUtil.MMAL_STATUS_T.MMAL_EAGAIN:
                        throw new MMALResourceUnavailableException(prefix);
                    case MMALUtil.MMAL_STATUS_T.MMAL_EFAULT:
                        throw new MMALBadAddressException(prefix);
                    default:
                        throw new MMALException(status, "Unknown error occurred");
                }
            }
        }
    }
    
}
