// <copyright file="BayerMetaProcessor.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Text;
using MMALSharp.Common;

namespace MMALSharp.Processors
{
    public class BayerMetaProcessor : IFrameProcessor
    {
        /// <summary>
        /// The camera version being used.
        /// </summary>
        public CameraVersion CameraVersion { get; }
        
        public const int BayerMetaLengthV1 = 6404096;
        public const int BayerMetaLengthV2 = 10270208;
        
        public BayerMetaProcessor(CameraVersion camVersion)
        {
            this.CameraVersion = camVersion;
        }

        /// <inheritdoc />
        public void Apply(IImageContext context)
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
