// <copyright file="IConnection.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Callbacks;
using MMALSharp.Components;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;

namespace MMALSharp
{
    /// <summary>
    /// Represents a connection between two ports.
    /// </summary>
    public interface IConnection : IMMALObject
    {
        /// <summary>
        /// The connection callback handler.
        /// </summary>
        IConnectionCallbackHandler CallbackHandler { get; }

        /// <summary>
        /// The downstream component associated with the connection.
        /// </summary>
        IDownstreamComponent DownstreamComponent { get; }

        /// <summary>
        /// The upstream component associated with the connection.
        /// </summary>
        IComponent UpstreamComponent { get; }

        /// <summary>
        /// The input port of this connection.
        /// </summary>
        IInputPort InputPort { get; }

        /// <summary>
        /// The output port of this connection.
        /// </summary>
        IOutputPort OutputPort { get; }

        /// <summary>
        /// The pool of buffer headers in this connection.
        /// </summary>
        IBufferPool ConnectionPool { get; set; }

        /// <summary>
        /// Name of this connection.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Indicates whether this connection is enabled.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Flags passed during the create call (Read Only). A bitwise combination of Connection flags values.
        /// </summary>
        uint Flags { get; }

        /// <summary>
        /// Time in microseconds taken to setup the connection.
        /// </summary>  
        long TimeSetup { get; }

        /// <summary>
        /// Time in microseconds taken to enable the connection.
        /// </summary>
        long TimeEnable { get; }

        /// <summary>
        /// Time in microseconds taken to disable the connection.
        /// </summary>
        long TimeDisable { get; }

        /// <summary>
        /// Enable a connection. The format of the two ports must have been committed before calling this function, although note that on creation, 
        /// the connection automatically copies and commits the output port's format to the input port.
        /// </summary>
        void Enable();

        /// <summary>
        /// Disable a connection.
        /// </summary>
        void Disable();

        /// <summary>
        /// Destroy a connection. Release an acquired reference on a connection. Only actually destroys the connection when the last reference is 
        /// being released. The actual destruction of the connection will start by disabling it, if necessary. Any pool, queue, and so on owned by 
        /// the connection shall then be destroyed.
        /// </summary>
        void Destroy();

        /// <summary>
        /// Associates a <see cref="IConnectionCallbackHandler"/> for use with this connection instance. This will only be used
        /// if callbacks have been enabled against this connection.
        /// </summary>
        /// <param name="handler">The callback handler to use.</param>
        void RegisterCallbackHandler(IConnectionCallbackHandler handler);
    }
}
