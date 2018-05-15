// <copyright file="ProcessResult.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Handlers
{
    public class ProcessResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public bool EOF { get; set; }
        public byte[] BufferFeed { get; set; }
        public int DataLength { get; set; }
        public int AllocSize { get; set; }
    }
}
