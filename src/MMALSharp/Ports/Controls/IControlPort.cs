namespace MMALSharp.Ports.Controls
{
    /// <summary>
    /// Represents a control port.
    /// </summary>
    public interface IControlPort : IPort
    {
        /// <summary>
        /// Starts the control port.
        /// </summary>
        void Start();
    }
}
