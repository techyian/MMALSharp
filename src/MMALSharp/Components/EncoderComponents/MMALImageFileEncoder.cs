using System.Threading.Tasks;
using MMALSharp.Native;
using System.Text;
using MMALSharp.Handlers;
using MMALSharp.Ports;
using MMALSharp.Ports.Inputs;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Components
{
    /// <summary>
    /// This component is used to encode image data stored in a file.
    /// </summary>
    public class MMALImageFileEncoder : MMALImageEncoder, IMMALConvert
    {
        /// <summary>
        /// Creates a new instance of <see cref="MMALImageFileEncoder"/>.
        /// </summary>
        /// <param name="handler">The capture handler.</param>
        public MMALImageFileEncoder(ICaptureHandler handler)
            : base(handler)
        {
        }

        /// <summary>
        /// The working queue of buffer headers.
        /// </summary>
        public static MMALQueueImpl WorkingQueue { get; set; }

        /// <inheritdoc />>
        public override unsafe MMALDownstreamComponent ConfigureInputPort(MMALEncoding encodingType, MMALEncoding pixelFormat, int width, int height, bool zeroCopy = false)
        {
            this.InitialiseInputPort(0);

            if (encodingType != null)
            {
                this.Inputs[0].Ptr->Format->Encoding = encodingType.EncodingVal;
            }

            if (pixelFormat != null)
            {
                this.Inputs[0].Ptr->Format->EncodingVariant = pixelFormat.EncodingVal;
            }

            this.Inputs[0].Ptr->Format->Type = MMALFormat.MMAL_ES_TYPE_T.MMAL_ES_TYPE_VIDEO;
            this.Inputs[0].Ptr->Format->Es->Video.Height = height;
            this.Inputs[0].Ptr->Format->Es->Video.Width = width;
            this.Inputs[0].Ptr->Format->Es->Video.FrameRate = new MMAL_RATIONAL_T(0, 1);
            this.Inputs[0].Ptr->Format->Es->Video.Par = new MMAL_RATIONAL_T(1, 1);
            this.Inputs[0].Ptr->Format->Es->Video.Crop = new MMAL_RECT_T(0, 0, width, height);

            this.Inputs[0].EncodingType = encodingType;

            this.Inputs[0].Commit();

            if (this.Outputs[0].Ptr->Format->Type == MMALFormat.MMAL_ES_TYPE_T.MMAL_ES_TYPE_UNKNOWN)
            {
                throw new PiCameraError("Unable to determine settings for output port.");
            }

            this.Inputs[0].Ptr->BufferNum = this.Inputs[0].Ptr->BufferNumMin;
            this.Inputs[0].Ptr->BufferSize = this.Inputs[0].Ptr->BufferSizeMin;

            if (zeroCopy)
            {
                this.Inputs[0].ZeroCopy = true;
                this.Inputs[0].SetParameter(MMALParametersCommon.MMAL_PARAMETER_ZERO_COPY, true);
            }

            return this;
        }
        
        /// <summary>
        /// Encodes/decodes user provided image data.
        /// </summary>
        /// <param name="outputPort">The output port to begin processing on. Usually will be 0.</param>
        /// <returns>An awaitable task.</returns>
        public virtual async Task Convert(int outputPort = 0)
        {
            MMALLog.Logger.Info("Beginning Image encode from filestream. Please note, this process may take some time depending on the size of the input image.");

            // Enable control, input and output ports. Input & Output ports should have been pre-configured by user prior to this point.
            this.Control.Start();
            this.Inputs[0].Start();
            this.Outputs[outputPort].Start();

            this.EnableComponent();

            WorkingQueue = MMALQueueImpl.Create();

            var eosReceived = false;

            while (!eosReceived)
            {
                await this.WaitForTriggers().ConfigureAwait(false);

                this.GetAndSendInputBuffer();

                MMALLog.Logger.Debug("Getting processed output pool buffer");
                while (true)
                {
                    MMALBufferImpl buffer;
                    lock (OutputPort.OutputLock)
                    {
                        buffer = WorkingQueue.GetBuffer();
                    }

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
                                lock (OutputPort.OutputLock)
                                {
                                    buffer.Release();
                                }
                            }
                            
                            continue;
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
                            lock (OutputPort.OutputLock)
                            {
                                buffer.Release();
                            }
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
        
        internal override void InitialiseInputPort(int inputPort)
        {
            this.Inputs[inputPort] = new ImageFileEncodeInputPort(this.Inputs[inputPort]);
        }

        internal override void InitialiseOutputPort(int outputPort)
        {
            this.Outputs[outputPort] = new ImageFileEncodeOutputPort(this.Outputs[outputPort]);
        }
        
        private async Task WaitForTriggers()
        {
            MMALLog.Logger.Debug("Waiting for trigger signal");

            // Wait until the process is complete.
            while (!this.Inputs[0].Trigger)
            {
                MMALLog.Logger.Info("Awaiting...");
                await Task.Delay(2000).ConfigureAwait(false);
                break;
            }
            
            MMALLog.Logger.Debug("Resetting trigger state.");
            this.Inputs[0].Trigger = false;
            this.Outputs[0].Trigger = false;
        }

        private unsafe void ConfigureOutputPortWithoutInit(int outputPort, MMALEncoding encodingType)
        {
            if (encodingType != null)
            {
                this.Outputs[outputPort].Ptr->Format->Encoding = encodingType.EncodingVal;
            }

            this.Outputs[outputPort].EncodingType = encodingType;

            this.Outputs[outputPort].Ptr->BufferNum = 2;
            this.Outputs[outputPort].Ptr->BufferSize = this.Outputs[outputPort].Ptr->BufferSizeRecommended;

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
                        sb.Append("Port Type: Input");
                        break;
                    case PortType.Output:
                        sb.Append("Port Type: Output");
                        break;
                    case PortType.Control:
                        sb.Append("Port Type: Control");
                        break;
                    default:
                        break;
                }
            }

            sb.Append($"FourCC: {format.FourCC}");
            sb.Append($"Width: {format.Width}");
            sb.Append($"Height: {format.Height}");
            sb.Append($"Crop: {format.CropX}, {format.CropY}, {format.CropWidth}, {format.CropHeight}");
            sb.Append($"Pixel aspect ratio: {format.ParNum}, {format.ParDen}. Frame rate: {format.FramerateNum}, {format.FramerateDen}");

            if (port != null)
            {
                sb.Append($"Port info: Buffers num: {port.BufferNum}(opt {port.BufferNumRecommended}, min {port.BufferNumMin}). Size: {port.BufferSize} (opt {port.BufferSizeRecommended}, min {port.BufferSizeMin}). Alignment: {port.BufferAlignmentMin}");
            }

            MMALLog.Logger.Info(sb.ToString());
        }
        
        private void GetAndSendInputBuffer()
        {
            // Get buffer from input port pool                
            MMALBufferImpl inputBuffer;
            lock (InputPort.InputLock)
            {
                inputBuffer = this.Inputs[0].BufferPool.Queue.GetBuffer();

                if (inputBuffer.CheckState())
                {
                    // Populate the new input buffer with user provided image data.
                    var result = this.Inputs[0].ManagedInputCallback.Callback(inputBuffer);
                    inputBuffer.ReadIntoBuffer(result.BufferFeed, result.DataLength, result.EOF);

                    this.Inputs[0].SendBuffer(inputBuffer);
                }
            }
        }

        private void GetAndSendOutputBuffer()
        {
            while (true)
            {
                lock (OutputPort.OutputLock)
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
        }

        private void ProcessFormatChangedEvent(MMALBufferImpl buffer)
        {
            MMALLog.Logger.Debug("Received MMAL_EVENT_FORMAT_CHANGED event");

            var ev = MMALEventFormat.GetEventFormat(buffer);

            MMALLog.Logger.Debug("-- Event format changed from -- ");
            this.LogFormat(new MMALEventFormat(this.Outputs[0].Format), this.Outputs[0]);

            MMALLog.Logger.Debug("-- To -- ");
            this.LogFormat(ev, null);

            // Port format changed
            this.Outputs[0].ManagedOutputCallback.Callback(buffer);
            
            lock (OutputPort.OutputLock)
            {
                buffer.Release();
            }

            this.Outputs[0].DisablePort();

            while (this.Outputs[0].BufferPool.Queue.QueueLength() < this.Outputs[0].BufferPool.HeadersNum)
            {
                MMALLog.Logger.Debug("Queue length less than buffer pool num");
                lock (OutputPort.OutputLock)
                {
                    MMALLog.Logger.Debug("Getting buffer via Queue.Wait");
                    var tempBuf = WorkingQueue.Wait();
                    tempBuf.Release();
                }
            }

            this.Outputs[0].BufferPool.Dispose();

            this.Outputs[0].FullCopy(ev);

            this.ConfigureOutputPortWithoutInit(0, this.Outputs[0].EncodingType);

            this.Outputs[0].EnableOutputPort(false);
        }
    }
}
