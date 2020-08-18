// <copyright file="FileStreamCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using Microsoft.Extensions.Logging;
using MMALSharp.Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Processes image data to a <see cref="FileStream"/>.
    /// </summary>
    public class FileStreamCaptureHandler : StreamCaptureHandler<FileStream>, IFileStreamCaptureHandler
    {
        private readonly bool _customFilename;
        private int _increment;

        /// <summary>
        /// A list of files that have been processed by this capture handler.
        /// </summary>
        public List<ProcessedFileResult> ProcessedFiles { get; set; } = new List<ProcessedFileResult>();

        /// <summary>
        /// The directory to save to (if applicable).
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// The extension of the file (if applicable).
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// The name of the current file associated with the FileStream.
        /// </summary>
        public string CurrentFilename { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="FileStreamCaptureHandler"/> class without provisions for writing to a file. Supports
        /// subclasses in which file output is optional.
        /// </summary>
        public FileStreamCaptureHandler()
        {
            MMALLog.Logger.LogDebug($"{nameof(FileStreamCaptureHandler)} empty ctor invoked, no file will be written");
        }

        /// <summary>
        /// Creates a new instance of the <see cref="FileStreamCaptureHandler"/> class with the specified directory and filename extension. Filenames will be in the
        /// format "dd-MMM-yy HH-mm-ss" taken from this moment in time.
        /// </summary>
        /// <param name="directory">The directory to save captured data.</param>
        /// <param name="extension">The filename extension for saving files.</param>
        public FileStreamCaptureHandler(string directory, string extension) 
        {
            this.Directory = directory.TrimEnd('/');
            this.Extension = extension.TrimStart('.');

            MMALLog.Logger.LogDebug($"{nameof(FileStreamCaptureHandler)} created for directory {this.Directory} and extension {this.Extension}");

            System.IO.Directory.CreateDirectory(this.Directory);
            
            var now = DateTime.Now.ToString("dd-MMM-yy HH-mm-ss");
            
            int i = 1;

            var fileName = $"{this.Directory}/{now}.{this.Extension}";

            while (File.Exists(fileName))
            {
                fileName = $"{this.Directory}/{now} {i}.{this.Extension}";
                i++;
            }

            var fileInfo = new FileInfo(fileName);

            this.CurrentFilename = Path.GetFileNameWithoutExtension(fileInfo.Name);
            this.CurrentStream = File.Create(fileName);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="FileStreamCaptureHandler"/> class with the specified file path.
        /// </summary>
        /// <param name="fullPath">The absolute full path to save captured data to.</param>
        public FileStreamCaptureHandler(string fullPath)
        {
            var fileInfo = new FileInfo(fullPath);

            this.Directory = fileInfo.DirectoryName;
            this.CurrentFilename = Path.GetFileNameWithoutExtension(fileInfo.Name);

            var ext = fullPath.Split('.').LastOrDefault();
            
            if (string.IsNullOrEmpty(ext))
            {
                throw new ArgumentNullException(nameof(ext), "Could not get file extension from path string.");
            }
            
            this.Extension = ext;

            MMALLog.Logger.LogDebug($"{nameof(FileStreamCaptureHandler)} created for directory {this.Directory} and extension {this.Extension}");

            _customFilename = true;

            System.IO.Directory.CreateDirectory(this.Directory);

            this.CurrentStream = File.Create(fullPath);
        }

        /// <summary>
        /// Gets the filename that a FileStream points to.
        /// </summary>
        /// <returns>The filename.</returns>
        public string GetFilename() => 
            (this.CurrentStream != null) ? Path.GetFileNameWithoutExtension(this.CurrentStream.Name) : string.Empty;

        /// <summary>
        /// Gets the filepath that a FileStream points to.
        /// </summary>
        /// <returns>The filepath.</returns>
        public string GetFilepath() => 
            this.CurrentStream?.Name ?? string.Empty;

        /// <summary>
        /// Creates a new File (FileStream), assigns it to the Stream instance of this class and disposes of any existing stream. 
        /// </summary>
        public virtual void NewFile()
        {
            if (this.CurrentStream == null)
            {
                return;
            }

            this.CurrentStream?.Dispose();

            string newFilename = string.Empty;
            
            if (_customFilename)
            {
                // If we're taking photos from video port, we don't want to be hammering File.Exists as this is added I/O overhead. Camera can take multiple photos per second
                // so we can't do this when filename uses the current DateTime.
                _increment++;
                newFilename = $"{this.Directory}/{this.CurrentFilename} {_increment}.{this.Extension}";
            }
            else
            {
                string tempFilename = DateTime.Now.ToString("dd-MMM-yy HH-mm-ss");
                int i = 1;

                newFilename = $"{this.Directory}/{tempFilename}.{this.Extension}";

                while (File.Exists(newFilename))
                {
                    newFilename = $"{this.Directory}/{tempFilename} {i}.{this.Extension}";
                    i++;
                }
            }

            this.CurrentStream = File.Create(newFilename);
        }

        /// <inheritdoc />
        public override void PostProcess()
        {
            if (this.CurrentStream == null)
            {
                return;
            }

            this.ProcessedFiles.Add(new ProcessedFileResult(this.Directory, this.GetFilename(), this.Extension));
            base.PostProcess();
        }

        /// <inheritdoc />
        public override string TotalProcessed()
        {
            return $"{this.Processed}";
        }
    }
}
