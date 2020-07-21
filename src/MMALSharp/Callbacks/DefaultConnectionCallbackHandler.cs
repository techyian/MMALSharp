// <copyright file="DefaultConnectionCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// Default callback handler for connections.
    /// </summary>
    public class DefaultConnectionCallbackHandler : ConnectionCallbackHandler
    {
        /// <summary>
        /// Create a new instance of <see cref="DefaultConnectionCallbackHandler"/>.
        /// </summary>
        /// <param name="connection">The connection object.</param>
        public DefaultConnectionCallbackHandler(IConnection connection) 
            : base(connection)
        {
        }
    }
}
