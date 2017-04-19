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
        
        /// <summary>
        /// Streams video from the standard output stream via FFmpeg to an RTMP server.
        /// </summary>
        /// <param name="streamName">The meta name of the stream</param>
        /// <param name="streamUrl">The url of your RTMP server - the url to stream to.</param>
        /// <returns></returns>
        public static FFmpegCaptureHandler RTMPStreamer(string streamName, string streamUrl)
        {
            return new FFmpegCaptureHandler(string.Format("-re -i - -c:v copy -an -f flv -metadata streamName={0} {1}", streamName, streamUrl));
        }

        /// <summary>
        /// Records video from the standard output stream via FFmpeg into a video file that can be opened without explicit command line flags.  
        /// </summary>
        /// <param name="directory">The directory to store the output video file</param>
        /// <param name="extension">The extension of the video file</param>
        /// <returns></returns>
        public static FFmpegCaptureHandler TakeVideoMultiplex(string directory, string extension)
        {            
            System.IO.Directory.CreateDirectory(directory);
                        
            return new FFmpegCaptureHandler(string.Format("-re -i - -c:v copy {0}/out.{1}", directory.TrimEnd(), extension.TrimStart('.')));
        }

        public FFmpegCaptureHandler(string argument)
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

                this.MyProcess.StartInfo.Arguments = argument;
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

        public void PostProcess() { }

        public void Process(byte[] data)
        {
            try
            {                
                this.MyProcess.StandardInput.BaseStream.Write(data, 0, data.Length);
                this.MyProcess.StandardInput.BaseStream.Flush();
            }
            catch
            {
                this.MyProcess.Kill();
                throw;         
            }            
        }

        public void Split()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this.MyProcess.Kill();
        }
                
    }
}
