// <copyright file="IControlPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Ports
{
    /// <summary>
    /// Represents a control port.
    /// </summary>
    public interface IControlPort : IPort
    {
        /// <summary>
        /// Enables processing on a control port.
        /// </summary>
        void EnableControlPort();
        
        /// <summary>
        /// Starts the control port.
        /// </summary>
        void Start();
    }
}