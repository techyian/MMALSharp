// <copyright file="MMALPortConfig.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Port configuration class.
    /// </summary>
    public class MMALPortConfig
    {
        /// <summary>
        /// The encoding type this output port will send data in.
        /// </summary>
        public MMALEncoding EncodingType { get; set; }

        /// <summary>
        /// The pixel format this output port will send data in.
        /// </summary>
        public MMALEncoding PixelFormat { get; set; }
        
        /// <summary>
        /// User provided width of output frame.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// User provided height of output frame.
        /// </summary>
        public int Height { get; set; }
        
        /// <summary>
        /// The framerate of the outputted data.
        /// </summary>
        public int Framerate { get; set; }
        
        /// <summary>
        /// The quality of our outputted data. 
        /// </summary>
        public int Quality { get; set; }
        
        /// <summary>
        /// The bitrate we are sending data at.
        /// </summary>
        public int Bitrate { get; set; }
        
        /// <summary>
        /// Instruct MMAL to not copy buffers to ARM memory (useful for large buffers and handling raw data).
        /// </summary>
        public bool ZeroCopy { get; set; }
        
        /// <summary>
        /// Time that processing shall stop. Relevant for video recording.
        /// </summary>
        public DateTime? Timeout { get; set; }

        /// <summary>
        /// Create a new instance of <see cref="MMALPortConfig"/> with parameters useful for image capture.
        /// </summary>
        /// <param name="encodingType">The encoding type.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <param name="quality">The output quality. Only affects JPEG quality for image stills.</param>
        public MMALPortConfig(MMALEncoding encodingType, MMALEncoding pixelFormat, int quality)
        {
            this.EncodingType = encodingType;
            this.PixelFormat = pixelFormat;
            this.Quality = quality;
        }

        /// <summary>
        /// Create a new instance of <see cref="MMALPortConfig"/> with parameters useful for video capture.
        /// </summary>
        /// <param name="encodingType">The encoding type.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <param name="quality">The output quality. Affects the quantization parameter for H.264 encoding. Set bitrate 0 and set this for variable bitrate.</param>
        /// <param name="bitrate">The output bitrate.</param>
        /// <param name="timeout">Video record timeout. This is useful if you have multiple video recording streams which you want to stop at different times.</param>
        public MMALPortConfig(MMALEncoding encodingType, MMALEncoding pixelFormat, int quality, int bitrate, DateTime? timeout)
        {
            this.EncodingType = encodingType;
            this.PixelFormat = pixelFormat;
            this.Quality = quality;
            this.Bitrate = bitrate;
            this.Timeout = timeout;
        }

        /// <summary>
        /// Create a new instance of <see cref="MMALPortConfig"/> with parameters useful for standalone image/video processing.
        /// </summary>
        /// <param name="encodingType">The encoding type.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <param name="width">The output width.</param>
        /// <param name="height">The output height.</param>
        /// <param name="framerate">The output framerate.</param>
        /// <param name="quality">The output quality.</param>
        /// <param name="bitrate">The output bitrate.</param>
        /// <param name="zeroCopy">Specify zero copy.</param>
        /// <param name="timeout">Video record timeout.</param>
        public MMALPortConfig(MMALEncoding encodingType, MMALEncoding pixelFormat, int width, int height, int framerate,
                                int quality, int bitrate, bool zeroCopy, DateTime? timeout)
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
        }
    }
}
