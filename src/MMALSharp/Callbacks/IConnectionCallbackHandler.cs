namespace MMALSharp.Callbacks
{
    /// <summary>
    /// Represents a connection callback handler.
    /// </summary>
    public interface IConnectionCallbackHandler : ICallbackHandler
    {
        /// <summary>
        /// The connection this callback handler is used with.
        /// </summary>
        IConnection WorkingConnection { get; }
        
        /// <summary>
        /// The input port callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        void InputCallback(IBuffer buffer);
        
        /// <summary>
        /// The output port callback function to carry out.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        void OutputCallback(IBuffer buffer);
    }
}