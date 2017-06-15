using MMALSharp.Handlers;
using System;
using System.Linq;
using System.Reflection;
using MMALSharp.Native;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a downstream component. A downstream component is a component that can have data passed to it from further up the component
    /// heirarchy.
    /// </summary>
    public abstract class MMALDownstreamComponent : MMALComponentBase
    {
        public MMALPortImpl InputPort { get; set; }
        public MMALPortImpl OutputPort { get; set; }

        public abstract int Width { get; set; }
        public abstract int Height { get; set; }

        protected MMALDownstreamComponent(string name, ICaptureHandler handler) : base(name)
        {
            this.Handler = handler;
            MMALCamera.Instance.DownstreamComponents.Add(this);
        }

        /// <summary>
        /// Call to configure changes on an Image Encoder output port.
        /// </summary>
        /// <param name="encodingType"></param>
        /// <param name="pixelFormat"></param>
        /// <param name="bitrate"></param>
        /// <param name="quality"></param>
        public virtual unsafe void ConfigureOutputPort(MMALEncoding encodingType, MMALEncoding pixelFormat, int bitrate = 0, int quality = 90)
        {
            this.OutputPort.Ptr->Format->encoding = encodingType.EncodingVal;
            this.OutputPort.Ptr->Format->encodingVariant = pixelFormat.EncodingVal;

            MMAL_VIDEO_FORMAT_T tempVid = this.OutputPort.Ptr->Format->es->video;

            this.OutputPort.Ptr->Format->es->video.frameRate = new MMAL_RATIONAL_T(0, 1);
            this.OutputPort.Ptr->Format->bitrate = bitrate;

            try
            {
                this.OutputPort.Commit();
            }
            catch
            {
                //If commit fails using new settings, attempt to reset using old temp MMAL_VIDEO_FORMAT_T.
                Helpers.PrintWarning("Commit of output port failed. Attempting to reset values.");
                this.OutputPort.Ptr->Format->es->video = tempVid;
                this.OutputPort.Commit();
            }

            if (encodingType == MMALEncoding.MMAL_ENCODING_JPEG)
            {
                this.OutputPort.SetParameter(MMALParametersCamera.MMAL_PARAMETER_JPEG_Q_FACTOR, quality);
            }

            this.OutputPort.EncodingType = encodingType;
            this.OutputPort.PixelFormat = pixelFormat;

            this.OutputPort.Ptr->BufferNum = Math.Max(this.OutputPort.Ptr->BufferNumRecommended, this.OutputPort.Ptr->BufferNumMin);
            this.OutputPort.Ptr->BufferSize = Math.Max(this.OutputPort.Ptr->BufferSizeRecommended, this.OutputPort.Ptr->BufferSizeMin);
        }

        private void ClosePipelineConnections()
        {
            //Find any components this component is connected to, recursively removing these components.

            foreach (MMALPortImpl port in this.Outputs)
            {
                var connection = MMALCamera.Instance.Connections.Where(c => c.InputPort == port).FirstOrDefault();
                if (connection != null)
                {
                    //This component has an output port connected to another component.
                    connection.DownstreamComponent.ClosePipelineConnections();

                    //Destroy the connection
                    connection.Dispose();
                }
            }

            //Close any connection held by this component
            var finalConnection = MMALCamera.Instance.Connections.Where(c => c.DownstreamComponent == this).FirstOrDefault();

            if (finalConnection != null)
            {
                finalConnection.Dispose();
                
                MMALCamera.Instance.Connections.Remove(finalConnection);
            }
        }
        
        public override void Dispose()
        {
            Debugger.Print("Removing downstream component");

            this.ClosePipelineConnections();

            //Remove any unmanaged resources held by the capture handler.
            this.Handler?.Dispose();

            MMALCamera.Instance.DownstreamComponents.Remove(this);

            base.Dispose();
        }

    }
}
