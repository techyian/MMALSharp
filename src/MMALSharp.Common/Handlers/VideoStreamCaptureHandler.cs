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
