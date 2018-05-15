// <copyright file="VideoStreamCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System.IO;

namespace MMALSharp.Handlers
{
    public class VideoStreamCaptureHandler : StreamCaptureHandler
    {
        public VideoStreamCaptureHandler(string directory, string extension) : base(directory, extension) { }
        
        public void Split()
        {
            if (this.CurrentStream.GetType() == typeof(FileStream))
            {                
                this.NewFile();
            }
        }
    }
}
