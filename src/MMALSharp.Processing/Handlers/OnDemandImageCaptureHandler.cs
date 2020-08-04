// <copyright file="ImageStreamCaptureHandler.cs" company="Techyian">
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
    /// Buffers single-frame Image data to a <see cref="MemoryStream"/> until <see cref="NewFile"/> is called.
    /// </summary>
    public class OnDemandImageCaptureHandler : MemoryStreamCaptureHandler, IStreamWriter, IFileStreamCaptureHandler
    {
        private readonly string _customFilename;
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
        /// The full pathname of the most-recently-written file.
        /// </summary>
        public string MostRecentPathname { get; set; }

        /// <summary>
        /// When true, the underlying callback handler will write the <see cref="MemoryStream"/> to <see cref="CurrentPathname"/> when the frame is completed.
        /// </summary>
        public bool WriteRequested { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="OnDemandImageCaptureHandler"/> class with the specified directory and filename extension.
        /// </summary>
        /// <param name="directory">The directory to save captured images.</param>
        /// <param name="extension">The filename extension for saving files.</param>
        public OnDemandImageCaptureHandler(string directory, string extension)
            : base()
        {
            this.Directory = directory.TrimEnd('/');
            this.Extension = extension.TrimStart('.');

            MMALLog.Logger.LogDebug($"{nameof(OnDemandImageCaptureHandler)} created for directory {this.Directory} and extension {this.Extension}");

            System.IO.Directory.CreateDirectory(this.Directory);

            this.MostRecentPathname = string.Empty;
            this.CurrentStream = new MemoryStream();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="OnDemandImageCaptureHandler"/> class with the specified file path.
        /// </summary>
        /// <param name="fullPath">The absolute full path to save captured data to.</param>
        public OnDemandImageCaptureHandler(string fullPath)
            : base()
        {

            var ext = fullPath.Split('.').LastOrDefault();
            if (string.IsNullOrEmpty(ext))
            {
                throw new ArgumentNullException(nameof(ext), "Could not get file extension from path string.");
            }

            this.Extension = ext;
            var fileInfo = new FileInfo(fullPath);
            this.Directory = fileInfo.DirectoryName;
            _customFilename = Path.GetFileNameWithoutExtension(fileInfo.Name);

            MMALLog.Logger.LogDebug($"{nameof(OnDemandImageCaptureHandler)} created for directory {this.Directory} and extension {this.Extension}");

            System.IO.Directory.CreateDirectory(this.Directory);

            this.MostRecentPathname = string.Empty;
            this.CurrentStream = new MemoryStream();
        }

        /// <summary>
        /// This handler doesn't create a file in advance. This will return an empty string.
        /// </summary>
        /// <returns>Empty string.</returns>
        public string GetFilename() => string.Empty;

        /// <summary>
        /// This handler doesn't create a file in advance. This will return an empty string.
        /// </summary>
        /// <returns>Empty string.</returns>
        public string GetFilepath() => string.Empty;

        /// <summary>
        /// Signals the underlying callback handler to call <see cref="WriteStreamToFile"/> when the frame is completely captured.
        /// </summary>
        public virtual void NewFile()
        {
            if (this.CurrentStream == null)
            {
                return;
            }

            WriteRequested = true;
        }

        /// <summary>
        /// The callback handler uses this to write the current completed buffer to a file.
        /// </summary>
        public void WriteStreamToFile()
        {
            if (this.CurrentStream == null || !this.WriteRequested)
            {
                return;
            }

            string newFilename;
            if (!string.IsNullOrEmpty(_customFilename))
            {
                // If we're taking photos from video port, we don't want to be hammering File.Exists as this is added I/O overhead. Camera can take multiple photos per second
                // so we can't do this when filename uses the current DateTime.
                _increment++;
                newFilename = $"{this.Directory}/{_customFilename} {_increment}.{this.Extension}";
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

            using (FileStream fs = new FileStream(newFilename, FileMode.Create, FileAccess.Write))
            {
                this.CurrentStream.WriteTo(fs);
            }

            this.MostRecentPathname = newFilename;
            this.WriteRequested = false;
        }

        /// <summary>
        /// Resets the underlying <see cref="MemoryStream"/> without re-allocating.
        /// </summary>
        public void ResetStream()
            => this.CurrentStream.SetLength(0);

        /// <inheritdoc />
        public override void PostProcess()
        {
            if (this.CurrentStream == null)
            {
                return;
            }

            this.ProcessedFiles.Add(new ProcessedFileResult(this.Directory, Path.GetFileNameWithoutExtension(this.MostRecentPathname), this.Extension));
            base.PostProcess();
        }

        /// <inheritdoc />
        public override string TotalProcessed()
        {
            return $"{this.Processed}";
        }
    }
}
