// <copyright file="IPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using MMALSharp.Handlers;
using MMALSharp.Native;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Represents a MMAL port of any type.
    /// </summary>
    public interface IPort
    {
        /// <summary>
        /// The handler to process the final data.
        /// </summary>
        ICaptureHandler Handler { get; set; }
        
        /// <summary>
        /// Specifies the type of port this is.
        /// </summary>
        PortType PortType { get; set; }
        
        /// <summary>
        /// Managed reference to the component this port is associated with.
        /// </summary>
        MMALComponentBase ComponentReference { get; set; }
        
        /// <summary>
        /// Managed reference to the downstream component this port is connected to.
        /// </summary>
        MMALConnectionImpl ConnectedReference { get; set; }
        
        /// <summary>
        /// Managed reference to the pool of buffer headers associated with this port.
        /// </summary>
        MMALPoolImpl BufferPool { get; set; }
        
        /// <summary>
        /// Managed name given to this object (user defined).
        /// </summary>
        Guid Guid { get; set; }
        
        /// <summary>
        /// The MMALEncoding encoding type that this port will process data in. Helpful for retrieving encoding name/FourCC value.
        /// </summary>
        MMALEncoding EncodingType { get; set; }
        
        /// <summary>
        /// The MMALEncoding pixel format that this port will process data in. Helpful for retrieving encoding name/FourCC value.
        /// </summary>
        MMALEncoding PixelFormat { get; set; }
        
        /// <summary>
        /// Indicates whether ZeroCopy mode should be enabled on this port. When enabled, data is not copied to the ARM processor and is handled directly by the GPU. Useful when
        /// transferring large amounts of data or raw capture.
        /// See: https://www.raspberrypi.org/forums/viewtopic.php?t=170024
        /// </summary>
        bool ZeroCopy { get; set; }
        
        /// <summary>
        /// Asynchronous trigger which is set when processing has completed on this port.
        /// </summary>
        bool Trigger { get; set; }
        
        /// <summary>
        /// Native pointer to the native callback function.
        /// </summary>
        IntPtr PtrCallback { get; set; }
        
        /// <summary>
        /// Native name of port.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Indicates whether this port is enabled.
        /// </summary>
        bool Enabled { get; }
        
        /// <summary>
        /// Specifies minimum number of buffer headers required for this port.
        /// </summary>
        int BufferNumMin { get; }
        
        /// <summary>
        /// Specifies minimum size of buffer headers required for this port.
        /// </summary>
        int BufferSizeMin { get; }
        
        /// <summary>
        /// Specifies minimum alignment value for buffer headers required for this port.
        /// </summary>
        int BufferAlignmentMin { get; }
        
        /// <summary>
        /// Specifies recommended number of buffer headers for this port.
        /// </summary>
        int BufferNumRecommended { get; }
        
        /// <summary>
        /// Specifies recommended size of buffer headers for this port.
        /// </summary>
        int BufferSizeRecommended { get; }
        
        /// <summary>
        /// Indicates the currently set number of buffer headers for this port.
        /// </summary>
        int BufferNum { get; set; }
        
        /// <summary>
        /// Indicates the currently set size of buffer headers for this port.
        /// </summary>
        int BufferSize { get; set; }
        
        /// <summary>
        /// Accessor for the elementary stream.
        /// </summary>
        MMAL_ES_FORMAT_T Format { get; }
        
        /// <summary>
        /// The Width/Height that this port will process data in.
        /// </summary>
        Resolution Resolution { get; set; }
        
        /// <summary>
        /// The region of interest that this port will process data in.
        /// </summary>
        Rectangle Crop { get; set; }
        
        /// <summary>
        /// The framerate we are processing data in.
        /// </summary>
        MMAL_RATIONAL_T FrameRate { get; set; }
        
        /// <summary>
        /// The Region of Interest width that this port will process data in.
        /// </summary>
        int CropWidth { get; }
        
        /// <summary>
        /// The Region of Interest height that this port will process data in.
        /// </summary>
        int CropHeight { get; }
        
        /// <summary>
        /// The encoding type that this port will process data in.
        /// </summary>
        int NativeEncodingType { get; set; }
        
        /// <summary>
        /// The pixel format that this port will process data in.
        /// </summary>
        int NativeEncodingSubformat { get; set; }
        
        /// <summary>
        /// Native pointer that represents this port.
        /// </summary>
        unsafe MMAL_PORT_T* Ptr { get; set; }
        
        /// <summary>
        /// Native pointer that represents the component this port is associated with.
        /// </summary>
        unsafe MMAL_COMPONENT_T* Comp { get; set; }
        
        /// <summary>
        /// Enables the specified port.
        /// </summary>
        /// <param name="callback">The function pointer MMAL will call back to.</param>
        void EnablePort(IntPtr callback);
        
        /// <summary>
        /// Disable the specified port.
        /// </summary>
        void Stop();
        
        /// <summary>
        /// Disable processing on a port. Disabling a port will stop all processing on this port and return all (non-processed)
        /// buffer headers to the client. If this is a connected output port, the input port to which it is connected shall also be disabled.
        /// Any buffer pool shall be released.
        /// </summary>
        /// <exception cref="MMALException"/>
        void DisablePort();
        
        /// <summary>
        /// Commit format changes on this port.
        /// </summary>
        /// <exception cref="MMALException"/>
        void Commit();
        
        /// <summary>
        /// Shallow copy a format structure. It is worth noting that the extradata buffer will not be copied in the new format.
        /// </summary>
        /// <param name="destination">The destination port we're copying to.</param>
        void ShallowCopy(IPort destination);
        
        /// <summary>
        /// Shallow copy a format structure. It is worth noting that the extradata buffer will not be copied in the new format.
        /// </summary>
        /// <param name="eventFormatSource">The source event format we're copying from.</param>
        void ShallowCopy(MMALEventFormat eventFormatSource);
        
        /// <summary>
        /// Fully copy a format structure, including the extradata buffer.
        /// </summary>
        /// <param name="destination">The destination port we're copying to.</param>
        void FullCopy(IPort destination);
        
        /// <summary>
        /// Fully copy a format structure, including the extradata buffer.
        /// </summary>
        /// <param name="eventFormatSource">The source event format we're copying from.</param>
        void FullCopy(MMALEventFormat eventFormatSource);
        
        /// <summary>
        /// Ask a port to release all the buffer headers it currently has. This is an asynchronous operation and the
        /// flush call will return before all the buffer headers are returned to the client.
        /// </summary>
        void Flush();
        
        /// <summary>
        /// Send a buffer header to a port.
        /// </summary>
        /// <param name="buffer">A managed buffer object.</param>
        void SendBuffer(MMALBufferImpl buffer);
        
        /// <summary>
        /// Attempts to send all available buffers in the queue to this port.
        /// </summary>
        /// <param name="sendBuffers">If false, only initialise the buffer pool with headers.</param>
        void SendAllBuffers(bool sendBuffers = true);
        
        /// <summary>
        /// Attempts to send all available buffers in the specified pool's queue to this port.
        /// </summary>
        /// <param name="pool">The specified pool.</param>
        void SendAllBuffers(MMALPoolImpl pool);
        
        /// <summary>
        /// Destroy a pool of MMAL_BUFFER_HEADER_T associated with a specific port. This will also deallocate all of the memory
        /// which was allocated when creating or resizing the pool.
        /// </summary>
        void DestroyPortPool();
        
        /// <summary>
        /// Initialises a new buffer pool.
        /// </summary>
        void InitialiseBufferPool();
    }
}