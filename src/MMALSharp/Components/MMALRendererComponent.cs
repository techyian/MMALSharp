using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a base class for all renderer components
    /// </summary>
    public abstract class MMALRendererBase : MMALDownstreamComponent
    {
        protected MMALRendererBase(string name) : base(name)
        {
        }
    }

    /// <summary>
    /// Represents a Null Sink component. This component should be used when a preview component is not required in order to measure exposure.
    /// </summary>
    public class MMALNullSinkComponent : MMALRendererBase
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

        public MMALNullSinkComponent() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_NULL_SINK)
        {
            this.EnableComponent();
        }

        public override void PrintComponent()
        {
            MMALLog.Logger.Info($"Component: Null sink renderer");
        }
    }

    /// <summary>
    /// Represents a Video Renderer component
    /// </summary>
    public class MMALVideoRenderer : MMALRendererBase
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

        public MMALVideoRenderer() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_RENDERER)
        {
            this.EnableComponent();
        }

        public override void PrintComponent()
        {
            MMALLog.Logger.Info($"Component: Video renderer");            
        }
    }
}
