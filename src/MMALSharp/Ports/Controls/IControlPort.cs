// <copyright file="IControlPort.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Ports.Controls
{
    /// <summary>
    /// Represents a control port.
    /// </summary>
    public interface IControlPort : IPort
    {
        /// <summary>
        /// Starts the control port.
        /// </summary>
        void Start();
    }
}
