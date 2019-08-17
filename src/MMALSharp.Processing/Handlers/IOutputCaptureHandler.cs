
namespace MMALSharp.Handlers
{
    public interface IOutputCaptureHandler : ICaptureHandler
    {
        /// <summary>
        /// Used to process the byte array containing our image data from an output port.
        /// </summary>
        /// <param name="data">A byte array containing image data.</param>
        /// <param name="eos">Is end of stream.</param>
        void Process(byte[] data, bool eos);

        /// <summary>
        /// Used for any further processing once we have completed capture.
        /// </summary>
        void PostProcess();
    }
}
