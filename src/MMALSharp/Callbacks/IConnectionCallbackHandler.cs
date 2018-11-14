namespace MMALSharp.Callbacks
{
    /// <summary>
    /// Represents a connection callback handler.
    /// </summary>
    public interface IConnectionCallbackHandler
    {
        /// <summary>
        /// The connection this callback handler is used with.
        /// </summary>
        MMALConnectionImpl WorkingConnection { get; }
        
        /// <summary>
        /// The input port callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        void InputCallback(MMALBufferImpl buffer);
        
        /// <summary>
        /// The output port callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        void OutputCallback(MMALBufferImpl buffer);
    }
}