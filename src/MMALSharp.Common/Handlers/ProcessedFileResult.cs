// <copyright file="ProcessedFileResult.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Handlers
{
    public class ProcessedFileResult
    {
        public string Directory { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }

        public ProcessedFileResult(string directory, string filename, string extension)
        {
            this.Directory = directory;
            this.Filename = filename;
            this.Extension = extension;
        }
    }
}
