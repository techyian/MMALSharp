using System;
using System.Diagnostics;
using System.IO;
using MMALSharp.Common;
using MMALSharp.Processors.Motion;

namespace MMALSharp.Handlers
{
    public class CircularBufferCaptureHandler : CaptureHandlerProcessorBase, IMotionCaptureHandler, IVideoCaptureHandler
    {
        private bool _isRecordingMotion;
        private int _bufferSize;
        private int _currentIndex;

        public MotionType MotionType { get; set; }

        protected byte[] Buffer { get; set; }

        protected bool ShouldDetectMotion { get; set; }

        protected FileStream MotionRecording { get; set; }

        protected Stopwatch RecordingElapsed { get; set; }

        protected MotionAnalyser Analyser { get; set; }

        public CircularBufferCaptureHandler(int bufferSize)
        {
            this.Buffer = new byte[bufferSize];
        }

        public override void Process(byte[] data, bool eos)
        {
            if (!_isRecordingMotion)
            {
                var length = (data.Length + _currentIndex > _bufferSize) ? _bufferSize - _currentIndex : data.Length;
                Array.Copy(data, 0, this.Buffer, length, _currentIndex);
            }
            else
            {
                this.MotionRecording.Write(data, 0, data.Length);
            }

            if (this.ShouldDetectMotion)
            {
                this.Analyser.Apply(data, eos);
            }
        }

        public void DetectMotion(MotionConfig config, Action onDetect, IImageContext imageContext)
        {
            this.ShouldDetectMotion = true;
            this.Analyser = new MotionAnalyser(config, onDetect, this.MotionType, imageContext);
        }

        public void StartRecording(string directory, string extension)
        {
            _isRecordingMotion = true;
            this.MotionRecording = File.Create(this.ProvideFilename(directory, extension));
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
