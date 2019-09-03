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
        /// <summary>
        /// The encoding type to restrict on.
        /// </summary>
        public MMALEncoding EncodingType { get; }

        /// <summary>
        /// The working port.
        /// </summary>
        public TPort WorkingPort { get; }

        /// <summary>
        /// The active capture handler.
        /// </summary>
        public TCaptureHandler CaptureHandler { get; }

        /// <summary>
        /// Creates a new instance of <see cref="InputPortCallbackHandler{TPort,TCaptureHandler}"/>.
        /// </summary>
        /// <param name="port">The working <see cref="TPort"/>.</param>
        /// <param name="handler">The input port capture handler.</param>
        protected InputPortCallbackHandler(TPort port, TCaptureHandler handler)
        {
            this.WorkingPort = port;
            this.CaptureHandler = handler;
        }

        /// <summary>
        /// Creates a new instance of <see cref="InputPortCallbackHandler{TPort,TCaptureHandler}"/>.
        /// </summary>
        /// <param name="port">The working <see cref="TPort"/>.</param>
        /// <param name="handler">The input port capture handler.</param>
        /// <param name="encodingType">The <see cref="MMALEncoding"/> type to restrict on.</param>
        protected InputPortCallbackHandler(TPort port, TCaptureHandler handler, MMALEncoding encodingType)
            : this(port, handler)
        {
            this.EncodingType = encodingType;
        }

        /// <summary>
        /// Responsible for feeding data into the input port.
        /// </summary>
        /// <param name="buffer">The working buffer.</param>
        /// <returns>A <see cref="ProcessResult"/> based on the result of the operation.</returns>
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

            MMALLog.Logger.Info($"Feeding: {Helpers.ConvertBytesToMegabytes(buffer.AllocSize)}. Total processed: {this.CaptureHandler?.TotalProcessed()}.");

            return this.CaptureHandler?.Process(buffer.AllocSize);
        }
    }
}
