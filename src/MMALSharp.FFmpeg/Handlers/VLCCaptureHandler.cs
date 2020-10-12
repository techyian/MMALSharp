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
        /// Listens for a request on the given port and begins streaming MJPEG images when a client connects. Requires h.264 encoded I420 (YUV420p) as input.
        /// </summary>
        /// <param name="listenPort">The port to listen on. Defaults to 8554.</param>
        /// <param name="echoOutput">Whether to echo stdout and stderr to the console or suppress it. Defaults to true.</param>
        /// <param name="maxBitrate">Maximum output bitrate. If source data is available at a higher bitrate, VLC caps to this. Defaults to 2500 (25Mbps).</param>
        /// <param name="maxFps">Maximum output framerate. If source data is available at a higher framerate, VLC caps to this. Defaults to 20.</param>
        /// <returns>An initialized instance of <see cref="ExternalProcessCaptureHandler"/></returns>
        public static ExternalProcessCaptureHandler StreamH264asMJPEG(int listenPort = 8554, bool echoOutput = true, int maxBitrate = 2500, int maxFps = 20)
        {
            var opts = new ExternalProcessCaptureHandlerOptions
            {
                Filename = "cvlc",
                Arguments = $"stream:///dev/stdin --sout \"#transcode{{vcodec=mjpg,vb={maxBitrate},fps={maxFps},acodec=none}}:standard{{access=http{{mime=multipart/x-mixed-replace;boundary={_VLCInternalMimeBoundaryName}}},mux=mpjpeg,dst=:{listenPort}/}}\" :demux=h264",
                EchoOutput = echoOutput,
                DrainOutputDelayMs = 500, // default
                TerminationSignals = ExternalProcessCaptureHandlerOptions.SignalsVLC
            };

            return new ExternalProcessCaptureHandler(opts);
        }

        /// <summary>
        /// Listens for a request on the given port and begins streaming MJPEG images when a client connects. Requires raw RGB24 frames as input.
        /// </summary>
        /// <param name="width">The width of the raw frames. Defaults to 640.</param>
        /// <param name="height">The height of the raw frames. Defaults to 480.</param>
        /// <param name="fps">Expected FPS of the raw frames. Defaults to 24.</param>
        /// <param name="listenPort">The port to listen on. Defaults to 8554.</param>
        /// <param name="echoOutput">Whether to echo stdout and stderr to the console or suppress it. Defaults to true.</param>
        /// <param name="maxBitrate">Maximum output bitrate. If source data is available at a higher bitrate, VLC caps to this. Defaults to 2500 (25Mbps).</param>
        /// <param name="maxFps">Maximum output framerate. If source data is available at a higher framerate, VLC caps to this. Defaults to 20.</param>
        /// <returns>An initialized instance of <see cref="ExternalProcessCaptureHandler"/></returns>
        public static ExternalProcessCaptureHandler StreamRawRGB24asMJPEG(int width = 640, int height = 480, int fps = 24, int listenPort = 8554, bool echoOutput = true, int maxBitrate = 2500, int maxFps = 20)
        {
            var opts = new ExternalProcessCaptureHandlerOptions
            {
                Filename = "/bin/bash",
                EchoOutput = true,
                Arguments = $"-c \"ffmpeg -hide_banner -f rawvideo -c:v rawvideo -pix_fmt rgb24 -s:v {width}x{height} -r {fps} -i - -f h264 -c:v libx264 -preset ultrafast -tune zerolatency -vf format=yuv420p - | cvlc stream:///dev/stdin --sout '#transcode{{vcodec=mjpg,vb={maxBitrate},fps={maxFps},acodec=none}}:standard{{access=http{{mime=multipart/x-mixed-replace;boundary={_VLCInternalMimeBoundaryName}}},mux=mpjpeg,dst=:{listenPort}/}}' :demux=h264\"",
                DrainOutputDelayMs = 500, // default = 500
                TerminationSignals = ExternalProcessCaptureHandlerOptions.SignalsFFmpeg
            };

            return new ExternalProcessCaptureHandler(opts);
        }
    }
}
