// <copyright file="BasicImageData.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Collections.Generic;

namespace MMALSharp.Tests
{
    public class BasicImageData
    {
        public static IEnumerable<object[]> Data
        {
            get
            {
                var list = new List<object[]>();

                list.AddRange(TestBase.JpegEncoderData);
              
                return list;
            }
        }
    }
}