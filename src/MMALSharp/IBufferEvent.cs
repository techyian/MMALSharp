// <copyright file="IBufferEvent.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Native;

namespace MMALSharp
{
    /// <summary>
    /// Represents a buffer event format.
    /// </summary>
    public interface IBufferEvent
    {
        /// <summary>
        /// Native pointer that represents this event format.
        /// </summary>
        unsafe MMAL_ES_FORMAT_T* Ptr { get; }

        /// <summary>
        /// The FourCC code of the component.
        /// </summary>
        string FourCC { get; }

        /// <summary>
        /// The working bitrate of the component.
        /// </summary>
        int Bitrate { get; }

        /// <summary>
        /// The width value.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The height value.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// The CropX value.
        /// </summary>
        int CropX { get; }

        /// <summary>
        /// The CropY value.
        /// </summary>
        int CropY { get; }

        /// <summary>
        /// The crop width value.
        /// </summary>
        int CropWidth { get; }

        /// <summary>
        /// The crop height value.
        /// </summary>
        int CropHeight { get; }

        /// <summary>
        /// The pixel aspect ratio numerator value.
        /// </summary>
        int ParNum { get; }

        /// <summary>
        /// The pixel aspect ratio denominator value.
        /// </summary>
        int ParDen { get; }

        /// <summary>
        /// The framerate numerator value.
        /// </summary>
        int FramerateNum { get; }

        /// <summary>
        /// The framerate denominator value.
        /// </summary>
        int FramerateDen { get; }
    }
}
