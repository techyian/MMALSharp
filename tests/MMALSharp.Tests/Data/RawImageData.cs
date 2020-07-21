// <copyright file="RawImageData.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Collections.Generic;

namespace MMALSharp.Tests
{
    public class RawImageData
    {
        public static IEnumerable<object[]> Data
        {
            get
            {
                yield return TestBase.Yuv420EncoderData;
                yield return TestBase.Rgb16EncoderData;
                yield return TestBase.Rgb24EncoderData;
                yield return TestBase.RgbaEncoderData;                
            }
        }
    }
}