namespace MMALSharp
{
    /// <summary>
    /// Supports checking if the native pointer is valid.
    /// </summary>
    public interface IMMALStatus
    {
        /// <summary>
        /// Checks whether this instance is attached to a valid native pointer.
        /// </summary>
        /// <returns></returns>
        bool CheckState();
    }
}
