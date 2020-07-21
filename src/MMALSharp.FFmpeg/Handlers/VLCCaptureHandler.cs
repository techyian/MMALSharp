// <copyright file="VLCCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Helper-methods for invoking ExternalProcessCaptureHandler to output to VLC.
    /// </summary>
    public static class VLCCaptureHandler
    {
        // MJPEG streams over HTTP send JPEGs separated by MIME boundaries.
        // VLC is hard-coded to specify this boundary-name in the MIME-type header.
        private static readonly string _VLCInternalMimeBoundaryName = "7b3cc56e5f51db803f790dad720ed50a";

        /// <summary>
        /// Listens for a request on the given port and begins streaming MJPEG images when a client connects.
        /// </summary>
        /// <param name="listenPort">The port to listen on. Defaults to 8554.</param>
        /// <param name="echoOutput">Whether to echo stdout and stderr to the console or suppress it. Defaults to true.</param>
        /// <param name="maxBitrate">Maximum output bitrate. If source data is available at a higher bitrate, VLC caps to this. Defaults to 2500 (25Mbps).</param>
        /// <param name="maxFps">Maximum output framerate. If source data is available at a higher framerate, VLC caps to this. Defaults to 20.</param>
        /// <returns>An initialized instance of <see cref="ExternalProcessCaptureHandler"/></returns>
        public static ExternalProcessCaptureHandler StreamMJPEG(int listenPort = 8554, bool echoOutput = true, int maxBitrate = 2500, int maxFps = 20)
        {
            var opts = new ExternalProcessCaptureHandlerOptions
            {
                Filename = "cvlc",
                Arguments = $"stream:///dev/stdin --sout \"#transcode{{vcodec=mjpg,vb={maxBitrate},fps={maxFps},acodec=none}}:standard{{access=http{{mime=multipart/x-mixed-replace;boundary=--{_VLCInternalMimeBoundaryName}}},mux=mpjpeg,dst=:{listenPort}/}}\" :demux=h264",
                EchoOutput = echoOutput,
                DrainOutputDelayMs = 500, // default
                TerminationSignals = ExternalProcessCaptureHandlerOptions.SignalsVLC
            };

            return new ExternalProcessCaptureHandler(opts);
        }
    }
}
