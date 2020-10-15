// <copyright file="IMMALPortConfig.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using MMALSharp.Common;
using MMALSharp.Config;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Represents a port configuration object.
    /// </summary>
    public interface IMMALPortConfig
    {
        /// <summary>
        /// The encoding type this output port will send data in.
        /// </summary>
        MMALEncoding EncodingType { get; }

        /// <summary>
        /// The pixel format this output port will send data in.
        /// </summary>
        MMALEncoding PixelFormat { get; }

        /// <summary>
        /// User provided width of output frame.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// User provided height of output frame.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// The framerate of the outputted data.
        /// </summary>
        double Framerate { get; }

        /// <summary>
        /// The quality of our outputted data. 
        /// </summary>
        int Quality { get; }

        /// <summary>
        /// The bitrate we are sending data at.
        /// </summary>
        int Bitrate { get; }

        /// <summary>
        /// Instruct MMAL to not copy buffers to ARM memory (useful for large buffers and handling raw data).
        /// </summary>
        bool ZeroCopy { get; }

        /// <summary>
        /// Time that processing shall stop. Relevant for video recording.
        /// </summary>
        DateTime? Timeout { get; }

        /// <summary>
        /// Requested number of buffer headers.
        /// </summary>
        int BufferNum { get; }

        /// <summary>
        /// Requested size of buffer headers.
        /// </summary>
        int BufferSize { get; }

        /// <summary>
        /// The Region of Interest requested.
        /// </summary>
        Rectangle? Crop { get; }

        /// <summary>
        /// Video split configuration object.
        /// </summary>
        Split Split { get; }

        /// <summary>
        /// Indicates whether motion vector data should be stored to a separate output stream. Only applies to Video recording.
        /// </summary>
        bool StoreMotionVectors { get; }

        /// <summary>
        /// User provided name for port. Helps with debugging.
        /// </summary>
        string UserPortName { get; }
    }
}
