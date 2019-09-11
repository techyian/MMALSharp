namespace MMALSharp.Processors
{
    /// <summary>
    /// Represents a frame analyser.
    /// </summary>
    public interface IFrameAnalyser
    {
        /// <summary>
        /// The operation to perform analysis.
        /// </summary>
        /// <param name="data">The working data.</param>
        /// <param name="eos">Signals end of stream.</param>
        void Apply(byte[] data, bool eos);
    }
}
