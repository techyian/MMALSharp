
using System;
using System.Diagnostics;

namespace MMALSharp.Processors.Motion
{
    public class MotionConfig
    {
        public MotionSensitivity Sensitivity { get; set; }
        public int FramesPerSecond { get; set; }
        public DateTime RecordDuration { get; set; }
        public Stopwatch RecordingElapsed { get; set; }
    }
}
