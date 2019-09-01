using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports.Inputs;

namespace MMALSharp.Callbacks
{
    public class FileEncoderInputCallbackHandler :InputPortCallbackHandler<IInputPort, IInputCaptureHandler>
    {
        /// <summary>
        /// Creates a new instance of <see cref="FileEncoderInputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IInputPort"/>.</param>
        /// <param name="handler">The input port capture handler.</param>
        public FileEncoderInputCallbackHandler(IInputPort port, IInputCaptureHandler handler)
            : base(port, handler)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="FileEncoderInputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IInputPort"/>.</param>
        /// <param name="handler">The input port capture handler.</param>
        /// <param name="encodingType">The <see cref="MMALEncoding"/> type to restrict on.</param>
        public FileEncoderInputCallbackHandler(IInputPort port, IInputCaptureHandler handler, MMALEncoding encodingType)
            : base(port, handler, encodingType)
        {
        }

        /// <inheritdoc />
        public override ProcessResult CallbackWithResult(IBuffer buffer)
        {
            if (buffer.CheckState())
            {
                var result = base.CallbackWithResult(buffer);

                buffer.ReadIntoBuffer(result.BufferFeed, result.DataLength, result.EOF);
                
                return result;
            }

            return null;
        }
    }
}
