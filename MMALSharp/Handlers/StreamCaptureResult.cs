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
    public class StreamCaptureResult : ICaptureHandler, IDisposable
    {
        private Stream _stream;
        private int _processed;

        public StreamCaptureResult(Stream stream)
        {
            this._stream = stream;
        }
                
        public void Process(byte[] data)
        {
            this._processed += data.Length;
                        
            if (this._stream.CanWrite)
                this._stream.Write(data, 0, data.Length);
            else
                throw new PiCameraError("Stream not writable.");

            if (MMALCameraConfig.Debug)
                Console.WriteLine("Currently processed: " + Helpers.ConvertBytesToMegabytes(this._processed));
        }

        public void PostProcess()
        {
            Console.WriteLine(string.Format("Successfully processed {0}", Helpers.ConvertBytesToMegabytes(this._processed)));
        }

        public bool CanSplit()
        {
            if(this._stream.GetType() == typeof(FileStream))
                return true;
            return false;
        }

        public void Split(string fileNameConstant)
        {
            if(this.CanSplit() && !string.IsNullOrEmpty(fileNameConstant))
            {
                this._stream.Dispose();
                this._stream = File.Create(this.GetDirectory() + "/" + fileNameConstant + DateTime.Now.ToString("dd-MMM-yy HH-mm-ss") + this.GetExtension());
            }
        }

        public string GetDirectory()
        {
            if(this._stream.GetType() == typeof(FileStream))
            {
                return Path.GetDirectoryName(((FileStream)this._stream).Name);                
            }
            return null;
        }

        private string GetExtension()
        {
            if (this._stream.GetType() == typeof(FileStream))
            {
                return Path.GetExtension(((FileStream)this._stream).Name);
            }
            return null;
        }

        public void Dispose()
        {
            if(this._stream != null)
                this._stream.Dispose();
        }
    }
}
