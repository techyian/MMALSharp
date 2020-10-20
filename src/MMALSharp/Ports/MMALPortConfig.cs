// <copyright file="MMALPortConfig.cs" company="Techyian">
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
    /// Port configuration class.
    /// </summary>
    public class MMALPortConfig : IMMALPortConfig
    {
        /// <summary>
        /// The encoding type this output port will send data in.
        /// </summary>
        public MMALEncoding EncodingType { get; }

        /// <summary>
        /// The pixel format this output port will send data in.
        /// </summary>
        public MMALEncoding PixelFormat { get; }

        /// <summary>
        /// The input/output width value.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The input/output height value.
        /// </summary>
        public int Height { get; }
        
        /// <summary>
        /// The framerate of the outputted data.
        /// </summary>
        public double Framerate { get; }

        /// <summary>
        /// The quality value. Can be used with JPEG encoding (value between 1-100). Can be used with H.264 encoding which affects the quantization parameter (typical values between 10-40, see wiki for info). Set both bitrate param and quality param to 0 for variable bitrate.
        /// </summary>
        public int Quality { get; }
        
        /// <summary>
        /// The bitrate we are sending data at.
        /// </summary>
        public int Bitrate { get; }
        
        /// <summary>
        /// Instruct MMAL to not copy buffers to ARM memory (useful for large buffers and handling raw data).
        /// </summary>
        public bool ZeroCopy { get; }
        
        /// <summary>
        /// Time that processing shall stop. Relevant for video recording.
        /// </summary>
        public DateTime? Timeout { get; }

        /// <summary>
        /// Requested number of buffer headers.
        /// </summary>
        public int BufferNum { get; }

        /// <summary>
        /// Requested size of buffer headers.
        /// </summary>
        public int BufferSize { get; }

        /// <summary>
        /// The Region of Interest requested.
        /// </summary>
        public Rectangle? Crop { get; }
        
        /// <summary>
        /// Video split configuration object.
        /// </summary>
        public Split Split { get; }

        /// <summary>
        /// Indicates whether motion vector data should be stored to a separate output stream. Only applies to Video recording.
        /// </summary>
        public bool StoreMotionVectors { get; }

        /// <summary>
        /// User provided name for port. Helps with debugging.
        /// </summary>
        public string UserPortName { get; set; }

        /// <summary>
        /// Create a new instance of <see cref="MMALPortConfig"/>.
        /// </summary>
        /// <param name="encodingType">The encoding type.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <param name="quality">The quality value. Can be used with JPEG encoding (value between 1-100). Can be used with H.264 encoding which affects the quantization parameter (typical values between 10-40, see wiki for info). Set both bitrate param and quality param to 0 for variable bitrate.</param>
        /// <param name="bitrate">The working bitrate, applies to Video Encoder only.</param>
        /// <param name="timeout">Video record timeout. This is useful if you have multiple video recording streams which you want to stop at different times.</param>
        /// <param name="split">Video split configuration object.</param>
        /// <param name="storeMotionVectors">Indicates whether to store motion vectors. Applies to H.264 video encoding.</param>
        /// <param name="width">The input/output width value.</param>
        /// <param name="height">The input/output height value.</param>
        /// <param name="framerate">Framerate value. Only useful when not using the camera component to specify input framerate.</param>
        /// <param name="zeroCopy">Instruct MMAL to not copy buffers to ARM memory (useful for large buffers and handling raw data).</param>
        /// <param name="bufferNum">Requested number of buffer headers.</param>
        /// <param name="bufferSize">Requested size of buffer headers.</param>
        /// <param name="crop">The Region of Interest requested.</param>
        /// <param name="userPortName">User provided name for port. Helps with debugging..</param>
        public MMALPortConfig(
            MMALEncoding encodingType, 
            MMALEncoding pixelFormat, 
            int quality = 0, 
            int bitrate = 0, 
            DateTime? timeout = null, 
            Split split = null, 
            bool storeMotionVectors = false,
            int width = 0, 
            int height = 0,
            double framerate = 0,
            bool zeroCopy = false,
            int bufferNum = 0, 
            int bufferSize = 0, 
            Rectangle? crop = null,
            string userPortName = "")
        {
            this.EncodingType = encodingType;
            this.PixelFormat = pixelFormat;
            this.Width = width;
            this.Height = height;
            this.Framerate = framerate;
            this.Quality = quality;
            this.Bitrate = bitrate;
            this.ZeroCopy = zeroCopy;
            this.Timeout = timeout;
            this.Split = split;
            this.BufferNum = bufferNum;
            this.BufferSize = bufferSize;
            this.Crop = crop;
            this.StoreMotionVectors = storeMotionVectors;
            this.UserPortName = userPortName;
        }
    }
}
