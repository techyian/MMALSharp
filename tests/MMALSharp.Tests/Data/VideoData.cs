// <copyright file="VideoData.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Collections.Generic;

namespace MMALSharp.Tests
{
    public class VideoData
    {
        public static IEnumerable<object[]> Data
        {
            get
            {
                var list = new List<object[]>();

                list.AddRange(TestData.H264EncoderData);
                list.AddRange(TestData.MjpegEncoderData);

                return list;
            }
        }
    }
}