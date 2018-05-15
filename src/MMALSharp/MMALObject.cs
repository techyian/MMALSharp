// <copyright file="MMALObject.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Collections.Generic;

namespace MMALSharp
{
    /// <summary>
    /// This class is the base class for all MMAL components.
    /// </summary>
    public abstract class MMALObject : IDisposable
    {
        /// <summary>
        /// Gets a list of all MMALObjects.
        /// </summary>
        public static readonly List<WeakReference<MMALObject>> Objects = new List<WeakReference<MMALObject>>();
        private WeakReference<MMALObject> reference;

        /// <summary>
        /// Creates a new instance of the MMALObject class and adds this instance to the Objects list.
        /// </summary>
        public MMALObject()
        {
            reference = new WeakReference<MMALObject>(this);
            Objects.Add(reference);
        }

        /// <summary>
        /// Removes this instance from the Objects list.
        /// </summary>
        public virtual void Dispose()
        {            
            Objects.Remove(reference);
        }

    }
}
