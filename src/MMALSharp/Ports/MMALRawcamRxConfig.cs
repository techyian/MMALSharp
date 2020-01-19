// <copyright file="MMALRawcamRxConfig.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Native;

namespace MMALSharp.Ports
{
    /// <summary>
    /// The receiver peripheral configuration for unpacking/packing DPCM, and decoding or encoding Bayer images.
    /// </summary>
    public class MMALRawcamRxConfig
    {
        /// <summary>
        /// Bayer decoding value.
        /// </summary>
        public MMAL_CAMERA_RX_CONFIG_DECODE DecodeConfig { get; set; }

        /// <summary>
        /// Bayer encoding value.
        /// </summary>
        public MMAL_CAMERA_RX_CONFIG_ENCODE EncodeConfig { get; set; }

        /// <summary>
        /// DPCM unpacking value.
        /// </summary>
        public MMAL_CAMERA_RX_CONFIG_UNPACK UnpackConfig { get; set; }

        /// <summary>
        /// DPCM packing value.
        /// </summary>
        public MMAL_CAMERA_RX_CONFIG_PACK PackConfig { get; set; }

        /// <summary>
        /// Unsure.
        /// </summary>
        public int DataLanes { get; set; }

        /// <summary>
        /// Unsure.
        /// </summary>
        public int EncodeBlockLength { get; set; }

        /// <summary>
        /// Unsure.
        /// </summary>
        public int EmbeddedDataLines { get; set; }

        /// <summary>
        /// Unsure.
        /// </summary>
        public int ImageId { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="MMALRawcamRxConfig"/> for use with the Rawcam component port configuration.
        /// </summary>
        /// <param name="decodeConfig">Bayer decoding value.</param>
        /// <param name="encodeConfig">Bayer encoding value.</param>
        /// <param name="unpackConfig">DPCM unpacking value.</param>
        /// <param name="packConfig">DPCM packing value.</param>
        /// <param name="dataLanes">Data lanes value - unsure.</param>
        /// <param name="encodeBlockLength">Encode block length value - Unsure.</param>
        /// <param name="embeddedDataLanes">Embedded data lanes value - Unsure.</param>
        /// <param name="imageId">Image ID value - Unsure.</param>
        public MMALRawcamRxConfig(MMAL_CAMERA_RX_CONFIG_DECODE decodeConfig, 
                                  MMAL_CAMERA_RX_CONFIG_ENCODE encodeConfig, 
                                  MMAL_CAMERA_RX_CONFIG_UNPACK unpackConfig,
                                  MMAL_CAMERA_RX_CONFIG_PACK packConfig,
                                  int dataLanes,
                                  int encodeBlockLength,
                                  int embeddedDataLanes,
                                  int imageId)
        {
            this.DecodeConfig = decodeConfig;
            this.EncodeConfig = encodeConfig;
            this.UnpackConfig = unpackConfig;
            this.PackConfig = packConfig;
            this.DataLanes = dataLanes;
            this.EncodeBlockLength = encodeBlockLength;
            this.EmbeddedDataLines = embeddedDataLanes;
            this.ImageId = imageId;
        }
    }
}
