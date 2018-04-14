// <copyright file="Exceptions.cs" company="Techyian">
// Copyright (c) Techyian. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Native;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;

namespace MMALSharp
{
    /// <summary>
    /// Represents unspecific errors that occur during working with the Pi Camera.
    /// </summary>
    public class PiCameraError : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PiCameraError"/> exception.
        /// </summary>
        public PiCameraError() : base() { }
        /// <summary>
        /// Creates a new instance of the <see cref="PiCameraError"/> exception with a specified message.
        /// </summary>
        /// <param name="msg">A message that describes the current error.</param>
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

    /// <summary>
    /// Provides methods which support calling native methods.
    /// </summary>
    public static class MMALCallerHelper
    {
        /// <summary>
        /// Checks whether the provided MMAL_STATUS_T is equal to MMAL_SUCCESS and throws the associated exception in case of an error.
        /// </summary>
        /// <param name="status">The MMAL_STATUS_T to search for an error.</param>
        /// <param name="message">The message for the exception that will be thrown if an error occurred.</param>
        public static void MMALCheck(MMALUtil.MMAL_STATUS_T status, string message)
        {
            if (status != MMALUtil.MMAL_STATUS_T.MMAL_SUCCESS)
            {
                switch (status)
                {
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENOMEM:
                        throw new MMALNoMemoryException(message);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENOSPC:
                        throw new MMALNoSpaceException(message);
                    case MMALUtil.MMAL_STATUS_T.MMAL_EINVAL:
                        throw new MMALInvalidException(message);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENOSYS:
                        throw new MMALNotImplementedException(message);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENOENT:
                        throw new MMALInvalidDirectoryException(message);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENXIO:
                        throw new MMALInvalidDeviceException(message);
                    case MMALUtil.MMAL_STATUS_T.MMAL_EIO:
                        throw new MMALIOException(message);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ESPIPE:
                        throw new MMALIllegalSeekException(message);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ECORRUPT:
                        throw new MMALCorruptException(message);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENOTREADY:
                        throw new MMALComponentNotReadyException(message);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ECONFIG:
                        throw new MMALComponentNotConfiguredException(message);
                    case MMALUtil.MMAL_STATUS_T.MMAL_EISCONN:
                        throw new MMALPortConnectedException(message);
                    case MMALUtil.MMAL_STATUS_T.MMAL_ENOTCONN:
                        throw new MMALPortNotConnectedException(message);
                    case MMALUtil.MMAL_STATUS_T.MMAL_EAGAIN:
                        throw new MMALResourceUnavailableException(message);
                    case MMALUtil.MMAL_STATUS_T.MMAL_EFAULT:
                        throw new MMALBadAddressException(message);
                    default:
                        throw new MMALException(status, $"Unknown error occurred. {message}");
                }
            }
        }
    }

}
