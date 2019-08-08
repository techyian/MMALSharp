using MMALSharp.Native;

namespace MMALSharp
{
    public interface IBufferQueue
    {
        unsafe MMAL_QUEUE_T* Ptr { get; }

        IBuffer GetBuffer();

        uint QueueLength();

        IBuffer Wait();

        void Put(IBuffer buffer);
    }
}
