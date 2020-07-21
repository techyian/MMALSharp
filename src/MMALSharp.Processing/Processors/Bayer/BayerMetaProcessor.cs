// <copyright file="BayerMetaProcessor.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Text;
using MMALSharp.Common;

namespace MMALSharp.Processors
{
    /// <summary>
    /// The BayerMetaProcessor is used to strip Bayer metadata from a JPEG image frame via the Image Processing API.
    /// </summary>
    public class BayerMetaProcessor : IFrameProcessor
    {
        /// <summary>
        /// The camera version being used.
        /// </summary>
        public CameraVersion CameraVersion { get; }

        /// <summary>
        /// The length of the metadata for the OmniVision OV5647.
        /// </summary>
        public const int BayerMetaLengthV1 = 6404096;

        /// <summary>
        /// The length of the metadata for the Sony IMX219.
        /// </summary>
        public const int BayerMetaLengthV2 = 10270208;
        
        /// <summary>
        /// Initialises a new instance of <see cref="BayerMetaProcessor"/>.
        /// </summary>
        /// <param name="camVersion">The camera version you're using.</param>
        public BayerMetaProcessor(CameraVersion camVersion)
        {
            this.CameraVersion = camVersion;
        }

        /// <inheritdoc />
        public void Apply(ImageContext context)
        {
            byte[] array = null;
            
            switch (this.CameraVersion)
            {
                case CameraVersion.OV5647:
                    array = new byte[BayerMetaLengthV1];
                    Array.Copy(context.Data, context.Data.Length - BayerMetaLengthV1, array, 0, BayerMetaLengthV1);
                    break;
                case CameraVersion.IMX219:
                    array = new byte[BayerMetaLengthV2];
                    Array.Copy(context.Data, context.Data.Length - BayerMetaLengthV2, array, 0, BayerMetaLengthV2);
                    break;
            }

            byte[] meta = new byte[4];
            Array.Copy(array, 0, meta, 0, 4);

            if (Encoding.ASCII.GetString(meta) != "BRCM")
            {
                throw new Exception("Could not find Bayer metadata in header");
            }
            
            context.Data = new byte[array.Length];
            Array.Copy(array, context.Data, array.Length);
        }
    }
}
