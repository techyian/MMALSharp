// <copyright file="IMMALObject.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;

namespace MMALSharp
{
    /// <summary>
    /// Represents a MMAL object.
    /// </summary>
    public interface IMMALObject : IDisposable
    {
        /// <summary>
        /// Checks whether a native MMAL pointer is valid.
        /// </summary>
        /// <returns>True if the pointer is valid.</returns>
        bool CheckState();

        /// <summary>
        /// Returns whether this MMAL object has been disposed of.
        /// </summary>
        bool IsDisposed { get; }
    }
}
