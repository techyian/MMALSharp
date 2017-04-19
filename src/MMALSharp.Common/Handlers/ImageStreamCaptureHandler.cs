using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    public class ImageStreamCaptureHandler : StreamCaptureHandler
    {
        public ImageStreamCaptureHandler(string directory, string extension) : base(directory, extension) { }
    }
}
