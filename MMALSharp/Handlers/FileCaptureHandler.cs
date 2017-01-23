using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    public class FileCaptureHandler : ICaptureHandler<ProcessResult>
    {
        private string _path;

        public FileCaptureHandler(string path)
        {
            this._path = path;
        }

        public ProcessResult Process(byte[] data)
        {
            ProcessResult result = new ProcessResult();

            try
            {
                File.WriteAllBytes(this._path, data);
                result.Success = true;
            }
            catch(Exception e)
            {
                result.Message = e.Message;
            }

            return result;
        }
    }
}
