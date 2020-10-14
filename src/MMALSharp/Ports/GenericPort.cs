// <copyright file="GenericPort.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Callbacks;
using MMALSharp.Common.Utility;
using MMALSharp.Components;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Represents a generic MMAL port of any type.
    /// </summary>
    /// <typeparam name="TCallback">The callback handler type.</typeparam>
    public class GenericPort<TCallback> : PortBase<TCallback>
        where TCallback : ICallbackHandler
    {
        private Resolution _resolution;

        /// <inheritdoc />
        public override Resolution Resolution
        {
            get
            {
                return _resolution;
            }

            internal set
            {
                this.NativeWidth = value.Pad().Width;
                this.NativeHeight = value.Pad().Height;
                _resolution = new Resolution(value.Width, value.Height);
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="GenericPort{TCallback}"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public GenericPort(IntPtr ptr, IComponent comp, PortType type, Guid guid) 
            : base(ptr, comp, type, guid)
        {
        }
    }
}