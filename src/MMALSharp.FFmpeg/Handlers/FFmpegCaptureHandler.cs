// <copyright file="FFmpegCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Helper-methods for invoking ExternalProcessCaptureHandler to output to ffmpeg.
    /// </summary>
    public static class FFmpegCaptureHandler
    {
        /// <summary>
        /// Streams video from the standard output stream via FFmpeg to an RTMP server.
        /// </summary>
        /// <param name="streamName">The meta name of the stream.</param>
        /// <param name="streamUrl">The url of your RTMP server - the url to stream to.</param>
        /// <param name="echoOutput">Whether to echo stdout and stderr to the console or suppress it. Defaults to true.</param>
        /// <returns>An initialized instance of <see cref="ExternalProcessCaptureHandler"/></returns>
        public static ExternalProcessCaptureHandler RTMPStreamer(string streamName, string streamUrl, bool echoOutput = true)
        {
            var opts = new ExternalProcessCaptureHandlerOptions
            {
                Filename = "ffmpeg",
                Arguments = $"-i - -vcodec copy -an -f flv -metadata streamName={streamName} {streamUrl}",
                EchoOutput = echoOutput,
                DrainOutputDelayMs = 500, // default
                TerminationSignals = ExternalProcessCaptureHandlerOptions.SignalsFFmpeg
            };

            return new ExternalProcessCaptureHandler(opts);
        }

        /// <summary>
        /// Records video from the standard output stream via FFmpeg, forcing it into an avi container that can be opened by media players without explicit command line flags.  
        /// </summary>
        /// <param name="directory">The directory to store the output video file.</param>
        /// <param name="filename">The name of the video file.</param>
        /// <param name="echoOutput">Whether to echo stdout and stderr to the console or suppress it. Defaults to true.</param>
        /// <returns>An initialized instance of <see cref="ExternalProcessCaptureHandler"/></returns>
        public static ExternalProcessCaptureHandler RawVideoToAvi(string directory, string filename, bool echoOutput = true)
        {
            System.IO.Directory.CreateDirectory(directory);

            var opts = new ExternalProcessCaptureHandlerOptions
            {
                Filename = "ffmpeg",
                Arguments = $"-i - -c:v copy -an -f avi -y {directory.TrimEnd()}/{filename}.avi", // -re option should not be specified here, it's meant to rate-limit scenarios like streaming a pre-recorded file; see: https://stackoverflow.com/a/48497672/152997
                EchoOutput = echoOutput,
                DrainOutputDelayMs = 500, // default
                TerminationSignals = ExternalProcessCaptureHandlerOptions.SignalsFFmpeg
            };

            return new ExternalProcessCaptureHandler(opts);
        }

        /// <summary>
        /// Transcodes the standard output stream via FFmpeg into an MP4 format with options that create a "fragmented" MP4 (more keyframes than usual, and no trailing
        /// MOOV atom trailing-header). Because FFmpeg doesn't do a clean shutdown when running as a child process, it is currently unable to output a "clean" standard
        /// MP4 without these settings, it will not generate the final MOOV trailing-header. This also means the fragmented MP4 may lose a couple seconds at the end
        /// of the final video.
        /// </summary>
        /// <param name="directory">The directory to store the output video file.</param>
        /// <param name="filename">The name of the video file.</param>
        /// <param name="echoOutput">Whether to echo stdout and stderr to the console or suppress it. Defaults to true.</param>
        /// <param name="bitrate">Output bitrate. Defaults to 2500 (25Mbps).</param>
        /// <param name="fps">Output framerate. Defaults to 24.</param>
        /// <returns>An initialized instance of <see cref="ExternalProcessCaptureHandler"/></returns>
        public static ExternalProcessCaptureHandler RawVideoToMP4(string directory, string filename, bool echoOutput = true, int bitrate = 2500, int fps = 24)
        {
            System.IO.Directory.CreateDirectory(directory);

            var opts = new ExternalProcessCaptureHandlerOptions
            {
                Filename = "ffmpeg",
                Arguments = $"-framerate {fps} -i - -b:v {bitrate}k -c copy -movflags +frag_keyframe+separate_moof+omit_tfhd_offset+empty_moov {directory.TrimEnd()}/{filename}.mp4",
                EchoOutput = echoOutput,
                DrainOutputDelayMs = 500, // default
                TerminationSignals = ExternalProcessCaptureHandlerOptions.SignalsFFmpeg
            };

            return new ExternalProcessCaptureHandler(opts);
        }
    }
}
