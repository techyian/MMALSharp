// <copyright file="PortBase.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using static MMALSharp.MMALNativeExceptionHelper;

namespace MMALSharp
{
    /// <summary>
    /// Describes a port type.
    /// </summary>
    public enum PortType
    {
        /// <summary>
        /// An input port.
        /// </summary>
        Input,
        
        /// <summary>
        /// An output port.
        /// </summary>
        Output,
        
        /// <summary>
        /// A clock port.
        /// </summary>
        Clock,
        
        /// <summary>
        /// A control port.
        /// </summary>
        Control,
        
        /// <summary>
        /// A generic port.
        /// </summary>
        Generic
    }

    /// <summary>
    /// Base class for port objects.
    /// </summary>
    public abstract unsafe class PortBase : MMALObject, IPort
    {
        /// <inheritdoc />
        public PortType PortType { get; set; }

        /// <inheritdoc />
        public MMALComponentBase ComponentReference { get; set; }

        /// <inheritdoc />
        public MMALConnectionImpl ConnectedReference { get; set; }

        /// <inheritdoc />
        public MMALPoolImpl BufferPool { get; set; }

        /// <inheritdoc />
        public Guid Guid { get; set; }

        /// <inheritdoc />
        public MMALEncoding EncodingType { get; set; }

        /// <inheritdoc />
        public MMALEncoding PixelFormat { get; set; }

        /// <inheritdoc />
        public ICaptureHandler Handler { get; set; }
        
        /// <inheritdoc />
        public bool ZeroCopy { get; set; }

        #region Native properties

        /// <inheritdoc />
        public string Name => Marshal.PtrToStringAnsi((IntPtr)this.Ptr->Name);

        /// <inheritdoc />
        public bool Enabled => this.Ptr->IsEnabled == 1;

        /// <inheritdoc />
        public int BufferNumMin => this.Ptr->BufferNumMin;
        
        /// <inheritdoc />
        public int BufferSizeMin => this.Ptr->BufferSizeMin;

        /// <inheritdoc />
        public int BufferAlignmentMin => this.Ptr->BufferAlignmentMin;

        /// <inheritdoc />
        public int BufferNumRecommended => this.Ptr->BufferNumRecommended;

        /// <inheritdoc />
        public int BufferSizeRecommended => this.Ptr->BufferSizeRecommended;

        /// <inheritdoc />
        public int BufferNum
        {
            get => this.Ptr->BufferNum;
            set => this.Ptr->BufferNum = value;
        }

        /// <inheritdoc />
        public int BufferSize
        {
            get => this.Ptr->BufferSize;
            set => this.Ptr->BufferSize = value;
        }

        /// <inheritdoc />
        public MMAL_ES_FORMAT_T Format => *this.Ptr->Format;

        /// <inheritdoc />
        public Resolution Resolution
        {
            get => new Resolution(this.Ptr->Format->Es->Video.Width, this.Ptr->Format->Es->Video.Height);
            set
            {
                this.Ptr->Format->Es->Video.Width = value.Width;
                this.Ptr->Format->Es->Video.Height = value.Height;
            }
        }

        /// <inheritdoc />
        public Rectangle Crop
        {
            get => new Rectangle(this.Ptr->Format->Es->Video.Crop.X, this.Ptr->Format->Es->Video.Crop.Y, this.Ptr->Format->Es->Video.Crop.Width, this.Ptr->Format->Es->Video.Crop.Height);
            set => this.Ptr->Format->Es->Video.Crop = new MMAL_RECT_T(value.X, value.Y, value.Width, value.Height);
        }

        /// <inheritdoc />
        public MMAL_RATIONAL_T FrameRate
        {
            get => this.Ptr->Format->Es->Video.FrameRate;
            set => this.Ptr->Format->Es->Video.FrameRate = new MMAL_RATIONAL_T(value.Num, value.Den);
        }
        
        /// <inheritdoc />
        public int CropWidth => this.Ptr->Format->Es->Video.Crop.Width;

        /// <inheritdoc />
        public int CropHeight => this.Ptr->Format->Es->Video.Crop.Height;

