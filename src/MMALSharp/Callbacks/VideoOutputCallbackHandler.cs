using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp.Callbacks
{
    public class VideoOutputCallbackHandler : DefaultCallbackHandler
    {
        private readonly MMALVideoEncoder _component;

        public VideoOutputCallbackHandler(MMALVideoEncoder component)
        {
            _component = component;
        }

        public VideoOutputCallbackHandler(MMALVideoEncoder component, MMALEncoding encoding)
            : base(encoding)
        {
            _component = component;
        }

        public override void Callback(MMALBufferImpl buffer)
        {
            if (_component.PrepareSplit && buffer.Properties.Any(c => c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CONFIG))
            {
                ((VideoStreamCaptureHandler)_component.Handler).Split();
                _component.LastSplit = DateTime.Now;
                _component.PrepareSplit = false;
            }

            // Ensure that if we need to split then this is done before processing the buffer data.
            if (_component.Split != null)
            {
                if (!_component.LastSplit.HasValue)
                {
                    _component.LastSplit = DateTime.Now;
                }

                if (DateTime.Now.CompareTo(_component.CalculateSplit()) > 0)
                {
                    _component.PrepareSplit = true;
                    this.WorkingPort.SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_REQUEST_I_FRAME, true);
                }
            }
            
            base.Callback(buffer);
        }
    }
}
