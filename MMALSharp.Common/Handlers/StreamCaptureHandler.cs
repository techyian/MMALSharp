using MMALSharp.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Processes the image data to a stream.
    /// </summary>
    public abstract class StreamCaptureHandler : ICaptureHandler
    {
        protected Stream CurrentStream { get; set; }
        public List<Tuple<string, string, string>> ProcessedStreams { get; set; } = new List<Tuple<string, string, string>>();
        protected int Processed { get; set; }
        protected string Directory { get; set; }
        protected string Extension { get; set; }

        public StreamCaptureHandler(string directory, string extension)
        {            
            this.Directory = directory.TrimEnd('/');
            this.Extension = extension.TrimStart('.');

            System.IO.Directory.CreateDirectory(this.Directory);
        }

        public void NewFile()
        {
            if (this.CurrentStream != null)
                this.CurrentStream.Dispose();
            
            this.CurrentStream = File.Create(this.Directory + "/" + DateTime.Now.ToString("dd-MMM-yy HH-mm-ss") + "." + this.Extension);
        }
                
        public void Process(byte[] data)
        {
            this.Processed += data.Length;
                        
            if (this.CurrentStream.CanWrite)
                this.CurrentStream.Write(data, 0, data.Length);
            else
                throw new IOException("Stream not writable.");
            
        }

        public void PostProcess()
        {
            try
            {                
                this.ProcessedStreams.Add(new Tuple<string, string, string> (this.GetDirectory(), this.GetFilename(), this.GetExtension()));
                Console.WriteLine(string.Format("Successfully processed {0}", Helpers.ConvertBytesToMegabytes(this.Processed)));
            }
            catch(Exception e)
            {
                Console.WriteLine("Something went wrong while processing stream.");
                Console.WriteLine(e.Message);
            }                   
        }
        
        public string GetDirectory()
        {
            if(this.CurrentStream.GetType() == typeof(FileStream))
            {
                return Path.GetDirectoryName(((FileStream)this.CurrentStream).Name);                
            }
            return null;
        }

        private string GetExtension()
        {
            if (this.CurrentStream.GetType() == typeof(FileStream))
            {
                return Path.GetExtension(((FileStream)this.CurrentStream).Name);
            }
            return null;
        }

        private string GetFilename()
        {
            if (this.CurrentStream.GetType() == typeof(FileStream))
            {
                return Path.GetFileNameWithoutExtension(((FileStream)this.CurrentStream).Name);
            }
            return null;
        }

        public void Dispose()
        {
            if (this.CurrentStream != null)
                this.CurrentStream.Dispose();
        }
                        
    }
}
