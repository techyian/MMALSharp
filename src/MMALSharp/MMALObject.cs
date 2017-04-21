using System;
using System.Collections.Generic;

namespace MMALSharp
{
    public abstract class MMALObject : IDisposable
    {
        public static List<WeakReference<MMALObject>> Objects = new List<WeakReference<MMALObject>>();
        private WeakReference<MMALObject> reference;

        public MMALObject()
        {
            reference = new WeakReference<MMALObject>(this);
            Objects.Add(reference);
        }
                        
        public virtual void Dispose()
        {            
            Objects.Remove(reference);
        }

    }
}
