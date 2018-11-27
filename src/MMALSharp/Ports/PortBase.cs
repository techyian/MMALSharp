// <copyright file="PortBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using MMALSharp.Handlers;
using MMALSharp.Native;
using static MMALSharp.MMALNativeExceptionHelper;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Base class for port objects.
    /// </summary>
    public abstract unsafe class PortBase : MMALObject
    {
        /// <summary>
        /// Specifies the type of port this is.
        /// </summary>
        public PortType PortType { get; }

        /// <summary>
        /// Managed reference to the component this port is associated with.
        /// </summary>
        public MMALComponentBase ComponentReference { get; }

        /// <summary>
        /// Managed reference to the downstream component this port is connected to.
        /// </summary>
        public MMALConnectionImpl ConnectedReference { get; internal set; }

        /// <summary>
        /// Managed reference to the pool of buffer headers associated with this port.
        /// </summary>
        public MMALPoolImpl BufferPool { get; internal set; }

        /// <summary>
        /// Managed name given to this object (user defined).
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// The MMALEncoding encoding type that this port will process data in. Helpful for retrieving encoding name/FourCC value.
        /// </summary>
        public MMALEncoding EncodingType { get; internal set; }

        /// <summary>
        /// The MMALEncoding pixel format that this port will process data in. Helpful for retrieving encoding name/FourCC value.
        /// </summary>
        public MMALEncoding PixelFormat { get; internal set; }

        /// <summary>
        /// The handler to process the final data.
        /// </summary>
        public ICaptureHandler Handler { get; internal set; }

        /// <summary>
        /// Indicates whether ZeroCopy mode should be enabled on this port. When enabled, data is not copied to the ARM processor and is handled directly by the GPU. Useful when
        /// transferring large amounts of data or raw capture.
        /// See: https://www.raspberrypi.org/forums/viewtopic.php?t=170024
        /// </summary>
        public bool ZeroCopy { get; internal set; }

        #region Native properties

        /// <summary>
        /// Native name of port.
        /// </summary>
        public string Name => Marshal.PtrToStringAnsi((IntPtr)this.Ptr->Name);

        /// <summary>
        /// Indicates whether this port is enabled.
        /// </summary>
        public bool Enabled => this.Ptr->IsEnabled == 1;

        /// <summary>
        /// Specifies minimum number of buffer headers required for this port.
        /// </summary>
        public int BufferNumMin => this.Ptr->BufferNumMin;

        /// <summary>
        /// Specifies minimum size of buffer headers required for this port.
        /// </summary>
        public int BufferSizeMin => this.Ptr->BufferSizeMin;

        /// <summary>
        /// Specifies minimum alignment value for buffer headers required for this port.
        /// </summary>
        public int BufferAlignmentMin => this.Ptr->BufferAlignmentMin;

        /// <summary>
        /// Specifies recommended number of buffer headers for this port.
        /// </summary>
        public int BufferNumRecommended => this.Ptr->BufferNumRecommended;

        /// <summary>
        /// Specifies recommended size of buffer headers for this port.
        /// </summary>
        public int BufferSizeRecommended => this.Ptr->BufferSizeRecommended;

        /// <summary>
        /// Indicates the currently set number of buffer headers for this port.
        /// </summary>
        public int BufferNum
        {
            get => this.Ptr->BufferNum;
            internal set => this.Ptr->BufferNum = value;
        }

        /// <summary>
        /// Indicates the currently set size of buffer headers for this port.
        /// </summary>
        public int BufferSize
        {
            get => this.Ptr->BufferSize;
            internal set => this.Ptr->BufferSize = value;
        }

        /// <summary>
        /// Accessor for the elementary stream.
        /// </summary>
        public MMAL_ES_FORMAT_T Format => *this.Ptr->Format;

        /// <summary>
        /// The Width/Height that this port will process data in.
        /// </summary>
        public Resolution Resolution
        {
            get => new Resolution(this.Ptr->Format->Es->Video.Width, this.Ptr->Format->Es->Video.Height);
            internal set
            {
                this.Ptr->Format->Es->Video.Width = value.Width;
                this.Ptr->Format->Es->Video.Height = value.Height;
            }
        }

        /// <summary>
        /// The region of interest that this port will process data in.
        /// </summary>
        public Rectangle Crop
        {
            get => new Rectangle(this.Ptr->Format->Es->Video.Crop.X, this.Ptr->Format->Es->Video.Crop.Y, this.Ptr->Format->Es->Video.Crop.Width, this.Ptr->Format->Es->Video.Crop.Height);
            internal set => this.Ptr->Format->Es->Video.Crop = new MMAL_RECT_T(value.X, value.Y, value.Width, value.Height);
        }

        /// <summary>
        /// The framerate we are processing data in.
        /// </summary>
        public MMAL_RATIONAL_T FrameRate
        {
            get => this.Ptr->Format->Es->Video.FrameRate;
            internal set => this.Ptr->Format->Es->Video.FrameRate = new MMAL_RATIONAL_T(value.Num, value.Den);
        }

        /// <summary>
        /// The Region of Interest width that this port will process data in.
        /// </summary>
        public int CropWidth => this.Ptr->Format->Es->Video.Crop.Width;

        /// <summary>
        /// The Region of Interest height that this port will process data in.
        /// </summary>
        public int CropHeight => this.Ptr->Format->Es->Video.Crop.Height;

        /// <summary>
        /// The encoding type that this port will process data in.
        /// </summary>
        public int NativeEncodingType
        {
            get => this.Ptr->Format->Encoding;
            internal set => this.Ptr->Format->Encoding = value;
        }

        /// <summary>
        /// The pixel format that this port will process data in.
        /// </summary>
        public int NativeEncodingSubformat
        {
            get => this.Ptr->Format->EncodingVariant;
            internal set => this.Ptr->Format->EncodingVariant = value;
        }

        #endregion

        /// <summary>
        /// Asynchronous trigger which is set when processing has completed on this port.
        /// </summary>
        public bool Trigger { get; internal set; }

        /// <summary>
        /// Native pointer that represents the component this port is associated with.
        /// </summary>
        internal MMAL_COMPONENT_T* Comp { get; set; }

        /// <summary>
        /// Native pointer that represents this port.
        /// </summary>
        internal MMAL_PORT_T* Ptr { get; set; }

        /// <summary>
        /// Native pointer to the native callback function.
        /// </summary>
        internal IntPtr PtrCallback { get; set; }

        /// <summary>
        /// Delegate for native port callback.
        /// </summary>
        internal MMALPort.MMAL_PORT_BH_CB_T NativeCallback { get; set; }

        /// <summary>
        /// Creates a new managed reference to a MMAL Component Port.
        /// </summary>
        /// <param name="ptr">The native pointer to the component port.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port this is.</param>
        /// <param name="guid">A managed unique identifier for this port.</param>
        protected PortBase(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid)
        {
            this.Ptr = ptr;
            this.Comp = ptr->Component;
            this.ComponentReference = comp;
            this.PortType = type;
            this.Guid = guid;
        }

        /// <summary>
        /// Creates a new managed reference to a MMAL Component Port.
        /// </summary>
        /// <param name="ptr">The native pointer to the component port.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port this is.</param>
        /// <param name="guid">A managed unique identifier for this port.</param>
        /// <param name="handler">The capture handler.</param>
        protected PortBase(MMAL_PORT_T* ptr, MMALComponentBase comp, PortType type, Guid guid, ICaptureHandler handler)
            : this(ptr, comp, type, guid)
        {
            this.Handler = handler;
        }

        /// <summary>
        /// Enables the specified port.
        /// </summary>
        /// <param name="callback">The function pointer MMAL will call back to.</param>
        public void EnablePort(IntPtr callback)
        {
            MMALLog.Logger.Debug("Enabling port.");
            MMALCheck(MMALPort.mmal_port_enable(this.Ptr, callback), "Unable to enable port.");
        }

        /// <summary>
        /// Disable processing on a port. Disabling a port will stop all processing on this port and return all (non-processed)
        /// buffer headers to the client. If this is a connected output port, the input port to which it is connected shall also be disabled.
        /// Any buffer pool shall be released.
        /// </summary>
        /// <exception cref="MMALException"/>
        public void DisablePort()
        {
            if (this.Enabled)
            {
                MMALLog.Logger.Debug($"Disabling port {this.Name}");
                MMALCheck(MMALPort.mmal_port_disable(this.Ptr), "Unable to disable port.");
            }
        }

        /// <summary>
        /// Commit format changes on this port.
        /// </summary>
        /// <exception cref="MMALException"/>
        internal void Commit()
        {
            MMALLog.Logger.Debug("Committing port format changes");
            MMALCheck(MMALPort.mmal_port_format_commit(this.Ptr), "Unable to commit port changes.");
        }

        /// <summary>
        /// Shallow copy a format structure. It is worth noting that the extradata buffer will not be copied in the new format.
        /// </summary>
        /// <param name="destination">The destination port we're copying to.</param>
        internal void ShallowCopy(PortBase destination)
        {
            MMALLog.Logger.Debug("Shallow copy port format");
            MMALFormat.mmal_format_copy(destination.Ptr->Format, this.Ptr->Format);
        }

        /// <summary>
        /// Shallow copy a format structure. It is worth noting that the extradata buffer will not be copied in the new format.
        /// </summary>
        /// <param name="eventFormatSource">The source event format we're copying from.</param>
        internal void ShallowCopy(MMALEventFormat eventFormatSource)
        {
            MMALLog.Logger.Debug("Shallow copy event format");
            MMALFormat.mmal_format_copy(this.Ptr->Format, eventFormatSource.Ptr);
        }

        /// <summary>
        /// Fully copy a format structure, including the extradata buffer.
        /// </summary>
        /// <param name="destination">The destination port we're copying to.</param>
        internal void FullCopy(PortBase destination)
        {
            MMALLog.Logger.Debug("Full copy port format");
            MMALFormat.mmal_format_full_copy(destination.Ptr->Format, this.Ptr->Format);
        }

        /// <summary>
        /// Fully copy a format structure, including the extradata buffer.
        /// </summary>
        /// <param name="eventFormatSource">The source event format we're copying from.</param>
        internal void FullCopy(MMALEventFormat eventFormatSource)
        {
            MMALLog.Logger.Debug("Full copy event format");
            MMALFormat.mmal_format_full_copy(this.Ptr->Format, eventFormatSource.Ptr);
        }

        /// <summary>
        /// Ask a port to release all the buffer headers it currently has. This is an asynchronous operation and the
        /// flush call will return before all the buffer headers are returned to the client.
        /// </summary>
        internal void Flush()
        {            
            MMALLog.Logger.Debug("Flushing port buffers");
            MMALCheck(MMALPort.mmal_port_flush(this.Ptr), "Unable to flush port.");
        }

        /// <summary>
        /// Send a buffer header to a port.
        /// </summary>
        /// <param name="buffer">A managed buffer object.</param>
        internal void SendBuffer(MMALBufferImpl buffer)
        {
            if (this.Enabled)
            {
                if (MMALCameraConfig.Debug)
                {
                    MMALLog.Logger.Debug("Sending buffer");
                }

                MMALCheck(MMALPort.mmal_port_send_buffer(this.Ptr, buffer.Ptr), "Unable to send buffer header.");
            }
        }

        /// <summary>
        /// Attempts to send all available buffers in the queue to this port.
        /// </summary>
        /// <param name="sendBuffers">If false, only initialise the buffer pool with headers.</param>
        internal void SendAllBuffers(bool sendBuffers = true)
        {
            this.InitialiseBufferPool();
            
            if (sendBuffers)
            {
                var length = this.BufferPool.Queue.QueueLength();

                for (int i = 0; i < length; i++)
                {
                    var buffer = this.BufferPool.Queue.GetBuffer();

                    MMALLog.Logger.Debug($"Sending buffer to output port: Length {buffer.Length}");

                    this.SendBuffer(buffer);
                }
            }            
        }

        /// <summary>
        /// Attempts to send all available buffers in the specified pool's queue to this port.
        /// </summary>
        /// <param name="pool">The specified pool.</param>
        internal void SendAllBuffers(MMALPoolImpl pool)
        {
            var length = pool.Queue.QueueLength();

            for (int i = 0; i < length; i++)
            {
                var buffer = pool.Queue.GetBuffer();

                MMALLog.Logger.Debug($"Sending buffer to output port: Length {buffer.Length}");

                this.SendBuffer(buffer);
            }
        }

        /// <summary>
        /// Destroy a pool of MMAL_BUFFER_HEADER_T associated with a specific port. This will also deallocate all of the memory
        /// which was allocated when creating or resizing the pool.
        /// </summary>
        internal void DestroyPortPool()
        {
            if (this.BufferPool != null)
            {
                this.DisablePort();
                MMALUtil.mmal_port_pool_destroy(this.Ptr, this.BufferPool.Ptr);
            }
        }

        /// <summary>
        /// Initialises a new buffer pool.
        /// </summary>
        internal void InitialiseBufferPool()
        {
            MMALLog.Logger.Debug($"Initialising buffer pool.");
            this.BufferPool = new MMALPoolImpl(this);
        }
    }
}
