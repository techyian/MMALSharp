namespace MMALSharp.Callbacks
{
    /// <summary>
    /// Represents an output port callback handler.
    /// </summary>
    public interface IOutputCallbackHandler : ICallbackHandler
    {
        /// <summary>
        /// The callback function to carry out. Applies to output, control and connection ports.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        void Callback(IBuffer buffer);
    }
}
