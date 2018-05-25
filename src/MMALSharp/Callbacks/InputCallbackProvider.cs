using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Callbacks
{
    public static class InputCallbackProvider
    {
        public static Dictionary<MMALPortBase, IInputCallbackHandler> WorkingHandlers { get; private set; } = new Dictionary<MMALPortBase, IInputCallbackHandler>();

        public static void RegisterCallback(MMALPortBase port, IInputCallbackHandler handler)
        {
            if (WorkingHandlers.ContainsKey(port))
            {
                WorkingHandlers[port] = handler;
            }
            else
            {
                WorkingHandlers.Add(port, handler);
            }
        }

        public static IInputCallbackHandler FindCallback(MMALPortBase port)
        {
            if (WorkingHandlers.ContainsKey(port))
            {
                return WorkingHandlers[port];
            }

            return new DefaultInputCallbackHandler(port);
        }

        public static void RemoveCallback(MMALPortBase port)
        {
            if (WorkingHandlers.ContainsKey(port))
            {
                WorkingHandlers.Remove(port);
            }
        }
    }
}
