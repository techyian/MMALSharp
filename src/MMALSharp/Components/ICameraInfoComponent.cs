namespace MMALSharp.Components
{
    public interface ICameraInfoComponent : IComponent
    {
        string SensorName { get; }
        int MaxWidth { get; }
        int MaxHeight { get; }
    }
}
