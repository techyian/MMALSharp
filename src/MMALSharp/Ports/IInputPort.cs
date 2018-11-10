// <copyright file="IInputPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Callbacks;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    public interface IInputPort : IPort
    {
        InputCallbackHandlerBase ManagedInputCallback { get; set; }
        void EnableInputPort();
        void ReleaseInputBuffer(MMALBufferImpl bufferImpl);
        void Start();
    }
}