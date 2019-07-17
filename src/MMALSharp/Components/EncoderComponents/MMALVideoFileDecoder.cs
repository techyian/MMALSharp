// <copyright file="MMALVideoFileDecoder.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MMALSharp.Callbacks.Providers;
using MMALSharp.Common.Utility;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    /// <summary>
    /// This component is used to decode video data stored in a stream.
    /// </summary>
    public class MMALVideoFileDecoder : MMALEncoderBase, IMMALConvert
    {
        /// <summary>
        /// Creates a new instance of <see cref="MMALImageFileDecoder"/>.
        /// </summary>
        /// <param name="handler">The capture handle to use.</param>
        public unsafe MMALVideoFileDecoder(ICaptureHandler handler)
            : base(MMALParameters.MMAL_COMPONENT_DEFAULT_VIDEO_DECODER)
        {
            this.Inputs.Add(new VideoFileDecodeInputPort((IntPtr)(&(*this.Ptr->Input[0])), this, PortType.Input, Guid.NewGuid(), handler));
            this.Outputs.Add(new VideoFileDecodeOutputPort((IntPtr)(&(*this.Ptr->Output[0])), this, PortType.Output, Guid.NewGuid(), handler));
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

            if (config.PixelFormat != null)
            {
                this.Inputs[0].Ptr->Format->EncodingVariant = config.PixelFormat.EncodingVal;
            }

            this.Inputs[0].Ptr->Format->Type = MMALFormat.MMAL_ES_TYPE_T.MMAL_ES_TYPE_VIDEO;

            this.Inputs[0].Bitrate = config.Bitrate;
            this.Inputs[0].Resolution = new Resolution(config.Width, config.Height).Pad();
            this.Inputs[0].Crop = new Rectangle(0, 0, config.Width, config.Height);
            this.Inputs[0].FrameRate = new MMAL_RATIONAL_T(config.Framerate, 1);
            this.Inputs[0].Ptr->Format->Es->Video.Par = new MMAL_RATIONAL_T(1, 1);
            
            this.Inputs[0].EncodingType = config.EncodingType;

            this.Inputs[0].Commit();

            this.Inputs[0].Ptr->BufferNum = Math.Max(this.Inputs[0].Ptr->BufferNumMin, this.Inputs[0].Ptr->BufferNumRecommended);
            this.Inputs[0].Ptr->BufferSize = Math.Max(this.Inputs[0].Ptr->BufferSizeMin, this.Inputs[0].Ptr->BufferSizeRecommended);

            return this;
        }

        /// <inheritdoc />
        public override unsafe MMALDownstreamComponent ConfigureOutputPort(int outputPort, MMALPortConfig config)
        {
            if (MMALCameraConfig.VideoColorSpace != null &&
                MMALCameraConfig.VideoColorSpace.EncType == MMALEncoding.EncodingType.ColorSpace)
            {
                this.Outputs[outputPort].VideoColorSpace = MMALCameraConfig.VideoColorSpace;
            }

            if (this.ProcessingPorts.ContainsKey(outputPort))
            {
                this.ProcessingPorts.Remove(outputPort);
            }

            this.ProcessingPorts.Add(outputPort, this.Outputs[outputPort]);

            if (config.EncodingType != null)
            {
                this.Outputs[outputPort].Ptr->Format->Encoding = config.EncodingType.EncodingVal;
            }

            if (config.ZeroCopy)
            {
                this.Outputs[outputPort].ZeroCopy = true;
                this.Outputs[outputPort].SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);
            }

            this.Outputs[outputPort].Ptr->Format->Bitrate = config.Bitrate;
            this.Outputs[outputPort].Ptr->Format->Es->Video.FrameRate = new MMAL_RATIONAL_T(config.Framerate, 1);
            this.Outputs[outputPort].Ptr->Format->Es->Video.Par = new MMAL_RATIONAL_T(1, 1);
            //this.Outputs[outputPort].Ptr->Format->Flags |= MMALFormat.MMAL_ES_FORMAT_FLAG_FRAMED;

            this.Outputs[outputPort].Resolution = new Resolution(config.Width, config.Height).Pad();
            this.Outputs[outputPort].Crop = new Rectangle(0, 0, config.Width, config.Height);
            
            this.Outputs[outputPort].Commit();

            this.Outputs[outputPort].Ptr->BufferSize = Math.Max(this.Outputs[0].Ptr->BufferSizeMin, this.Outputs[0].Ptr->BufferSizeRecommended);
            this.Outputs[outputPort].Ptr->BufferNum = Math.Max(this.Outputs[outputPort].Ptr->BufferNumMin, this.Outputs[outputPort].Ptr->BufferNumRecommended);

            this.Outputs[outputPort].EncodingType = config.EncodingType;
            
            this.Outputs[outputPort].ManagedOutputCallback = PortCallbackProvider.FindCallback(this.Outputs[outputPort]);
            
            return this;
        }

        /// <summary>
        /// Encodes/decodes user provided image data.
        /// </summary>
        /// <param name="outputPort">The output port to begin processing on. Usually will be 0.</param>
        public virtual void Convert(int outputPort = 0)
        {
            MMALLog.Logger.Info("Beginning Video decode from stream. Please note, this process may take some time depending on the size of the input stream.");

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

            this.Outputs[outputPort].Commit();
            
            this.Outputs[outputPort].Ptr->BufferNum = Math.Max(this.Outputs[outputPort].Ptr->BufferNumRecommended, this.Outputs[outputPort].Ptr->BufferNumMin);
            this.Outputs[outputPort].Ptr->BufferSize = Math.Max(this.Outputs[0].Ptr->BufferSizeRecommended, this.Outputs[0].Ptr->BufferSizeMin);
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
            sb.AppendLine($"Bitrate: {format.Bitrate}");
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
                var result = this.Inputs[0].ManagedInputCallback.CallbackWithResult(inputBuffer);
                inputBuffer.ReadIntoBuffer(result.BufferFeed, result.DataLength, result.EOF);

                this.Inputs[0].SendBuffer(inputBuffer);
            }
        }

        private void GetAndSendOutputBuffer(int outputPort = 0)
        {
            while (true)
            {
                var tempBuf2 = this.Outputs[outputPort].BufferPool.Queue.GetBuffer();

                if (tempBuf2.CheckState())
                {
                    // Send empty buffers to the output port of the decoder                                          
                    this.Outputs[outputPort].SendBuffer(tempBuf2);
                }
                else
                {
                    MMALLog.Logger.Debug("GetAndSendOutputBuffer: Buffer null.");
                    break;
                }
            }
        }

        private void ProcessFormatChangedEvent(MMALBufferImpl buffer, int outputPort = 0)
        {
            MMALLog.Logger.Info("Received MMAL_EVENT_FORMAT_CHANGED event");

            var ev = MMALEventFormat.GetEventFormat(buffer);

            MMALLog.Logger.Info("-- Event format changed from -- ");
            this.LogFormat(new MMALEventFormat(this.Outputs[outputPort].Format), this.Outputs[outputPort]);

            MMALLog.Logger.Info("-- To -- ");
            this.LogFormat(ev, null);
            
            buffer.Release();

            this.Outputs[outputPort].DisablePort();

            while (this.Outputs[outputPort].BufferPool.Queue.QueueLength() < this.Outputs[outputPort].BufferPool.HeadersNum)
            {
                MMALLog.Logger.Debug("Queue length less than buffer pool num");
                MMALLog.Logger.Debug("Getting buffer via Queue.Wait");
                var tempBuf = WorkingQueue.Wait();
                tempBuf.Release();
            }

            this.Outputs[outputPort].BufferPool.Dispose();
            this.Outputs[outputPort].BufferPool = null;

            this.Outputs[outputPort].FullCopy(ev);

            this.ConfigureOutputPortWithoutInit(0, this.Outputs[outputPort].EncodingType);

            this.Outputs[outputPort].EnableOutputPort(false);
        }

        private void WaitForTriggers()
        {
            // Wait until the process is complete.
            MMALLog.Logger.Info("Awaiting...");
            Thread.Sleep(2000);
        }
    }
}
