// <copyright file="ImageFileDecodeOutputPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Text;
using System.Threading.Tasks;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Native;

namespace MMALSharp.Ports.Outputs
{
    /// <summary>
    /// A custom port definition used specifically when using encoder conversion functionality.
    /// </summary>
    public unsafe class FileEncodeOutputPort : OutputPort
    {
        /// <summary>
        /// Creates a new instance of <see cref="FileEncodeOutputPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this port.</param>
        public FileEncodeOutputPort(IntPtr ptr, IComponent comp, PortType type, Guid guid)
            : base(ptr, comp, type, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="FileEncodeOutputPort"/>.
        /// </summary>
        /// <param name="copyFrom">The port to copy data from.</param>
        public FileEncodeOutputPort(IPort copyFrom)
            : base((IntPtr)copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.PortType, copyFrom.Guid)
        {
        }

        /// <summary>
        /// The native callback MMAL passes buffer headers to.
        /// </summary>
        /// <param name="port">The port the buffer is sent to.</param>
        /// <param name="buffer">The buffer header.</param>
        internal override void NativeOutputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.Debug($"In native {nameof(FileEncodeOutputPort)} callback");
            }

            var bufferImpl = new MMALBufferImpl(buffer);
            bufferImpl.PrintProperties();

            var eos = ((int)bufferImpl.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS;

            if (bufferImpl.CheckState())
            {
                if (bufferImpl.Cmd > 0)
                {
                    if (bufferImpl.Cmd == MMALEvents.MMAL_EVENT_FORMAT_CHANGED)
                    {
                        this.ProcessFormatChangedEvent(bufferImpl);
                    }
                    else
                    {
                        this.ReleaseBuffer(bufferImpl);
                    }
                }
                else
                {
                    if ((bufferImpl.Length > 0 && !eos && !this.Trigger.Task.IsCompleted) || (eos && !this.Trigger.Task.IsCompleted))
                    {
                        this.CallbackHandler.Callback(bufferImpl);
                    }
                    else
                    {
                        MMALLog.Logger.Debug("Buffer length empty.");
                    }

                    // Ensure we release the buffer before any signalling or we will cause a memory leak due to there still being a reference count on the buffer.                    
                    this.ReleaseBuffer(bufferImpl);
                }
            }
            else
            {
                MMALLog.Logger.Debug($"Invalid output buffer received");
            }

            // If this buffer signals the end of data stream, allow waiting thread to continue.
            if (eos)
            {
                MMALLog.Logger.Debug($"{this.ComponentReference.Name} {this.Name} End of stream. Signaling completion...");

                Task.Run(() => { this.Trigger.SetResult(true); });
            }
        }

        private void ProcessFormatChangedEvent(IBuffer buffer, int outputPort = 0)
        {
            MMALLog.Logger.Info("Received MMAL_EVENT_FORMAT_CHANGED event");

            var ev = MMALEventFormat.GetEventFormat(buffer);

            MMALLog.Logger.Info("-- Event format changed from -- ");
            this.LogFormat(new MMALEventFormat(this.Format), this);

            MMALLog.Logger.Info("-- To -- ");
            this.LogFormat(ev, null);

            buffer.Release();

            this.DisablePort();

            while (this.BufferPool.Queue.QueueLength() < this.BufferPool.HeadersNum)
            {
                MMALLog.Logger.Debug("Queue length less than buffer pool num");

                MMALLog.Logger.Debug("Getting buffer via Queue.Wait");
                var tempBuf = this.BufferPool.Queue.Wait();
                tempBuf.Release();
            }

            this.BufferPool.Dispose();

            this.FullCopy(ev);

            this.ConfigureOutputPortWithoutInit(outputPort);

            this.Enable(false);
        }

        private void LogFormat(MMALEventFormat format, IPort port)
        {
            StringBuilder sb = new StringBuilder();

            if (port != null)
            {
                switch (port.PortType)
                {
                    case PortType.Input:
                        sb.AppendLine("Port Type: Input");
                        break;
                    case PortType.Output:
                        sb.AppendLine("Port Type: Output");
                        break;
                    case PortType.Control:
                        sb.AppendLine("Port Type: Control");
                        break;
                }
            }

            sb.AppendLine($"FourCC: {format.FourCC}");
            sb.AppendLine($"Width: {format.Width}");
            sb.AppendLine($"Height: {format.Height}");
            sb.AppendLine($"Crop: {format.CropX}, {format.CropY}, {format.CropWidth}, {format.CropHeight}");
            sb.AppendLine($"Pixel aspect ratio: {format.ParNum}, {format.ParDen}. Frame rate: {format.FramerateNum}, {format.FramerateDen}");

            if (port != null)
            {
                sb.AppendLine($"Port info: Buffers num: {port.BufferNum}(opt {port.BufferNumRecommended}, min {port.BufferNumMin}). Size: {port.BufferSize} (opt {port.BufferSizeRecommended}, min {port.BufferSizeMin}). Alignment: {port.BufferAlignmentMin}");
            }

            MMALLog.Logger.Info(sb.ToString());
        }

        private void ConfigureOutputPortWithoutInit(int outputPort)
        {
            MMALLog.Logger.Info($"New buffer number {this.BufferNum}");
            MMALLog.Logger.Info($"New buffer size {this.BufferSize}");

            var config = this.PortConfig;
            config.EncodingType = this.EncodingType;
            config.BufferNum = this.BufferNum;
            config.BufferSize = this.BufferSize;

            this.Configure(config, null, null);
        }
    }
}
