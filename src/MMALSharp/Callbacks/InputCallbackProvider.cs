using System;
using System.Collections.Generic;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// Provides a facility to retrieve callback handlers for Input ports.
    /// </summary>
    public static class InputCallbackProvider
    {
        /// <summary>
        /// The list of active callback handlers.
        /// </summary>
        public static Dictionary<MMALPortBase, IInputCallbackHandler> WorkingHandlers { get; private set; } = new Dictionary<MMALPortBase, IInputCallbackHandler>();

        /// <summary>
        /// Register a new callback handler with a given port.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void RegisterCallback(IInputCallbackHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
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
        /// Finds and returns a <see cref="IInputCallbackHandler"/> for a given port. If no handler is registered, a 
        /// <see cref="DefaultInputCallbackHandler"/> will be returned.
        /// </summary>
        /// <param name="port">The port we are retrieving the callback handler on.</param>
        /// <returns>A <see cref="IInputCallbackHandler"/> for a given port. If no handler is registered, a 
        /// <see cref="DefaultInputCallbackHandler"/> will be returned.</returns>
        public static IInputCallbackHandler FindCallback(MMALPortBase port)
        {
            if (WorkingHandlers.ContainsKey(port))
            {
                return WorkingHandlers[port];
            }

            return new DefaultInputCallbackHandler(port);
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
