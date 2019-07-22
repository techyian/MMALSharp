// <copyright file="IImageContext.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.Drawing.Imaging;
using MMALSharp.Common.Utility;

namespace MMALSharp.Common
{
    public interface IImageContext
    {
        byte[] Data { get; set; }
        bool Raw { get; }
        Resolution Resolution { get; }
        PixelFormat PixelFormat { get; }
        ImageFormat StoreFormat { get; }
    }
}