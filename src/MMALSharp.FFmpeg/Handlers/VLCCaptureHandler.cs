// <copyright file="VLCCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Helper-methods for invoking ExternalProcessCaptureHandler to output to VLC.
    /// </summary>
    public static class VLCCaptureHandler
    {
        /// <summary>
        /// Listens for a request on the given port and begins streaming MJPEG images when a client connects.
        /// </summary>
        /// <param name="listenPort">The port to listen on. Defaults to 8554.</param>
        /// <param name="echoOutput">Whether to echo stdout and stderr to the console or suppress it. Defaults to true.</param>
        /// <returns>An initialized instance of <see cref="ExternalProcessCaptureHandler"/></returns>
        public static ExternalProcessCaptureHandler StreamMJPEG(int listenPort = 8554, bool echoOutput = true)
        {
            var opts = new ExternalProcessCaptureHandlerOptions
            {
                Filename = "cvlc",
                Arguments = $"stream:///dev/stdin --sout \"#transcode{{vcodec=mjpg,vb=2500,fps=20,acodec=none}}:standard{{access=http{{mime=multipart/x-mixed-replace;boundary=7b3cc56e5f51db803f790dad720ed50a}},mux=mpjpeg,dst=:{listenPort}/}}\" :demux=h264",
                EchoOutput = echoOutput,
                DrainOutputDelayMs = 500, // default
                TerminationSignals = ExternalProcessCaptureHandlerOptions.signalsVLC
            };

            return new ExternalProcessCaptureHandler(opts);
        }
    }
}
