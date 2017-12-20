using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Handlers;
using MMALSharp.Native;
using static MMALSharp.Native.MMALParameters;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents the resizer component. This component has the ability to change the encoding type & pixel format, as well
    /// as the width/height of resulting frames.
    /// </summary>
    public sealed class MMALResizerComponent : MMALDownstreamHandlerComponent
    {
        public override int Width { get; set; }
        public override int Height { get; set; }

        public MMALResizerComponent(int width, int height, ICaptureHandler handler) : base(MMAL_COMPONENT_DEFAULT_RESIZER, handler)
        {
            this.Width = width;
            this.Height = height;
        }

        public override void PrintComponent()
        {
            base.PrintComponent();
            MMALLog.Logger.Info($"    Width: {this.Width}. Height: {this.Height}");
        }
    }
}
