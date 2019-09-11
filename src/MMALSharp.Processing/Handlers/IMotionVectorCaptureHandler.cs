using System.IO;

namespace MMALSharp.Handlers
{
    /// <summary>
    /// Represents a capture handler which can process motion vectors.
    /// </summary>
    public interface IMotionVectorCaptureHandler
    {
        /// <summary>
        /// Call to initialise the stream to write motion vectors to.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        void InitialiseMotionStore(FileStream stream);
    }
}
