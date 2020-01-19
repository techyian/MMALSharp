// <copyright file="MMALRawcamPortConfig.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Native;
using System;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Represents a port configuration object for use with the Rawcam component.
    /// </summary>
    public class MMALRawcamPortConfig : MMALPortConfig
    {
        /// <summary>
        /// The physical camera interface type.
        /// </summary>
        public MMAL_CAMERA_INTERFACE_T CameraInterface { get; set; }

        /// <summary>
        /// Camera peripheral clocking mode.
        /// </summary>
        public MMAL_CAMERA_CLOCKING_MODE_T ClockingMode { get; set; }

        /// <summary>
        /// The receiver peripheral configuration for unpacking/packing DPCM, and decoding or encoding Bayer images.
        /// </summary>
        public MMALRawcamRxConfig RxConfig { get; set; }

        /// <summary>
        /// Camera peripheral timing registers.
        /// </summary>
        public MMALRawcamTimingConfig TimingConfig { get; set; }

        /// <summary>
        /// Create a new instance of <see cref="MMALRawcamPortConfig"/>.
        /// </summary>
        /// <param name="encodingType">The encoding type. Set this to specify the output format.  Colour format should be
        /// RGB565, RGB888, ABGR8888, YUV420 packed planar, YUV422 packed
        /// planar, or one of the flavours of YUYV.</param>
        /// <param name="pixelFormat">The pixel format.</param>        
        /// <param name="bitrate">The output bitrate.</param>
        /// <param name="timeout">Video record timeout.</param>
        /// <param name="cameraInterface">The physical camera interface type.</param>
        /// <param name="clockingMode">Camera peripheral clocking mode.</param>
        /// <param name="rxConfig">The receiver peripheral configuration for unpacking/packing DPCM, and decoding or encoding Bayer images.</param>        
        /// <param name="timingConfig">Camera peripheral timing registers.</param>
        public MMALRawcamPortConfig(MMALEncoding encodingType, MMALEncoding pixelFormat, int bitrate, DateTime? timeout,
                                MMAL_CAMERA_INTERFACE_T cameraInterface, MMAL_CAMERA_CLOCKING_MODE_T clockingMode, MMALRawcamRxConfig rxConfig,
                                MMALRawcamTimingConfig timingConfig)
            : base(encodingType, pixelFormat, 0, bitrate, timeout, null, false)
        {
            this.CameraInterface = cameraInterface;
            this.ClockingMode = clockingMode;
            this.RxConfig = rxConfig;
            this.TimingConfig = timingConfig;
        }
    }
}
