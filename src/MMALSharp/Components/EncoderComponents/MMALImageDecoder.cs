using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Common.Handlers;
using MMALSharp.Components.EncoderComponents;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents an image decoder component
    /// </summary>
    public class MMALImageDecoder : MMALEncoderBase
    {
        private int _width;
        private int _height;

        public override int Width
        {
            get
            {
                if (_width == 0)
                {
                    return MMALCameraConfig.StillResolution.Width;
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
                    return MMALCameraConfig.StillResolution.Height;
                }
                return _height;
            }
            set { _height = value; }
        }

        public MMALImageDecoder(ICaptureHandler handler) : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_DECODER, handler)
        {
            
        }

        public override void PrintComponent()
        {
            base.PrintComponent();
            MMALLog.Logger.Info($"    Width: {this.Width}. Height: {this.Height}");
        }
    }

    public class MMALImageDecoderConverter : MMALImageDecoder, IMMALConvert
    {
        public MMALImageDecoderConverter(TransformStreamCaptureHandler handler) : base(handler)
        {
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
            //Enable both input and output ports. Ports should have been pre-configured by user prior to this point.
            this.Start(this.Inputs[0], this.ManagedInputCallback);
            this.Start(this.Outputs[outputPort], new Action<MMALBufferImpl, MMALPortBase>(this.ManagedOutputCallback));

            this.EnableComponent();

            //Wait until the process is complete.
            this.Outputs[outputPort].Trigger = new Nito.AsyncEx.AsyncCountdownEvent(1);
            await this.Outputs[outputPort].Trigger.WaitAsync();

            this.DisableComponent();

            this.CleanPortPools();
        }


    }
}
