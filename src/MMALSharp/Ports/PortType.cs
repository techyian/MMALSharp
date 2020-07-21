// <copyright file="PortType.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Ports
{
    /// <summary>
    /// Describes a port type.
    /// </summary>
    public enum PortType
    {
        /// <summary>
        /// An input port.
        /// </summary>
        Input,

        /// <summary>
        /// An output port.
        /// </summary>
        Output,

        /// <summary>
        /// A clock port.
        /// </summary>
        Clock,

        /// <summary>
        /// A control port.
        /// </summary>
        Control,

        /// <summary>
        /// A generic port.
        /// </summary>
        Generic
    }
}
