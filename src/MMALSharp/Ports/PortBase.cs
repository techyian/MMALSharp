// <copyright file="PortBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MMALSharp.Callbacks;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Native;
using static MMALSharp.MMALNativeExceptionHelper;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Base class for port objects.
    /// </summary>
    /// <typeparam name="TCallback">The callback handler type.</typeparam>
    public abstract unsafe class PortBase<TCallback> : MMALObject, IPort
        where TCallback : ICallbackHandler
    {
        /// <summary>
        /// The callback handler associated with this port.
        /// </summary>
        public TCallback CallbackHandler { get; internal set; }

        /// <summary>
        /// Specifies the type of port this is.
        /// </summary>
        public PortType PortType { get; }

        /// <summary>
        /// Managed reference to the component this port is associated with.
        /// </summary>
        public IComponent ComponentReference { get; }

        /// <summary>
        /// Managed reference to the downstream component this port is connected to.
        /// </summary>
        public IConnection ConnectedReference { get; internal set; }

        /// <summary>
        /// Managed reference to the pool of buffer headers associated with this port.
        /// </summary>
        public IBufferPool BufferPool { get; internal set; }

        /// <summary>
        /// User defined identifier given to this object.
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
        /// The config for this port.
        /// </summary>
        public MMALPortConfig PortConfig { get; internal set; }

        /// <summary>
        /// Indicates whether ZeroCopy mode should be enabled on this port. When enabled, data is not copied to the ARM processor and is handled directly by the GPU. Useful when
        /// transferring large amounts of data or raw capture.
        /// See: https://www.raspberrypi.org/forums/viewtopic.php?t=170024
        /// </summary>
        public bool ZeroCopy { get; set; }

        /// <summary>
        /// Asynchronous trigger which is set when processing has completed on this port.
        /// </summary>
        public TaskCompletionSource<bool> Trigger { get; internal set; }

        #region Native properties

        /// <summary>
        /// Native pointer that represents this port.
        /// </summary>
        public MMAL_PORT_T* Ptr { get; }

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
        public abstract Resolution Resolution { get; internal set; }

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
        /// The working video color space, specific to video ports.
        /// </summary>
        public MMALEncoding VideoColorSpace
        {
            get => this.Ptr->Format->Es->Video.ColorSpace.ParseEncoding();
            internal set => this.Ptr->Format->Es->Video.ColorSpace = value.EncodingVal;
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

        /// <summary>
        /// The working bitrate of this port.
        /// </summary>
        public int Bitrate
        {
            get => this.Ptr->Format->Bitrate;
            internal set => this.Ptr->Format->Bitrate = value;
        }

        /// <summary>
        /// The working aspect ratio of this port.
        /// </summary>
        public MMAL_RATIONAL_T Par
        {
            get => this.Ptr->Format->Es->Video.Par;
            internal set => this.Ptr->Format->Es->Video.Par = value;
        }

        internal int Width
        {
            get => this.Ptr->Format->Es->Video.Width;
            set => this.Ptr->Format->Es->Video.Width = value;
        }

        internal int Height
        {
            get => this.Ptr->Format->Es->Video.Height;
            set => this.Ptr->Format->Es->Video.Height = value;
        }

        #endregion
        
        /// <summary>
        /// Native pointer that represents the component this port is associated with.
        /// </summary>
        internal MMAL_COMPONENT_T* Comp { get; }
        
        /// <summary>
        /// Native pointer to the native callback function.
        /// </summary>
        internal IntPtr PtrCallback { get; set; }

        /// <summary>
        /// Delegate for native port callback.
        /// </summary>
        internal MMALPort.MMAL_PORT_BH_CB_T NativeCallback { get; set; }
        
        /// <inheritdoc />
        public override bool CheckState()
        {
            return this.Ptr != null && (IntPtr)this.Ptr != IntPtr.Zero;
        }

        /// <summary>
        /// Creates a new managed reference to a MMAL Component Port.
        /// </summary>
        /// <param name="ptr">The native pointer to the component port.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port this is.</param>
        /// <param name="guid">A managed unique identifier for this port.</param>
        protected PortBase(IntPtr ptr, IComponent comp, PortType type, Guid guid)
        {
            this.Ptr = (MMAL_PORT_T*)ptr;
            this.Comp = ((MMAL_PORT_T*)ptr)->Component;
            this.ComponentReference = comp;
            this.PortType = type;
            this.Guid = guid;
        }

        /// <summary>
        /// Enables the specified port.
        /// </summary>
        /// <param name="callback">The function pointer MMAL will call back to.</param>
        /// <exception cref="MMALException"/>
        public void EnablePort(IntPtr callback)
        {
            MMALLog.Logger.LogDebug("Enabling port.");
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
                MMALLog.Logger.LogDebug($"Disabling port {this.Name}");
                MMALCheck(MMALPort.mmal_port_disable(this.Ptr), "Unable to disable port.");
            }
        }

        /// <summary>
        /// Commit format changes on this port.
        /// </summary>
        /// <exception cref="MMALException"/>
        public void Commit()
        {
            MMALLog.Logger.LogDebug("Committing port format changes");
            MMALCheck(MMALPort.mmal_port_format_commit(this.Ptr), "Unable to commit port changes.");
        }

        /// <summary>
        /// Shallow copy a format structure. It is worth noting that the extradata buffer will not be copied in the new format.
        /// </summary>
        /// <param name="destination">The destination port we're copying to.</param>
        public void ShallowCopy(IPort destination)
        {
            MMALLog.Logger.LogDebug("Shallow copy port format");
            MMALFormat.mmal_format_copy(destination.Ptr->Format, this.Ptr->Format);
        }

        /// <summary>
        /// Shallow copy a format structure. It is worth noting that the extradata buffer will not be copied in the new format.
        /// </summary>
        /// <param name="eventFormatSource">The source event format we're copying from.</param>
        public void ShallowCopy(IBufferEvent eventFormatSource)
        {
            MMALLog.Logger.LogDebug("Shallow copy event format");
            MMALFormat.mmal_format_copy(this.Ptr->Format, eventFormatSource.Ptr);
        }

        /// <summary>
        /// Fully copy a format structure, including the extradata buffer.
        /// </summary>
        /// <param name="destination">The destination port we're copying to.</param>
        public void FullCopy(IPort destination)
        {
            MMALLog.Logger.LogDebug("Full copy port format");
            MMALFormat.mmal_format_full_copy(destination.Ptr->Format, this.Ptr->Format);
        }

        /// <summary>
        /// Fully copy a format structure, including the extradata buffer.
        /// </summary>
        /// <param name="eventFormatSource">The source event format we're copying from.</param>
        public void FullCopy(IBufferEvent eventFormatSource)
        {
            MMALLog.Logger.LogDebug("Full copy event format");
            MMALFormat.mmal_format_full_copy(this.Ptr->Format, eventFormatSource.Ptr);
        }

        /// <summary>
        /// Ask a port to release all the buffer headers it currently has. This is an asynchronous operation and the
        /// flush call will return before all the buffer headers are returned to the client.
        /// </summary>
        /// <exception cref="MMALException"/>
        public void Flush()
        {            
            MMALLog.Logger.LogDebug("Flushing port buffers");
            MMALCheck(MMALPort.mmal_port_flush(this.Ptr), "Unable to flush port.");
        }

        /// <summary>
        /// Send a buffer header to a port.
        /// </summary>
        /// <param name="buffer">A managed buffer object.</param>
        /// <exception cref="MMALException"/>
        public void SendBuffer(IBuffer buffer)
        {
            if (this.Enabled)
            {
                if (MMALCameraConfig.Debug)
                {
                    MMALLog.Logger.LogDebug("Sending buffer");
                }

                MMALCheck(MMALPort.mmal_port_send_buffer(this.Ptr, buffer.Ptr), "Unable to send buffer header.");
            }
        }

        /// <summary>
        /// Attempts to send all available buffers in the queue to this port.
        /// </summary>
        public void SendAllBuffers()
        {
            this.InitialiseBufferPool();
            
            var length = this.BufferPool.Queue.QueueLength();

            for (int i = 0; i < length; i++)
            {
                var buffer = this.BufferPool.Queue.GetBuffer();

                MMALLog.Logger.LogDebug($"Sending buffer to output port: Length {buffer.Length}");
                
                this.SendBuffer(buffer);
            }
        }

        /// <summary>
        /// Attempts to send all available buffers in the specified pool's queue to this port.
        /// </summary>
        /// <param name="pool">The specified pool.</param>
        public void SendAllBuffers(IBufferPool pool)
        {
            var length = pool.Queue.QueueLength();

            for (int i = 0; i < length; i++)
            {
                var buffer = pool.Queue.GetBuffer();

                MMALLog.Logger.LogDebug($"Sending buffer to output port: Length {buffer.Length}");

                this.SendBuffer(buffer);
            }
        }

        /// <summary>
        /// Destroy a pool of MMAL_BUFFER_HEADER_T associated with a specific port. This will also deallocate all of the memory
        /// which was allocated when creating or resizing the pool.
        /// </summary>
        public void DestroyPortPool()
        {
            if (this.BufferPool != null && !this.BufferPool.IsDisposed)
            {
                this.DisablePort();

                MMALLog.Logger.LogDebug($"Releasing active buffers for port {this.Name}.");
                while (this.BufferPool.Queue.QueueLength() < this.BufferPool.HeadersNum)
                {
                    var tempBuf = this.BufferPool.Queue.TimedWait(1000);

                    if (tempBuf != null)
                    {
                        tempBuf.Release();
                    }
                    else
                    {
                        MMALLog.Logger.LogWarning("Attempted to release buffer but retrieved null.");
                    }
                }

                this.BufferPool.Dispose();
            }
            else
            {
                MMALLog.Logger.LogDebug("Buffer pool already null or disposed of.");
            }
        }

        /// <summary>
        /// Initialises a new buffer pool.
        /// </summary>
        public void InitialiseBufferPool()
        {
            MMALLog.Logger.LogDebug($"Initialising buffer pool.");
            this.BufferPool = new MMALPoolImpl(this);
        }

        /// <summary>
        /// To be called once connection has been disposed of.
        /// </summary>
        public void CloseConnection()
        {
            this.ConnectedReference = null;
        }

        /// <summary>
        /// Attempts to allocate the native extradata store with the given size.
        /// </summary>
        /// <param name="size">The size to allocate.</param>
        /// <exception cref="MMALException"/>
        public void ExtraDataAlloc(int size)
        {
            MMALCheck(MMALFormat.mmal_format_extradata_alloc(this.Ptr->Format, (uint)size), "Unable to alloc extradata.");
        }
    }
}
