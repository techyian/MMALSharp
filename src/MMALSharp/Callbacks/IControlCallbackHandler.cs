using MMALSharp.Ports.Controls;

namespace MMALSharp.Callbacks
{
    /// <summary>
    /// Represents a control port callback handler.
    /// </summary>
    public interface IControlCallbackHandler
    {
        /// <summary>
        /// The port this callback handler is used with.
        /// </summary>
        ControlPortBase WorkingPort { get; }
        
        /// <summary>
        /// The callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        void Callback(MMALBufferImpl buffer);
    }
}