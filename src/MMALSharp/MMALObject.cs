// <copyright file="MMALObject.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp
{
    /// <summary>
    /// This class is the base class for all MMAL components.
    /// </summary>
    public abstract class MMALObject : IMMALObject
    {
        /// <summary>
        /// Returns whether this MMAL object has been disposed of.
        /// </summary>
        public bool IsDisposed { get; internal set; }

        /// <summary>
        /// Creates a new instance of the MMALObject class and adds this instance to the Objects list.
        /// </summary>
        protected MMALObject()
        {
        }

        /// <summary>
        /// Marks this object as disposed.
        /// </summary>
        public virtual void Dispose()
        {
            this.IsDisposed = true;
        }

        /// <inheritdoc />
        public abstract bool CheckState();
    }
}
