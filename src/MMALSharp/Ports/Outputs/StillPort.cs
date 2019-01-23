// <copyright file="StillPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Common.Utility;
using MMALSharp.Native;

namespace MMALSharp.Ports.Outputs
{
    /// <summary>
    /// Represents a still image encoder/decoder port.
    /// </summary>
    public unsafe class StillPort : OutputPort
    {
        private int _width;
        private int _height;

        /// <inheritdoc />
        public override Resolution Resolution
        {
            get
            {
                if (_width == 0 || _height == 0)
                {
                    return new Resolution(this.Ptr->Format->Es->Video.Width, this.Ptr->Format->Es->Video.Height);
                }

                return new Resolution(_width, _height);
            }
            
            internal set
            {
                if (value.Width == 0 || value.Height == 0)
                {
                    _width = MMALCameraConfig.StillResolution.Pad().Width;
                    _height = MMALCameraConfig.StillResolution.Pad().Height;

                    this.Ptr->Format->Es->Video.Width = MMALCameraConfig.StillResolution.Pad().Width;
                    this.Ptr->Format->Es->Video.Height = MMALCameraConfig.StillResolution.Pad().Height;
                }
                else
                {
                    _width = value.Pad().Width;
                    _height = value.Pad().Height;

                    this.Ptr->Format->Es->Video.Width = value.Pad().Width;
                    this.Ptr->Format->Es->Video.Height = value.Pad().Height;
                }
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="StillPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public StillPort(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid)
            : base(ptr, comp, type, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="StillPort"/>.
        /// </summary>
        /// <param name="copyFrom">The port to copy data from.</param>
        public StillPort(PortBase copyFrom)
            : base(copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.PortType, copyFrom.Guid, copyFrom.Handler)
        {
        }
    }    
}
