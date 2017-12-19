using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Currently experimental. Not working fully.
    /// </summary>
    public class FFmpegCaptureHandler : ICaptureHandler
    {
        private Process _process;
                    
        /// <summary>
        /// Streams video from the standard output stream via FFmpeg to an RTMP server.
        /// </summary>
        /// <param name="streamName">The meta name of the stream</param>
        /// <param name="streamUrl">The url of your RTMP server - the url to stream to.</param>
        /// <returns></returns>
        public static FFmpegCaptureHandler RTMPStreamer(string streamName, string streamUrl)
        {
            return new FFmpegCaptureHandler($"-i - -vcodec copy -an -f flv -metadata streamName={streamName} {streamUrl}");
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
            var processStartInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                FileName = "ffmpeg",
                Arguments = argument
            };

            this._process = new Process();
            this._process.StartInfo = processStartInfo;
            
            try
            {
                Console.InputEncoding = Encoding.ASCII;
                
                this._process.EnableRaisingEvents = true;
                this._process.OutputDataReceived += (object sendingProcess, DataReceivedEventArgs e) =>
                {
                    if (e.Data != null)
                    {                        
                        Console.WriteLine(e.Data);                       
                    }
                };

                this._process.ErrorDataReceived += (object sendingProcess, DataReceivedEventArgs e) =>
                {
                    if (e.Data != null)
                    {
                        Console.WriteLine(e.Data);
                    }
                };
                
                this._process.Start();

                this._process.BeginOutputReadLine();
                this._process.BeginErrorReadLine();                                
            }
            catch (Exception e)
            {
                MMALLog.Logger.Fatal(e.Message);
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

        public ProcessResult Process()
        {
            throw new NotImplementedException();
        }

        public void Process(byte[] data)
        {
            try
            {
                this._process.StandardInput.BaseStream.Write(data, 0, data.Length);
                this._process.StandardInput.BaseStream.Flush();
            }
            catch
            {
                this._process.Kill();             
                throw;         
            }            
        }

        public void Split()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {          
            this._process.Kill();
        }

    }
}
