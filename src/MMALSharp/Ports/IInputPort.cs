// <copyright file="IInputPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Callbacks;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Represents an input port.
    /// </summary>
    public interface IInputPort : IPort
    {
        /// <summary>
        /// Managed callback which is called by the native function callback method.
        /// </summary>
        IInputCallbackHandler ManagedInputCallback { get; set; }
        
        /// <summary>
        /// Enables processing on an input port.
        /// </summary>
        void EnableInputPort();
        
        /// <summary>
        /// Releases an input port buffer and reads further data from user provided image data if not reached end of file.
        /// </summary>
        /// <param name="bufferImpl">A managed buffer object.</param>
        void ReleaseInputBuffer(MMALBufferImpl bufferImpl);
        
        /// <summary>
        /// Starts the input port.
        /// </summary>
        void Start();
    }
}