using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Callbacks
{
    public static class OutputCallbackProvider
    {
        public static Dictionary<MMALPortBase, ICallbackHandler> WorkingHandlers { get; private set; } = new Dictionary<MMALPortBase, ICallbackHandler>();

        public static void RegisterCallback(MMALPortBase port, ICallbackHandler handler)
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

        public static ICallbackHandler FindCallback(MMALPortBase port)
        {
            if (WorkingHandlers.ContainsKey(port))
            {
                return WorkingHandlers[port];
            }

            return new DefaultCallbackHandler(port);
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
