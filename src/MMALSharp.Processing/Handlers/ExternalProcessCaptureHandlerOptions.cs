// <copyright file="ExternalProcessCaptureHandlerOptions.cs" company="Techyian">
// Copyright (c) Ian Auty and contributors. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using Mono.Unix.Native;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Options to pass to the constructor of <see cref="ExternalProcessCaptureHandler"/>.
    /// </summary>
    public class ExternalProcessCaptureHandlerOptions
    {
        /// <summary>
        /// In theory, ffmpeg responds to a pair of SIGINT signals with a clean shutdown, although in
        /// practice this doesn't appear to work when ffmpeg is running as a child process.
        /// </summary>
        public static Signum[] SignalsFFmpeg = new[] { Signum.SIGINT, Signum.SIGINT };

        /// <summary>
        /// Clean termination signals for a VLC / cvlc process.
        /// </summary>
        public static Signum[] SignalsVLC = new[] { Signum.SIGINT };

        // --------------------------------------------------------------------------------------------
        //
        // VLC termination signals are documented here:
        // https://wiki.videolan.org/Hacker_Guide/Interfaces/#A_typical_VLC_run_course
        //
        // With ffmpeg realtime transcoding, the following combinations are all likely to result in
        // a corrupt video file (for example, MP4 encoding will be missing the end-header MOOV atom).
        // Testing also shows it doesn't help to delay between sending the signals.
        //
        // Termination signal combos tested include SIGINT followed by:
        //    SIGINT, SIGABRT, SIGALRM, SIGBUS, SIGTERM, SIGHUP - immediate stop, no output of any kind
        //    SIGQUIT - ouputs a message, tries to write trailer (MOOV atom), aborts
        //
        // Certain simpler video formats like AVI may either complete successfully, or at least play
        // without obvious issues but they are still likely to be "technically" corrupted at the end.
        //
        // --------------------------------------------------------------------------------------------

        /// <summary>
        /// The name of the process to be launched (e.g. ffmpeg, cvlc, etc.)
        /// </summary>
        public string Filename = string.Empty;

        /// <summary>
        /// Command line arguments used to start the process.
        /// </summary>
        public string Arguments = string.Empty;

        /// <summary>
        /// When true, stdout and stderr data is asynchronously buffered and output. When false, output is
        /// completely suppressed, which may improve release-build performance. If true and MMAL is also
        /// configured for logging, process output will also be logged.
        /// </summary>
        public bool EchoOutput = true;

        /// <summary>
        /// When the <see cref= "ExternalProcessCaptureHandler.ProcessExternalAsync" /> token is canceled,
        /// a short delay will ensure any final output from the process is echoed. Ignored if EchoOutput is
        /// false. This delay occurs after any TerminationSignals are issued.
        /// </summary>
        public int DrainOutputDelayMs = 500;

        /// <summary>
        /// If present, when the <see cref="ExternalProcessCaptureHandler.ProcessExternalAsync"/> token is
        /// canceled, these signals will be sent to the process. Some processes expect a CTRL+C (SIGINT).
        /// </summary>
        public Signum[] TerminationSignals = new Signum[] { };
    }
}
