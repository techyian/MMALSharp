using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// Represents an output port callback handler.
    /// </summary>
    public interface IOutputCallbackHandler
    {
        /// <summary>
        /// A whitelisted Encoding Type that this callback handler will operate on.
        /// </summary>
        MMALEncoding EncodingType { get; }
        
        /// <summary>
        /// The port this callback handler is used with.
        /// </summary>
        IOutputPort WorkingPort { get; }
        
        /// <summary>
        /// The callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        void Callback(MMALBufferImpl buffer);
    }
}