        /// <inheritdoc />
        public int NativeEncodingType
        {
            get => this.Ptr->Format->Encoding;
            set => this.Ptr->Format->Encoding = value;
        }

        /// <inheritdoc />
        public int NativeEncodingSubformat
        {
            get => this.Ptr->Format->EncodingVariant;
            set => this.Ptr->Format->EncodingVariant = value;
        } 

        #endregion
        
        /// <inheritdoc />
        public bool Trigger { get; set; }
        
        /// <inheritdoc />
        public MMAL_COMPONENT_T* Comp { get; set; }

        /// <inheritdoc />
        public MMAL_PORT_T* Ptr { get; set; }
        
        /// <inheritdoc />
        public IntPtr PtrCallback { get; set; }

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
        
        /// <inheritdoc />
        public void EnablePort(IntPtr callback)
        {
            MMALLog.Logger.Debug("Enabling port.");
            MMALCheck(MMALPort.mmal_port_enable(this.Ptr, callback), "Unable to enable port.");
        }
        
        /// <inheritdoc />
        public void DisablePort()
        {
            if (this.Enabled)
            {
                MMALLog.Logger.Debug($"Disabling port {this.Name}");
                MMALCheck(MMALPort.mmal_port_disable(this.Ptr), "Unable to disable port.");
            }
        }

        /// <inheritdoc />
        public void Commit()
        {
            MMALLog.Logger.Debug("Committing port format changes");
            MMALCheck(MMALPort.mmal_port_format_commit(this.Ptr), "Unable to commit port changes.");
        }

        /// <inheritdoc />
        public void ShallowCopy(IPort destination)
        {
            MMALLog.Logger.Debug("Shallow copy port format");
            MMALFormat.mmal_format_copy(destination.Ptr->Format, this.Ptr->Format);
        }

        /// <inheritdoc />
        public void ShallowCopy(MMALEventFormat eventFormatSource)
        {
            MMALLog.Logger.Debug("Shallow copy event format");
            MMALFormat.mmal_format_copy(this.Ptr->Format, eventFormatSource.Ptr);
        }

        /// <inheritdoc />
        public void FullCopy(IPort destination)
        {
            MMALLog.Logger.Debug("Full copy port format");
            MMALFormat.mmal_format_full_copy(destination.Ptr->Format, this.Ptr->Format);
        }

        /// <inheritdoc />
        public void FullCopy(MMALEventFormat eventFormatSource)
        {
            MMALLog.Logger.Debug("Full copy event format");
            MMALFormat.mmal_format_full_copy(this.Ptr->Format, eventFormatSource.Ptr);
        }

        /// <inheritdoc />
        public void Flush()
        {            
            MMALLog.Logger.Debug("Flushing port buffers");
            MMALCheck(MMALPort.mmal_port_flush(this.Ptr), "Unable to flush port.");
        }

        /// <inheritdoc />
        public void SendBuffer(MMALBufferImpl buffer)
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

        /// <inheritdoc />
        public void SendAllBuffers(bool sendBuffers = true)
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

        /// <inheritdoc />
        public void SendAllBuffers(MMALPoolImpl pool)
        {
            var length = pool.Queue.QueueLength();

            for (int i = 0; i < length; i++)
            {
                var buffer = pool.Queue.GetBuffer();

                MMALLog.Logger.Debug($"Sending buffer to output port: Length {buffer.Length}");

                this.SendBuffer(buffer);
            }
        }

        /// <inheritdoc />
        public void DestroyPortPool()
        {
            if (this.BufferPool != null)
            {
                this.DisablePort();
                MMALUtil.mmal_port_pool_destroy(this.Ptr, this.BufferPool.Ptr);
            }
        }

        /// <inheritdoc />
        public void InitialiseBufferPool()
        {
            MMALLog.Logger.Debug($"Initialising buffer pool.");
            this.BufferPool = new MMALPoolImpl(this);
        }
        
        /// <inheritdoc />
        public void Stop()
        {
            this.DisablePort();
        }
    }
}
