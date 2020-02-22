// <copyright file="FFmpegCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Diagnostics;
using System.Text;
using MMALSharp.Common;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Experimental FFmpeg specific capture handler.
    /// </summary>
    public class FFmpegCaptureHandler : IOutputCaptureHandler, IVideoCaptureHandler
    {
        private Process _process;
        
        /// <summary>
        /// The total size of data that has been processed by this capture handler.
        /// </summary>
        protected int Processed { get; set; }
                
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

        /// <summary>
        /// Returns whether this capture handler features the split file functionality.
        /// </summary>
        /// <returns>True if can split.</returns>
        public bool CanSplit() => false;
        
        /// <summary>
        /// Not used.
        /// </summary>
        public void PostProcess() { }

        /// <summary>
        /// Not used.
        /// </summary>
        /// <returns>A NotImplementedException.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public string GetDirectory()
            => throw new NotImplementedException();

        /// <summary>
        /// Not used.
        /// </summary>
        /// <param name="allocSize">N/A.</param>
        /// <returns>A NotImplementedException.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public ProcessResult Process(uint allocSize)
            => throw new NotImplementedException();

        /// <summary>
        /// Writes frame data to the StandardInput stream to be processed by FFmpeg.
        /// </summary>
        /// <param name="context">Contains the data and metadata for an image frame.</param>
        public void Process(ImageContext context)
        {
            try
            {
                _process.StandardInput.BaseStream.Write(context.Data, 0, context.Data.Length);
                _process.StandardInput.BaseStream.Flush();
                this.Processed += context.Data.Length;
            }
            catch
            {
                _process.Kill();             
                throw;         
            }            
        }

        /// <summary>
        /// Not used.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Split()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the total number of bytes processed by this capture handler.
        /// </summary>
        /// <returns>The total number of bytes processed by this capture handler.</returns>
        public string TotalProcessed()
        {
            return $"{this.Processed}";
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_process.HasExited)
            {
                _process.Kill();
            }
        }
    }
}
