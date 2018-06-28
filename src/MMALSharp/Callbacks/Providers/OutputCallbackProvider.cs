// <copyright file="OutputCallbackProvider.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;

namespace MMALSharp.Callbacks.Providers
{
    /// <summary>
    /// Provides a facility to retrieve callback handlers for Output ports.
    /// </summary>
    internal static class OutputCallbackProvider
    {
        /// <summary>
        /// The list of active callback handlers.
        /// </summary>
        public static Dictionary<MMALPortBase, OutputCallbackHandlerBase> WorkingHandlers { get; private set; } = new Dictionary<MMALPortBase, OutputCallbackHandlerBase>();

        /// <summary>
        /// Register a new <see cref="OutputCallbackHandlerBase"/>.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void RegisterCallback(OutputCallbackHandlerBase handler)
        {
            if (handler?.WorkingPort == null)
            {
                throw new NullReferenceException("Callback handler not configured correctly.");
            }

            if (WorkingHandlers.ContainsKey(handler.WorkingPort))
            {
                WorkingHandlers[handler.WorkingPort] = handler;
            }
            else
            {
                WorkingHandlers.Add(handler.WorkingPort, handler);
            }
        }

        /// <summary>
        /// Finds and returns a <see cref="OutputCallbackHandlerBase"/> for a given port. If no handler is registered, a 
        /// <see cref="DefaultOutputCallbackHandler"/> will be returned.
        /// </summary>
        /// <param name="port">The port we are retrieving the callback handler on.</param>
        /// <returns>A <see cref="OutputCallbackHandlerBase"/> for a given port. If no handler is registered, a 
        /// <see cref="DefaultOutputCallbackHandler"/> will be returned.</returns>
        public static OutputCallbackHandlerBase FindCallback(MMALPortBase port)
        {
            if (WorkingHandlers.ContainsKey(port))
            {
                return WorkingHandlers[port];
            }
            
            return new DefaultOutputCallbackHandler(port);
        }

        /// <summary>
        /// Remove a callback handler for a given port.
        /// </summary>
        /// <param name="port">The port we are removing the callback handler on.</param>
        public static void RemoveCallback(MMALPortBase port)
        {
            if (WorkingHandlers.ContainsKey(port))
            {
                WorkingHandlers.Remove(port);
            }
        }
    }
}
