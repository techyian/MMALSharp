using System;

namespace MMALSharp
{
    public interface IMMALObject : IDisposable
    {
        /// <summary>
        /// Checks whether a native MMAL pointer is valid.
        /// </summary>
        /// <returns>True if the pointer is valid.</returns>
        bool CheckState();
    }
}
