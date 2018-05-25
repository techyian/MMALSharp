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
        public VideoOutputCallbackHandler(MMALPortBase port)
            : base(port)
        {
        }

        public VideoOutputCallbackHandler(MMALEncoding encoding, MMALPortBase port)
            : base(encoding, port)
        {
        }

        public override void Callback(MMALBufferImpl buffer)
        {
            if (this.WorkingPort.ComponentReference.GetType() != typeof(MMALVideoEncoder))
            {
                throw new ArgumentException($"Working port component is not of type {nameof(MMALVideoEncoder)}");
            }

            var component = (MMALVideoEncoder)this.WorkingPort.ComponentReference;

            if (component.PrepareSplit && buffer.Properties.Any(c => c == MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_CONFIG))
            {
                ((VideoStreamCaptureHandler)component.Handler).Split();
                component.LastSplit = DateTime.Now;
                component.PrepareSplit = false;
            }

            // Ensure that if we need to split then this is done before processing the buffer data.
            if (component.Split != null)
            {
                if (!component.LastSplit.HasValue)
                {
                    component.LastSplit = DateTime.Now;
                }

                if (DateTime.Now.CompareTo(component.CalculateSplit()) > 0)
                {
                    component.PrepareSplit = true;
                    this.WorkingPort.SetParameter(MMALParametersVideo.MMAL_PARAMETER_VIDEO_REQUEST_I_FRAME, true);
                }
            }
            
            base.Callback(buffer);
        }
    }
}
