using MMALSharp.Common.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using System;
using System.Threading.Tasks;

namespace MMALSharp.Components
{
    public class MMALImageFileDecoder : MMALImageDecoder, IMMALConvert
    {
        public MMALImageFileDecoder(TransformStreamCaptureHandler handler)
            : base(handler)
        {
        }

        public override unsafe void ConfigureOutputPort(int outputPort, MMALEncoding encodingType, MMALEncoding pixelFormat, int quality, int bitrate = 0)
        {
            this.InitialiseOutputPort(outputPort);
            this.ProcessingPorts.Add(outputPort);

            this.Inputs[0].ShallowCopy(this.Outputs[outputPort]);

            if (encodingType != null)
            {
                this.Outputs[outputPort].Ptr->Format->encoding = encodingType.EncodingVal;
            }

            // Don't do anything with pixelFormat param in this override.
            MMAL_VIDEO_FORMAT_T tempVid = this.Outputs[outputPort].Ptr->Format->es->video;

            try
            {
                this.Outputs[outputPort].Commit();
            }
            catch
            {
                // If commit fails using new settings, attempt to reset using old temp MMAL_VIDEO_FORMAT_T.
                MMALLog.Logger.Warn("Commit of output port failed. Attempting to reset values.");
                this.Outputs[outputPort].Ptr->Format->es->video = tempVid;
                this.Outputs[outputPort].Commit();
            }

            this.Outputs[outputPort].EncodingType = encodingType;
            this.Outputs[outputPort].PixelFormat = pixelFormat;

            this.Outputs[outputPort].Ptr->Format->es->video.height = 0;
            this.Outputs[outputPort].Ptr->Format->es->video.width = 0;
            this.Outputs[outputPort].Ptr->Format->es->video.crop = new MMAL_RECT_T(0, 0, 0, 0);

            this.Outputs[outputPort].Ptr->BufferNum = 1;

            // It is important to re-commit changes to width and height.
            this.Outputs[outputPort].Commit();
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

        internal override unsafe void InitialiseInputPort(int inputPort)
        {
            this.Inputs[inputPort] = new MMALStillConvertPort(&(*this.Ptr->Input[inputPort]), this, PortType.Input);
        }

        internal override unsafe void InitialiseOutputPort(int outputPort)
        {
            this.Outputs[outputPort] = new MMALStillConvertPort(&(*this.Ptr->Output[outputPort]), this, PortType.Output);
        }
    }
}
