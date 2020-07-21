// <copyright file="VideoData.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Collections.Generic;

namespace MMALSharp.Tests
{
    public class VideoData
    {
        public static IEnumerable<object[]> H264Data
        {
            get
            {
                var list = new List<object[]>();

                list.AddRange(TestBase.H264EncoderData);

                return list;
            }
        }

        public static IEnumerable<object[]> MJPEGData
        {
            get
            {
                var list = new List<object[]>();

                list.AddRange(TestBase.MjpegEncoderData);

                return list;
            }
        }

        public static IEnumerable<object[]> Data
        {
            get
            {
                var list = new List<object[]>();

                list.AddRange(TestBase.H264EncoderData);
                list.AddRange(TestBase.MjpegEncoderData);

                return list;
            }
        }
    }
}