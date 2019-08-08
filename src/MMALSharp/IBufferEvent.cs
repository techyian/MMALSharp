
namespace MMALSharp
{
    public interface IBufferEvent
    {
        string FourCC { get; }
        int Bitrate { get; }
        int Width { get; }
        int Height { get; }
        int CropX { get; }
        int CropY { get; }
        int CropWidth { get; }
        int CropHeight { get; }
        int ParNum { get; }
        int ParDen { get; }
        int FramerateNum { get; }
        int FramerateDen { get; }
    }
}
