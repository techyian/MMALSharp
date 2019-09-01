namespace MMALSharp.Processors
{
    public interface IFrameAnalyser
    {
        void Apply(byte[] data, bool eos);
    }
}
