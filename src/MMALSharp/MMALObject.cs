// <copyright file="MMALObject.cs" company="Techyian">
// Copyright (c) Techyian. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;

namespace MMALSharp
{
    public abstract class MMALObject : IDisposable
    {
        public static List<WeakReference<MMALObject>> Objects = new List<WeakReference<MMALObject>>();
        private WeakReference<MMALObject> reference;

        protected MMALObject()
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
