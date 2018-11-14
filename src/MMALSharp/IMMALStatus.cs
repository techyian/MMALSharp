namespace MMALSharp
{
    /// <summary>
    /// Supports checking if the native pointer is valid.
    /// </summary>
    public interface IMMALStatus
    {
        /// <summary>
        /// Checks whether a native MMAL pointer is valid.
        /// </summary>
        /// <returns>True if the pointer is valid.</returns>
        bool CheckState();
    }
}
