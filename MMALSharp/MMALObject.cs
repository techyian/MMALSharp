using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp
{
    public class MMALObject : IDisposable
    {
        public static List<WeakReference<MMALObject>> Objects = new List<WeakReference<MMALObject>>();
        private WeakReference<MMALObject> reference;

        public MMALObject()
        {
            reference = new WeakReference<MMALObject>(this);
            Objects.Add(reference);
        }

        public static unsafe void UpdatePort(MMALPortImpl port)
        {            
            for(int i = 0; i < Objects.Count; i++)
            {
                MMALObject target;

                if (Objects[i].TryGetTarget(out target))
                {
                    if (target is MMALPortImpl && ((MMALPortImpl)target).Ptr == port.Ptr)
                    {
                        Objects[i] = new WeakReference<MMALObject>(port);
                    }
                }
            } 
        }
                
        public virtual void Dispose()
        {            
            Objects.Remove(reference);
        }

    }
}
