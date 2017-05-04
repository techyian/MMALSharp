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
            return new FFmpegCaptureHandler($"-re -i - -c:v copy -an -f flv -metadata streamName={streamName} {streamUrl}");
        }

        /// <summary>
        /// Records video from the standard output stream via FFmpeg, forcing it into an avi container that can be opened by media players without explicit command line flags.  
        /// </summary>
        /// <param name="directory">The directory to store the output video file</param>
        /// <param name="filename">The name of the video file</param>
        /// <returns></returns>
        public static FFmpegCaptureHandler RawVideoToAvi(string directory, string filename)
        {            
            System.IO.Directory.CreateDirectory(directory);
                        
            return new FFmpegCaptureHandler($"-re -i - -c:v copy -an -f avi {directory.TrimEnd()}/{filename}.avi");
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
