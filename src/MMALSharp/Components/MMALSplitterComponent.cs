using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp.Components
{
    /// <summary>
    /// The Splitter Component is intended on being connected to the camera video output port. In turn, it
    /// provides an additional 4 output ports which can be used to produce multiple image/video outputs
    /// from the single camera video port.
    /// </summary>
    public class MMALSplitterComponent : MMALDownstreamHandlerComponent
    {
        private int _width;
        private int _height;

        public override int Width
        {
            get
            {
                if (_width == 0)
                {
                    return MMALCameraConfig.VideoResolution.Width;
                }
                return _width;
            }
            set { _width = value; }
        }

        public override int Height
        {
            get
            {
                if (_height == 0)
                {
                    return MMALCameraConfig.VideoResolution.Height;
                }
                return _height;
            }
            set { _height = value; }
        }

        public MMALSplitterComponent(ICaptureHandler handler) : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_SPLITTER, handler)
        {     
                
        }

        public override void PrintComponent()
        {
            MMALLog.Logger.Info($"Component: Splitter");

            for(var i = 0; i < this.Inputs.Count; i++)
            {
                MMALLog.Logger.Info($"Port {i} Input encoding: {this.Inputs[i].EncodingType.EncodingName}.");
            }
            for(var i = 0; i < this.Outputs.Count; i++)
            {
                MMALLog.Logger.Info($"Port {i} Output encoding: {this.Outputs[i].EncodingType.EncodingName}");
            }
                        
            MMALLog.Logger.Info($"Width: {this.Width}. Height: {this.Height}");
        }
    }
}
