using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Currently experimental. Not working fully.
    /// </summary>
    public class FFmpegCaptureHandler : ICaptureHandler
    {
        public Process MyProcess { get; set; }
        
        public FFmpegCaptureHandler(string streamName, string streamUrl, int framerate)
        {
            this.MyProcess = new Process();

            try
            {
                Console.InputEncoding = Encoding.ASCII;
                this.MyProcess.StartInfo.UseShellExecute = false;
                this.MyProcess.StartInfo.RedirectStandardInput = true;
                this.MyProcess.StartInfo.RedirectStandardOutput = true;
                this.MyProcess.StartInfo.RedirectStandardError = true;
                this.MyProcess.StartInfo.CreateNoWindow = true;                               
                this.MyProcess.StartInfo.FileName = "ffmpeg";

                this.MyProcess.EnableRaisingEvents = true;
                this.MyProcess.OutputDataReceived += (object sendingProcess, DataReceivedEventArgs e) =>
                {
                    if (e.Data != null)
                    {                        
                        Console.WriteLine(e.Data);                       
                    }
                };

                this.MyProcess.ErrorDataReceived += (object sendingProcess, DataReceivedEventArgs e) =>
                {
                    if (e.Data != null)
                    {
                        Console.WriteLine(e.Data);
                    }
                };

                this.MyProcess.StartInfo.Arguments = string.Format("-i - -c:v copy -an -r {0} -f flv -metadata streamName={1} {2}", framerate, streamName, streamUrl);                
                this.MyProcess.Start();
                this.MyProcess.BeginOutputReadLine();
                this.MyProcess.BeginErrorReadLine();
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public bool CanSplit()
        {
            return false;
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
                this.MyProcess.Close();
                throw;         
            }            
        }

        public void Split()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this.MyProcess.Close();
        }
                
    }
}
