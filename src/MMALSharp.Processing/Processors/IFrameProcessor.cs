namespace MMALSharp.Processors
{
    public interface IFrameProcessor
    {
        void Apply(byte[] store);
    }
}
