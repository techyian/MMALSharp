// <copyright file="MMALImageFileEncoder.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using System.Threading.Tasks;
using MMALSharp.Native;
using System.Text;
using System.Threading;
using MMALSharp.Callbacks.Providers;
using MMALSharp.Common.Utility;
using MMALSharp.Handlers;
using MMALSharp.Ports;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    /// <summary>
    /// This component is used to encode image data stored in a stream.
    /// </summary>
    public class MMALImageFileEncoder : MMALEncoderBase, IMMALConvert
    {
        /// <summary>
        /// Creates a new instance of <see cref="MMALImageFileEncoder"/>.
        /// </summary>
        /// <param name="handler">The capture handler.</param>
        public unsafe MMALImageFileEncoder(ICaptureHandler handler)
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_IMAGE_ENCODER)
        {
            this.Inputs.Add(new ImageFileEncodeInputPort((IntPtr)(&(*this.Ptr->Input[0])), this, PortType.Input, Guid.NewGuid(), handler));
            this.Outputs.Add(new ImageFileEncodeOutputPort((IntPtr)(&(*this.Ptr->Output[0])), this, PortType.Output, Guid.NewGuid(), handler));
        }

        /// <summary>
        /// The working queue of buffer headers.
        /// </summary>
        public static MMALQueueImpl WorkingQueue { get; set; }

        /// <inheritdoc />
        public override unsafe MMALDownstreamComponent ConfigureInputPort(MMALPortConfig config)
        {
            if (config.EncodingType != null)
            {
                this.Inputs[0].Ptr->Format->Encoding = config.EncodingType.EncodingVal;
            }

            /*if (pixelFormat != null)
            {
                this.Inputs[0].Ptr->Format->EncodingVariant = pixelFormat.EncodingVal;
            }*/

            this.Inputs[0].Ptr->Format->Type = MMALFormat.MMAL_ES_TYPE_T.MMAL_ES_TYPE_VIDEO;
            this.Inputs[0].Ptr->Format->Es->Video.Height = config.Height;
            this.Inputs[0].Ptr->Format->Es->Video.Width = config.Width;
            this.Inputs[0].Ptr->Format->Es->Video.FrameRate = new MMAL_RATIONAL_T(0, 1);
            this.Inputs[0].Ptr->Format->Es->Video.Par = new MMAL_RATIONAL_T(1, 1);
            this.Inputs[0].Ptr->Format->Es->Video.Crop = new MMAL_RECT_T(0, 0, config.Width, config.Height);

            this.Inputs[0].EncodingType = config.EncodingType;

            this.Inputs[0].Commit();

            if (this.Outputs[0].Ptr->Format->Type == MMALFormat.MMAL_ES_TYPE_T.MMAL_ES_TYPE_UNKNOWN)
            {
                throw new PiCameraError("Unable to determine settings for output port.");
            }

            this.Inputs[0].Ptr->BufferNum = this.Inputs[0].Ptr->BufferNumMin;
            this.Inputs[0].Ptr->BufferSize = this.Inputs[0].Ptr->BufferSizeMin;

            if (config.ZeroCopy)
            {
                this.Inputs[0].ZeroCopy = true;
                this.Inputs[0].SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);
            }

            return this;
        }

        /// <inheritdoc />
        public override unsafe MMALDownstreamComponent ConfigureOutputPort(int outputPort, MMALPortConfig config)
        {
            this.Outputs[outputPort].Ptr->Format->Es->Video.Par = new MMAL_RATIONAL_T(1, 1);
            
            if (this.ProcessingPorts.ContainsKey(outputPort))
            {
                this.ProcessingPorts.Remove(outputPort);
            }

            this.ProcessingPorts.Add(outputPort, this.Outputs[outputPort]);

            if (config.EncodingType != null)
            {
                this.Outputs[outputPort].Ptr->Format->Encoding = config.EncodingType.EncodingVal;
            }

            if (config.PixelFormat != null)
            {
                this.Outputs[outputPort].Ptr->Format->EncodingVariant = config.PixelFormat.EncodingVal;
            }

            if (config.ZeroCopy)
            {
                this.Outputs[outputPort].ZeroCopy = true;
                this.Outputs[outputPort].SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);
            }
            
            this.Outputs[outputPort].Resolution = new Resolution(config.Width, config.Height).Pad();
            this.Outputs[outputPort].Crop = new Rectangle(0, 0, config.Width, config.Height);
          
            this.Outputs[outputPort].Commit();

            this.Outputs[outputPort].EncodingType = config.EncodingType;

            this.Outputs[outputPort].ManagedOutputCallback = PortCallbackProvider.FindCallback(this.Outputs[outputPort]);

            this.Outputs[outputPort].Ptr->BufferNum = Math.Max(this.Outputs[outputPort].Ptr->BufferNumMin, this.Outputs[outputPort].Ptr->BufferNumRecommended);
            this.Outputs[outputPort].Ptr->BufferSize = Math.Max(this.Outputs[outputPort].Ptr->BufferSizeMin, this.Outputs[outputPort].Ptr->BufferSizeRecommended);

            return this;
        }

        /// <summary>
        /// Encodes/decodes user provided image data.
        /// </summary>
        /// <param name="outputPort">The output port to begin processing on. Usually will be 0.</param>
        public virtual void Convert(int outputPort = 0)
        {
            MMALLog.Logger.Info("Beginning Image encode from filestream. Please note, this process may take some time depending on the size of the input image.");

            this.Inputs[0].Trigger = new TaskCompletionSource<bool>();
            this.Outputs[0].Trigger = new TaskCompletionSource<bool>();

            // Enable control, input and output ports. Input & Output ports should have been pre-configured by user prior to this point.
            this.Control.Start();
            this.Inputs[0].Start();
            this.Outputs[outputPort].Start();

            this.EnableComponent();

            WorkingQueue = MMALQueueImpl.Create();

            var eosReceived = false;

            while (!eosReceived)
            {
                this.WaitForTriggers();

                this.GetAndSendInputBuffer();

                MMALLog.Logger.Debug("Getting processed output pool buffer");
                while (true)
                {
                    MMALBufferImpl buffer = WorkingQueue.GetBuffer();
                    
                    if (buffer.CheckState())
                    {
                        eosReceived = ((int)buffer.Flags & (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS) == (int)MMALBufferProperties.MMAL_BUFFER_HEADER_FLAG_EOS;

                        if (buffer.Cmd > 0)
                        {
                            if (buffer.Cmd == MMALEvents.MMAL_EVENT_FORMAT_CHANGED)
                            {
                                this.ProcessFormatChangedEvent(buffer);
                            }
                            else
                            {
                                buffer.Release();
                            }
                        }
                        else
                        {
                            if (buffer.Length > 0)
                            {
                                this.Outputs[0].ManagedOutputCallback.Callback(buffer);
                            }
                            else
                            {
                                MMALLog.Logger.Debug("Buffer length empty.");
                            }

                            // Ensure we release the buffer before any signalling or we will cause a memory leak due to there still being a reference count on the buffer.                    
                            buffer.Release();
                        }
                    }
                    else
                    {
                        MMALLog.Logger.Debug("Buffer null.");
                        break;
                    }
                }

                this.GetAndSendOutputBuffer();
            }

            MMALLog.Logger.Info("Received EOS. Exiting.");

            this.DisableComponent();
            this.CleanPortPools();
            WorkingQueue.Dispose();
        }
        
        private unsafe void ConfigureOutputPortWithoutInit(int outputPort, MMALEncoding encodingType)
        {
            if (encodingType != null)
            {
                this.Outputs[outputPort].Ptr->Format->Encoding = encodingType.EncodingVal;
            }

            this.Outputs[outputPort].EncodingType = encodingType;

            this.Outputs[outputPort].Ptr->BufferNum = Math.Max(this.Outputs[outputPort].Ptr->BufferNumMin, this.Outputs[outputPort].Ptr->BufferNumRecommended);
            this.Outputs[outputPort].Ptr->BufferSize = Math.Max(this.Outputs[outputPort].Ptr->BufferSizeMin, this.Outputs[outputPort].Ptr->BufferSizeRecommended);

            MMALLog.Logger.Info($"New buffer number {this.Outputs[outputPort].Ptr->BufferNum}");
            MMALLog.Logger.Info($"New buffer size {this.Outputs[outputPort].Ptr->BufferSize}");

            this.Outputs[outputPort].Commit();
        }

        private void LogFormat(MMALEventFormat format, PortBase port)
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
        
        private void GetAndSendInputBuffer()
        {
            // Get buffer from input port pool                
            MMALBufferImpl inputBuffer = this.Inputs[0].BufferPool.Queue.GetBuffer();

            if (inputBuffer.CheckState())
            {
                // Populate the new input buffer with user provided image data.
                var result = this.Inputs[0].ManagedInputCallback.InputCallback(inputBuffer);
                inputBuffer.ReadIntoBuffer(result.BufferFeed, result.DataLength, result.EOF);

                this.Inputs[0].SendBuffer(inputBuffer);
            }
        }

        private void GetAndSendOutputBuffer()
        {
            while (true)
            {
                var tempBuf2 = this.Outputs[0].BufferPool.Queue.GetBuffer();

                if (tempBuf2.CheckState())
                {
                    // Send empty buffers to the output port of the decoder                                          
                    this.Outputs[0].SendBuffer(tempBuf2);
                }
                else
                {
                    MMALLog.Logger.Debug("GetAndSendOutputBuffer: Buffer null.");
                    break;
                }
            }
        }

        private void ProcessFormatChangedEvent(MMALBufferImpl buffer)
        {
            MMALLog.Logger.Info("Received MMAL_EVENT_FORMAT_CHANGED event");

            var ev = MMALEventFormat.GetEventFormat(buffer);

            MMALLog.Logger.Info("-- Event format changed from -- ");
            this.LogFormat(new MMALEventFormat(this.Outputs[0].Format), this.Outputs[0]);

            MMALLog.Logger.Info("-- To -- ");
            this.LogFormat(ev, null);
            
            buffer.Release();
            
            this.Outputs[0].DisablePort();

            while (this.Outputs[0].BufferPool.Queue.QueueLength() < this.Outputs[0].BufferPool.HeadersNum)
            {
                MMALLog.Logger.Debug("Queue length less than buffer pool num");
                MMALLog.Logger.Debug("Getting buffer via Queue.Wait");
                var tempBuf = WorkingQueue.Wait();
                tempBuf.Release();
            }

            this.Outputs[0].BufferPool.Dispose();

            this.Outputs[0].FullCopy(ev);

            this.ConfigureOutputPortWithoutInit(0, this.Outputs[0].EncodingType);

            this.Outputs[0].EnableOutputPort(false);
        }

        private void WaitForTriggers()
        {
            // Wait until the process is complete.
            MMALLog.Logger.Info("Awaiting...");
            Thread.Sleep(2000);
        }
    }
}
