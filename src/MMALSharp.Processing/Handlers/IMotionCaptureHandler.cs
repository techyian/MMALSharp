// <copyright file="IMotionCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Processors.Motion;
using System;
using MMALSharp.Common;

namespace MMALSharp.Handlers
{
    public interface IMotionCaptureHandler
    {
        void DetectMotion(MotionConfig config, Action onDetect, IImageContext imageContext);
    }
}
