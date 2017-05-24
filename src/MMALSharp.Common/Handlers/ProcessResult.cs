
namespace MMALSharp.Handlers
{
    public class ProcessResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public bool EOF { get; set; }
        public byte[] BufferFeed { get; set; }
    }
}
