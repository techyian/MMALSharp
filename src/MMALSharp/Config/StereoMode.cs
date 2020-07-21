// <copyright file="StereoMode.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Native;

namespace MMALSharp.Config
{
    /// <summary>
    /// The Stereoscopic mode code has mainly been added for completeness.
    /// It requires a Raspberry Pi Compute Module with two cameras connected.
    /// This functionality has not been tested.
    /// </summary>
    public class StereoMode
    {
        /// <summary>
        /// Gets or sets the stereoscopic mode.
        /// </summary>
        public MMAL_STEREOSCOPIC_MODE_T Mode { get; set; } = MMAL_STEREOSCOPIC_MODE_T.MMAL_STEREOSCOPIC_MODE_NONE;

        /// <summary>
        /// Gets or sets a value indicating whether to half the width and height of a stereoscopic image.
        /// </summary>
        /// <remarks>https://github.com/raspberrypi/userland/blob/master/host_applications/linux/apps/raspicam/RaspiCamControl.c#L204</remarks>
        public int Decimate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating a swap of camera order for stereoscopic mode.
        /// </summary>
        /// <remarks>https://github.com/raspberrypi/userland/blob/master/host_applications/linux/apps/raspicam/RaspiCamControl.c#L205</remarks>
        public int SwapEyes { get; set; }
    }
}
