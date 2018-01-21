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

        public override unsafe void ConfigureInputPort(MMALEncoding encodingType, MMALEncoding pixelFormat, int width, int height, uint bufferSize)
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
            
            MMALLog.Logger.Info($"Input encoding {MMALEncodingHelpers.ParseEncoding(this.Inputs[0].Ptr->Format->encoding).EncodingName}");

            this.Inputs[0].Commit();

            if (this.Outputs[0].Ptr->Format->type == MMALFormat.MMAL_ES_TYPE_T.MMAL_ES_TYPE_UNKNOWN)
            {
                throw new PiCameraError("Unable to determine settings for output port.");
            }

            this.Inputs[0].Ptr->BufferNum = Math.Max(this.Inputs[0].Ptr->BufferNumRecommended, this.Inputs[0].Ptr->BufferNumMin);
            this.Inputs[0].Ptr->BufferSize = Math.Max(bufferSize, this.Inputs[0].Ptr->BufferSizeMin);

            this.Inputs[0].EncodingType = encodingType;
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

            //Don't do anything with pixelFormat param in this override.

            MMAL_VIDEO_FORMAT_T tempVid = this.Outputs[outputPort].Ptr->Format->es->video;
            
            try
            {
                this.Outputs[outputPort].Commit();
            }
            catch
            {
                //If commit fails using new settings, attempt to reset using old temp MMAL_VIDEO_FORMAT_T.
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
           
            //It is important to re-commit changes to width and height.
            this.Outputs[outputPort].Commit();
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
            this.Inputs[0].Trigger = new Nito.AsyncEx.AsyncCountdownEvent(1);
            await this.Inputs[0].Trigger.WaitAsync();

            this.DisableComponent();

            this.CleanPortPools();
        }


    }
}
