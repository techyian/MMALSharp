using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    public class VideoStreamCaptureHandler : StreamCaptureHandler
    {
        public VideoStreamCaptureHandler(string directory, string extension) : base(directory, extension) { }
        
        public bool CanSplit()
        {
            if (this.CurrentStream.GetType() == typeof(FileStream))
                return true;
            return false;
        }

        public void Split()
        {
            if (this.CanSplit())
            {                
                this.NewFile();
            }
        }
    }
}
