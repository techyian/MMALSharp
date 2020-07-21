// <copyright file="IMMALConvert.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Components
{
    /// <summary>
    /// Supports converting user provided image data.
    /// </summary>
    public interface IMMALConvert
    {
        /// <summary>
        /// Encodes/decodes user provided image data.
        /// </summary>
        /// <param name="outputPort">The output port to begin processing on.</param>
        void Convert(int outputPort);
    }
}
