// <copyright file="BufferMetadata.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Common
{
    /// <summary>
    /// This class contains metadata for a MMAL Buffer header.
    /// </summary>
    public class BufferMetadata
    {
        /// <summary>
        /// The buffer represents the end of stream.
        /// </summary>
        public bool Eos { get; set; }
        
        /// <summary>
        /// The buffer contains IFrame data.
        /// </summary>
        public bool IFrame { get; set; }
    }
}
