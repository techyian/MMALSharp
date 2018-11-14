using MMALSharp.Handlers;
using MMALSharp.Native;
using MMALSharp.Ports;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// Represents an input port callback handler.
    /// </summary>
    public interface IInputCallbackHandler
    {
        /// <summary>
        /// A whitelisted Encoding Type that this callback handler will operate on.
        /// </summary>
        MMALEncoding EncodingType { get; }
        
        /// <summary>
        /// The port this callback handler is used with.
        /// </summary>
        IInputPort WorkingPort { get; }
        
        /// <summary>
        /// The callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        /// <returns>A <see cref="ProcessResult"/> object based on the result of the callback function.</returns>
        ProcessResult Callback(MMALBufferImpl buffer);
    }
}