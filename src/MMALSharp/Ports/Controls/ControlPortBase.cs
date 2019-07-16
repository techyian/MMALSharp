// <copyright file="ControlPortBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Callbacks;

namespace MMALSharp.Ports.Controls
{
    /// <summary>
    /// Represents a control port.
    /// </summary>
    public abstract class ControlPortBase : PortBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="ControlPortBase"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        protected ControlPortBase(IntPtr ptr, MMALComponentBase comp, PortType type, Guid guid)
            : base(ptr, comp, type, guid)
        {
        }

        internal abstract ICallbackHandler ManagedControlCallback { get; set; }

        /// <summary>
        /// Enables processing on a control port.
        /// </summary>
        internal abstract void EnableControlPort();

        /// <summary>
        /// Starts the control port.
        /// </summary>
        internal abstract void Start();
    }
}