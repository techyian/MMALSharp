using System;
using MMALSharp.Common.Utility;
using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports.Inputs;

namespace MMALSharp.Callbacks
{
    public abstract class InputPortCallbackHandler<TPort, TCaptureHandler> : IInputCallbackHandler
        where TPort : IInputPort
        where TCaptureHandler : IInputCaptureHandler
    {
        public MMALEncoding EncodingType { get; }

        public TPort WorkingPort { get; }

        public TCaptureHandler CaptureHandler { get; }

        /// <summary>
        /// Creates a new instance of <see cref="InputPortCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="TPort"/>.</param>
        protected InputPortCallbackHandler(TPort port, TCaptureHandler captureHandler)
        {
            this.WorkingPort = port;
            this.CaptureHandler = captureHandler;
        }

        /// <summary>
        /// Creates a new instance of <see cref="InputPortCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="TPort"/>.</param>
        /// <param name="encodingType">The <see cref="MMALEncoding"/> type to restrict on.</param>
        protected InputPortCallbackHandler(TPort port, TCaptureHandler captureHandler, MMALEncoding encodingType)
            : this(port, captureHandler)
        {
            this.EncodingType = encodingType;
        }

        public virtual ProcessResult CallbackWithResult(IBuffer buffer)
        {
            if (MMALCameraConfig.Debug)
            {
                MMALLog.Logger.Debug($"In managed {this.WorkingPort.PortType.GetPortType()} callback");
            }

            if (this.EncodingType != null && this.WorkingPort.EncodingType != this.EncodingType)
            {
                throw new ArgumentException("Port Encoding Type not supported for this handler.");
            }

            MMALLog.Logger.Info($"Processing {this.CaptureHandler?.TotalProcessed()}");

            return this.CaptureHandler?.Process(buffer.AllocSize);
        }
    }
}
