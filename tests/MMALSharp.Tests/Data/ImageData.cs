// <copyright file="ImageData.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Collections.Generic;

namespace MMALSharp.Tests
{
    public class ImageData
    {
        public static IEnumerable<object[]> Data
        {
            get
            {
                var list = new List<object[]>();

                list.AddRange(TestBase.JpegEncoderData);
                list.AddRange(TestBase.GifEncoderData);
                list.AddRange(TestBase.PngEncoderData);
                list.AddRange(TestBase.BmpEncoderData);

                // TGA/PPM support is enabled by performing a firmware update "sudo rpi-update".
                // See: https://github.com/techyian/MMALSharp/issues/23

                // list.AddRange(TestData.TgaEncoderData);
                // list.AddRange(TestData.PpmEncoderData);
                return list;
            }
        }
    }
}