// <copyright file="IVideoOutputCallbackHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// Represents a video output port callback handler.
    /// </summary>
    public interface IVideoOutputCallbackHandler
    {
        /// <summary>
        /// Prepares the callback handler to process an IFrame. Relevant to H.264 encoding only.
        /// </summary>
        void ForcePrepareSplit();
    }
}
