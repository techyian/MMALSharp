using System;
using System.Collections.Generic;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Processors.Effects;

namespace MMALSharp.Processors.Motion
{
    public class MotionAnalyser : FrameDiffAnalyser
    {
        private MotionType _motionType;
        
        internal Action OnDetect { get; set; }
        
        public MotionAnalyser(MotionConfig config, Action onDetect, MotionType type, IImageContext imageContext)
            : base(config, imageContext)
        {
            this.OnDetect = onDetect;
            _motionType = type;
        }

        public override void Apply(byte[] data, bool eos)
        {
            if (this.FullTestFrame)
            {
                MMALLog.Logger.Info("Have full test frame");
                // If we have a full test frame stored then we can start storing subsequent frame data to check.
                base.Apply(data, eos);
            }
            else
            {
                this.TestFrame.AddRange(data);

                if (eos)
                {
                    MMALLog.Logger.Info("EOS reached for test frame. Applying edge detection.");

                    // We want to apply Edge Detection to the test frame to make it easier to detect changes.
                    var edgeDetection = new EdgeDetection(this.MotionConfig.Sensitivity);
                    this.ImageContext.Data = this.WorkingData.ToArray();
                    edgeDetection.ApplyConvolution(edgeDetection.Kernel, EdgeDetection.KernelWidth, EdgeDetection.KernelHeight, this.ImageContext);
                }

                this.FullTestFrame = true;
            }
            
            if (this.FullFrame)
            {
                MMALLog.Logger.Info("Have full frame, checking for changes.");

                // TODO: Check for changes.
                this.CheckForChanges(this.OnDetect);
            }
        }

        public void ClearTestFrame()
        {
            this.TestFrame.Clear();
            this.FullTestFrame = false;
        }
    }
}
