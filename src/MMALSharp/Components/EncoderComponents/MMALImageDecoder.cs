using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Common.Handlers;
using MMALSharp.Components.EncoderComponents;
using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents an image decoder component
    /// </summary>
    public sealed unsafe class MMALImageDecoder : MMALEncoderBase
    {
        public MMALImageDecoder(ICaptureHandler handler) : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_DECODER, handler)
        {
            throw new NotImplementedException();
        }
    }
}
