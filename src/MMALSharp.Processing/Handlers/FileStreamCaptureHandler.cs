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
using System.Runtime.CompilerServices;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Processes image data to a <see cref="FileStream"/>.
    /// </summary>
    public class FileStreamCaptureHandler : MemoryStreamCaptureHandler, IFileStreamCaptureHandler
    {
        private readonly bool _customFilename;
        private int _increment;
        private bool _skippingFirstPartialFrame = true;
        private bool _continuousCapture;

        /// <summary>
        /// When true, the next full frame will be written. If <see cref="ContinuousCapture"/> is not also
        /// true, this property will be reset to false after writing the image so that only one image is written.
        /// </summary>
        public bool CaptureNextFrame { get; set; }

        /// <summary>
        /// When true, every frame is written to storage.
        /// </summary>
        public bool ContinuousCapture 
        {
            get => _continuousCapture; 

            set
            {
                _continuousCapture = value;
                if(_continuousCapture)
                {
                    CaptureNextFrame = true;
                }
            }
        }

        /// <summary>
        /// Defines the image files' DateTime format string that is applied when the object is constructed with directory and extension arguments.
        /// </summary>
        public string FilenameDateTimeFormat { get; set; } = "dd-MMM-yy HH-mm-ss";

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
        /// The filename to write next (if applicable).
        /// </summary>
        public string CurrentFilename { get; set; }

        /// <summary>
        /// The full pathname of the most recently written image file (if any).
        /// </summary>
        public string LastWrittenPathname { get; set; }

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
        /// format defined by the <see cref="FilenameDateTimeFormat"/> property.
        /// </summary>
        /// <param name="directory">The directory to save captured data.</param>
        /// <param name="extension">The filename extension for saving files.</param>
        /// <param name="continuousCapture">When true, every frame is written to a file.</param>
        public FileStreamCaptureHandler(string directory, string extension, bool continuousCapture = true)
        {
            this.Directory = directory.TrimEnd('/');
            this.Extension = extension.TrimStart('.');
            this.ContinuousCapture = continuousCapture;

            MMALLog.Logger.LogDebug($"{nameof(FileStreamCaptureHandler)} created for directory {this.Directory} and extension {this.Extension}");

            System.IO.Directory.CreateDirectory(this.Directory);

            this.LastWrittenPathname = string.Empty;
            this.CurrentStream = new MemoryStream();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="FileStreamCaptureHandler"/> class with the specified file pathname. An auto-incrementing number is added to each
        /// new filename.
        /// </summary>
        /// <param name="fullPath">The absolute full path to save captured data to.</param>
        /// <param name="continuousCapture">When true, every frame is written to a file.</param>
        public FileStreamCaptureHandler(string fullPath, bool continuousCapture = true)
        {

            var ext = fullPath.Split('.').LastOrDefault();
            if (string.IsNullOrEmpty(ext))
            {
                throw new ArgumentNullException(nameof(ext), "Could not get file extension from path string.");
            }

            this.ContinuousCapture = continuousCapture;
            this.Extension = ext;
            var fileInfo = new FileInfo(fullPath);
            this.Directory = fileInfo.DirectoryName;

            this.CurrentFilename = Path.GetFileNameWithoutExtension(fileInfo.Name);
            _customFilename = true;

            MMALLog.Logger.LogDebug($"{nameof(FileStreamCaptureHandler)} created for pathname {fullPath}");

            System.IO.Directory.CreateDirectory(this.Directory);

            this.LastWrittenPathname = string.Empty;
            this.CurrentStream = new MemoryStream();
        }

        /// <summary>
        /// Gets the filename of the most recently stored image file.
        /// </summary>
        /// <returns>The filename.</returns>
        public string GetFilename() =>
            (!string.IsNullOrEmpty(this.LastWrittenPathname)) ? Path.GetFileNameWithoutExtension(this.LastWrittenPathname) : string.Empty;

        /// <summary>
        /// Gets the pathname of the most recently stored image file.
        /// </summary>
        /// <returns>The filepath.</returns>
        public string GetFilepath() =>
            this.LastWrittenPathname;

        /// <summary>
        /// Outputs the current frame to a file. If a full frame hasn't been captured, a flag is set to capture the frame
        /// once the end of stream is indicated.
        /// </summary>
        public virtual void NewFile()
        {
            if(this.CurrentStream == null)
            {
                return;
            }

            // Wait for EOS
            if(!CaptureNextFrame)
            {
                CaptureNextFrame = true;
                return;
            }

            string newFilename;
            if (!string.IsNullOrEmpty(CurrentFilename))
            {
                // If we're taking photos from video port, we don't want to be hammering File.Exists as this is added I/O overhead. Camera can take multiple photos per second
                // so we can't do this when filename uses the current DateTime.
                _increment++;
                newFilename = $"{this.Directory}/{_customFilename} {_increment}.{this.Extension}";
            }
            else
            {
                string tempFilename = DateTime.Now.ToString(FilenameDateTimeFormat);
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

            this.LastWrittenPathname = newFilename;
        }

        /// <summary>
        /// If capture is active, output a new file.
        /// </summary>
        public virtual void NewFrame()
        {
            if (_skippingFirstPartialFrame)
            {
                _skippingFirstPartialFrame = false;
                return;
            }

            if(_continuousCapture || this.CaptureNextFrame)
                this.NewFile();

            this.CaptureNextFrame = _continuousCapture;

            if (this.CurrentStream != null)
            {
                this.CurrentStream.SetLength(0);
            }
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
