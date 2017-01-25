using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MMALSharp.MMALParameterHelpers;

namespace MMALSharp.Components
{
    internal unsafe class MMALEncoderComponent : MMALComponentBase
    {        
        public MMALEncoderComponent() : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_ENCODER)
        {
            this.Initialize();           
        }

        public void EncoderBufferCallback(MMALBufferImpl buffer)
        {
            Console.WriteLine("Inside encoder buffer callback");

            Console.WriteLine("Buffer alloc size " + buffer.AllocSize);
            Console.WriteLine("Buffer length " + buffer.Length);
            Console.WriteLine("Buffer offset " + buffer.Offset);
            buffer.PrintProperties();
        }

        public override void Initialize()
        {
            var input = this.Inputs.ElementAt(0);
            var output = this.Outputs.ElementAt(0);

            input.ShallowCopy(output);

            output.Ptr->format->encoding = MMALCameraConfigImpl.Config.Encoding;
            output.Ptr->bufferNum = Math.Max(output.Ptr->bufferNumRecommended, output.Ptr->bufferNumMin);
            output.Ptr->bufferSize = Math.Max(output.Ptr->bufferSizeRecommended, output.Ptr->bufferSizeMin);

            output.Commit();

            SetParameter(MMALParametersCamera.MMAL_PARAMETER_JPEG_Q_FACTOR, 90, output.Ptr);
                        
        }
    }
}
