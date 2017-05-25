using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Native;

namespace MMALSharp.Components
{
    public class MMALSplitterComponent : MMALDownstreamComponent
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

        public MMALSplitterComponent() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_SPLITTER, null)
        {     
            throw new NotImplementedException();
        }

        
    }
}
