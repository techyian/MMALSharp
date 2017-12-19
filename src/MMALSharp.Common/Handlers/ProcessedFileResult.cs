using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    public class ProcessedFileResult
    {
        public string Directory { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }

        public ProcessedFileResult(string directory, string filename, string extension)
        {
            this.Directory = directory;
            this.Filename = filename;
            this.Extension = extension;
        }
    }
}
