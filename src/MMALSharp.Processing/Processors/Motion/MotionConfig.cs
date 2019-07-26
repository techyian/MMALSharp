
using System;
using MMALSharp.Processors.Effects;

namespace MMALSharp.Processors.Motion
{
    public class MotionConfig
    {
        public EDStrength Sensitivity { get; set; }
        public DateTime RecordDuration { get; set; }
        public int Threshold { get; set; }

        public MotionConfig(EDStrength sensitivity, DateTime recordDuration, int threshold)
        {
            this.Sensitivity = sensitivity;
            this.RecordDuration = recordDuration;
            this.Threshold = threshold;
        }
    }
}
