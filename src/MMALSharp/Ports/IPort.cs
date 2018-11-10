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
    public interface IPort
    {
        ICaptureHandler Handler { get; set; }
        PortType PortType { get; set; }
        MMALComponentBase ComponentReference { get; set; }
        MMALConnectionImpl ConnectedReference { get; set; }
        MMALPoolImpl BufferPool { get; set; }
        Guid Guid { get; set; }
        MMALEncoding EncodingType { get; set; }
        MMALEncoding PixelFormat { get; set; }
        bool ZeroCopy { get; set; }
        bool Trigger { get; set; }
        IntPtr PtrCallback { get; set; }
        
        string Name { get; }
        bool Enabled { get; }
        int BufferNumMin { get; }
        int BufferSizeMin { get; }
        int BufferAlignmentMin { get; }
        int BufferNumRecommended { get; }
        int BufferSizeRecommended { get; }
        int BufferNum { get; set; }
        int BufferSize { get; set; }
        MMAL_ES_FORMAT_T Format { get; }
        Resolution Resolution { get; set; }
        Rectangle Crop { get; set; }
        MMAL_RATIONAL_T FrameRate { get; set; }
        int CropWidth { get; }
        int CropHeight { get; }
        int NativeEncodingType { get; set; }
        int NativeEncodingSubformat { get; set; }
        
        unsafe MMAL_PORT_T* Ptr { get; set; }
        unsafe MMAL_COMPONENT_T* Comp { get; set; }
        
        void EnablePort(IntPtr callback);
        void Stop();
        void DisablePort();
        void Commit();
        void ShallowCopy(IPort destination);
        void ShallowCopy(MMALEventFormat eventFormatSource);
        void FullCopy(IPort destination);
        void FullCopy(MMALEventFormat eventFormatSource);
        void Flush();
        void SendBuffer(MMALBufferImpl buffer);
        void SendAllBuffers(bool sendBuffers = true);
        void SendAllBuffers(MMALPoolImpl pool);
        void DestroyPortPool();
        void InitialiseBufferPool();
    }
}