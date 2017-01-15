using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharPicam.MMALParameterHelpers;

namespace SharPicam.Components
{
    public unsafe class MMALEncoderComponent : MMALComponentBase
    {
        public MMALEncoderComponent() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_ENCODER)
        {
            var input = this.Inputs.ElementAt(0);
            var output = this.Outputs.ElementAt(0);

            input.ShallowCopy(output.Ptr->format);

            output.Ptr->format->encoding = MMALEncodings.MMAL_ENCODING_JPEG;
            output.Ptr->bufferNum = Math.Max(output.Ptr->bufferNumRecommended, output.Ptr->bufferNumMin);
            output.Ptr->bufferSize = Math.Max(output.Ptr->bufferSizeRecommended, output.Ptr->bufferSizeMin);

            output.Commit();

            SetParameter(MMALParametersCamera.MMAL_PARAMETER_JPEG_Q_FACTOR, 90, output.Ptr);

            Console.WriteLine("Enabling encoder");

            this.EnableComponent();

            Console.WriteLine("Create pool");

            this.BufferPool = new MMALPoolImpl(output);            
        }

        public void EncoderBufferCallback(MMALBufferImpl buffer)
        {
            Console.WriteLine("Inside encoder buffer callback");

            Console.WriteLine("Buffer alloc size " + buffer.AllocSize);
            Console.WriteLine("Buffer length " + buffer.Length);
            Console.WriteLine("Buffer offset " + buffer.Offset);
            buffer.Properties();
        }

    }
}
