// <copyright file="ImageFileDecodeInputPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Common.Utility;
using MMALSharp.Components;

namespace MMALSharp.Ports.Inputs
{
    /// <summary>
    /// A custom port definition used specifically when using encoder conversion functionality.
    /// </summary>
    public unsafe class FileEncodeInputPort : InputPort
    {
        /// <inheritdoc />
        public override Resolution Resolution
        {
            get => new Resolution(this.Width, this.Height);
            internal set
            {
                // Do not pad user provided resolution.
                this.Width = value.Width;
                this.Height = value.Height;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="FileEncodeInputPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="guid">Managed unique identifier for this port.</param>
        public FileEncodeInputPort(IntPtr ptr, IComponent comp, Guid guid)
            : base(ptr, comp, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="FileEncodeInputPort"/>.
        /// </summary>
        /// <param name="copyFrom">The port to copy data from.</param>
        public FileEncodeInputPort(IPort copyFrom)
            : base((IntPtr)copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.Guid)
        {
        }
    }
}
