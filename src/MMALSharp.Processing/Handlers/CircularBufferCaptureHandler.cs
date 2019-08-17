using System;
using System.Diagnostics;
using System.IO;
using MMALSharp.Common;
using MMALSharp.Processors;
using MMALSharp.Processors.Motion;

namespace MMALSharp.Handlers
{
    public sealed class CircularBufferCaptureHandler : OutputCaptureHandlerProcessor, IMotionCaptureHandler, IVideoCaptureHandler
    {
        private bool _isRecordingMotion;
        private int _bufferSize;
        private int _currentIndex;

        public MotionType MotionType { get; set; }
        
        private byte[] Buffer { get; }

        private bool ShouldDetectMotion { get; set; }

        private FileStream MotionRecording { get; set; }

        private Stopwatch RecordingElapsed { get; set; }

        private IFrameAnalyser Analyser { get; set; }

        private MotionConfig Config { get; set; }

        public CircularBufferCaptureHandler(int bufferSize)
        {
            this.Buffer = new byte[bufferSize];
        }

        public void Split()
        {
            throw new NotImplementedException();
        }

        public override void Process(byte[] data, bool eos)
        {
            if (!_isRecordingMotion)
            {
                var length = (data.Length + _currentIndex > _bufferSize) ? _bufferSize - _currentIndex : data.Length;
                Array.Copy(data, 0, this.Buffer, length, _currentIndex);
                this.CheckRecordingProgress();
            }
            else
            {
                this.MotionRecording.Write(data, 0, data.Length);
            }

            if (this.ShouldDetectMotion && !_isRecordingMotion)
            {
                this.Analyser.Apply(data, eos);
            }
        }

        public void DetectMotion(MotionConfig config, Action onDetect, IImageContext imageContext)
        {
            this.Config = config;
            this.ShouldDetectMotion = true;

            if (this.MotionType == MotionType.FrameDiff)
            {
                this.Analyser = new FrameDiffAnalyser(config, onDetect, imageContext);
            }
            else
            {
                // TODO: Motion vector analyser
            }
        }

        public void StartRecording(string directory, string extension)
        {
            _isRecordingMotion = true;
            this.MotionRecording = File.Create(this.ProvideFilename(directory, extension));
            this.RecordingElapsed = new Stopwatch();
            this.RecordingElapsed.Start();
            this.MotionRecording.Write(this.Buffer, 0, this.Buffer.Length);
        }

        public override void Dispose()
        {
            if (this.ShouldDetectMotion)
            {
                this.MotionRecording?.Dispose();
            }
        }

        public override string TotalProcessed()
        {
            throw new NotImplementedException();
        }

        private void CheckRecordingProgress()
        {
            if (this.RecordingElapsed != null)
            {
                if (this.RecordingElapsed.Elapsed >= this.Config.RecordDuration.TimeOfDay)
                {
                    _isRecordingMotion = false;
                    this.RecordingElapsed.Stop();
                    this.RecordingElapsed.Reset();
                }
            }
        }

        private string ProvideFilename(string directory, string extension)
        {
            var dir = directory.TrimEnd('/');
            var ext = extension.TrimStart('.');

            System.IO.Directory.CreateDirectory(dir);

            var now = DateTime.Now.ToString("dd-MMM-yy HH-mm-ss");

            int i = 1;

            var fileName = $"{dir}/{now}.{ext}";

            while (File.Exists(fileName))
            {
                fileName = $"{dir}/{now} {i}.{ext}";
                i++;
            }

            return fileName;
        }
    }
}
