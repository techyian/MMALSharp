using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a video decoder component
    /// </summary>
    public class MMALVideoDecoder : MMALEncoderBase
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

        public MMALVideoDecoder(ICaptureHandler handler) : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_DECODER, handler)
        {
        }

        public override void PrintComponent()
        {
            base.PrintComponent();
            MMALLog.Logger.Info($"Width: {this.Width}. Height: {this.Height}");        
        }
    }
}
