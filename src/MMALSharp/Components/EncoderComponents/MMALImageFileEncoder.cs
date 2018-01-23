using MMALSharp.Common.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Components
{
    public class MMALImageFileEncoder : MMALImageEncoder, IMMALConvert
    {
        public unsafe MMALImageFileEncoder(TransformStreamCaptureHandler handler)
            : base(handler)
        {
        }

        public override unsafe void ConfigureInputPort(MMALEncoding encodingType, MMALEncoding pixelFormat, int width, int height)
        {
            this.InitialiseInputPort(0);

            if (encodingType != null)
            {
                this.Inputs[0].Ptr->Format->encoding = encodingType.EncodingVal;
            }

            if (pixelFormat != null)
            {
                this.Inputs[0].Ptr->Format->encodingVariant = pixelFormat.EncodingVal;
            }

            this.Inputs[0].Ptr->Format->es->video.height = height;
            this.Inputs[0].Ptr->Format->es->video.width = width;
            this.Inputs[0].Ptr->Format->es->video.crop = new MMAL_RECT_T(0, 0, width, height);

            this.Inputs[0].Ptr->BufferNum = Math.Max(this.Inputs[0].Ptr->BufferNumRecommended, this.Inputs[0].Ptr->BufferNumMin);
            this.Inputs[0].Ptr->BufferSize = Math.Max(this.Inputs[0].Ptr->BufferSizeRecommended, this.Inputs[0].Ptr->BufferSizeMin);

            this.Inputs[0].Commit();

            if (this.Outputs[0].Ptr->Format->type == MMALFormat.MMAL_ES_TYPE_T.MMAL_ES_TYPE_UNKNOWN)
            {
                throw new PiCameraError("Unable to determine settings for output port.");
            }

            this.Inputs[0].EncodingType = encodingType;
        }

        internal override unsafe void InitialiseInputPort(int inputPort)
        {
            this.Inputs[inputPort] = new MMALStillConvertPort(&(*this.Ptr->Input[inputPort]), this, PortType.Input);
        }

        internal override unsafe void InitialiseOutputPort(int outputPort)
        {
            this.Outputs[outputPort] = new MMALStillConvertPort(&(*this.Ptr->Output[outputPort]), this, PortType.Output);
        }

        /// <summary>
        /// Encodes/decodes user provided image data
        /// </summary>
        /// <param name="outputPort">The output port to begin processing on. Usually will be 0.</param>
        /// <returns>An awaitable task</returns>
        public virtual async Task Convert(int outputPort = 0)
        {
            // Enable both input and output ports. Ports should have been pre-configured by user prior to this point.
            this.Start(this.Inputs[0], this.ManagedInputCallback);
            this.Start(this.Outputs[outputPort], new Action<MMALBufferImpl, MMALPortBase>(this.ManagedOutputCallback));
            this.Control.EnablePort(new Action<MMALBufferImpl, MMALPortBase>(this.ManagedControlCallback));

            this.EnableComponent();

            // Wait until the process is complete.
            this.Inputs[0].Trigger = new Nito.AsyncEx.AsyncCountdownEvent(1);
            await this.Inputs[0].Trigger.WaitAsync();

            this.DisableComponent();

            this.CleanPortPools();
        }
    }
}
