// <copyright file="IControlPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Ports
{
    public interface IControlPort : IPort
    {
        void EnableControlPort();
        void Start();
    }
}