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
    /// A Capture Handler that processes the captured image to a file in the specified path
    /// </summary>
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
                Console.WriteLine(string.Format("Successfully captured image {0}. Total capture size {1}.", this._path, Helpers.ConvertBytesToMegabytes(data.LongLength)));
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }

            return result;
        }
    }
}
