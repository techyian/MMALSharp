// <copyright file="IInputCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using MMALSharp.Handlers;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// Represents a callback handler linked to an Input port.
    /// </summary>
    public interface IInputCallbackHandler : ICallbackHandler
    {
        /// <summary>
        /// The callback function to carry out. Applies to input ports.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        /// <returns>A <see cref="ProcessResult"/> object based on the result of the callback function.</returns>
        ProcessResult CallbackWithResult(IBuffer buffer);
    }
}
