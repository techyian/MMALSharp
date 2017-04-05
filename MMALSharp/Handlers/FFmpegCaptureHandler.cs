using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    public class FFmpegCaptureHandler : ICaptureHandler
    {
        public Process MyProcess { get; set; }
        
        public FFmpegCaptureHandler(string streamName, string streamUrl)
        {
            this.MyProcess = new Process();

            try
            {
                Console.InputEncoding = Encoding.ASCII;
                this.MyProcess.StartInfo.UseShellExecute = false;
                this.MyProcess.StartInfo.RedirectStandardInput = true;
                this.MyProcess.StartInfo.CreateNoWindow = true;                
                this.MyProcess.StartInfo.FileName = "ffmpeg";
                this.MyProcess.StartInfo.Arguments = string.Format("-i - -c:v copy -an -r 25 -f flv -metadata streamName={0} {1}", streamName, streamUrl);                
                this.MyProcess.Start();                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public bool CanSplit()
        {
            throw new NotImplementedException();
        }

        public string GetDirectory()
        {
            throw new NotImplementedException();
        }

        public void PostProcess()
        {
            
        }

        public void Process(byte[] data)
        {
            try
            {                
                this.MyProcess.StandardInput.BaseStream.Write(data, 0, data.Length);
                this.MyProcess.StandardInput.BaseStream.Flush();
            }
            catch
            {
                this.Dispose();
                throw;         
            }            
        }

        public void Split()
        {
            throw new NotImplementedException();
        }

        ~FFmpegCaptureHandler()
        {
            this.MyProcess.Close();
        }
    }
}
