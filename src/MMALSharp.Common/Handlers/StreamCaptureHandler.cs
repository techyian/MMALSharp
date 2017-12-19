using System;
using System.Collections.Generic;
using System.IO;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Processes the image data to a stream.
    /// </summary>
    public abstract class StreamCaptureHandler : ICaptureHandler
    {
        /// <summary>
        /// A Stream instance that we can process image data to
        /// </summary>
        protected Stream CurrentStream { get; set; }

        /// <summary>
        /// A list of files that have been processed by this capture handler
        /// </summary>
        public List<ProcessedFileResult> ProcessedFiles { get; set; } = new List<ProcessedFileResult>();

        /// <summary>
        /// The total size of data that has been processed by this capture handler
        /// </summary>
        protected int Processed { get; set; }

        /// <summary>
        /// The directory to save to (if applicable)
        /// </summary>
        public string Directory { get; protected set; }

        /// <summary>
        /// The extension of the file (if applicable)
        /// </summary>
        public string Extension { get; protected set; }

        protected StreamCaptureHandler(string directory, string extension)
        {            
            this.Directory = directory.TrimEnd('/');
            this.Extension = extension.TrimStart('.');

            System.IO.Directory.CreateDirectory(this.Directory);
        }

        /// <summary>
        /// Creates a new File (FileStream), assigns it to the Stream instance of this class and disposes of any existing stream. 
        /// </summary>
        public void NewFile()
        {
            this.CurrentStream?.Dispose();

            var now = DateTime.Now.ToString("dd-MMM-yy HH-mm-ss");

            var filename = this.Directory + "/" + now + "." + this.Extension;

            int i = 0;
            while(File.Exists(filename))
            {
                filename = this.Directory + "/" + now + " " + i + "." + this.Extension;
                i++;
            }
            
            this.CurrentStream = File.Create(filename);
        }

        public virtual ProcessResult Process()
        {
            return new ProcessResult();
        }

        /// <summary>
        /// Processes the data passed into this method to this class' Stream instance.
        /// </summary>
        /// <param name="data">The image data</param>
        public virtual void Process(byte[] data)
        {
            this.Processed += data.Length;
                        
            if (this.CurrentStream.CanWrite)
                this.CurrentStream.Write(data, 0, data.Length);
            else
                throw new IOException("Stream not writable.");
        }

        /// <summary>
        /// Allows us to do any further processing once the capture method has completed.
        /// </summary>
        public virtual void PostProcess()
        {
            try
            {        
                if (this.CurrentStream.GetType() == typeof(FileStream))
                {
                    this.ProcessedFiles.Add(new ProcessedFileResult(this.Directory, this.GetFilename(), this.Extension));
                }
                
                MMALLog.Logger.Info($"Successfully processed {Helpers.ConvertBytesToMegabytes(this.Processed)}");
            }
            catch(Exception e)
            {
                MMALLog.Logger.Warn($"Something went wrong while processing stream: {e.Message}");                
            }                   
        }
        
        /// <summary>
        /// Gets the filename that a FileStream points to
        /// </summary>
        /// <returns>The filename</returns>
        public string GetFilename()
        {
            if (this.CurrentStream.GetType() == typeof(FileStream))
            {
                return Path.GetFileNameWithoutExtension(((FileStream)this.CurrentStream).Name);
            }

            throw new NotSupportedException("Cannot get filename from non FileStream object");
        }

        public string GetFilepath()
        {
            if (this.CurrentStream.GetType() == typeof(FileStream))
            {
                return ((FileStream)this.CurrentStream).Name;
            }

            throw new NotSupportedException("Cannot get path from non FileStream object");
        }

        public void Dispose()
        {
            CurrentStream?.Dispose();
        }
                        
    }
}
