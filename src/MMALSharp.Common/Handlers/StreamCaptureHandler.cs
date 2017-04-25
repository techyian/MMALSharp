using MMALSharp.Utility;
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
        /// A list of FileStreams that have been processed by this capture handler
        /// </summary>
        public List<Tuple<string, string, string>> ProcessedFiles { get; set; } = new List<Tuple<string, string, string>>();

        /// <summary>
        /// The total size of data that has been processed by this capture handler
        /// </summary>
        protected int Processed { get; set; }

        /// <summary>
        /// The directory
        /// </summary>
        public string Directory { get; protected set; }
        public string Extension { get; protected set; }

        public StreamCaptureHandler(string directory, string extension)
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
            if (this.CurrentStream != null)
                this.CurrentStream.Dispose();
            
            this.CurrentStream = File.Create(this.Directory + "/" + DateTime.Now.ToString("dd-MMM-yy HH-mm-ss") + "." + this.Extension);
        }
                
        /// <summary>
        /// Processes the data passed into this method to this class' Stream instance.
        /// </summary>
        /// <param name="data">The image data</param>
        public void Process(byte[] data)
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
        public void PostProcess()
        {
            try
            {        
                if (this.CurrentStream.GetType() == typeof(FileStream))
                {
                    this.ProcessedFiles.Add(new Tuple<string, string, string>(this.Directory, this.GetFilename(), this.Extension));
                }
                
                Console.WriteLine(string.Format("Successfully processed {0}", Helpers.ConvertBytesToMegabytes(this.Processed)));
            }
            catch(Exception e)
            {
                Console.WriteLine("Something went wrong while processing stream.");
                Console.WriteLine(e.Message);
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
            if (this.CurrentStream != null)
                this.CurrentStream.Dispose();
        }
                        
    }
}
