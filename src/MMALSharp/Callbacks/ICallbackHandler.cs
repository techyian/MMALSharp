// <copyright file="ICallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// Represents an output port callback handler.
    /// </summary>
    public interface ICallbackHandler
    {        
        /// <summary>
        /// A whitelisted Encoding Type that this callback handler will operate on.
        /// </summary>
        MMALEncoding EncodingType { get; }
        
        /// <summary>
        /// The port this callback handler is used with.
        /// </summary>
        IPort WorkingPort { get; }

        /// <summary>
        /// The callback function to carry out. Generally applies to input ports.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        /// <returns>A <see cref="ProcessResult"/> object based on the result of the callback function.</returns>
        ProcessResult CallbackWithResult(IBuffer buffer);

        /// <summary>
        /// The callback function to carry out. Applies to output, control and connection ports.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        void Callback(IBuffer buffer);     
    }
}