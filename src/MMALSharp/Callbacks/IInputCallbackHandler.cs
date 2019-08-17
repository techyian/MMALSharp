using MMALSharp.Handlers;

namespace MMALSharp.Callbacks
{
    public interface IInputCallbackHandler : ICallbackHandler
    {
        /// <summary>
        /// The callback function to carry out. Applies to input ports.
        /// </summary>
        /// <param name="buffer">The working buffer header.</param>
        /// <returns>A <see cref="ProcessResult"/> object based on the result of the callback function.</returns>
        ProcessResult CallbackWithResult(IBuffer buffer);
    }
}
