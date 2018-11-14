// <copyright file="ConnectionCallbackProvider.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;

namespace MMALSharp.Callbacks.Providers
{
    internal static class ConnectionCallbackProvider
    {
        /// <summary>
        /// The list of active callback handlers.
        /// </summary>
        public static Dictionary<MMALConnectionImpl, IConnectionCallbackHandler> WorkingHandlers { get; private set; } = new Dictionary<MMALConnectionImpl, IConnectionCallbackHandler>();

        /// <summary>
        /// Register a new <see cref="IConnectionCallbackHandler"/>.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void RegisterCallback(IConnectionCallbackHandler handler)
        {
            if (handler?.WorkingConnection == null)
            {
                throw new NullReferenceException("Callback handler not configured correctly.");
            }

            if (WorkingHandlers.ContainsKey(handler.WorkingConnection))
            {
                WorkingHandlers[handler.WorkingConnection] = handler;
            }
            else
            {
                WorkingHandlers.Add(handler.WorkingConnection, handler);
            }
        }

        /// <summary>
        /// Finds and returns a <see cref="IConnectionCallbackHandler"/> for a given connection. If no handler is registered, a 
        /// <see cref="DefaultConnectionCallbackHandler"/> will be returned.
        /// </summary>
        /// <param name="connection">The connection for which we are retrieving the callback handler.</param>
        /// <returns>A <see cref="IConnectionCallbackHandler"/> for a given connection. If no handler is registered, a 
        /// <see cref="DefaultConnectionCallbackHandler"/> will be returned.</returns>
        public static IConnectionCallbackHandler FindCallback(MMALConnectionImpl connection)
        {
            if (WorkingHandlers.ContainsKey(connection))
            {
                return WorkingHandlers[connection];
            }
            
            return new DefaultConnectionCallbackHandler(connection);
        }

        /// <summary>
        /// Remove a callback handler for a given connection.
        /// </summary>
        /// <param name="connection">The connection we are removing the callback handler for.</param>
        public static void RemoveCallback(MMALConnectionImpl connection)
        {
            if (WorkingHandlers.ContainsKey(connection))
            {
                WorkingHandlers.Remove(connection);
            }
        }
    }
}
