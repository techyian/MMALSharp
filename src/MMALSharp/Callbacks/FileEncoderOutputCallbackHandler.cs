using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports.Outputs;

namespace MMALSharp.Callbacks
{
    public class FileEncoderOutputCallbackHandler : PortCallbackHandler<IOutputPort, IOutputCaptureHandler>
    {
        /// <summary>
        /// Creates a new instance of <see cref="FileEncoderOutputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IOutputPort"/>.</param>
        public FileEncoderOutputCallbackHandler(IOutputPort port, IOutputCaptureHandler handler)
            : base(port, handler)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="FileEncoderOutputCallbackHandler"/>.
        /// </summary>
        /// <param name="port">The working <see cref="IOutputPort"/>.</param>
        /// <param name="encodingType">The <see cref="MMALEncoding"/> type to restrict on.</param>
        public FileEncoderOutputCallbackHandler(IOutputPort port, IOutputCaptureHandler handler, MMALEncoding encodingType)
            : base(port, handler, encodingType)
        {
        }
    }
}
