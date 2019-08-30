using System;
using MMALSharp.Callbacks;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports.Inputs;

namespace MMALSharp.Ports.Outputs
{
    public unsafe class SplitterVideoPort : VideoPort
    {
        /// <summary>
        /// Creates a new instance of <see cref="SplitterVideoPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this port.</param>
        public SplitterVideoPort(IntPtr ptr, IComponent comp, PortType type, Guid guid)
            : base(ptr, comp, type, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SplitterVideoPort"/>.
        /// </summary>
        /// <param name="copyFrom">The port to copy data from.</param>
        public SplitterVideoPort(IPort copyFrom)
            : base((IntPtr)copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.PortType, copyFrom.Guid)
        {
        }

        /// <inheritdoc />
        public override void Configure(MMALPortConfig config, IInputPort copyFrom, IOutputCaptureHandler handler)
        {
            // The splitter component should not have its resolution set on the output port so override method accordingly.
            if (config != null)
            {
                this.PortConfig = config;

                copyFrom?.ShallowCopy(this);

                if (config.EncodingType != null)
                {
                    this.NativeEncodingType = config.EncodingType.EncodingVal;
                }

                if (config.PixelFormat != null)
                {
                    this.NativeEncodingSubformat = config.PixelFormat.EncodingVal;
                }

                MMAL_VIDEO_FORMAT_T tempVid = this.Ptr->Format->Es->Video;

                try
                {
                    this.Commit();
                }
                catch
                {
                    // If commit fails using new settings, attempt to reset using old temp MMAL_VIDEO_FORMAT_T.
                    MMALLog.Logger.Warn("Commit of output port failed. Attempting to reset values.");
                    this.Ptr->Format->Es->Video = tempVid;
                    this.Commit();
                }

                if (config.ZeroCopy)
                {
                    this.ZeroCopy = true;
                    this.SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);
                }

                if (MMALCameraConfig.VideoColorSpace != null &&
                    MMALCameraConfig.VideoColorSpace.EncType == MMALEncoding.EncodingType.ColorSpace)
                {
                    this.VideoColorSpace = MMALCameraConfig.VideoColorSpace;
                }

                if (config.Bitrate > 0)
                {
                    this.Bitrate = config.Bitrate;
                }

                this.EncodingType = config.EncodingType;
                this.PixelFormat = config.PixelFormat;
                
                this.Commit();

                this.BufferNum = Math.Max(this.BufferNumMin, config.BufferNum > 0 ? config.BufferNum : this.BufferNumRecommended);
                this.BufferSize = Math.Max(this.BufferSizeMin, config.BufferSize > 0 ? config.BufferSize : this.BufferSizeRecommended);
            }
            
            this.CallbackHandler = new VideoOutputCallbackHandler(this, (IVideoCaptureHandler)handler);
        }

        internal override void NativeOutputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.Debug($"In native {nameof(SplitterVideoPort)} output callback");
            }

            base.NativeOutputPortCallback(port, buffer);
        }
    }
}
