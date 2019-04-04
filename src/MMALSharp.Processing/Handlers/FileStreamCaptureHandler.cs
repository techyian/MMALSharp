using System;
using System.Collections.Generic;
using System.IO;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Processes image data to a <see cref="FileStream"/>.
    /// </summary>
    public class FileStreamCaptureHandler : StreamCaptureHandler<FileStream>
    {
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
        /// Creates a new instance of the <see cref="FileStreamCaptureHandler"/> class with the specified directory and filename extension.
        /// </summary>
        /// <param name="directory">The directory to save captured data.</param>
        /// <param name="extension">The filename extension for saving files.</param>
        public FileStreamCaptureHandler(string directory, string extension) 
        {
            this.Directory = directory.TrimEnd('/');
            this.Extension = extension.TrimStart('.');

            System.IO.Directory.CreateDirectory(this.Directory);
        }

        /// <summary>
        /// Gets the filename that a FileStream points to.
        /// </summary>
        /// <returns>The filename.</returns>
        public string GetFilename() => Path.GetFileNameWithoutExtension(this.CurrentStream.Name);

        /// <summary>
        /// Gets the filepath that a FileStream points to.
        /// </summary>
        /// <returns>The filepath.</returns>
        public string GetFilepath() => this.CurrentStream.Name;

        /// <summary>
        /// Creates a new File (FileStream), assigns it to the Stream instance of this class and disposes of any existing stream. 
        /// </summary>
        public virtual void NewFile()
        {
            this.CurrentStream?.Dispose();

            var now = DateTime.Now.ToString("dd-MMM-yy HH-mm-ss");

            var filename = this.Directory + "/" + now + "." + this.Extension;

            int i = 0;
            while (File.Exists(filename))
            {
                filename = this.Directory + "/" + now + " " + i + "." + this.Extension;
                i++;
            }

            this.CurrentStream = File.Create(filename);
        }
        
        /// <summary>
        /// Allows us to do any further processing once the capture method has completed.
        /// </summary>
        public override void PostProcess()
        {
            this.ProcessedFiles.Add(new ProcessedFileResult(this.Directory, this.GetFilename(), this.Extension));
            base.PostProcess();
        }

        public override string TotalProcessed()
        {
            return $"{this.Processed}";
        }
    }
}
