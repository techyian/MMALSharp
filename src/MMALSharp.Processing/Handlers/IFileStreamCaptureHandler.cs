
namespace MMALSharp.Handlers
{
    public interface IFileStreamCaptureHandler : IOutputCaptureHandler
    {
        /// <summary>
        /// Creates a new File (FileStream), assigns it to the Stream instance of this class and disposes of any existing stream. 
        /// </summary>
        void NewFile();

        /// <summary>
        /// Gets the filepath that a FileStream points to.
        /// </summary>
        /// <returns>The filepath.</returns>
        string GetFilepath();

        /// <summary>
        /// Gets the filename that a FileStream points to.
        /// </summary>
        /// <returns>The filename.</returns>
        string GetFilename();
    }
}
