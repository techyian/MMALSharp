// <copyright file="ExternalProcessCaptureHandler.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using Mono.Unix.Native;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// This handler is a general-purpose manager for external processes like ffmpeg and VLC.
    /// It uses Channel-based async buffering of console output, and supports options to properly
    /// terminate the child process through one or more signals (SIGINT, SIGQUIT, etc.).
    /// </summary>
    public class ExternalProcessCaptureHandler : IVideoCaptureHandler
    {
        private readonly ExternalProcessCaptureHandlerOptions _options;
        private readonly Process _process;
        private readonly Channel<string> _stdoutBuffer;

        /// <summary>
        /// The total size of data that has been processed by this capture handler.
        /// </summary>
        protected int Processed { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="ExternalProcessCaptureHandler"/> with the specified options.
        /// </summary>
        public ExternalProcessCaptureHandler(ExternalProcessCaptureHandlerOptions options)
        {
            if(MMALLog.Logger != null)
            {
                MMALLog.Logger.LogTrace("Starting ExternalProcessCaptureHandler");
                MMALLog.Logger.LogTrace($"  File: {options.Filename}");
                MMALLog.Logger.LogTrace($"  Args: {options.Arguments}");
                if(options.TerminationSignals.Length == 0)
                {
                    MMALLog.Logger.LogTrace($"  Signal count: 0 (process will be killed upon Dispose)");
                }
                else
                {
                    MMALLog.Logger.LogTrace($"  Signal count: {options.TerminationSignals.Length}");
                }
            }

            _options = options;

            if(options.EchoOutput)
            {
                _stdoutBuffer = Channel.CreateUnbounded<string>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });
            }

            var processStartInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                FileName = options.Filename,
                Arguments = options.Arguments
            };

            _process = new Process();
            _process.StartInfo = processStartInfo;

            Console.InputEncoding = Encoding.ASCII;

            _process.EnableRaisingEvents = true;
            if(options.EchoOutput)
            {
                _process.OutputDataReceived += WriteToBuffer;
                _process.ErrorDataReceived += WriteToBuffer;
            }
            else
            {
                _process.OutputDataReceived += DiscardBuffer;
                _process.ErrorDataReceived += DiscardBuffer;
            }

            _process.Start();

            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        /// <summary>
        /// Returns whether this capture handler features the split file functionality.
        /// </summary>
        /// <returns>True if can split.</returns>
        public bool CanSplit() 
            => false;

        /// <summary>
        /// Not used.
        /// </summary>
        public void PostProcess() 
        { }

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
        /// Writes frame data to the StandardInput stream for processing.
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
                Dispose();
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

        /// <summary>
        /// Manages echoing the output buffer and handles attempts to cleanly terminate the
        /// child process. Use the same CancellationToken passed to MMALCamera.ProcessAsync
        /// and execute both of these with Task.WhenAll.
        /// </summary>
        /// <param name="cancellationToken">The same timeout token used for MMALCamera.ProcessAsync</param>
        public async Task ManageProcessLifecycleAsync(CancellationToken cancellationToken)
        {
            var outputToken = new CancellationTokenSource();
            
            await Task.WhenAny(new[]
            {
                // this Task is the one that will be cancelled by the ProcessAsync timeout
                WaitForCancellationAsync(cancellationToken),

                // we control this token so this will keep running when the above expires
                ConsoleWriteLineAsync(outputToken.Token)
            });

            // now we can do a clean shutdown; ConsoleWriteLineAsync is still running
            if (_options.TerminationSignals.Length > 0)
            {
                MMALLog.Logger?.LogTrace($"Sending process termination signals");
                foreach (var sigint in _options.TerminationSignals)
                {
                    if (_process.HasExited) break;
                    Syscall.kill(_process.Id, sigint);
                }
            }

            // gives output buffer a chance to drain, and also allows the process to do exit-cleanup
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (!_process.HasExited && stopwatch.ElapsedMilliseconds < _options.DrainOutputDelayMs)
                await Task.Delay(50);

            MMALLog.Logger?.LogTrace($"Process exited? {_process.HasExited}");

            // now we terminate ConsoleWriteLineAsync
            outputToken.Cancel();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_process.HasExited)
            {
                MMALLog.Logger?.LogTrace($"Killing PID: {_process.Id}");
                _process.Kill();
            }

            MMALLog.Logger?.LogTrace($"Disposing PID: {_process.Id}");
            _process.Dispose();

            MMALLog.Logger?.LogTrace("Disposed ExternalProcessCaptureHandler");
        }

        // Using "async void" is ok for an event-handler. The purpose of a Task is to communicate the
        // result of an operation to some external "observer" -- but by definition the caller that fires
        // an event doesn't care about the result of the operation. The only caveat is that you get no
        // exception handling; an unhandled exception would terminate the process.
        private async void WriteToBuffer(object sendingProcess, DataReceivedEventArgs e)
        {
            try
            {
                // Technically the faster TryWrite method is guaranteed to work for an
                // unbounded channel, but in this case non-blocking async is more important.
                if (_stdoutBuffer != null && e.Data != null)
                    await _stdoutBuffer.Writer.WriteAsync(e.Data);
            }
            catch
            { }
        }

        // Used when output is not echoed; the Process class requires that stdout/stderr be received.
        private void DiscardBuffer(object sendingProcess, DataReceivedEventArgs e)
        { }

        private async Task WaitForCancellationAsync(CancellationToken cancellationToken)
        {
            // https://github.com/dotnet/runtime/issues/14991#issuecomment-388776983
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => 
            {
                    taskCompletionSource.TrySetResult(true);
            }))
            {
                await taskCompletionSource.Task;
            }
        }

        // When console output is buffered, this asynchronously outputs the buffer without
        // blocking the Process, unlike immediate inline calls to Console.WriteLine.
        private async Task ConsoleWriteLineAsync(CancellationToken cancellationToken)
        {
            try
            {
                if(_options.EchoOutput)
                {
                    while (await _stdoutBuffer.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        var data = await _stdoutBuffer.Reader.ReadAsync();
                        Console.WriteLine(data);
                        MMALLog.Logger?.LogTrace(data);
                    }
                }
                else
                {
                    await WaitForCancellationAsync(cancellationToken);
                }
            }
            catch (OperationCanceledException)
            { } // token cancellation, disregard
        }
    }

    /// <summary>
    /// Options to pass to the constructor of <see cref="ExternalProcessCaptureHandler"/>.
    /// </summary>
    public class ExternalProcessCaptureHandlerOptions
    {
        /// <summary>
        /// In theory, ffmpeg responds to a pair of SIGINT signals with a clean shutdown, although in
        /// practice this doesn't appear to work when ffmpeg is running as a child process.
        /// </summary>
        public static Signum[] signalsFFmpeg = new[] { Signum.SIGINT, Signum.SIGINT };

        /// <summary>
        /// Clean termination signals for a VLC / cvlc process.
        /// </summary>
        public static Signum[] signalsVLC = new[] { Signum.SIGINT };

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
        /// When the <see cref= "ExternalProcessCaptureHandler.ManageProcessLifecycle" /> token is canceled,
        /// a short delay will ensure any final output from the process is echoed. Ignored if EchoOutput is
        /// false. This delay occurs after any TerminationSignals are issued.
        /// </summary>
        public int DrainOutputDelayMs = 500;

        /// <summary>
        /// If present, when the <see cref="ExternalProcessCaptureHandler.ManageProcessLifecycle"/> token is
        /// canceled, these signals will be sent to the process. Some processes expect a CTRL+C (SIGINT).
        /// </summary>
        public Signum[] TerminationSignals = new Signum[] { };
    }
}
