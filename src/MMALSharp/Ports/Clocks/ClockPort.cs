// <copyright file="ClockPort.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Callbacks;
using MMALSharp.Components;

namespace MMALSharp.Ports.Clocks
{
    /// <summary>
    /// Represents a clock port.
    /// </summary>
    public class ClockPort : GenericPort<ICallbackHandler>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClockPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public ClockPort(IntPtr ptr, IComponent comp, Guid guid) 
            : base(ptr, comp, PortType.Clock, guid)
        {
        }
    }
}