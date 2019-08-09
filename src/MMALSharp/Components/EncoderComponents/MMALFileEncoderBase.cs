using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MMALSharp.Common.Utility;
using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Components.EncoderComponents
{
    public abstract class MMALFileEncoderBase : MMALEncoderBase, IMMALConvert
    {
        protected MMALFileEncoderBase(string encoderName) 
            : base(encoderName)
        {
        }

        /// <summary>
        /// The working queue of buffer headers.
        /// </summary>
        public static MMALQueueImpl WorkingQueue { get; set; }

        /// <summary>
        /// Encodes/decodes user provided frame data.
        /// </summary>
        /// <param name="outputPort">The output port to begin processing on. Usually will be 0.</param>
        public virtual void Convert(int outputPort = 0)
        {
            MMALLog.Logger.Info("Beginning Encode/Decode from filestream. Please note, this process may take some time depending on the size of the input data.");

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
                    IBuffer buffer = WorkingQueue.GetBuffer();

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
                                this.Outputs[0].ManagedCallback.Callback(buffer);
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

        private void ConfigureOutputPortWithoutInit(int outputPort)
        {
            MMALLog.Logger.Info($"New buffer number {this.Outputs[outputPort].BufferNum}");
            MMALLog.Logger.Info($"New buffer size {this.Outputs[outputPort].BufferSize}");

            var config = this.Outputs[outputPort].PortConfig;
            config.EncodingType = this.Outputs[outputPort].EncodingType;
            config.BufferNum = this.Outputs[outputPort].BufferNum;
            config.BufferSize = this.Outputs[outputPort].BufferSize;

            this.Outputs[outputPort].Configure(config, null);
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

        private void GetAndSendInputBuffer()
        {
            // Get buffer from input port pool                
            var inputBuffer = this.Inputs[0].BufferPool.Queue.GetBuffer();

            if (inputBuffer.CheckState())
            {
                // Populate the new input buffer with user provided image data.
                var result = this.Inputs[0].ManagedCallback.CallbackWithResult(inputBuffer);
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

        private void ProcessFormatChangedEvent(IBuffer buffer, int outputPort = 0)
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

            this.Outputs[outputPort].FullCopy(ev);

            this.ConfigureOutputPortWithoutInit(outputPort);

            this.Outputs[outputPort].Enable(false);
        }

        private void WaitForTriggers()
        {
            MMALLog.Logger.Info("Awaiting...");
            Thread.Sleep(2000);
        }
    }
}
