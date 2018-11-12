// <copyright file="IOutputPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Callbacks;
using MMALSharp.Components;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    public interface IOutputPort : IPort
    {
        IOutputCallbackHandler ManagedOutputCallback { get; set; }
        IInputPort ConnectTo(MMALDownstreamComponent destinationComponent, int inputPort = 0, bool useCallback = false);
        IInputPort ConnectTo(MMALDownstreamComponent destinationComponent, int inputPort, Func<IPort> callback);
        void EnableOutputPort(bool sendBuffers = true);
        void ReleaseOutputBuffer(MMALBufferImpl bufferImpl);
        void Start();
    }
}