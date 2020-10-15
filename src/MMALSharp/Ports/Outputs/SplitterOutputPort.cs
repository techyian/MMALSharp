// <copyright file="SplitterOutputPort.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Common.Utility;
using MMALSharp.Components;

namespace MMALSharp.Ports.Outputs
{
    /// <summary>
    /// Represents a splitter component output port.
    /// </summary>
    public unsafe abstract class SplitterOutputPort : OutputPort
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
                // The splitter component doesn't support resolution changes. This override has been
                // added in case the user wants to use the splitter component with the Image Processing
                // library and will use this property to inform what resolution it's running at. Applied
                // via port config.
                _resolution = new Resolution(value.Width, value.Height);
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="SplitterOutputPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="guid">Managed unique identifier for this port.</param>
        protected SplitterOutputPort(IntPtr ptr, IComponent comp, Guid guid)
            : base(ptr, comp, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SplitterOutputPort"/>.
        /// </summary>
        /// <param name="copyFrom">The port to copy data from.</param>
        protected SplitterOutputPort(IPort copyFrom)
            : base((IntPtr)copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.Guid)
        {
        }
    }
}
