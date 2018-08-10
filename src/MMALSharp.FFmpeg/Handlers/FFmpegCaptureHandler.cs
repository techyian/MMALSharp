// <copyright file="FFmpegCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Diagnostics;
using System.Text;
using MMALSharp.Processors;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Experimental FFmpeg specific capture handler.
    /// </summary>
    public class FFmpegCaptureHandler : ICaptureHandler
    {
        private Process _process;
        
        /// <summary>
        /// Streams video from the standard output stream via FFmpeg to an RTMP server.
        /// </summary>
        /// <param name="streamName">The meta name of the stream.</param>
        /// <param name="streamUrl">The url of your RTMP server - the url to stream to.</param>
        /// <returns>A new instance of <see cref="FFmpegCaptureHandler"/> with process arguments to push to an RTMP stream.</returns>
        public static FFmpegCaptureHandler RTMPStreamer(string streamName, string streamUrl)
        {
            return new FFmpegCaptureHandler($"-i - -vcodec copy -an -f flv -metadata streamName={streamName} {streamUrl}");
        }

        /// <summary>
        /// Records video from the standard output stream via FFmpeg, forcing it into an avi container that can be opened by media players without explicit command line flags.  
        /// </summary>
        /// <param name="directory">The directory to store the output video file.</param>
        /// <param name="filename">The name of the video file.</param>
        /// <returns>A new instance of <see cref="FFmpegCaptureHandler"/> with process arguments to convert raw video into a compatible AVI container.</returns>
        public static FFmpegCaptureHandler RawVideoToAvi(string directory, string filename)
        {            
            System.IO.Directory.CreateDirectory(directory);                        
            
            return new FFmpegCaptureHandler($"-re -i - -c:v copy -an -f avi -y {directory.TrimEnd()}/{filename}.avi");
        }

        /// <summary>
        /// Creates a new instance of <see cref="FFmpegCaptureHandler"/> with the specified process arguments.
        /// </summary>
        /// <param name="argument">The <see cref="ProcessStartInfo"/> argument.</param>
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

            _process = new Process();
            _process.StartInfo = processStartInfo;
            
            try
            {
                Console.InputEncoding = Encoding.ASCII;
                
                _process.EnableRaisingEvents = true;
                _process.OutputDataReceived += (object sendingProcess, DataReceivedEventArgs e) =>
                {
                    if (e.Data != null)
                    {                        
                        Console.WriteLine(e.Data);                       
                    }
                };

                _process.ErrorDataReceived += (object sendingProcess, DataReceivedEventArgs e) =>
                {
                    if (e.Data != null)
                    {
                        Console.WriteLine(e.Data);
                    }
                };
                
                _process.Start();

                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();                                
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

        public void Manipulate(IManipulate process)
        {
            throw new NotImplementedException();
        }

        public ProcessResult Process(uint allocSize)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes frame data to the StandardInput stream to be processed by FFmpeg.
        /// </summary>
        /// <param name="data">The frame data to push to FFmpeg.</param>
        public void Process(byte[] data)
        {
            try
            {
                _process.StandardInput.BaseStream.Write(data, 0, data.Length);
                _process.StandardInput.BaseStream.Flush();
            }
            catch
            {
                _process.Kill();             
                throw;         
            }            
        }
        
        public void Split()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (!_process.HasExited)
            {
                _process.Kill();
            }
        }
    }
}
