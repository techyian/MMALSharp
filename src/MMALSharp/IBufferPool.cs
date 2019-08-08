
namespace MMALSharp
{
    public interface IBufferPool
    {
        IBufferQueue Queue { get; }
        uint HeadersNum { get; }

        void Resize(uint numHeaders, uint size);
    }
}
