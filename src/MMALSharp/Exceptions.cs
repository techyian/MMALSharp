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

    /// <summary>
    /// Base class for all native error exception wrappers.
    /// </summary>
    public class MMALException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MMALException"/> class with the specified error code and message.
        /// This exception is the base class for all exception wrappers.
        /// </summary>
        /// <param name="status">The native status code.</param>
        /// <param name="message">The error message to print.</param>
        public MMALException(MMALUtil.MMAL_STATUS_T status, string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Native error corresponding to an <see cref="OutOfMemoryException"/>.
    /// </summary>
    public class MMALNoMemoryException : MMALException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MMALNoMemoryException"/> class with the specified message.
        /// This exception is similar to an <see cref="OutOfMemoryException"/>.
        /// </summary>
        /// <param name="message">The error message to print.</param>
        public MMALNoMemoryException(string message) : base(MMALUtil.MMAL_STATUS_T.MMAL_ENOMEM, $"Out of memory. {message}")
        {
        }
    }

    /// <summary>
    /// Native error that occurs when running out of resources other than memory.
    /// </summary>
    public class MMALNoSpaceException : MMALException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MMALNoSpaceException"/> class with the specified message.
        /// This exception indicates running out of resources other than memory.
        /// </summary>
        /// <param name="prefix">The error message to print.</param>
        public MMALNoSpaceException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_ENOSPC, $"Out of resources. {prefix}")
        {
        }
    }

    /// <summary>
    /// Native error corresponding to an <see cref="ArgumentException"/>.
    /// </summary>
    public class MMALInvalidException : MMALException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MMALInvalidException"/> class with the specified message.
        /// This exception is similar to an <see cref="ArgumentException"/>.
        /// </summary>
        /// <param name="prefix">The error message to print.</param>
        public MMALInvalidException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_EINVAL, $"Argument is invalid. {prefix}")
        {
        }
    }

    /// <summary>
    /// Native error corresponding to a <see cref="NotImplementedException"/>.
    /// </summary>
    public class MMALNotImplementedException : MMALException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MMALNotImplementedException"/> class with the specified message.
        /// This exception is similar to a <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="prefix">The error message to print.</param>
        public MMALNotImplementedException(string prefix) : base(MMALUtil.MMAL_STATUS_T.MMAL_ENOSYS, $"Function not implemented. {prefix}")
        {
        }
    }

    /// <summary>
    /// Native error corresponding to a <see cref="System.IO.FileNotFoundException"/> or <see cref="System.IO.DirectoryNotFoundException"/>.
    /// </summary>
    public class MMALInvalidDirectoryException : MMALException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MMALInvalidDeviceException"/> class with the specified message.
        /// This exception is similar to a <see cref="System.IO.FileNotFoundException"/> or <see cref="System.IO.DirectoryNotFoundException"/>.
        /// </summary>
        /// <param name="prefix">The error message to print.</param>
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

    /// <summary>
    /// Native error corresponding to an <see cref="System.IO.IOException"/>.
    /// </summary>
    public class MMALIOException : MMALException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MMALIOException"/> class with the specified message.
        /// This exception is similar to an <see cref="System.IO.IOException"/>.
        /// </summary>
        /// <param name="prefix">The error message to print.</param>
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
